namespace Assets._Scripts.LevelEditor
{
    public interface IPlacedObject
    {
        string Type { get; }

        string Serialize();
    }
}