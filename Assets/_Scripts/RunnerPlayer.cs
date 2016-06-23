using Assets._Scripts.GameObjects;
using FMOD.Studio;
using FMODUnity;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets._Scripts
{
    public class RunnerPlayer : MonoBehaviour
    {
		[EventRef, UsedImplicitly]
		public string SoundChewEventName = "event:/Runner_chew";

        private EventInstance chewSound;

		/// <summary>The full running speed of the running mouse in units per second.</summary>
        [AssignedInUnity]
        public float RunningSpeed = 2f;

        [AssignedInUnity]
        public bool PlayerMovementFrozen;

        /// <summary>This speed modifier is used for glue traps.</summary>
        [AssignedInUnity]
        public float GlueTrapSpeedModifier = .25f;

        /// <summary>The number of glue traps affecting player. Set by the traps.</summary>
        public int GlueTrapsAffectingPlayer { get; set; }

        [AssignedInUnity]
        public float InputDeadzoneSize = 0.01f;

        [AssignedInUnity]
        public float MaxDistanceForWallChewing = 0.5f;

        [AssignedInUnity]
        public GameObject ChewPrompt;

        [AssignedInUnity]
        public ChewProgressBar ChewProgressBar;

        [AssignedInUnity]
        public GameObject HackPrompt;

        [AssignedInUnity]
        public GameObject HackProjectilePrompt;

        [AssignedInUnity]
        public float CatHackDisableTime = 5;

		[AssignedInUnity]
		public Sprite Front;

		[AssignedInUnity]
		public Sprite Back;

		[AssignedInUnity]
		public Sprite Side;

        public Cat CurrentHackedCat { get; private set; }

        private SpriteRenderer spriteRenderer;
        private new Rigidbody2D rigidbody;

        private Vector2 lastRequestedMovement;
        private Vector2 requestedMovement;
#pragma warning disable 414
        private bool isChewingWall;
#pragma warning restore 414
        private float targettedWallChewTime;
        private float chewAccumulator;

        [UnityMessage]
        public void Start()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            rigidbody = GetComponent<Rigidbody2D>();

            chewSound = RuntimeManager.CreateInstance(SoundChewEventName);

            foreach (var obj in GetComponentsInChildren<Transform>())
            {
                obj.gameObject.layer = LevelLoader.RunnerLayer;
            }
        }
        
        [UnityMessage]
        private void Update()
        {
            if (!PlayerMovementFrozen)
                HandlePlayerMovement();

            const int layer = 2;

            var bottomOfSpritePosition = spriteRenderer.bounds.min;
            spriteRenderer.sortingOrder = InGameObject.GetSortPosition(bottomOfSpritePosition, layer);

            if (requestedMovement.IsZero() == false)
            {
                lastRequestedMovement = requestedMovement;
                var movementDirection = new Vector3().DirectionToDegrees(requestedMovement);
				if ( movementDirection < 0f )
					movementDirection += 360f;
				if ( ( movementDirection < 45f ) || ( movementDirection > 315f ) )
				{
					spriteRenderer.sprite = Side;
					spriteRenderer.flipX = true;
				}
				else if ( ( movementDirection >= 45f ) && ( movementDirection <= 135f ) )
				{
					spriteRenderer.sprite = Back;
				}
				else if ( ( movementDirection > 135f ) && ( movementDirection < 215f ) )
				{
					spriteRenderer.sprite = Side;
					spriteRenderer.flipX = false;
				}
				else
				{
					spriteRenderer.sprite = Front;
				}
                //spriteRenderer.transform.rotation = Quaternion.AngleAxis(movementDirection, Vector3.forward);

            }

            CheckForWallChewing();

            CheckForCatDisable();
        }

        private void CheckForCatDisable()
        {
            if (HackerInterface.Instance.TerminalCats.PowerReader.PowerLevel != 1)
            {
                HackPrompt.SetActive(false);
                return; // Touch cat disable is only power level 1
            }

            if (lastRequestedMovement.IsZero())
            {
                HackPrompt.SetActive(false);
                return;
            }

            var facingUnitVector = lastRequestedMovement.normalized;

            var results = Physics2D.RaycastAll(transform.position, facingUnitVector, MaxDistanceForWallChewing);
            if (results.Length == 0)
                return;

            GameObject nearbyCat = null;
            foreach (var result in results)
            {
                if (result.collider.gameObject == gameObject)
                    continue;

                if (result.collider.isTrigger)
                    continue;

                if (result.collider.gameObject.CompareTag("Cat"))
                    nearbyCat = result.collider.gameObject;
                break;
            }

            if (nearbyCat == null)
            {
                CatNotNearby();
            }
            else
            {
                var cat = nearbyCat.GetComponent<Cat>();
                CatNearby(cat);
            }
        }

        private void CatNearby(Cat cat)
        {
            HackPrompt.SetActive(true);

            if(Input.GetButtonDown("Hack"))
            {
                if (CurrentHackedCat != null)
                {
                    CurrentHackedCat.UnHack();
                }

                CurrentHackedCat = cat;
                cat.HackDisable(CatHackDisableTime); //TODO controlling
            }
        }

        private void CatNotNearby()
        {
            HackPrompt.SetActive(false);
        }

        private void CheckForWallChewing()
        {
            if (lastRequestedMovement.IsZero())
            {
				ChewProgressBar.gameObject.transform.parent.gameObject.SetActive(false);
                ChewProgressBar.gameObject.SetActive(false);
                ChewPrompt.gameObject.SetActive(false);
                return;
            }

            var facingUnitVector = lastRequestedMovement.normalized;

            var results = Physics2D.RaycastAll(transform.position, facingUnitVector, MaxDistanceForWallChewing);
            if (results.Length == 0)
            {
                ChewPrompt.gameObject.SetActive(false);
                StopChewing();
                return;
            }

            GameObject nearbyWall = null;
            foreach (var result in results)
            {
                if (result.collider.gameObject == gameObject)
                    continue;

                if (result.collider.isTrigger)
                    continue;
                
                if (result.collider.gameObject.CompareTag("Wall"))
                    nearbyWall = result.collider.gameObject;
                break;
            }

            if (nearbyWall == null)
            {
                WallNotNearby();
            }
            else
            {
                WallNearby(nearbyWall.GetComponent<Wall>());
            }
        }

        private void WallNearby(Wall wall)
        {
            if (wall.IsChewedThrough)
                return;

            ChewPrompt.SetActive(true);

            if (Input.GetButton("Chewing"))
            {
                Chew(wall);
            }
            else
            {
                StopChewing();
            }
        }

        private void Chew(Wall wall)
        {
            ChewProgressBar.gameObject.SetActive(true);
            ChewProgressBar.gameObject.transform.parent.gameObject.SetActive(true);

            PLAYBACK_STATE chewSoundState;
            chewSound.getPlaybackState(out chewSoundState);
            if (chewSoundState != PLAYBACK_STATE.PLAYING)
                chewSound.start();

            targettedWallChewTime = wall.GetWallInfo().TimeToChewThroughWall;
            chewAccumulator += Time.deltaTime;
            isChewingWall = true;
            ChewProgressBar.SetPercent(chewAccumulator / targettedWallChewTime);
            PlayerMovementFrozen = true;

            if (chewAccumulator >= targettedWallChewTime)
            {
                chewSound.stop(STOP_MODE.ALLOWFADEOUT);
                wall.SetEmpty();
                StopChewing();
            }
            else if (chewAccumulator >= (targettedWallChewTime / 2.0f))
            {
                wall.SetChewed();
            }
        }

        private void WallNotNearby()
        {
            StopChewing();

            ChewPrompt.SetActive(false);
        }

        private void StopChewing()
        {
            ChewProgressBar.gameObject.transform.parent.gameObject.SetActive(false);
            ChewProgressBar.gameObject.SetActive(false);
            chewAccumulator = 0;
            isChewingWall = false;
            PlayerMovementFrozen = false;
        }

        [UnityMessage]
        public void FixedUpdate()
        {
            if (requestedMovement.IsZero())
                return;

            var movement = requestedMovement * Time.deltaTime;
            rigidbody.MovePosition(transform.position + (Vector3)movement);
            rigidbody.AddForce( Vector2.zero ); // required for OnTriggerStay2D when player is not moving
        }
        
        private void HandlePlayerMovement()
        {
            var horizontalAxis = Input.GetAxisRaw("Horizontal");
            var verticalAxis = Input.GetAxisRaw("Vertical");

            if (horizontalAxis > -InputDeadzoneSize && horizontalAxis < InputDeadzoneSize)
                horizontalAxis = 0;

            if (verticalAxis > -InputDeadzoneSize && verticalAxis < InputDeadzoneSize)
                verticalAxis = 0;

            requestedMovement = new Vector2(horizontalAxis, verticalAxis);

            if (requestedMovement.sqrMagnitude > 1f)
                requestedMovement = requestedMovement.normalized;

            requestedMovement *= RunningSpeed;

            if(GlueTrapsAffectingPlayer > 0)
                requestedMovement *= GlueTrapSpeedModifier;
        }
    }
}
