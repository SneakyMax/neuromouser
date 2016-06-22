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
        public WallType Type;

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
            return Type.ToString();
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

        public bool IsChewedThrough { get; private set; }

        private SpriteRenderer mainSpriteRenderer;
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
            
            mainSpriteRenderer = SpriteChild.GetComponent<SpriteRenderer>();
            overlaySpriteRenderer = SideWallOverlayChild.GetComponent<SpriteRenderer>();

            SetNotChewed();
            
            mainSpriteRenderer.gameObject.layer = LevelLoader.RunnerLayer;
            overlaySpriteRenderer.gameObject.layer = LevelLoader.RunnerLayer;
        }

        public void SetNotChewed()
        {
            var wallInfo = GetWallInfo(WallType);

            mainSpriteRenderer.sprite = wallInfo.MainSprite;
            overlaySpriteRenderer.sprite = wallInfo.TopOverlaySprite;
        }

        public void SetChewed()
        {
            var wallInfo = GetWallInfo(WallType);

            mainSpriteRenderer.sprite = wallInfo.ChewedSprite;
            overlaySpriteRenderer.sprite = wallInfo.TopOverlayChewedSprite;
        }

        public void SetEmpty()
        {
            mainSpriteRenderer.sprite = null;
            overlaySpriteRenderer.sprite = null;
            IsChewedThrough = true;

            GetComponent<Collider2D>().enabled = false;
        }

        public WallInfo GetWallInfo()
        {
            return GetWallInfo(WallType);
        }

        public WallInfo GetWallInfo(WallType wallType)
        {
            foreach (var info in WallInfos)
            {
                if (info.Type == wallType)
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

        public override bool IsTraversableAt(GridPosition position)
        {
            return IsChewedThrough;
        }
    }
}