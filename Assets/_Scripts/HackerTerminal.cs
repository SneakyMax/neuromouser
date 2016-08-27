using UnityEngine;

namespace Assets._Scripts
{
    public class HackerTerminal : MonoBehaviour
    {
        /// <summary>
        /// Used for calling when the power from the terminal is changed.
        /// </summary>
        public delegate void TerminalPowerChange(int currentPower);

        /// <summary>
        /// This event fires when the power from the terminal is changed.
        /// Note: I recommend tying into this during Start() instead of OnEnable() to eliminate
        /// any null reference issues. -Joe
        /// </summary>
        public event TerminalPowerChange OnPowerChanged;

        /// <summary>
        /// Gets the amount of power allocated to the terminal.
        /// </summary>
        /// <value>The terminal power.</value>
        public int TerminalPower
        {
            get
            {
                return terminalPower;
            }
        }

        /// <summary>
        /// PowerToggle level 1 instance.
        /// </summary>
        [AssignedInUnity]
        public PowerToggle PowerToggle1;

        /// <summary>
        /// PowerToggle level 2 instance.
        /// </summary>
        [AssignedInUnity]
        public PowerToggle PowerToggle2;

        /// <summary>
        /// PowerToggle level 3 instance.
        /// </summary>
        [AssignedInUnity]
        public PowerToggle PowerToggle3;

        /// <summary>
        /// The ICE instance.
        /// </summary>
        [AssignedInUnity]
        public ICEHandler PowerReader;

        [AssignedInUnity]
        public Transform HackerSpritePosition;

        [AssignedInUnity]
        public GameObject SelectedOverlay;

        [AssignedInUnity]
        public ParticleSystem[] Particles;

        /// <summary>
        /// The amount of power allocated to the terminal.
        /// </summary>
        private int terminalPower;

        /// <summary>
        /// Does basic error checking.
        /// </summary>
        /// <exception cref="UnityException">Throws if instances aren't set.</exception>
        [UnityMessage]
        private void Awake()
        {
            if (PowerReader == null)
            {
                throw new UnityException("PowerReader not set for terminal");
            }
        }

        /// <summary>
        /// Sets up the callback.
        /// </summary>
        [UnityMessage]
        private void Start()
        {
            PowerReader.OnPowerLevelChange += OnPowerLevelChange;
            SelectedOverlay.SetActive(false);

            GameStateController.Instance.GameStarted += OnGameStarted;
        }

        private void OnGameStarted()
        {
            PowerReader.Reset();
        }

        /// <summary>
        /// Handles the OnPowerChange event
        /// </summary>
        /// <param name="newPower">New power value.</param>
        /// <exception cref="UnityException">If newPower is not value from 0-3.</exception>
        private void OnPowerLevelChange(int newPower)
        {
            terminalPower = newPower;

            if (OnPowerChanged != null)
                OnPowerChanged(terminalPower);

            if (PowerToggle1 == null || PowerToggle2 == null || PowerToggle3 == null)
                return;
        
            switch (newPower)
            {
                case 0:
                    PowerToggle1.PowerOnStatus = false;
                    PowerToggle2.PowerOnStatus = false;
                    PowerToggle3.PowerOnStatus = false;
                    break;
                case 1:
                    PowerToggle1.PowerOnStatus = true;
                    PowerToggle2.PowerOnStatus = false;
                    PowerToggle3.PowerOnStatus = false;
                    break;
                case 2:
                    PowerToggle1.PowerOnStatus = true;
                    PowerToggle2.PowerOnStatus = true;
                    PowerToggle3.PowerOnStatus = false;
                    break;
                case 3:
                    PowerToggle1.PowerOnStatus = true;
                    PowerToggle2.PowerOnStatus = true;
                    PowerToggle3.PowerOnStatus = true;
                    break;
                default:
                    throw new UnityException("Terminal OnPowerChange received invalid power value.");
            }
        }
    }
}
