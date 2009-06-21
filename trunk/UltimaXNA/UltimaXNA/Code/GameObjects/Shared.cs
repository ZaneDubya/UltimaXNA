#region File Description & Usings
//-----------------------------------------------------------------------------
// Helpers.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
#endregion

namespace UltimaXNA.GameObjects
{
    public enum ObjectType
    {
        Object = 1,
        Unit = ObjectType.Object | 2,
        Player = ObjectType.Unit | 4,
        GameObject = ObjectType.Object | 8,
        DynamicObject = ObjectType.GameObject | 16,
        Corpse = ObjectType.GameObject | 32,
        Container = ObjectType.GameObject | 64,
    }

    struct ItemEnchantment
    {
        int EnchantmentType;
        int EnchantedByGUID;

        ItemEnchantment(int nType, int nByGUID)
        {
            EnchantmentType = nType;
            EnchantedByGUID = nByGUID;
        }
    }

    class PlayerQuest
    {
        int[] QuestData;

        PlayerQuest()
        {
            QuestData = new int[4];
        }
    }

    struct CurrentMaxValue
    {
        public int Current;
        public int Max;
        public bool Updated;

        CurrentMaxValue(int nCurrent, int nMax)
        {
            Current = nCurrent;
            Max = nMax;
            Updated = true;
        }

        public void Update(int nCurrent, int nMax)
        {
            Current = nCurrent;
            Max = nMax;
            Updated = true;
        }
    }

    struct BaseModValue
    {
        public int Base;
        public int Buff;

        BaseModValue(int nBase, int nBuff)
        {
            Base = nBase;
            Buff = nBuff;
        }

        int Current
        {
            get { return Base + Buff; }
        }
    }

    enum UnitFlags
    {
        IsNPC,
        IsMonster,
        IsPet,
        IsPlayer,
        IsInvulnerable,
        IsInvisible
    }

    enum NPCFlags
    {
        Gossip,
        QuestGiver,
        Vendor,
        Taxi,
        Trainer,
        Innkeeper,
        Banker,
        Guildmaster,
        TabardDesigner,
        Battlemaster,
        Auctioneer,
        StableMaster,
        Repair,
        Directions
    }

    enum DynamicFlags
    {
        InCombat,
        Mounted,
        Buffed,
        InDuel
    }

    enum OBJECT_UpdateMasks
    {
        OBJECT_Start = 0,
        OBJECT_GUID = OBJECT_Start,
        OBJECT_Type,
        OBJECT_End = OBJECT_Type + 1,
    }

    enum ITEM_UpdateMasks
    {
        ITEM_Start = OBJECT_UpdateMasks.OBJECT_End,
        ITEM_Type = ITEM_UpdateMasks.ITEM_Start,
        ITEM_SubType,
        ITEM_OwnerGUID,
        ITEM_ContainedWithinGUID,
        ITEM_CreatorGUID,
        ITEM_GiftedByGUID,
        ITEM_StackCount,
        ITEM_Duration,
        ITEM_PropertySeed,
        ITEM_RandomPropertiesID,
        ITEM_TextID,
        ITEM_CurrentDurability,
        ITEM_CurrentSpellCharges,
        ITEM_Enchantments6_2,
        ITEM_End = ITEM_Enchantments6_2 + 12,
    }

    enum CONTAINER_UpdateMasks
    {
        CONTAINER_Start = ITEM_UpdateMasks.ITEM_End,
        CONTAINER_NumSlots = CONTAINER_UpdateMasks.CONTAINER_Start,
        CONTAINER_Contents20_1,
        CONTAINER_End = CONTAINER_Contents20_1 + 20,
    }

    enum UNIT_UpdateMasks
    {
        UNIT_Start = OBJECT_UpdateMasks.OBJECT_End,
        UNIT_CharmingGUID = UNIT_UpdateMasks.UNIT_Start,
        UNIT_SummoningGUID,
        UNIT_CharmedByGUID,
        UNIT_SummonedByGUID,
        UNIT_CreatedByGUID,
        UNIT_CritterGUID,
        UNIT_PetGUID,
        UNIT_TargetGUID,
        UNIT_ChannelObjectGUID,
        UNIT_Bytes0,
        UNIT_Bytes1,
        UNIT_Bytes2,
        UNIT_HealthCurrentMax,
        UNIT_PowerCurrentMax = UNIT_HealthCurrentMax + 2,
        UNIT_Level = UNIT_PowerCurrentMax + 2,
        UNIT_PetLevel,
        UNIT_FactionID,
        UNIT_Flags,
        UNIT_FlagsDynamic,
        UNIT_FlagsNPC,
        UNIT_BaseAttackTime,
        UNIT_RangedAttackTime,
        UNIT_DisplayBodyID,
        UNIT_MountDisplayID,
        UNIT_VirtualDisplayID3,
        UNIT_MinMaxDamage = UNIT_VirtualDisplayID3 + 3,
        UNIT_MinMaxDamageOffhand = UNIT_MinMaxDamage + 2,
        UNIT_MinMaxDamageRanged = UNIT_MinMaxDamageOffhand + 2,
        UNIT_AttackPower = UNIT_MinMaxDamageRanged + 2,
        UNIT_StatsBaseMod5_2,
        UNIT_ResistancesBaseMod8_2 = UNIT_StatsBaseMod5_2 + 10,
        UNIT_End = UNIT_ResistancesBaseMod8_2 + 16,
    }

    enum PLAYER_UpdateMasks
    {
        PLAYER_Start = UNIT_UpdateMasks.UNIT_End,
        PLAYER_FlagsPlayer = PLAYER_UpdateMasks.PLAYER_Start,
        PLAYER_GuildID,
        PLAYER_GuildRank,
        PLAYER_ChosenTitle,
        PLAYER_BytesPlayer,
        PLAYER_Experience,
        PLAYER_ExperienceNextLevel,
        PLAYER_PetExperience,
        PLAYER_PetExperienceNextLevel,
        PLAYER_PartyID,
        PLAYER_DuelOpponentGUID,
        PLAYER_EquipSlots12,
        PLAYER_PackSlots20 = PLAYER_EquipSlots12 + 12,
        PLAYER_BankSlots30 = PLAYER_PackSlots20 + 20,
        PLAYER_QuestLog20_4 = PLAYER_BankSlots30 + 30,
        PLAYER_End = PLAYER_QuestLog20_4 + 80,
    }


}
