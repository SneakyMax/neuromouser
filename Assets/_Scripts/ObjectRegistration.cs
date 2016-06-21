using System;
using UnityEngine;

namespace Assets._Scripts
{
    [Serializable]
    public struct ObjectRegistrationInfo
    {
        [AssignedInUnity]
        public string Name;

        [AssignedInUnity]
        public GameObject ObjLevelPrefab;

        [AssignedInUnity]
        public GameObject ObjEditorPrefab;
    }

    [UnityComponent]
    public class ObjectRegistration : MonoBehaviour
    {
        public static ObjectRegistration Instance { get; private set; }

        [AssignedInUnity]
        public ObjectRegistrationInfo[] Objects;

        [UnityMessage]
        public void Awake()
        {
            Instance = this;
        }

        public ObjectRegistrationInfo GetInfo(int id)
        {
            if (id < 0 || id > Objects.Length - 1)
                throw new InvalidOperationException("Unknown id " + id + ". Deserialization error, or missing a registered object.");
            return Objects[id];
        }

        public int GetId(GameObject prefab)
        {
            for (var i = 0; i < Objects.Length; i++)
            {
                if (Objects[i].ObjLevelPrefab == prefab || Objects[i].ObjEditorPrefab == prefab)
                    return i;
            }
            throw new InvalidOperationException("Could not find prefab " + prefab.name + " registered. Register it in the Object Registration prefab.");
        }

        public int GetId(string objectTypeName)
        {
            for (var i = 0; i < Objects.Length; i++)
            {
                if (Objects[i].Name == objectTypeName || Objects[i].Name == objectTypeName)
                    return i;
            }
            throw new InvalidOperationException("Could not find object with type " + objectTypeName + " registered. Register it in the Object Registration prefab and make sure it matches keys in the code.");
        }
    }
}