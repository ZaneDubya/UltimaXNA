namespace UltimaXNA.UltimaEntities
{
    public struct CurrentMaxValue
    {
        public int Current;
        public int Max;

        CurrentMaxValue(int current, int max)
        {
            Current = current;
            Max = max;
        }

        public void Update(int current, int max)
        {
            Current = current;
            Max = max;
        }

        public override string ToString()
        {
            return string.Format("{0} / {1}", Current, Max);
        }
    }
}