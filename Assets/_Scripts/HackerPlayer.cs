using System;
using System.Collections;
using UnityEngine;

namespace Assets._Scripts
{
    /// <summary>
    /// Hacker-mouse player handling (including input).
    /// </summary>
    public class HackerPlayer : MonoBehaviour
    {
        [AssignedInUnity]
        public Sprite CameraHackerSprite;

        [AssignedInUnity]
        public Sprite DoorHackerSprite;

        [AssignedInUnity]
        public Sprite TrapHackerSprite;

        [AssignedInUnity]
        public Sprite CatHackerSprite;

        /// <summary>
        /// The type of attached terminal.
        /// </summary>
        private enum TerminalType
        {
            Cameras,
            Doors,
            Traps,
            Cats
        }

        /// <summary>
        /// The regular shot prefab to instantiate when firing.
        /// </summary>
        [AssignedInUnity]
        public GameObject RegularShot;

        /// <summary>
        /// The repeat time for limiting movement presses.
        /// </summary>
        [AssignedInUnity]
        public float MoveRepeatTime = 0.25f;

        /// <summary>
        /// The currently accessed terminal.
        /// </summary>
        private TerminalType currentTerminal = TerminalType.Cameras;

        /// <summary>
        /// The coroutine set for limiting keystrokes.
        /// </summary>
        private IEnumerator cantMove;
    
        public float PowerCharge;

        [AssignedInUnity]
        public float MaxPowerCharge = 100;

        [AssignedInUnity]
        public float PowerForShot = 1;

        [AssignedInUnity]
        public float PowerChargeRate;

        [AssignedInUnity]
        public float FireRate;

        private HackerTerminal currentTerminalObject;

        private SpriteRenderer spriteRenderer;

        private Coroutine shootingCoroutine;

        /// <summary>
        /// Checks prefabs.
        /// </summary>
        /// <exception cref="UnityException">Thrown if Shot prefabs are not specified.</exception>
        [UnityMessage]
        private void Awake()
        {
            if (RegularShot == null)
            {
                throw new UnityException("Error: RegularShot prefab not specified in HackerPlayer.");
            }
        }

        /// <summary>
        /// Sets the starting player location.
        /// </summary>
        [UnityMessage]
        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            SetTerminal(TerminalType.Cameras);

            GameStateController.Instance.GameStarted += OnGameStarted;
        }

        private void OnGameStarted()
        {
			PowerCharge = MaxPowerCharge;
            SetTerminal(TerminalType.Cameras);
        }

        /// <summary>
        /// Head to next terminal.
        /// </summary>
        private void GoToNextTerminal()
        {
            switch (currentTerminal)
            {
                case TerminalType.Cameras:
                    SetTerminal(TerminalType.Doors);
                    break;
                case TerminalType.Doors:
                    SetTerminal(TerminalType.Traps);
                    break;
                case TerminalType.Traps:
                    SetTerminal(TerminalType.Cats);
                    break;
            }
        }

        /// <summary>
        /// Head to previous terminal.
        /// </summary>
        private void GoToPreviousTerminal()
        {
            switch ( currentTerminal )
            {
                case TerminalType.Doors:
                    SetTerminal(TerminalType.Cameras);
                    break;
                case TerminalType.Traps:
                    SetTerminal(TerminalType.Doors);
                    break;
                case TerminalType.Cats:
                    SetTerminal(TerminalType.Traps);
                    break;
            }
        }

        private void SetTerminal(TerminalType type)
        {
            currentTerminal = type;

            if(currentTerminalObject != null)
                currentTerminalObject.SelectedOverlay.SetActive(false);

            switch (type)
            {
                case TerminalType.Cameras:
                    currentTerminalObject = HackerInterface.Instance.TerminalCamera;
                    spriteRenderer.sprite = CameraHackerSprite;
                    break;
                case TerminalType.Doors:
                    currentTerminalObject = HackerInterface.Instance.TerminalDoors;
                    spriteRenderer.sprite = DoorHackerSprite;
                    break;
                case TerminalType.Traps:
                    currentTerminalObject = HackerInterface.Instance.TerminalTraps;
                    spriteRenderer.sprite = TrapHackerSprite;
                    break;
                case TerminalType.Cats:
                    currentTerminalObject = HackerInterface.Instance.TerminalCats;
                    spriteRenderer.sprite = CatHackerSprite;
                    break;
                default:
                    throw new NotSupportedException();
            }

            currentTerminalObject.SelectedOverlay.SetActive(true);
            transform.position = currentTerminalObject.HackerSpritePosition.transform.position;
        }

        /// <summary>
        /// This turns on the horizontal axis for the player for MoveRepeatTime number of seconds.
        /// </summary>
        /// <returns>IEnumerator</returns>
        private IEnumerator HorizontalAxisRepeatSwitch()
        {
            yield return new WaitForSeconds(MoveRepeatTime);
            cantMove = null;
        }

        /// <summary>
        /// Update the player this frame.
        /// </summary>
        [UnityMessage]
        private void Update()
        {
            float horizontal = Input.GetAxisRaw("HorizontalHackerAxis");

            if (cantMove == null)
            {
                if (horizontal > float.Epsilon)
                {
                    GoToNextTerminal();
                    TempStopMove();
                }
                else if (horizontal < -float.Epsilon)
                {
                    GoToPreviousTerminal();
                    TempStopMove();
                }
            }
            else if (Mathf.Abs(horizontal) <= float.Epsilon)
            {
                StopCoroutine(cantMove);
                cantMove = null;
            }
        
            if (Input.GetButtonDown("Fire1"))
            {
                if (shootingCoroutine != null)
                    StopCoroutine(shootingCoroutine);

                shootingCoroutine = StartCoroutine(FireRepeatedly());
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                if (shootingCoroutine != null)
                    StopCoroutine(shootingCoroutine);
            }

            PowerCharge += PowerChargeRate * Time.deltaTime;

            if (PowerCharge > MaxPowerCharge)
                PowerCharge = MaxPowerCharge;
        }

        private IEnumerator FireRepeatedly()
        {
            while (true)
            {
                ShootNormal();
                yield return new WaitForSeconds(FireRate);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        /// <summary>
        /// Shoots a normal shot.
        /// </summary>
        private void ShootNormal()
        {
            if (PowerCharge < PowerForShot)
                return;

            var shotInstance = Instantiate(RegularShot);

            var firstPosition = currentTerminalObject.PowerReader.GetComponent<PowerLevelIndicator>().StartPosition.position;

            shotInstance.transform.position = firstPosition;
            shotInstance.GetComponent<HackerShot>().ParentTerminal = currentTerminalObject;

            PowerCharge -= PowerForShot;
        }

        /// <summary>
        /// Puts a temporary stop in place for repeating keystrokes by starting HorizontalAxisRepeatSwitch()
        /// as a coroutine.
        /// </summary>
        private void TempStopMove()
        {
            cantMove = HorizontalAxisRepeatSwitch();
            StartCoroutine(cantMove);
        }
    }
}
