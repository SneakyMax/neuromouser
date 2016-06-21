using System;
using System.Linq;
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

        private new Rigidbody2D rigidbody;
        private CatAI catAI;

        [UnityMessage]
        public void Start()
        {
            catAI = new CatAI(this);

            rigidbody = GetComponent<Rigidbody2D>();
        }

        [UnityMessage]
        public void Update()
        {
            SortObjectThatMoves();
            catAI.Update();
        }

        [UnityMessage]
        public void FixedUpdate()
        {
            catAI.FixedUpdate();
        }

        /// <summary>Moves the cat in <see cref="direction"/> at <see cref="speed"/> speed. Speed is in units/second. See <see cref="PatrolSpeed"/>. Call this only in FixedUpdate.</summary>
        public void Move(Vector2 direction, float speed)
        {
            if (direction.sqrMagnitude < 0.000001f)
                return; //Can't normalize zero vector

            Move(direction.normalized * speed);
        }

        public void Move(Vector2 velocity)
        {
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