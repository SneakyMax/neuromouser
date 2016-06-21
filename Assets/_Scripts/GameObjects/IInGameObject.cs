namespace Assets._Scripts.GameObjects
{
    public interface IInGameObject
    {
        LevelLoader LevelLoader { get; set; }

        /// <summary>Drawing layer 0 = floor, 1 = on floor, 2 = mid-level, 3 = ceiling</summary>
        int Layer { get; }

        /// <summary>If the object is aligned with the grid, this is its position in the grid.</summary>
        GridPosition? StartGridPosition { get; set; }

        /// <summary>A dynamic object is for pathfinding. Generally it means that the object moves. If overridden to true, also override IsTraversable.</summary>
        bool IsDynamic { get; }

        int Id { get; set; }

        void Deserialize(string serialized);

        void Initialize();

        void PostAllDeserialized();

        void GameStart();

        /// <summary>Override this if <see cref="IsDynamic"/> is true. Return whether an object can move through the grid position indicated. E.g. doors will be traversable if they can move.</summary>
        bool IsTraversableAt(GridPosition position);
    }
}