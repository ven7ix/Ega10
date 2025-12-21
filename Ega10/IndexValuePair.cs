namespace Ega10
{
    internal struct IndexValuePair(int index, double value) : IComparable<IndexValuePair>
    {
        public int Index { get; private set; } = index;
        public double Value { get; set; } = value;

        public readonly int CompareTo(IndexValuePair other)
        {
            return Value.CompareTo(other.Value);
        }
    }
}
