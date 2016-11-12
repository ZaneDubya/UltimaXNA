namespace UltimaXNA.Core.Data
{
    struct Pair<T>
    {
        public readonly T A;
        public readonly T B;

        public Pair(T a, T b)
        {
            A = a;
            B = b;
        }
    }
}
