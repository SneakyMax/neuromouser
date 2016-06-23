﻿using System;
using System.Linq;
using Assets._Scripts.AI;
using UnityEngine;

namespace Assets._Scripts.GameObjects
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Cat : InGameObject
    {
		[FMODUnity.EventRef]
		FMOD.Studio.PLAYBACK_STATE  playbackState;
		public string moveSound = "event:/Cat_movement";
		FMOD.Studio.EventInstance moveSoundInstance;
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

        public MeshFilter MeshFilter { get; private set; }

        public CatAI AI { get; private set; }

        private new Rigidbody2D rigidbody;

        private float lastFieldOfView;
        private float lastLengthOfView;

        public int StartRotation { get; private set; }

        public Vector3 LastDesiredVelocity { get; private set; }

        [UnityMessage]
        public void Start()
        {
            AI = new CatAI(this);

            rigidbody = GetComponent<Rigidbody2D>();
            MeshFilter = GetComponentInChildren<MeshFilter>();

            GenerateFieldOfViewMesh();

			moveSoundInstance = FMODUnity.RuntimeManager.CreateInstance (moveSound);
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

            GetComponentInChildren<MeshRenderer>().sortingOrder = 50000;
            GetComponentInChildren<MeshRenderer>().sortingLayerName = "RunnerOnTop";

            lastFieldOfView = FieldOfView;
            lastLengthOfView = LengthOfView;
        }

        [UnityMessage]
        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                GameStateController.Instance.PlayerDied();
            }
        }

        [UnityMessage]
        public void Update()
        {
            SortObjectThatMoves();
            AI.Update();

            CheckFieldOfViewChangedForMesh();

			if (!walkSoundIsPlaying) 
			{
				moveSoundInstance.start ();
				walkSoundIsPlaying = true;
			}
			if (walkSoundIsPlaying) 
			{
				moveSoundInstance.getPlaybackState (out playbackState);
				if (playbackState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
				{
					walkSoundIsPlaying = false;
				}
			}
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
        }

        /// <summary>Magnitude is in units/second. See <see cref="PatrolSpeed"/>. Call this only in FixedUpdate.</summary>
        public void Move(Vector2 velocity)
        {
            LastDesiredVelocity = velocity;

            if (velocity.sqrMagnitude < 0.001f)
                return; //No movement

            var movement = velocity * Time.deltaTime;
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
        }
    }
}