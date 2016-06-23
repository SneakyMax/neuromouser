using UnityEngine;

namespace Assets._Scripts
{
    /// <summary>
    /// Hacker-mouse's interface.
    /// </summary>
    [UnityComponent]
    public class HackerInterface : MonoBehaviour
    {
        /// <summary>
        /// This delegate lets a script know that the terminal power has changed.
        /// </summary>
        /// <param name="terminalPower">The current power level of the terminal</param>
        public delegate void TerminalPowerChanged(int terminalPower);

        /// <summary>
        /// This event fires when the camera terminal power has changed.
        /// </summary>
        public event TerminalPowerChanged OnCameraPowerChanged;

        /// <summary>
        /// This event fires when the trap terminal power has changed.
        /// </summary>
        public event TerminalPowerChanged OnTrapPowerChanged;

        /// <summary>
        /// This event fires when the door terminal power has changed.
        /// </summary>
        public event TerminalPowerChanged OnDoorPowerChanged;

        /// <summary>
        /// This event fires when the cat terminal power has changed.
        /// </summary>
        public event TerminalPowerChanged OnCatPowerChanged;

        /// <summary>
        /// The singleton instance of the interface.
        /// </summary>
        public static HackerInterface Instance { get; private set; }

        [AssignedInUnity]
        public Camera RunnerCamera;

        [AssignedInUnity]
        public float Level0CameraZoom = 1f;

        [AssignedInUnity]
        public float Level1CameraZoom = 2f;

        [AssignedInUnity]
        public float Level2CameraZoom = 3f;

        [AssignedInUnity]
        public float Level3CameraZoom = 4f;
        
        [AssignedInUnity]
        public HackerTerminal TerminalCamera;
        
        [AssignedInUnity]
        public HackerTerminal TerminalTraps;
        
        [AssignedInUnity]
        public HackerTerminal TerminalDoors;
        
        [AssignedInUnity]
        public HackerTerminal TerminalCats;

		[AssignedInUnity]
		public TimerDevice LevelTimer;

        /// <summary>
        /// Called when the script is loaded
        /// </summary>
        /// <exception cref="UnityException">Thrown if any terminal or the camera is unassociated, or if
        /// the camera is non-orthographic.</exception>
        [UnityMessage]
        private void Awake()
        {
            Instance = this;

            if ((TerminalCamera == null) || (TerminalTraps == null) || (TerminalDoors == null) || (TerminalCats == null))
                throw new UnityException("Error: Terminals not set up with HackerInterface!");

			if ( LevelTimer == null )
				throw new UnityException("Error: LevelTimer not set up with HackerInterface!");

            if (RunnerCamera == null)
                throw new UnityException("Error: HackerInterface Runner must have an associated camera!");

            if (RunnerCamera.orthographic == false)
                throw new UnityException("Error: RunnerCamera must be set to orthographic.");
        }

        /// <summary>
        /// Called when the game starts. Sets up the power changed events.
        /// </summary>
        [UnityMessage]
        private void Start()
        {
            TerminalCamera.OnPowerChanged += OnCameraPowerChange;
            TerminalTraps.OnPowerChanged += OnTrapPowerChange;
            TerminalDoors.OnPowerChanged += OnDoorPowerChange;
            TerminalCats.OnPowerChanged += OnCatPowerChange;
			LevelTimer.TimerZero += OnTimerZero;

            LevelLoader.Instance.AddPostLevelLoadAction(ClearEvents);
        }

        private void ClearEvents()
        {
            OnCameraPowerChanged = null;
            OnDoorPowerChanged = null;
            OnCatPowerChanged = null;
            OnTrapPowerChanged = null;
        }

		public void OnTimerZero()
		{
			GameStateController.Instance.PlayerDied();
		}

        /// <summary>
        /// Raises the camera power changed event, changing the orthographic size of the camera.
        /// </summary>
        /// <param name="currentPower">Current camera terminal power.</param>
        public void OnCameraPowerChange(int currentPower)
        {
            if (OnCameraPowerChanged != null)
            {
                OnCameraPowerChanged(currentPower);
            }
        }

        /// <summary>
        /// Raises the trap power changed event.
        /// </summary>
        /// <param name="currentPower">Current trap terminal power.</param>
        public void OnTrapPowerChange(int currentPower)
        {
            if (OnTrapPowerChanged != null)
            {
                OnTrapPowerChanged(currentPower);
            }
        }

        /// <summary>
        /// Raises the door power changed event.
        /// </summary>
        /// <param name="currentPower">Current door terminal power.</param>
        public void OnDoorPowerChange(int currentPower)
        {
            if (OnDoorPowerChanged != null)
            {
                OnDoorPowerChanged(currentPower);
            }
        }

        /// <summary>
        /// Raises the cat power changed event.
        /// </summary>
        /// <param name="currentPower">Current cat terminal power.</param>
        public void OnCatPowerChange(int currentPower)
        {
            if (OnCatPowerChanged != null)
            {
                OnCatPowerChanged(currentPower);
            }
        }
    }
}
