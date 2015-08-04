namespace UltimaXNA.Ultima.Data
{
    static class Hues
    {
        public static int[] SkinTones
        {
            get
            {
                int max = 7 * 8;
                int[] hues = new int[max];
                for (int i = 0; i < max; i++)
                {
                    hues[i] = (i < 37) ? i + 1002 : i + 1003;
                }
                return hues;
            }
        }

        public static int[] HairTones
        {
            get
            {
                int max = 8 * 6;
                int[] hues = new int[max];
                for (int i = 0; i < max; i++)
                {
                    hues[i] = i + 1102;
                }
                return hues;
            }
        }

        public static int[] TextTones
        {
            get
            {
                int max = 1024;
                int[] hues = new int[max];
                for (int i = 0; i < max; i++)
                {
                    hues[i] = i+2;
                }
                return hues;
            }
        }
    }
}
