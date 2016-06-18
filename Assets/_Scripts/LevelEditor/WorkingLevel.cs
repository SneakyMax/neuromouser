using System;
using System.Collections.Generic;
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

        private IDictionary<GridPosition, IPlacedObject> grid;
        private IList<PrecisedPlacedObject> nonGridObjects;

        [UnityMessage]
        public void Awake()
        {
            Instance = this;
        }

        [UnityMessage]
        public void Start()
        {
            nonGridObjects = new List<PrecisedPlacedObject>();
            grid = new Dictionary<GridPosition, IPlacedObject>();
        }

        private GridPosition GetGridPosition(Vector2 worldPosition)
        {
            return PlacementGrid.Instance.GetGridPosition(worldPosition);
        }

        public void Place(IPlacedObject obj, float x, float y)
        {
            nonGridObjects.Add(new PrecisedPlacedObject { X = x, Y = y, PlacedObject = obj });
        }

        public bool IsGridObjectAt(Vector2 worldPosition)
        {
            var gridPosition = GetGridPosition(worldPosition);
            return IsGridObjectAt(gridPosition.X, gridPosition.Y);
        }

        public bool IsGridObjectAt(int x, int y)
        {
            return grid.ContainsKey(new GridPosition(x, y));
        }

        public IPlacedObject GetGridObjectAt(Vector2 worldPosition)
        {
            var gridPosition = GetGridPosition(worldPosition);
            return GetGridObjectAt(gridPosition.X, gridPosition.Y);
        }

        public IPlacedObject GetGridObjectAt(int x, int y)
        {
            IPlacedObject obj;
            grid.TryGetValue(new GridPosition(x, y), out obj);
            return obj;
        }

        public void PlaceGridObject(IPlacedObject obj, Vector2 worldPosition)
        {
            grid[GetGridPosition(worldPosition)] = obj;
        }

        public void RemoveGridObjectAt(Vector2 worldPosition)
        {
            var gridPosition = GetGridPosition(worldPosition);
            RemoveGridObjectAt(gridPosition.X, gridPosition.Y);
        }

        public void RemoveGridObjectAt(int x, int y)
        {
            grid.Remove(new GridPosition(x, y));
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

            if (grid.Any(x => x.Value == obj))
            {
                var key = grid.FirstOrDefault(x => x.Value == obj);
                grid.Remove(key);
            }
        }

        public void Reset()
        {
            grid.Clear();
            nonGridObjects.Clear();
        }

        public string SerializeLevel()
        {
            var serialized = new StringBuilder();

            serialized.AppendLine("grid:");

            foreach (var gridItem in grid)
            {
                serialized.Append(gridItem.Key.X);
                serialized.Append(',');
                serialized.Append(gridItem.Key.Y);
                serialized.Append(',');
                serialized.Append(ObjectRegistration.Instance.GetId(gridItem.Value.Type));
                serialized.Append(',');
                serialized.Append(gridItem.Value.Serialize());
                serialized.AppendLine();
            }

            serialized.AppendLine("nonGrid:");

            foreach (var item in nonGridObjects)
            {
                serialized.Append(item.X);
                serialized.Append(',');
                serialized.Append(item.Y);
                serialized.Append(',');
                serialized.Append(ObjectRegistration.Instance.GetId(item.PlacedObject.Type));
                serialized.Append(',');
                serialized.Append(item.PlacedObject.Serialize());
                serialized.AppendLine();
            }

            return serialized.ToString();
        }
    }
}