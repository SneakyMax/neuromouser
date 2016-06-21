using Assets._Scripts.GameObjects;

namespace Assets._Scripts.AI
{
    public abstract class CatAIState
    {
        public Cat Cat {get { return CatAI.Cat; } }

        public CatAI CatAI { get; set; }

        public bool IsActive { get; set; }

        public virtual void Enter()
        {
            
        }

        public virtual void Exit()
        {
            
        }

        public virtual void Update()
        {
            
        }

        public virtual void FixedUpdate()
        {
            
        }
    }
}