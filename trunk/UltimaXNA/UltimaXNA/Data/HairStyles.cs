using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Data
{
    class HairStyles
    {
        static readonly int[] _maleStyles = new int[10] { 3000340, 3000341, 3000342, 3000343, 3000344, 3000345, 3000346, 3000347, 3000348, 3000349 };
        static string[] _male;
        public static string[] Male
        {
            get
            {
                if (_male == null)
                {
                    _male = new string[_maleStyles.Length];
                    for (int i = 0; i < _maleStyles.Length; i++)
                    {
                        _male[i] = Data.StringList.Entry(_maleStyles[i]);
                        if (_male[i] == "Pigtails")
                            _male[i] = "2 Tails";
                    }
                }
                return _male;
            }
        }

        static readonly int[] _facialStyles = new int[8] { 3000340, 3000351, 3000352, 3000353, 3000354, 1011060, 1011061, 3000357 };
        static string[] _facial;
        public static string[] FacialHair
        {
            get
            {
                if (_facial == null)
                {
                    _facial = new string[_facialStyles.Length];
                    for (int i = 0; i < _facialStyles.Length; i++)
                    {
                        _facial[i] = Data.StringList.Entry(_facialStyles[i]);
                    }
                }
                return _facial;
            }
        }

        static readonly int[] _femaleStyles = new int[10] { 3000340, 3000341, 3000342, 3000343, 3000344, 3000345, 3000346, 3000347, 3000349, 3000350 };
        static string[] _female;
        public static string[] Female
        {
            get
            {
                if (_female == null)
                {
                    _female = new string[_femaleStyles.Length];
                    for (int i = 0; i < _femaleStyles.Length; i++)
                    {
                        _female[i] = Data.StringList.Entry(_femaleStyles[i]);
                    }
                }
                return _female;
            }
        }
    }
}
