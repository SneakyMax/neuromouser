using System;
using Assets._Scripts.AI;
using UnityEngine;

namespace Assets._Scripts.GameObjects
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Cat : InGameObject
    {
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

        private new Rigidbody2D rigidbody;
        private CatAI catAI;

        private float lastFieldOfView;
        private float lastLengthOfView;

        public Vector3 LastDesiredVelocity { get; private set; }

        [UnityMessage]
        public void Start()
        {
            catAI = new CatAI(this);

            rigidbody = GetComponent<Rigidbody2D>();
            MeshFilter = GetComponentInChildren<MeshFilter>();

            GenerateFieldOfViewMesh();
        }

        private void GenerateFieldOfViewMesh()
        {
            var triHeight = Mathf.Tan(FieldOfView * Mathf.Deg2Rad / 2.0f) * LengthOfView;

            var vertices = new[]
            {
                new Vector3(),
                new Vector3(LengthOfView, triHeight, 0),
                new Vector3(LengthOfView, -triHeight, 0)
            };

            var indices = new[] { 0, 1, 2 };

            var uv = new[] { new Vector2(), new Vector2(1, 1), new Vector2(1, 1) };

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
        public void Update()
        {
            SortObjectThatMoves();
            catAI.Update();

            CheckFieldOfViewChangedForMesh();
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
            catAI.FixedUpdate();
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
            catAI.Start();
        }

        public override bool IsTraversableAt(GridPosition position)
        {
            // Treat cats is if they aren't there for pathfinding.
            return true;
        }
    }
}