using UltimaXNA.Ultima.Resources;

namespace UltimaXNA.Ultima.World.Entities.Mobiles.Animations
{
    public enum MobileAction
    {
        None,
        Walk,
        Run,
        Stand,
        Death,
        Attack,
        Cast_Directed,
        Cast_Area,
        GetHit,
        Block,
        Emote_Fidget_1,
        Emote_Fidget_2,
        Emote_Bow,
        Emote_Salute,
        Emote_Eat,
        MonsterAction
    }

    enum ActionIndexMonster
    {
        Walk = 0x00,
        Stand = 0x01,
        Die_Backwards = 0x02,
        Die_Forwards = 0x03,
        Attack1 = 0x04,
        Attack2 = 0x05,
        Attack3 = 0x06,
        Stumble = 0x07,
        MonsterMisc = 0x08,
        GetHit2 = 0x09,
        GetHit3 = 0x0A,
        Emote_Fidget_1 = 0x0B,
        Emote_Fidget_2 = 0x0C,
        Run = 0x13,
    }

    enum ActionIndexAnimal
    {
        Walk = 0x00,
        Run = 0x01,
        Stand = 0x02,
        Graze = 0x03,
        Unknown1 = 0x04,
        Attack1 = 0x05,
        Attack2 = 0x06,
        Attack3 = 0x07,
        Die_Backwards = 0x08,
        Fidget1 = 0x09,
        Fidget2 = 0x0A,
        LieDown = 0x0B,
        Die_Forwards = 0x0C,
    }

    enum ActionIndexHumanoid
    {
        Walk = 0x00,
        Walk_Armed = 0x01,
        Run = 0x02,
        Run_Armed = 0x03,
        Stand = AnimationResource.HUMANOID_STAND_INDEX,
        Fidget_1 = 0x05,
        Fidget_2 = 0x06,
        Stand_Warmode1H = 0x07,
        Stand_Warmode2H = 0x08,
        Attack_1H = 0x09,
        Attack_Unarmed1 = 0x0A,
        Attack_Unarmed2 = 0x0B,
        Attack_2H_Down = 0x0C,
        Attack_2H_Across = 0x0D,
        Attack_2H_Jab = 0x0E,
        Walk_Warmode = 0x0F,
        Cast_Directed = 0x10,
        Cast_Area = 0x11,
        Attack_Bow = 0x12,
        Attack_BowX = 0x13,
        Hit = 0x14,
        Die_Backwards = 0x15,
        Die_Forwards = 0x16,
        Mounted_RideSlow = 0x17,
        Mounted_RideFast = 0x18,
        Mounted_Stand = AnimationResource.HUMANOID_MOUNT_INDEX,
        Mounted_Attack_1H = 0x1A,
        Mounted_Attack_Bow = 0x1B,
        Mounted_Attack_BowX = 0x1C,
        Mounted_SlapHorse = 0x1D,
        Block_WithShield = 0x1E,
        Attack_Unarmed3 = 0x1F,
        Emote_Bow = 0x20,
        Emote_Salute = 0x21,
        Emote_Eat = 0x22,
        Sit = AnimationResource.HUMANOID_SIT_INDEX
    }
}
