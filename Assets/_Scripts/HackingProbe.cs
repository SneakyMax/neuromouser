using System;
using Assets._Scripts.GameObjects;
using UnityEngine;

namespace Assets._Scripts
{
    [UnityComponent]
    public class HackingProbe : MonoBehaviour
    {
        public event Action<Cat> OnCollidedWithCat;

        [AssignedInUnity]
        public float Speed;

        private RunnerPlayer parent;
        private Vector2 desiredVelocity;

        private new Rigidbody2D rigidbody;
        private SpriteRenderer spriteRenderer;

        [UnityMessage]
        public void Start()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), parent.GetComponent<Collider2D>());
            spriteRenderer = GetComponent<SpriteRenderer>();

            Destroy(gameObject, 3);
        }

        public void SetInfo(RunnerPlayer player, Vector2 direction)
        {
            parent = player;
            desiredVelocity = direction.normalized * Speed;
        }

        [UnityMessage]
        public void Update()
        {
            var bottomOfSpritePosition = spriteRenderer.bounds.min;
            spriteRenderer.sortingOrder = InGameObject.GetSortPosition(bottomOfSpritePosition, 2);

            var movementDirection = Vector3.zero.DirectionToDegrees(desiredVelocity);
            transform.rotation = Quaternion.AngleAxis(movementDirection, Vector3.forward);
        }

        [UnityMessage]
        public void FixedUpdate()
        {
            rigidbody.MovePosition(transform.position + ((Vector3)desiredVelocity * Time.deltaTime));
        }

        [UnityMessage]
        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Cat"))
            {
                if (OnCollidedWithCat != null)
                    OnCollidedWithCat(collision.gameObject.GetComponent<Cat>());
            }

            Destroy(gameObject);
        }
    }
}