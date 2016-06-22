using UnityEngine;

namespace Assets._Scripts
{
    /// <summary>http://answers.unity3d.com/questions/826027/how-sort-mesh-renderer-and-sprite-renderer.html seems to work</summary>
    [UnityComponent]
    public class MeshSortingOrder : MonoBehaviour
    {
        [AssignedInUnity]
        public string LayerName;

        [AssignedInUnity]
        public int Order;

        private MeshRenderer meshRenderer;

        [UnityMessage]
        public void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.sortingLayerName = LayerName;
            meshRenderer.sortingOrder = Order;
        }

        [UnityMessage]
        public void Update()
        {
            if (meshRenderer.sortingLayerName != LayerName)
                meshRenderer.sortingLayerName = LayerName;
            if (meshRenderer.sortingOrder != Order)
                meshRenderer.sortingOrder = Order;
        }

        [UnityMessage]
        public void OnValidate()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.sortingLayerName = LayerName;
            meshRenderer.sortingOrder = Order;
        }
    }
}