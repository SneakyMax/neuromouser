namespace Assets._Scripts.GameObjects
{
    public class TrapAlarm : InGameObject
    {
        public override int Layer { get { return 2; } }

        public override bool IsDynamic { get { return true; } }

        public override bool IsTraversableAt(GridPosition position)
        {
            return true;
        }
    }
}