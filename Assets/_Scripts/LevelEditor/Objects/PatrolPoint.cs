namespace Assets._Scripts.LevelEditor.Objects
{
    public class PatrolPoint : PlacedObject
    {
        private static readonly int[] layers = { 4 };
        public override int[] Layers { get { return layers; } }
    }
}