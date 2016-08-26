using System;
using System.Linq;
using Assets._Scripts.AI;
using FMOD.Studio;
using FMODUnity;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets._Scripts.GameObjects
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Cat : InGameObject
    {
        [EventRef, UsedImplicitly]
		public string SoundGrowlEventName = "event:/Cat_Growl";

        [EventRef, UsedImplicitly]
        public string SoundHissEventName = "event:/Cat_Hiss";

        [EventRef, UsedImplicitly]
        public string SoundMoveEventName = "event:/Cat_movement";

        [EventRef, UsedImplicitly]
        public string SoundDeathEventName = "event:/Mouse_death";

		private EventInstance moveSoundInstance;

        private bool walkSoundIsPlaying;

		public override int Layer { get { return 2; } }

        public override bool IsDynamic { get { return true; } }

        [AssignedInUnity]
        public float PatrolSpeed;

        [AssignedInUnity]
        public float ChaseSpeed;

        [AssignedInUnity]
        public float FieldOfView = 90;

        [AssignedInUnity]
        public float LengthOfView = 10;

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

        public MeshFilter MeshFilter { get; private set; }

        public CatAI AI { get; private set; }

        private new Rigidbody2D rigidbody;

        private float lastFieldOfView;
        private float lastLengthOfView;

        public int StartRotation { get; private set; }

        public Vector3 LastDesiredVelocity { get; private set; }

        private MeshRenderer fieldOfViewMesh;

        [UnityMessage]
        public void Start()
        {
            AI = new CatAI(this);

            rigidbody = GetComponent<Rigidbody2D>();
            MeshFilter = GetComponentInChildren<MeshFilter>();

            fieldOfViewMesh = GetComponentInChildren<MeshRenderer>();
            GenerateFieldOfViewMesh();

            moveSoundInstance = RuntimeManager.CreateInstance(SoundMoveEventName);
        }

        private void GenerateFieldOfViewMesh()
        {
            var halfFieldOfView = FieldOfView / 2.0f;
            var tanTheta = Mathf.Tan(halfFieldOfView * Mathf.Deg2Rad);
            var sinTheta = Mathf.Sin(halfFieldOfView * Mathf.Deg2Rad);
            
            var midpoint = new Vector3(LengthOfView, 0, 0);
            var height = LengthOfView * sinTheta;
            var lPrime = height / tanTheta;

            var top = new Vector3(lPrime, height, 0);
            var bottom = new Vector3(lPrime, -height, 0);

            var vertices = new[]
            {
                new Vector3(),
                top,
                midpoint, 
                bottom
            };

            var indices = new[] { 0, 1, 2, 0, 2, 3 };

            var uv = new[] { new Vector2(), new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1) };

            var mesh = new Mesh
            {
                vertices = vertices,
                triangles = indices,
                uv = uv
            };

            MeshFilter.mesh = mesh;

            fieldOfViewMesh.sortingOrder = 50000;
            fieldOfViewMesh.sortingLayerName = "RunnerOnTop";

            lastFieldOfView = FieldOfView;
            lastLengthOfView = LengthOfView;
        }

        [UnityMessage]
        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (AI.CurrentState is DisabledByRunner)
                    return; // Can't die to a disabled cat.
				RuntimeManager.PlayOneShot (SoundDeathEventName, transform.position);
                GameStateController.Instance.PlayerDied();
            }
        }

        [UnityMessage]
        public void Update()
        {
            SortObjectThatMoves();
            AI.Update();

            CheckFieldOfViewChangedForMesh();

            RotateCatBasedOnMovement();
            //CheckWalkSound();
        }

        private void RotateCatBasedOnMovement()
        {
            if (LastDesiredVelocity.IsZero())
                return;

            var movementDirection = transform.position.DirectionToDegrees(transform.position + LastDesiredVelocity); // kinda dumb don't care
            
            Turn(movementDirection);
        }

        public void Turn(Quaternion rotation)
        {
            Turn(Vector3.zero.DirectionToDegrees(rotation * Vector3.right));
        }

        public void Turn(float toDegrees)
        {
            SpriteRenderer target;
            var flip = false;

            toDegrees = toDegrees.NormalizeDegrees();

            if (toDegrees < 45)
            {
                target = Right;
                flip = true;
            }
            else if (toDegrees < 135)
            {
                target = Up;
            }
            else if (toDegrees < 225)
            {
                target = Left;
            }
            else if (toDegrees < 315)
            {
                target = Down;
            }
            else
            {
                target = Right;
                flip = true;
            }

            SpriteRenderer.sprite = target.sprite;
            SpriteRenderer.flipX = flip;
            SpriteRenderer.transform.localPosition = target.transform.localPosition;

            var rotationQuaternion = Quaternion.AngleAxis(toDegrees, Vector3.forward);
            fieldOfViewMesh.gameObject.transform.rotation = rotationQuaternion;
        }

        private void CheckWalkSound()
        {
            if (LastDesiredVelocity.IsZero())
            {
                StopWalkSound();
            }
            else
            {
                StartWalkSound();
            }
        }

        public void StartWalkSound()
        {
            if (walkSoundIsPlaying)
                return;

            moveSoundInstance.start();
            walkSoundIsPlaying = true;
        }

        public void StopWalkSound()
        {
            if (walkSoundIsPlaying == false)
                return;

            moveSoundInstance.stop(STOP_MODE.ALLOWFADEOUT);
            walkSoundIsPlaying = false;
        }

        private void CheckFieldOfViewChangedForMesh()
        {
            if (Math.Abs(lastFieldOfView - FieldOfView) > 0.001f || Math.Abs(lastLengthOfView - LengthOfView) > 0.001f)
            {
                GenerateFieldOfViewMesh();
            }
        }

        [UnityMessage]
        public void FixedUpdate()
        {
            AI.FixedUpdate();

            if (LastDesiredVelocity.IsZero())
            {
                rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }

        /// <summary>Magnitude is in units/second. See <see cref="PatrolSpeed"/>. Call this only in FixedUpdate.</summary>
        public void Move(Vector2 velocity)
        {
            LastDesiredVelocity = velocity;

            if (velocity.sqrMagnitude < 0.001f)
                return; //No movement

            var movement = velocity * Time.deltaTime;

            rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            
            rigidbody.MovePosition(transform.position + (Vector3)movement);
        }

        public override void GameStart()
        {
            AI.Start();
        }

        public override bool IsTraversableAt(GridPosition position)
        {
            // Treat cats is if they aren't there for pathfinding.
            return true;
        }

        public override void Deserialize(string serialized)
        {
            int rotation;
            try
            {
                rotation = Convert.ToInt32(serialized);
            }
            catch (FormatException)
            {
                rotation = 0;
            }

            StartRotation = rotation;
            transform.rotation = Quaternion.AngleAxis(rotation, Vector3.forward);
        }

        public override void PostAllDeserialized()
        {
            // O(n^2)
            var allOtherCats = LevelLoader.AllInGameObjects.OfType<Cat>().Where(x => x != this).ToList();

            var thisCollider = GetComponent<Collider2D>();

            foreach (var otherCat in allOtherCats)
            {
                var otherCollider = otherCat.GetComponent<Collider2D>();

                Physics2D.IgnoreCollision(thisCollider, otherCollider);
            }

            SpriteRenderer = MainSpriteRenderer;
        }

        public void HackDisable(float time)
        {
            AI.GetState<DisabledByRunner>().SetTime(time);
            AI.SetState<DisabledByRunner>();
        }

        public void UnHack()
        {
            AI.GetState<DisabledByRunner>().EnableCat();
        }

        public void HideFieldOfViewMesh()
        {
            fieldOfViewMesh.gameObject.SetActive(false);
        }

        public void ShowFieldOfViewMesh()
        {
            fieldOfViewMesh.gameObject.SetActive(true);
        }
    }
}