using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Scripts.LevelEditor
{
    public struct GridPosition
    {
        public readonly int X;
        public readonly int Y;

        public GridPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }

        public override string ToString()
        {
            return "" + X + ", " + Y;
        }

        public static bool operator ==(GridPosition a, GridPosition b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(GridPosition a, GridPosition b)
        {
            return a.X != b.X || a.Y != b.Y;
        }
    }

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

        public void Load(IDictionary<GridPosition, IPlacedObject> map)
        {
            throw new NotImplementedException();
        } 
    }
}