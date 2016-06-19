namespace Assets._Scripts.LevelEditor.Objects
{
    [UnityComponent]
    public class PlayerSpawn : PlacedObject
    {
        private static readonly int[] layers = { 4 };
        public override int[] Layers { get { return layers; } }
    }
}