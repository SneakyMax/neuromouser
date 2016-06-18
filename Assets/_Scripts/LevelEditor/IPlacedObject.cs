using UnityEngine;

namespace Assets._Scripts.LevelEditor
{
    public interface IPlacedObject
    {
        GameObject UnityObject { get; }

        string Type { get; }

        string Serialize();

        void Deserialize(string serialized);
    }
}