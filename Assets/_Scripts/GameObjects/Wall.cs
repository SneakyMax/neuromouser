using System;
using System.Linq;
using Assets._Scripts.LevelEditor;
using Assets._Scripts.LevelEditor.Objects;
using UnityEngine;

namespace Assets._Scripts.GameObjects
{
    [Serializable]
    public struct WallInfo
    {
        [AssignedInUnity]
        public WallType WallType;

        [AssignedInUnity]
        public Sprite MainSprite;

        [AssignedInUnity]
        public Sprite TopOverlaySprite;

        [AssignedInUnity]
        public Sprite ChewedSprite;

        [AssignedInUnity]
        public Sprite TopOverlayChewedSprite;

        [AssignedInUnity, Range(1, 60)]
        public float TimeToChewThroughWall;

        public override string ToString()
        {
            return WallType.ToString();
        }
    }

    [UnityComponent]
    public class Wall : InGameObject
    {
        [AssignedInUnity]
        public WallInfo[] WallInfos;

        [AssignedInUnity]
        public Transform SpriteChild;

        [AssignedInUnity]
        public Transform SideWallOverlayChild;

        public override int Layer { get { return 2; } }

        public override bool IsDynamic { get { return true; } }

        public WallType WallType { get; set; }

        private SpriteRenderer spriteRenderer;
        private SpriteRenderer overlaySpriteRenderer;
        
        public override void Deserialize(string serialized)
        {
            try
            {
                WallType = (WallType)Convert.ToInt32(serialized);
            }
            catch (FormatException)
            {
                WallType = WallType.Metal;
            }
            
            spriteRenderer = SpriteChild.GetComponent<SpriteRenderer>();
            overlaySpriteRenderer = SideWallOverlayChild.GetComponent<SpriteRenderer>();

            SetNotChewed();
            
            spriteRenderer.gameObject.layer = LevelLoader.RunnerLayer;
            overlaySpriteRenderer.gameObject.layer = LevelLoader.RunnerLayer;
        }

        private void SetNotChewed()
        {
            var wallInfo = GetWallInfo(WallType);

            spriteRenderer.sprite = wallInfo.MainSprite;
            overlaySpriteRenderer.sprite = wallInfo.TopOverlaySprite;
        }

        private void SetChewed()
        {
            var wallInfo = GetWallInfo(WallType);

            spriteRenderer.sprite = wallInfo.ChewedSprite;
            overlaySpriteRenderer.sprite = wallInfo.TopOverlayChewedSprite;
        }

        private void SetEmpty()
        {
            spriteRenderer.sprite = null;
            overlaySpriteRenderer.sprite = null;
        }

        private WallInfo GetWallInfo(WallType wallType)
        {
            foreach (var info in WallInfos)
            {
                if (info.WallType == wallType)
                    return info;
            }
            return default(WallInfo);
        }

        public override void PostAllDeserialized()
        {
            CheckForWallAbove();
        }

        private void CheckForWallAbove()
        {
            var above = PlacementGrid.Instance.GetGridPosition(transform.position) + new GridPosition(0, 1);
            var objectsAbove = LevelLoader.GetGridObjectsThatStartedAtPosition(above);

            SideWallOverlayChild.gameObject.SetActive(objectsAbove.OfType<Wall>().Any());
            SideWallOverlayChild.GetComponent<SpriteRenderer>().sortingOrder = SpriteChild.GetComponent<SpriteRenderer>().sortingOrder + 1; // Need to draw the overlay on top.
        }
    }
}