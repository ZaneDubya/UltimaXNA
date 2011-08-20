using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Data;

namespace UltimaXNA.Support
{
    class WebSafeHues
    {
        public static string GetWebSafeColors()
        {
            System.Drawing.Color[] colors = new System.Drawing.Color[2998];
            colors[0] = System.Drawing.Color.FromArgb(255, 255, 255, 255);
            for (int i = 1; i < 2998; i++)
            {
                colors[i] = Hues.GetHue(i + 0).GetColor(31);
            }

            List<Pair<System.Drawing.Color, int>> noDupes =
                new List<Pair<System.Drawing.Color, int>>();
            for (int i = 0; i < 2998; i++)
            {
                bool isDupe = false;
                foreach (Pair<System.Drawing.Color, int> p in noDupes)
                {
                    if (p.ItemA == colors[i])
                    {
                        isDupe = true;
                        break;
                    }
                    
                }
                if (!isDupe)
                    noDupes.Add(new Pair<System.Drawing.Color, int>(colors[i], i));
            }

            System.Drawing.Color[] desireds = new System.Drawing.Color[216];
            int desiredindex = 0;
            int[] vals = new int[6] { 0x00, 0x33, 0x66, 0x99, 0xCC, 0xFF };

            for (int g = 0; g < 6; g++)
                for (int b = 0; b < 6; b++)
                    for (int r = 0; r < 6; r++)
                    {
                        desireds[desiredindex++] = System.Drawing.Color.FromArgb(255, vals[r], vals[g], vals[b]);
                    }

            int[] safeIndexes = new int[216];
            for (int i = 0; i < 216; i++)
            {
                double delta = 250000f;
                for (int j = 0; j < noDupes.Count; j++)
                {
                    // compute the Euclidean distance between the two colors
                    // note, that the alpha-component is not used in this example
                    double dbl_test_red = System.Math.Pow(
                        (noDupes[j].ItemA.R - desireds[i].R), 2.0);
                    double dbl_test_green = System.Math.Pow(
                        (noDupes[j].ItemA.G - desireds[i].G), 2.0);
                    double dbl_test_blue = System.Math.Pow(
                        (noDupes[j].ItemA.B - desireds[i].B), 2.0);
                    // it is not necessary to compute the square root
                    // it should be sufficient to use:
                    // temp = dbl_test_blue + dbl_test_green + dbl_test_red;
                    // if you plan to do so, the distance should be initialized by 250000.0
                    double temp = System.Math.Sqrt(dbl_test_blue + dbl_test_green + dbl_test_red);
                    // explore the result and store the nearest color
                    if (temp == 0.0)
                    {
                        // the lowest possible distance is - of course - zero
                        // so I can break the loop (thanks to Willie Deutschmann)
                        // here I could return the input_color itself
                        // but in this example I am using a list with named colors
                        // and I want to return the Name-property too
                        safeIndexes[i] = noDupes[j].ItemB;
                        break;
                    }
                    else if (temp < delta)
                    {
                        delta = temp;
                        safeIndexes[i] = noDupes[j].ItemB;
                    }
                }
            }
            string outSafe = "int[] WebSafeHues = new int[216] { ";
            for (int i = 0; i < 216; i++)
            {
                outSafe += safeIndexes[i].ToString("D4") + ", ";
            }
            outSafe += " };";
            return outSafe;
        }
    }
}
