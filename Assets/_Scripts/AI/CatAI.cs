using System;
using System.Collections.Generic;
using System.Linq;
using Assets._Scripts.GameObjects;

namespace Assets._Scripts.AI
{
    public class CatAI
    {
        public Cat Cat { get; private set; }

        private readonly IList<CatAIState> states;

        private CatAIState currentState;

        public CatAI(Cat cat)
        {
            Cat = cat;

            states = new List<CatAIState>();

            AddState<ChasingRunner>();
            AddState<Idle>();
            AddState<DisabledByCatnip>();
            AddState<Patrolling>();
        }

        private void AddState<T>() where T : CatAIState, new()
        {
            var state = new T { CatAI = this };

            states.Add(state);
        }

        public void SetState<T>() where T : CatAIState
        {
            var newState = GetState<T>();

            if (currentState != null)
            {
                currentState.Exit();
                currentState.IsActive = false;
            }

            currentState = newState;

            currentState.IsActive = true;
            currentState.Enter();
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
                GetState<Patrolling>().SetStartPatrolNode(possiblePatrolPoint);
                SetState<Patrolling>();
            }
            else
            {
                SetState<Idle>();
            }
        }

        public void Update()
        {
            if (currentState != null)
            {
                currentState.Update();
            }
        }

        public void FixedUpdate()
        {
            if (currentState != null)
            {
                currentState.FixedUpdate();
            }
        }
    }
}