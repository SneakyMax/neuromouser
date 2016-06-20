namespace Assets._Scripts.GameObjects
{
    public interface IInGameObject
    {
        LevelLoader LevelLoader { get; set; }

        /// <summary>Drawing layer 0 = floor, 1 = on floor, 2 = mid-level, 3 = ceiling</summary>
        int Layer { get; }

        void Deserialize(string serialized);

        void Initialize();
    }
}