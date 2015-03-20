/***************************************************************************
 *   StatusInfoPacket.cs
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.UltimaPackets.Server
{
    public class MobileStatusCompactPacket : RecvPacket
    {
        readonly Serial m_serial;
        readonly string m_playerName;
        readonly short m_currentHealth;
        readonly short m_maxHealth;
        readonly byte m_nameChangeFlag;
        readonly byte m_statusTypeFlag;
        readonly Sex sex;
        readonly short m_strength; 
        readonly short m_dexterity;
        readonly short m_intelligence;
        readonly short m_currentStamina; 
        readonly short m_maxStamina;
        readonly short m_currentMana;
        readonly short m_maxMana;
        readonly int m_goldInInventory;
        readonly short m_armorRating;
        readonly short m_weight;
        readonly short m_maxWeight;
        readonly Race m_race;
        readonly short m_statCap;
        readonly byte m_followers;
        readonly byte m_maxFollowers;
        readonly short m_resistFire;
        readonly short m_resistCold;
        readonly short m_resistPoison;
        readonly short m_resistEnergy;
        readonly short m_luck;
        readonly short m_dmgMin;
        readonly short m_dmgMax;
        readonly short m_tithingPoints;

        public Serial Serial 
        {
            get { return m_serial; }
        }

        public string PlayerName
        {
            get { return m_playerName; } 
        }

        public short CurrentHealth
        {
            get { return m_currentHealth; } 
        }

        public short MaxHealth
        {
            get { return m_maxHealth; }
        }

        public bool NameChangeFlag 
        {
            get { return (m_nameChangeFlag == 0x01); } 
        }
        
        public byte StatusType
        {
            get { return m_statusTypeFlag; } 
        }
        
        public Sex Sex 
        {
            get { return sex; }
        }
        
        public short Strength 
        {
            get { return m_strength; } 
        }

        public short Dexterity 
        {
            get { return m_dexterity; }
        }

        public short Intelligence
        {
            get { return m_intelligence; }
        }
        
        public short CurrentStamina
        {
            get { return m_currentStamina; } 
        }

        public short MaxStamina
        {
            get { return m_maxStamina; } 
        }

        public short CurrentMana
        {
            get { return m_currentMana; } 
        }

        public short MaxMana
        {
            get { return m_maxMana; } 
        }

        public int GoldInInventory
        {
            get { return m_goldInInventory; } 
        }

        public short ArmorRating
        {
            get { return m_armorRating; }
        }
        
        public short Weight 
        {
            get { return m_weight; } 
        }

        public short WeightMax
        {
            get { return m_maxWeight; } 
        }
        
        public Race Race 
        {
            get { return m_race; }
        } 
        
        public short StatCap
        {
            get { return m_statCap; } 
        }

        public byte FollowersCurrent 
        {
            get { return m_followers; } 
        }

        public byte FollowersMax
        {
            get { return m_maxFollowers; } 
        } 

        public short ResistFire 
        {
            get { return m_resistFire; } 
        }

        public short ResistCold 
        {
            get { return m_resistCold; } 
        }

        public short ResistPoison 
        {
            get { return m_resistPoison; }
        }

        public short ResistEnergy 
        {
            get { return m_resistEnergy; } 
        }

        public short Luck
        {
            get { return m_luck; }
        }

        public short DamageMin 
        {
            get { return m_dmgMin; }
        }

        public short DamageMax 
        {
            get { return m_dmgMax; } 
        }

        public short TithingPoints
        {
            get { return m_tithingPoints; } 
        }

        public MobileStatusCompactPacket(PacketReader reader)
            : base(0x11, "StatusInfo")
        {
            m_serial = reader.ReadInt32();
            m_playerName = reader.ReadString(30);
            m_currentHealth = reader.ReadInt16();
            m_maxHealth = reader.ReadInt16();
            m_nameChangeFlag = reader.ReadByte(); // 0x1 = allowed, 0 = not allowed
            m_statusTypeFlag = reader.ReadByte();
            sex = (Sex)reader.ReadByte(); // 0=male, 1=female
            m_strength = reader.ReadInt16();
            m_dexterity = reader.ReadInt16();
            m_intelligence = reader.ReadInt16();
            m_currentStamina = reader.ReadInt16();
            m_maxStamina = reader.ReadInt16();
            m_currentMana = reader.ReadInt16();
            m_maxMana = reader.ReadInt16();
            m_goldInInventory = reader.ReadInt32();
            m_armorRating = reader.ReadInt16();
            m_weight = reader.ReadInt16();

            if (m_statusTypeFlag >= 5)
            {
                m_maxWeight = reader.ReadInt16();
                m_race = (Race)reader.ReadByte();
            }

            if (m_statusTypeFlag >= 3)
            {
                m_statCap = reader.ReadInt16();
                m_followers = reader.ReadByte();
                m_maxFollowers = reader.ReadByte();
            }

            if (m_statusTypeFlag >= 4)
            {
                m_resistFire = reader.ReadInt16();
                m_resistCold = reader.ReadInt16();
                m_resistPoison = reader.ReadInt16();
                m_resistEnergy = reader.ReadInt16();
                m_luck = reader.ReadInt16();
                m_dmgMin = reader.ReadInt16();
                m_dmgMax = reader.ReadInt16();
                m_tithingPoints = reader.ReadInt16();
            }
        }
    }
}
