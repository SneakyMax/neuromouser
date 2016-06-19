using System;
using System.Linq;
using UnityEngine;

namespace Assets._Scripts.LevelEditor
{
    [UnityComponent]
    public class EditorCursor : MonoBehaviour
    {
        public Tool HoldingTool;

        /// <summary>Clicks won't go to the game world in the palette area.</summary>
        [AssignedInUnity]
        public RectTransform PaletteArea;

        [AssignedInUnity]
        public RectTransform WorldArea;

        private GridPosition lastGridLocation;

        private bool isPanning;
        private Vector2 startPanMousePosition;
        private Vector3 startPanCameraPosition;

        [AssignedInUnity, Range(1, 4)]
        public float PanSpeed;

        [UnityMessage]
        public void Awake()
        {
            Cursor.visible = false;
        }

        [UnityMessage]
        public void Update()
        {
            SetCursorPosition();
            CheckClick();
            CheckMovement();
            CheckPan();
            CheckKeyPress();
        }

        private void SetCursorPosition()
        {
            var screenCursorPosition = Input.mousePosition;
            transform.position = new Vector3(screenCursorPosition.x, screenCursorPosition.y, 0);
        }

        private bool OneMouseButtonWasPressed()
        {
            return Input.GetMouseButtonDown(0) ^ Input.GetMouseButtonDown(1);
        }

        private bool OneMouseButtonIsDown()
        {
            return Input.GetMouseButton(0) ^ Input.GetMouseButton(1);
        }

        private void CheckClick()
        {
            if(OneMouseButtonWasPressed() == false)
                return;
            
            if (CursorIsInPalette())
            {
                HandlePalleteClick(Input.mousePosition);
            }
            else if(CursorIsInWorld())
            {
                HandleWorldClick();
            }
        }

        private bool CursorIsInWorld()
        {
            return WorldArea.GetWorldRect().Contains((Vector2)Input.mousePosition);
        }

        private bool CursorIsInPalette()
        {
            return PaletteArea.GetWorldRect().Contains((Vector2)Input.mousePosition);
        }

        private void CheckMovement()
        {
            var movementWithMouseDown = OneMouseButtonWasPressed() == false && OneMouseButtonIsDown();
            if (movementWithMouseDown == false || HoldingTool == null || HoldingTool.ShouldSnapToGrid == false || !CursorIsInWorld())
                return;
            
            var currentGridPosition = PlacementGrid.Instance.GetGridPosition(HoldingTool.CurrentPosition);

            if (currentGridPosition != lastGridLocation)
            {
                HandleWorldClick();
            }

            lastGridLocation = currentGridPosition;
        }

        private void CheckPan()
        {
            var middleClickOrCommandClick = Input.GetMouseButton(2) ||
                                            (Input.GetMouseButton(0) && (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.LeftAlt)));

            if (middleClickOrCommandClick == false)
                isPanning = false;

            if (CursorIsInWorld() == false)
                return;

            var middleClickOrCommandClickDown = Input.GetMouseButtonDown(2) ||
                                                (Input.GetMouseButtonDown(0) && (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.LeftAlt)));

            if (middleClickOrCommandClickDown)
            {
                isPanning = true;
                startPanMousePosition = Input.mousePosition;
                startPanCameraPosition = Camera.main.transform.position;
            }

            if (isPanning)
            {
                var mousePanDelta = startPanMousePosition - (Vector2)Input.mousePosition;
                var scrollDelta = mousePanDelta * (PanSpeed / 100f);
                var newCameraPosition = (Vector2)startPanCameraPosition + scrollDelta;

                Debug.Log(String.Format("Start Camera Position: {3}, Pan Delta: {0}, Scroll Delta: {1}, new camera position: {2}", mousePanDelta, scrollDelta, newCameraPosition, startPanCameraPosition));

                Camera.main.transform.position = new Vector3(newCameraPosition.x, newCameraPosition.y, startPanCameraPosition.z);
            }
        }

        private void HandlePalleteClick(Vector2 mousePosition)
        {
            var allTools = PaletteArea.GetComponentsInChildren<PaletteItem>();

            var selectedTool = allTools.FirstOrDefault(x => ((RectTransform)x.transform).GetWorldRect().Contains(mousePosition));

            if (selectedTool == null)
                return;

            if (HoldingTool != null)
                Destroy(HoldingTool.gameObject);

            var toolInstance = (GameObject)Instantiate(selectedTool.ToolPrefab, GetWorldPosition(), Quaternion.identity);
            HoldingTool = toolInstance.GetComponent<Tool>();
            HoldingTool.Cursor = this;
        }

        private void HandleWorldClick()
        {
            if (HoldingTool == null)
                return; //todo?

            var alternatePressed = Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.LeftAlt);

            if (alternatePressed)
                return;

            if (Input.GetMouseButton(0))
            {
                HoldingTool.ActivateTool(HoldingTool.CurrentPosition);
            }
            else if(Input.GetMouseButton(1))
            {
                HoldingTool.SecondaryActivateTool(HoldingTool.CurrentPosition);
            }
        }

        public static Vector2 GetWorldPosition()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private void CheckKeyPress()
        {
            if (HoldingTool == null)
                return;

            // stupid unity doesn't have a good way for this so we'll just do it manually
            if (Input.GetKeyDown(KeyCode.R))
            {
                HoldingTool.KeyPressed(KeyCode.R);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                HoldingTool.KeyPressed(KeyCode.Space);
            }
        }
    }
}