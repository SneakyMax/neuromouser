using UnityEngine;

namespace Assets._Scripts.GameObjects
{
    [UnityComponent]
    public abstract class InGameObject : MonoBehaviour, IInGameObject
    {
        public LevelLoader LevelLoader { get; set; }

        /// <summary>Drawing layer 0 = floor, 1 = on floor, 2 = mid-level, 3 = ceiling</summary>
        public abstract int Layer { get; }
        
        public virtual void Deserialize(string serialized)
        {
            
        }

        public void Initialize()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                var sortPosition = -transform.position.y * 100 + Layer;
                spriteRenderer.sortingOrder = -Mathf.RoundToInt(sortPosition);
            }
        }

        public virtual void PostAllDeserialized()
        {
            
        }
    }
}