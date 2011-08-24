/***************************************************************************
 *   HairStyles.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
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
        public static string[] MaleHairNames
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
        static readonly int[] _maleIDs = new int[10] { 0, 8251, 8252, 8253, 8260, 8261, 8266, 8263, 8264, 8265 };
        public static int[] MaleIDs
        {
            get
            {
                return _maleIDs;
            }
        }
        static readonly int[] _maleIDsForCreation = new int[10] { 0, 1875, 1876, 1879, 1877, 1871, 1874, 1873, 1880, 1870 };
        public static int MaleGumpIDForCharacterCreationFromItemID(int id)
        {
            int gumpID = 0;
            for (int i = 0; i < _maleIDsForCreation.Length; i++)
                if (_maleIDs[i] == id)
                    gumpID = _maleIDsForCreation[i];
            return gumpID;
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
        static readonly int[] _facialIDs = new int[8] { 0, 8256, 8254, 8255, 8257, 8267, 8268, 8269 };
        public static int[] FacialHairIDs
        {
            get
            {
                return _facialIDs;
            }
        }
        static readonly int[] _facialGumpIDsForCreation = new int[8] { 0, 1881, 1883, 1885, 1884, 1886, 1882, 1887 };
        public static int FacialHairGumpIDForCharacterCreationFromItemID(int id)
        {
            int gumpID = 0;
            for (int i = 0; i < _facialGumpIDsForCreation.Length; i++)
                if (_facialIDs[i] == id)
                    gumpID = _facialGumpIDsForCreation[i];
            return gumpID;
        }

        static readonly int[] _femaleStyles = new int[10] { 3000340, 3000341, 3000342, 3000343, 3000344, 3000345, 3000346, 3000347, 3000349, 3000350 };
        static string[] _female;
        public static string[] FemaleHairNames
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
        static readonly int[] _femaleIDs = new int[10] { 0, 8251, 8252, 8253, 8260, 8261, 8266, 8263, 8265, 8262 };
        public static int[] FemaleIDs
        {
            get
            {
                return _femaleIDs;
            }
        }
        static readonly int[] _femaleIDsForCreation = new int[10] { 0, 1847, 1842, 1845, 1843, 1844, 1840, 1839, 1836, 1841 };
        public static int FemaleGumpIDForCharacterCreationFromItemID(int id)
        {
            int gumpID = 0;
            for (int i = 0; i < _femaleIDsForCreation.Length; i++)
                if (_femaleIDs[i] == id)
                    gumpID = _femaleIDsForCreation[i];
            return gumpID;
        }
    }
}
