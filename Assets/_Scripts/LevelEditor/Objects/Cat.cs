namespace Assets._Scripts.LevelEditor.Objects
{
    public class Cat : PlacedObject
    {
        private static readonly int[] layers = { 1, 2 }; 
        public override int[] Layers { get { return layers; } }
    }
}