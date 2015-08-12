using System;
using UltimaXNA.Ultima.Resources;

namespace ExamplePlugin
{
    class WebSafeHueCreator
    {
        public static void CreateHues()
        {
            uint[] hues = HueData.GetAllHues();

            uint[] toMatches = new uint[216];
            for (int b = 0; b < 6; b++)
                for (int g = 0; g < 6; g++)
                    for (int r = 0; r < 6; r++)
                        toMatches[r + g * 6 + b * 36] = (uint)(0x000033 * r + 0x003300 * g + 0x330000 * b);

            Tuple<int, double>[] matches = new Tuple<int, double>[216];

            for (int i = 0; i < 216; i++)
            {
                int nearestHue = -1;
                var distance = double.MaxValue;
                var dbl_input_red = toMatches[i] & 0x0000ff;
                var dbl_input_green = (toMatches[i] & 0x00ff00) >> 8;
                var dbl_input_blue = (toMatches[i] & 0xff0000) >> 16;
                for (int j = 0; j < HueData.HueCount; j++)
                {
                    // compute the Euclidean distance between the two colors
                    // note, that the alpha-component is not used in this example
                    var dbl_test_red = Math.Pow(Convert.ToDouble(hues[j] & 0x0000ff) - dbl_input_red, 2.0);
                    var dbl_test_green = Math.Pow(Convert.ToDouble((hues[j] & 0x00ff00) >> 8) - dbl_input_green, 2.0);
                    var dbl_test_blue = Math.Pow(Convert.ToDouble((hues[j] & 0xff0000) >> 16) - dbl_input_blue, 2.0);

                    var temp = Math.Sqrt(dbl_test_blue + dbl_test_green + dbl_test_red);
                    // explore the result and store the nearest color
                    if (temp == 0.0)
                    {
                        nearestHue = j;
                        distance = 0;
                        break;
                    }
                    else if (temp < distance)
                    {
                        distance = temp;
                        nearestHue = j;
                    }
                }
                matches[i] = new Tuple<int, double>(nearestHue, distance);
            }
            string m_kWebSafeHues = "static int[] m_kWebSafeHues = new int[216] {";
            for (int i = 0; i < 36; i++)
            {
                m_kWebSafeHues += "\n            ";
                for (int j = 0; j < 6; j++)
                    m_kWebSafeHues += string.Format("{0:0000}, ", matches[i * 6 + j].Item1);
            }
            m_kWebSafeHues += "};";
        }
    }
}
