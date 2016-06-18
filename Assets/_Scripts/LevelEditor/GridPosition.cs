namespace Assets._Scripts.LevelEditor
{
    public struct GridPosition
    {
        public bool Equals(GridPosition other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is GridPosition && Equals((GridPosition)obj);
        }

        public readonly int X;
        public readonly int Y;

        public GridPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }

        public override string ToString()
        {
            return "" + X + ", " + Y;
        }

        public static bool operator ==(GridPosition a, GridPosition b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(GridPosition a, GridPosition b)
        {
            return a.X != b.X || a.Y != b.Y;
        }
    }
}