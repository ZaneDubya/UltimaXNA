/***************************************************************************
 *   HairStyles.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using UltimaXNA.Ultima.Resources;
using UltimaXNA.Core.Resources;

namespace UltimaXNA.Ultima.Data
{
    class HairStyles
    {
        static readonly int[] m_maleStyles = new int[10] { 3000340, 3000341, 3000342, 3000343, 3000344, 3000345, 3000346, 3000347, 3000348, 3000349 };
        static string[] m_male;
        public static string[] MaleHairNames
        {
            get
            {
                if (m_male == null)
                {
                    // get the resource provider
                    IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();

                    m_male = new string[m_maleStyles.Length];
                    for (int i = 0; i < m_maleStyles.Length; i++)
                    {
                        m_male[i] = provider.GetString(m_maleStyles[i]);
                        if (m_male[i] == "Pigtails")
                            m_male[i] = "2 Tails";
                    }
                }
                return m_male;
            }
        }
        static readonly int[] m_maleIDs = new int[10] { 0, 8251, 8252, 8253, 8260, 8261, 8266, 8263, 8264, 8265 };
        public static int[] MaleIDs
        {
            get
            {
                return m_maleIDs;
            }
        }
        static readonly int[] m_maleIDsForCreation = new int[10] { 0, 1875, 1876, 1879, 1877, 1871, 1874, 1873, 1880, 1870 };
        public static int MaleGumpIDForCharacterCreationFromItemID(int id)
        {
            int gumpID = 0;
            for (int i = 0; i < m_maleIDsForCreation.Length; i++)
                if (m_maleIDs[i] == id)
                    gumpID = m_maleIDsForCreation[i];
            return gumpID;
        }


        static readonly int[] m_facialStyles = new int[8] { 3000340, 3000351, 3000352, 3000353, 3000354, 1011060, 1011061, 3000357 };
        static string[] m_facial;
        public static string[] FacialHair
        {
            get
            {
                if (m_facial == null)
                {
                    // get the resource provider
                    IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();

                    m_facial = new string[m_facialStyles.Length];
                    for (int i = 0; i < m_facialStyles.Length; i++)
                    {
                        m_facial[i] = provider.GetString(m_facialStyles[i]);
                    }
                }
                return m_facial;
            }
        }
        static readonly int[] m_facialIDs = new int[8] { 0, 8256, 8254, 8255, 8257, 8267, 8268, 8269 };
        public static int[] FacialHairIDs
        {
            get
            {
                return m_facialIDs;
            }
        }
        static readonly int[] m_facialGumpIDsForCreation = new int[8] { 0, 1881, 1883, 1885, 1884, 1886, 1882, 1887 };
        public static int FacialHairGumpIDForCharacterCreationFromItemID(int id)
        {
            int gumpID = 0;
            for (int i = 0; i < m_facialGumpIDsForCreation.Length; i++)
                if (m_facialIDs[i] == id)
                    gumpID = m_facialGumpIDsForCreation[i];
            return gumpID;
        }

        static readonly int[] m_femaleStyles = new int[10] { 3000340, 3000341, 3000342, 3000343, 3000344, 3000345, 3000346, 3000347, 3000349, 3000350 };
        static string[] m_female;
        public static string[] FemaleHairNames
        {
            get
            {
                if (m_female == null)
                {
                    // get the resource provider
                    IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();

                    m_female = new string[m_femaleStyles.Length];
                    for (int i = 0; i < m_femaleStyles.Length; i++)
                    {
                        m_female[i] = provider.GetString(m_femaleStyles[i]);
                    }
                }
                return m_female;
            }
        }
        static readonly int[] m_femaleIDs = new int[10] { 0, 8251, 8252, 8253, 8260, 8261, 8266, 8263, 8265, 8262 };
        public static int[] FemaleIDs
        {
            get
            {
                return m_femaleIDs;
            }
        }
        static readonly int[] m_femaleIDsForCreation = new int[10] { 0, 1847, 1842, 1845, 1843, 1844, 1840, 1839, 1836, 1841 };
        public static int FemaleGumpIDForCharacterCreationFromItemID(int id)
        {
            int gumpID = 0;
            for (int i = 0; i < m_femaleIDsForCreation.Length; i++)
                if (m_femaleIDs[i] == id)
                    gumpID = m_femaleIDsForCreation[i];
            return gumpID;
        }
    }
}