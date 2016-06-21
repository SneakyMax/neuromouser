using System.Linq;
using UnityEngine;

namespace Assets._Scripts.LevelEditor
{
    public interface IPlacedObject
    {
        int Id { get; set; }

        GameObject UnityObject { get; }

        string Type { get; }

        /// <summary>0 = floor, 1 - on floor, 2 - mid-level, 3 - ceiling, 4 - logical (aka patrol point)</summary>
        int[] Layers { get; }

        string Serialize();

        void Deserialize(string serialized);

        void Destroy();
    }

    public static class PlacedObjectExtensions
    {
        public static bool IsInLayer(this IPlacedObject obj, int layer)
        {
            return obj.Layers.Contains(layer);
        }
    }
}