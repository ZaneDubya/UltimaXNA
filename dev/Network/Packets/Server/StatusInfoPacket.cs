/***************************************************************************
 *   StatusInfoPacket.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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
using UltimaXNA.Network;
#endregion

namespace UltimaXNA.Network.Packets.Server
{
    public class MobileStatusCompactPacket : RecvPacket
    {
        readonly Serial _serial;
        readonly string _playerName;
        readonly short _currentHealth;
        readonly short _maxHealth;
        readonly byte _nameChangeFlag;
        readonly byte _statusTypeFlag;
        readonly Sex sex;
        readonly short _strength; 
        readonly short _dexterity;
        readonly short _intelligence;
        readonly short _currentStamina; 
        readonly short _maxStamina;
        readonly short _currentMana;
        readonly short _maxMana;
        readonly int _goldInInventory;
        readonly short _armorRating;
        readonly short _weight;
        readonly short _maxWeight;
        readonly Race _race;
        readonly short _statCap;
        readonly byte _followers;
        readonly byte _maxFollowers;
        readonly short _resistFire;
        readonly short _resistCold;
        readonly short _resistPoison;
        readonly short _resistEnergy;
        readonly short _luck;
        readonly short _dmgMin;
        readonly short _dmgMax;
        readonly short _tithingPoints;

        public Serial Serial 
        {
            get { return _serial; }
        }

        public string PlayerName
        {
            get { return _playerName; } 
        }

        public short CurrentHealth
        {
            get { return _currentHealth; } 
        }

        public short MaxHealth
        {
            get { return _maxHealth; }
        }

        public bool NameChangeFlag 
        {
            get { return (_nameChangeFlag == 0x01); } 
        }
        
        public byte StatusType
        {
            get { return _statusTypeFlag; } 
        }
        
        public Sex Sex 
        {
            get { return sex; }
        }
        
        public short Strength 
        {
            get { return _strength; } 
        }

        public short Dexterity 
        {
            get { return _dexterity; }
        }

        public short Intelligence
        {
            get { return _intelligence; }
        }
        
        public short CurrentStamina
        {
            get { return _currentStamina; } 
        }

        public short MaxStamina
        {
            get { return _maxStamina; } 
        }

        public short CurrentMana
        {
            get { return _currentMana; } 
        }

        public short MaxMana
        {
            get { return _maxMana; } 
        }

        public int GoldInInventory
        {
            get { return _goldInInventory; } 
        }

        public short ArmorRating
        {
            get { return _armorRating; }
        }
        
        public short Weight 
        {
            get { return _weight; } 
        }

        public short WeightMax
        {
            get { return _maxWeight; } 
        }
        
        public Race Race 
        {
            get { return _race; }
        } 
        
        public short StatCap
        {
            get { return _statCap; } 
        }

        public byte FollowersCurrent 
        {
            get { return _followers; } 
        }

        public byte FollowersMax
        {
            get { return _maxFollowers; } 
        } 

        public short ResistFire 
        {
            get { return _resistFire; } 
        }

        public short ResistCold 
        {
            get { return _resistCold; } 
        }

        public short ResistPoison 
        {
            get { return _resistPoison; }
        }

        public short ResistEnergy 
        {
            get { return _resistEnergy; } 
        }

        public short Luck
        {
            get { return _luck; }
        }

        public short DamageMin 
        {
            get { return _dmgMin; }
        }

        public short DamageMax 
        {
            get { return _dmgMax; } 
        }

        public short TithingPoints
        {
            get { return _tithingPoints; } 
        }

        public MobileStatusCompactPacket(PacketReader reader)
            : base(0x11, "StatusInfo")
        {
            _serial = reader.ReadInt32();
            _playerName = reader.ReadString(30);
            _currentHealth = reader.ReadInt16();
            _maxHealth = reader.ReadInt16();
            _nameChangeFlag = reader.ReadByte(); // 0x1 = allowed, 0 = not allowed
            _statusTypeFlag = reader.ReadByte();
            sex = (Sex)reader.ReadByte(); // 0=male, 1=female
            _strength = reader.ReadInt16();
            _dexterity = reader.ReadInt16();
            _intelligence = reader.ReadInt16();
            _currentStamina = reader.ReadInt16();
            _maxStamina = reader.ReadInt16();
            _currentMana = reader.ReadInt16();
            _maxMana = reader.ReadInt16();
            _goldInInventory = reader.ReadInt32();
            _armorRating = reader.ReadInt16();
            _weight = reader.ReadInt16();

            if (_statusTypeFlag >= 5)
            {
                _maxWeight = reader.ReadInt16();
                _race = (Race)reader.ReadByte();
            }

            if (_statusTypeFlag >= 3)
            {
                _statCap = reader.ReadInt16();
                _followers = reader.ReadByte();
                _maxFollowers = reader.ReadByte();
            }

            if (_statusTypeFlag >= 4)
            {
                _resistFire = reader.ReadInt16();
                _resistCold = reader.ReadInt16();
                _resistPoison = reader.ReadInt16();
                _resistEnergy = reader.ReadInt16();
                _luck = reader.ReadInt16();
                _dmgMin = reader.ReadInt16();
                _dmgMax = reader.ReadInt16();
                _tithingPoints = reader.ReadInt16();
            }
        }
    }
}
