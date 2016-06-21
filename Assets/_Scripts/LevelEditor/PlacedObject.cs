using System.Linq;
using Assets._Scripts.GameObjects;
using UnityEngine;

namespace Assets._Scripts.LevelEditor
{
    [UnityComponent]
    public abstract class PlacedObject : MonoBehaviour, IPlacedObject
    {
        public int Id { get; set; }

        public GameObject UnityObject { get { return gameObject; } }

        public virtual string Type { get { return GetType().Name; } }

        public abstract int[] Layers { get; }

        [UnityMessage]
        public void Start()
        {
            RefreshSpriteOrdering();
        }

        protected void RefreshSpriteOrdering()
        {
            foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
            {
                spriteRenderer.sortingOrder = InGameObject.GetSortPosition(transform.position, Layers.Length == 0 ? 0 : Layers.Max());
            }

            AfterRefreshSpriteOrdering();
        }

        protected virtual void AfterRefreshSpriteOrdering()
        {
            // This func is dumb
        }

        public virtual string Serialize()
        {
            return "";
        }

        public virtual void Deserialize(string serialized)
        {
        }

        public virtual void PostAllDeserialized()
        {
            
        }

        public virtual void Destroy()
        {
            Destroy(gameObject);
        }

        public virtual void AfterPlace()
        {
            
        }

        public virtual void BeforeRemove()
        {
            
        }

        public virtual void NearbyObjectChanged()
        {
            
        }
    }
}