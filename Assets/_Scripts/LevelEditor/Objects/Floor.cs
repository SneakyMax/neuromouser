namespace Assets._Scripts.LevelEditor.Objects
{
    [UnityComponent]
    public class Floor : PlacedObject
    {
        private static readonly int[] layers = { 0 };
        public override int[] Layers { get { return layers; } }
    }
}