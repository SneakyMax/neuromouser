using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets._Scripts.LevelEditor
{
    public class PrecisedPlacedObject
    {
        public float X { get; set; }
        public float Y { get; set; }
        public IPlacedObject PlacedObject { get; set; }
    }

    public class WorkingLevel : MonoBehaviour
    {
        public static WorkingLevel Instance { get; private set; }

        private IDictionary<GridPosition, IList<IPlacedObject>> grid;
        private IList<PrecisedPlacedObject> nonGridObjects;

        private int objectIdCounter;
        private int largestFoundId;

        [UnityMessage]
        public void Awake()
        {
            Instance = this;
            objectIdCounter = 0;
            largestFoundId = 0;
        }

        [UnityMessage]
        public void Start()
        {
            nonGridObjects = new List<PrecisedPlacedObject>();
            grid = new Dictionary<GridPosition, IList<IPlacedObject>>();
        }

        private GridPosition GetGridPosition(Vector2 worldPosition)
        {
            return PlacementGrid.Instance.GetGridPosition(worldPosition);
        }

        public void Place(IPlacedObject obj, float x, float y)
        {
            nonGridObjects.Add(new PrecisedPlacedObject { X = x, Y = y, PlacedObject = obj });
            obj.Id = objectIdCounter++;
        }

        public bool IsGridObjectAt(Vector2 worldPosition, int layer)
        {
            var gridPosition = GetGridPosition(worldPosition);
            return IsGridObjectAt(gridPosition.X, gridPosition.Y, layer);
        }

        public bool IsGridObjectAt(int x, int y, int layer)
        {
            var position = new GridPosition(x, y);
            return grid.ContainsKey(position) && grid[position].Any(o => o.IsInLayer(layer));
        }

        public bool IsAnyGridObjectAt(Vector2 worldPosition, int[] layers)
        {
            var gridPosition = GetGridPosition(worldPosition);
            return IsAnyGridObjectAt(gridPosition.X, gridPosition.Y, layers);
        }

        public bool IsAnyGridObjectAt(int x, int y, int[] layers)
        {
            var position = new GridPosition(x, y);
            return grid.ContainsKey(position) && grid[position].Any(o => layers.Any(o.IsInLayer));
        }

        public IPlacedObject GetGridObjectAt(Vector2 worldPosition, int layer)
        {
            var gridPosition = GetGridPosition(worldPosition);
            return GetGridObjectAt(gridPosition.X, gridPosition.Y, layer);
        }

        public IPlacedObject GetGridObjectAt(int x, int y, int layer)
        {
            IList<IPlacedObject> gridPositionObjects;
            grid.TryGetValue(new GridPosition(x, y), out gridPositionObjects);

            if (gridPositionObjects == null)
                return null;

            return gridPositionObjects.FirstOrDefault(o => o.IsInLayer(layer));
        }

        public void PlaceGridObject(IPlacedObject obj, Vector2 worldPosition)
        {
            var gridPosition = GetGridPosition(worldPosition);
            if (grid.ContainsKey(gridPosition) == false)
                grid[gridPosition] = new List<IPlacedObject>();

            if (IsAnyGridObjectAt(gridPosition.X, gridPosition.Y, obj.Layers))
                return;

            grid[gridPosition].Add(obj);
            obj.Id = objectIdCounter++;
        }

        public void RemoveGridObjectAt(Vector2 worldPosition, int layer)
        {
            var gridPosition = GetGridPosition(worldPosition);

            IList<IPlacedObject> gridPositionObjects;
            grid.TryGetValue(gridPosition, out gridPositionObjects);

            if (gridPositionObjects == null || gridPositionObjects.Count == 0)
                return;

            var match = gridPositionObjects.FirstOrDefault(x => x.Layers.Contains(layer));

            if (match == null)
                return;

            gridPositionObjects.Remove(match);
            match.Destroy();
        }

        public void RemoveTopmostGridObjectAt(Vector2 worldPosition)
        {
            var gridPosition = GetGridPosition(worldPosition);

            IList<IPlacedObject> gridPositionObjects;
            grid.TryGetValue(gridPosition, out gridPositionObjects);

            if (gridPositionObjects == null || gridPositionObjects.Count == 0)
                return;

            var maxLayer = gridPositionObjects.Max(x => x.Layers.Max(y => y));
            var topmost = gridPositionObjects.FirstOrDefault(x => x.Layers.Contains(maxLayer));

            if (topmost == null)
                return;

            gridPositionObjects.Remove(topmost);

            topmost.Destroy();
        }

        public void Remove(IPlacedObject obj)
        {
            PrecisedPlacedObject found = null;
            foreach (var existing in nonGridObjects)
            {
                if (obj == existing.PlacedObject)
                {
                    found = existing;
                }
            }

            if (found != null)
            {
                nonGridObjects.Remove(found);
                return;
            }

            IPlacedObject foundPlaced = null;
            foreach (var objects in grid.Values)
            {
                foreach(var placedObj in objects)
                {
                    if (placedObj == obj)
                    {
                        foundPlaced = placedObj;
                        break;
                    }
                }
                if (foundPlaced != null)
                {
                    objects.Remove(foundPlaced);
                    foundPlaced.Destroy();
                }
            }
        }

        public void Reset()
        {
            foreach (var placedObjects in grid.Values)
            {
                foreach (var placedObject in placedObjects)
                {
                    placedObject.Destroy();
                }
            }

            foreach (var placedObject in nonGridObjects)
            {
                placedObject.PlacedObject.Destroy();
            }

            grid.Clear();
            nonGridObjects.Clear();
            objectIdCounter = 0;
            largestFoundId = 0;
        }

        public string SerializeLevel()
        {
            var serialized = new StringBuilder();

            serialized.AppendLine("version:2");

            SerializeGridItems(serialized);

            SerializeNonGridItems(serialized);

            return serialized.ToString();
        }

        public void DeserializeLevel(string levelData)
        {
            var version = 1;
            using (var reader = new StringReader(levelData))
            {
                string currentMode = null;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if(line.StartsWith("version:"))
                    {
                        version = Convert.ToInt32(line.Substring(0, "version:".Length));
                        continue;
                    }

                    switch (line)
                    {
                        case "grid:":
                            currentMode = "grid";
                            break;
                        case "nonGrid:":
                            currentMode = "nonGrid";
                            break;
                        default:
                            HandleLine(line, currentMode, version);
                            break;
                    }
                }
            }
        }

        private void HandleLine(string line, string currentMode, int version)
        {
            if (String.IsNullOrEmpty(currentMode))
                return;

            switch (currentMode)
            {
                case "grid":
                    DeserializeGridItem(line, version);
                    break;
                case "nonGrid":
                    DeserializeNonGridItem(line, version);
                    break;
            }
        }

        private void SerializeGridItems(StringBuilder serialized)
        {
            serialized.AppendLine("grid:");

            foreach (var pairs in grid)
            {
                foreach (var gridItem in pairs.Value)
                {
                    serialized.Append(gridItem.Id);
                    serialized.Append(',');
                    serialized.Append(pairs.Key.X);
                    serialized.Append(',');
                    serialized.Append(pairs.Key.Y);
                    serialized.Append(',');
                    serialized.Append(ObjectRegistration.Instance.GetId(gridItem.Type));
                    serialized.Append(',');
                    serialized.Append(gridItem.Serialize());
                    serialized.AppendLine();
                }
            }
        }

        private void DeserializeGridItem(string line, int version)
        {
            if(version == 1)
                DeserializeGridItemV1(line);
            else if (version == 2)
                DeserializeGridItemV2(line);
        }

        private void DeserializeGridItemV1(string line)
        {
            var parts = line.Split(',');
            var x = Convert.ToInt32(parts[0]);
            var y = Convert.ToInt32(parts[1]);
            var typeId = Convert.ToInt32(parts[2]);
            var serializedObject = String.Join(",", parts.Skip(3).ToArray());

            var info = ObjectRegistration.Instance.GetInfo(typeId);

            var worldPosition = PlacementGrid.Instance.GetWorldPosition(x, y);

            var instance = (GameObject)Instantiate(info.ObjEditorPrefab, worldPosition, Quaternion.identity);

            var placedObject = instance.GetInterfaceComponent<IPlacedObject>();
            placedObject.Deserialize(serializedObject);

            PlaceGridObject(placedObject, worldPosition);
        }

        private void DeserializeGridItemV2(string line)
        {
            var parts = line.Split(',');

            var objectId = Convert.ToInt32(parts[0]);
            var x = Convert.ToInt32(parts[1]);
            var y = Convert.ToInt32(parts[2]);
            var typeId = Convert.ToInt32(parts[3]);
            var serializedObject = String.Join(",", parts.Skip(4).ToArray());

            var info = ObjectRegistration.Instance.GetInfo(typeId);

            var worldPosition = PlacementGrid.Instance.GetWorldPosition(x, y);

            var instance = (GameObject)Instantiate(info.ObjEditorPrefab, worldPosition, Quaternion.identity);

            var placedObject = instance.GetInterfaceComponent<IPlacedObject>();
            placedObject.Deserialize(serializedObject);

            PlaceGridObject(placedObject, worldPosition);
            placedObject.Id = objectId;
            CheckId(objectId);
        }

        private void SerializeNonGridItems(StringBuilder serialized)
        {
            serialized.AppendLine("nonGrid:");

            foreach (var item in nonGridObjects)
            {
                serialized.Append(item.PlacedObject.Id);
                serialized.Append(',');
                serialized.Append(item.X);
                serialized.Append(',');
                serialized.Append(item.Y);
                serialized.Append(',');
                serialized.Append(ObjectRegistration.Instance.GetId(item.PlacedObject.Type));
                serialized.Append(',');
                serialized.Append(item.PlacedObject.Serialize());
                serialized.AppendLine();
            }
        }

        private void DeserializeNonGridItem(string line, int version)
        {
            if(version == 1)
                DeserializeNonGridItemV1(line);
            else if (version == 2)
                DeserializeNonGridItemV2(line);
        }

        private void DeserializeNonGridItemV1(string line)
        {
            var parts = line.Split(',');
            var x = Convert.ToSingle(parts[0]);
            var y = Convert.ToSingle(parts[1]);
            var typeId = Convert.ToInt32(parts[2]);
            var serializedObject = String.Join(",", parts.Skip(3).ToArray());

            var info = ObjectRegistration.Instance.GetInfo(typeId);

            var instance = (GameObject)Instantiate(info.ObjEditorPrefab, new Vector2(x, y), Quaternion.identity);

            var placedObject = instance.GetInterfaceComponent<IPlacedObject>();
            placedObject.Deserialize(serializedObject);

            Place(placedObject, x, y);
        }

        private void DeserializeNonGridItemV2(string line)
        {
            var parts = line.Split(',');
            var objectId = Convert.ToInt32(parts[0]);
            var x = Convert.ToSingle(parts[0]);
            var y = Convert.ToSingle(parts[1]);
            var typeId = Convert.ToInt32(parts[2]);
            var serializedObject = String.Join(",", parts.Skip(3).ToArray());

            var info = ObjectRegistration.Instance.GetInfo(typeId);

            var instance = (GameObject)Instantiate(info.ObjEditorPrefab, new Vector2(x, y), Quaternion.identity);

            var placedObject = instance.GetInterfaceComponent<IPlacedObject>();
            placedObject.Deserialize(serializedObject);

            Place(placedObject, x, y);
            placedObject.Id = objectId;
            CheckId(objectId);
        }

        private void CheckId(int id)
        {
            largestFoundId = Mathf.Max(largestFoundId, id);
            objectIdCounter = largestFoundId + 1;
        }
    }
}