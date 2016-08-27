using Assets._Scripts.AI;
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

        [AssignedInUnity, Header("Sprites")]
        public SpriteRenderer MainSpriteRenderer;

        [AssignedInUnity]
        public SpriteRenderer Left;

        [AssignedInUnity]
        public SpriteRenderer Right;

        [AssignedInUnity]
        public SpriteRenderer Up;

        [AssignedInUnity]
        public SpriteRenderer Down;

        [AssignedInUnity]
        public GameObject HackingProbePrefab;

        public Cat CurrentHackedCat { get; private set; }

        private GameObject currentProbe;
        
        private new Rigidbody2D rigidbody;

        private Vector2 lastRequestedMovement;
        private Vector2 requestedMovement;
#pragma warning disable 414
        private bool isChewingWall;
#pragma warning restore 414
        private float targettedWallChewTime;
        private float chewAccumulator;
        private Wall lastFacingWall;

        [UnityMessage]
        public void Start()
        {
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

            CheckSprite();

            CheckForWallChewing();

            CheckForCatDisable();
        }

        private void CheckSprite()
        {
            const int layer = 2;

            var spriteRenderer = MainSpriteRenderer;
            
            if (requestedMovement.IsZero() == false)
            {
                var flip = false;
                SpriteRenderer target;

                lastRequestedMovement = requestedMovement;
                var movementDirection = requestedMovement.VectorDirectionDegrees().NormalizeDegrees();
                
                if ((movementDirection < 45f) || (movementDirection > 315f))  // Right
                {
                    target = Right;
                    flip = true;
                }
                else if ((movementDirection >= 45f) && (movementDirection <= 135f)) // Up
                {
                    target = Up;
                }
                else if ((movementDirection > 135f) && (movementDirection < 215f)) // Left
                {
                    target = Left;
                }
                else // Down
                {
                    target = Down;
                }

                spriteRenderer.sprite = target.sprite;
                spriteRenderer.flipX = flip;
                spriteRenderer.transform.localPosition = target.transform.localPosition;
            }

            var bottomOfSpritePosition = spriteRenderer.bounds.min;
            spriteRenderer.sortingOrder = InGameObject.GetSortPosition(bottomOfSpritePosition, layer);
        }

        private void CheckForCatDisable()
        {
            if (HackerInterface.Instance.TerminalCats.PowerReader.PowerLevel == 1)
            {
                CheckCatTouchDisable();
            }
            else
            {
                HackPrompt.SetActive(false);
            }


            if (HackerInterface.Instance.TerminalCats.PowerReader.PowerLevel >= 2)
            {
                CheckHackingProbe();
            }
            else
            {
                HackProjectilePrompt.SetActive(false);
            }
        }

        private void CheckHackingProbe()
        {
            HackProjectilePrompt.SetActive(true);

            if (Input.GetButtonDown("Hack") == false || currentProbe != null)
                return;

            currentProbe = Instantiate(HackingProbePrefab);
            
            currentProbe.transform.SetParent(transform.parent, false);
            currentProbe.transform.position = transform.position;

            currentProbe.layer = LevelLoader.RunnerLayer;

            var probe = currentProbe.GetComponent<HackingProbe>();
            probe.SetInfo(this, lastRequestedMovement.normalized);

            probe.OnCollidedWithCat += HackCat;
        }

        private void CheckCatTouchDisable()
        {
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
                HackCat(cat);
            }
        }

        private void HackCat(Cat cat)
        {
            if (cat.AI.CurrentState is ChasingRunner && HackerInterface.Instance.TerminalCats.PowerReader.PowerLevel < 3)
            {
                return; // Can only hack cats that are chasing you at level 3
            }

            if (CurrentHackedCat != null)
            {
                CurrentHackedCat.UnHack();
            }

            CurrentHackedCat = cat;
            cat.HackDisable(CatHackDisableTime); //TODO controlling
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
                WallNotNearby();
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
			if (wall.IsChewedThrough || wall.WallType == LevelEditor.Objects.WallType.Glass)
			{
				StopChewing();
				PlayerMovementFrozen = false;
				return;
			}

            if (lastFacingWall != null && lastFacingWall != wall)
            {
                lastFacingWall.UnsetHighlighted();
            }

            lastFacingWall = wall;
            wall.SetHighlighted();

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
            if (lastFacingWall != null)
                lastFacingWall.UnsetHighlighted();

            StopChewing();

            ChewPrompt.SetActive(false);
        }

        private void StopChewing()
        {
            chewSound.stop(STOP_MODE.ALLOWFADEOUT);
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

			if ( PlayerMovementFrozen )
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
