using System;
using System.Collections.Generic;
using System.Linq;
using Assets._Scripts.GameObjects;
using UnityEngine;

namespace Assets._Scripts.AI
{
    public class CatAI
    {
        public const float ReachedPositionThreshold = 0.1f;

        public Cat Cat { get; private set; }

        public bool IsPatroller { get; private set; }

        private readonly IList<CatAIState> states;

        public CatAIState CurrentState { get; private set; }

        public CatAIState PreviousState { get; private set; }

        public CatAIState StartingState { get; private set; }

        public CatAI(Cat cat)
        {
            Cat = cat;

            states = new List<CatAIState>();

            AddState<ChasingRunner>();
            AddState<Idle>();
            AddState<DisabledByRunner>();
            AddState<Patrolling>();
            AddState<InvestigatingAlarm>();
            AddState<AttractedToCatnip>();
        }

        public void ReturnToDefaultState()
        {
            SetState(StartingState);
        }

        private void AddState<T>() where T : CatAIState, new()
        {
            var state = new T { AI = this };

            states.Add(state);
            state.Init();
        }

        public void SetState<T>() where T : CatAIState
        {
            var newState = GetState<T>();
            SetState(newState);
        }

        public void SetState(CatAIState newState)
        {
            if (CurrentState == newState)
                return;

            if (CurrentState != null)
            {
                if (CurrentState.AllowStateChangeFrom() == false)
                    return;

                CurrentState.Exit();
                CurrentState.IsActive = false;
            }

            PreviousState = CurrentState;
            CurrentState = newState;

            CurrentState.IsActive = true;
            CurrentState.Enter();
        }

        public T GetState<T>() where T : CatAIState
        {
            return states.OfType<T>().FirstOrDefault();
        }

        public void Start()
        {
            if (Cat.StartGridPosition == null)
                throw new InvalidOperationException("Cat is not on grid??");

            var possiblePatrolPoint = Cat.LevelLoader.GetGridObjectsThatStartedAtPosition(Cat.StartGridPosition.Value).OfType<PatrolPoint>().FirstOrDefault();

            if (possiblePatrolPoint != null)
            {
                IsPatroller = true;
                GetState<Patrolling>().SetStartPatrolNode(possiblePatrolPoint);
                SetState<Patrolling>();
            }
            else
            {
                IsPatroller = false;
                SetState<Idle>();
            }

            StartingState = CurrentState;
        }

        public RunnerPlayer CheckFieldOfViewForMouse()
        {
            const int fanPoints = 5;

            RunnerPlayer player = null;

            for (var i = 0; i <= fanPoints; i++)
            {
                // Half field of view, divide by the number of fan points, spread out from the center with i
                var angle = Cat.FieldOfView / 2.0f / fanPoints * i;
                var unitVectorDirectionFacing = Cat.transform.rotation * Vector3.right;

                var left = Quaternion.AngleAxis(-angle, Vector3.forward);
                var right = Quaternion.AngleAxis(angle, Vector3.forward);

                var leftVector = left * unitVectorDirectionFacing;
                var rightVector = right * unitVectorDirectionFacing;

                var leftResults = Physics2D.RaycastAll(Cat.transform.position, leftVector, Cat.LengthOfView);
                var rightResults = Physics2D.RaycastAll(Cat.transform.position, rightVector, Cat.LengthOfView);

                // Check left, skip cat
                foreach (var result in leftResults)
                {
                    if (result.collider.gameObject == Cat.gameObject)
                        continue;

                    if (result.collider.isTrigger)
                        continue;

                    if (result.collider.gameObject.CompareTag("Player"))
                    {
                        player = result.collider.gameObject.GetComponent<RunnerPlayer>();
                    }
                    break;
                }

                // Check right, skip cat
                foreach (var result in rightResults)
                {
                    if (result.collider.gameObject == Cat.gameObject)
                        continue;

                    if (result.collider.isTrigger)
                        continue;

                    if (result.collider.gameObject.CompareTag("Player"))
                    {
                        player = result.collider.gameObject.GetComponent<RunnerPlayer>();
                    }
                    break;
                }

                if (player != null)
                    break;
            }

            return player;
        }

        private void LineOfSightLines()
        {
            var angle = Cat.FieldOfView / 2.0f;
            var unitVectorDirectionFacing = Cat.transform.rotation * Vector3.right;

            var left = Quaternion.AngleAxis(-angle, Vector3.forward);
            var right = Quaternion.AngleAxis(angle, Vector3.forward);

            var leftVector = left * unitVectorDirectionFacing * Cat.LengthOfView;
            var rightVector = right * unitVectorDirectionFacing * Cat.LengthOfView;

            var start = Cat.transform.position + new Vector3(0, 0, -1);

            var line = new []
            {
                start,
                start + leftVector,
                start + rightVector,
                start
            };

            var lineRenderer = Cat.GetComponent<LineRenderer>();
            lineRenderer.SetVertexCount(4);
            lineRenderer.SetPositions(line);
            lineRenderer.sortingLayerName = "RunnerOnTop";
            lineRenderer.SetWidth(0.1f, 0.1f);
        }

        public void Update()
        {
            RotateCatBasedOnMovement();

            if (CurrentState != null)
            {
                CurrentState.Update();
            }
        }

        private void RotateCatBasedOnMovement()
        {
            if (Cat.LastDesiredVelocity.IsZero())
                return;

            var movementDirection = Cat.transform.position.DirectionToDegrees(Cat.transform.position + Cat.LastDesiredVelocity); // kinda dumb don't care
            var rotationQuaternion = Quaternion.AngleAxis(movementDirection, Vector3.forward);
            Cat.transform.rotation = rotationQuaternion;
        }

        public void FixedUpdate()
        {
            if (CurrentState != null)
            {
                CurrentState.FixedUpdate();
            }
        }
    }
}