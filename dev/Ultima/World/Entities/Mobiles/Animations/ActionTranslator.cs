using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Ultima.World.Entities.Items.Containers;

namespace UltimaXNA.Ultima.World.Entities.Mobiles.Animations
{
    static class ActionTranslator
    {
        public static int GetActionIndex(AEntity entity, MobileAction action)
        {
            return GetActionIndex(entity, action, -1);
        }

        public static int GetActionIndex(AEntity entity, MobileAction action, int index)
        {
            Body body = 0;
            bool isMounted = false, isWarMode = false, isSitting = false, dieForwards = false;
            int lightSourceBodyID = 0;

            if (entity is Corpse)
            {
                body = (entity as Corpse).Body;
                dieForwards = (entity as Corpse).DieForwards;
            }
            else if (entity is Mobile)
            {
                Mobile mobile = entity as Mobile;
                body = mobile.Body;
                isMounted = mobile.IsMounted;
                isWarMode = mobile.Flags.IsWarMode;
                isSitting = mobile.IsSitting;
                lightSourceBodyID = mobile.LightSourceBodyID;
            }
            else
            {
                Tracer.Critical("Entity of type {0} cannot get an action index.", entity.ToString());
            }

            if (body.IsHumanoid)
            {
                switch (action)
                {
                    case MobileAction.None:
                        // this will never be called.
                        return GetActionIndex(entity, MobileAction.Stand, index);
                    case MobileAction.Walk:
                        if (isMounted)
                            return (int)ActionIndexHumanoid.Mounted_RideSlow;
                        else
                            if (isWarMode)
                            return (int)ActionIndexHumanoid.Walk_Warmode;
                        else
                        {
                            // if carrying a light source, return Walk_Armed.
                            if (lightSourceBodyID != 0)
                                return (int)ActionIndexHumanoid.Walk_Armed;
                            else
                                return (int)ActionIndexHumanoid.Walk;
                        }
                    case MobileAction.Run:
                        if (isMounted)
                            return (int)ActionIndexHumanoid.Mounted_RideFast;
                        else
                        {
                            // if carrying a light source, return Run_Armed.
                            if (lightSourceBodyID!= 0)
                                return (int)ActionIndexHumanoid.Run_Armed;
                            else
                                return (int)ActionIndexHumanoid.Run;
                        }
                    case MobileAction.Stand:
                        if (isMounted)
                        {
                            return (int)ActionIndexHumanoid.Mounted_Stand;
                        }
                        else
                        {
                            if (isSitting)
                            {
                                return (int)ActionIndexHumanoid.Sit;
                            }
                            else if (isWarMode)
                            {
                                // TODO: Also check if weapon type is 2h. Can be 1H or 2H
                                return (int)ActionIndexHumanoid.Stand_Warmode1H;
                            }
                            else
                            {
                                return (int)ActionIndexHumanoid.Stand;
                            }
                        }
                    case MobileAction.Death:
                        if (dieForwards)
                            return (int)ActionIndexHumanoid.Die_Backwards;
                        else
                            return (int)ActionIndexHumanoid.Die_Forwards;
                    case MobileAction.Attack:
                        if (isMounted)
                        {
                            // check weapon type. Can be 1H, Bow, or XBow
                            return (int)ActionIndexHumanoid.Mounted_Attack_1H;
                        }
                        else
                        {
                            // check weapon type. Can be 1H, 2H across, 2H down, 2H jab, bow, xbow, or unarmed.
                            return (int)ActionIndexHumanoid.Attack_1H;
                        }
                    case MobileAction.Cast_Directed:
                        if (isMounted)
                            return GetActionIndex(entity, MobileAction.Stand, index);
                        else
                            return (int)ActionIndexHumanoid.Cast_Directed;
                    case MobileAction.Cast_Area:
                        if (isMounted)
                            return GetActionIndex(entity, MobileAction.Stand, index);
                        else
                            return (int)ActionIndexHumanoid.Cast_Area;
                    case MobileAction.GetHit:
                        if (isMounted)
                            return GetActionIndex(entity, MobileAction.Stand, index);
                        else
                            return (int)ActionIndexHumanoid.Hit;
                    case MobileAction.Block:
                        if (isMounted)
                            return GetActionIndex(entity, MobileAction.Stand, index);
                        else
                            return (int)ActionIndexHumanoid.Block_WithShield;
                    case MobileAction.Emote_Fidget_1:
                        if (isMounted)
                            return GetActionIndex(entity, MobileAction.Stand, index);
                        else
                            return (int)ActionIndexHumanoid.Fidget_1;
                    case MobileAction.Emote_Fidget_2:
                        if (isMounted)
                            return GetActionIndex(entity, MobileAction.Stand, index);
                        else
                            return (int)ActionIndexHumanoid.Fidget_2;
                    case MobileAction.Emote_Bow:
                        if (isMounted)
                            return GetActionIndex(entity, MobileAction.Stand, index);
                        else
                            return (int)ActionIndexHumanoid.Emote_Bow;
                    case MobileAction.Emote_Salute:
                        if (isMounted)
                            return GetActionIndex(entity, MobileAction.Stand, index);
                        else
                            return (int)ActionIndexHumanoid.Emote_Salute;
                    case MobileAction.Emote_Eat:
                        if (isMounted)
                            return GetActionIndex(entity, MobileAction.Stand, index);
                        else
                            return (int)ActionIndexHumanoid.Emote_Eat;
                    default:
                        return (int)-1;
                }
            }
            else if (body.IsAnimal)
            {
                switch (action)
                {
                    case MobileAction.None:
                        return GetActionIndex(entity, MobileAction.Stand, index);
                    case MobileAction.Walk:
                        return (int)ActionIndexAnimal.Walk;
                    case MobileAction.Run:
                        return (int)ActionIndexAnimal.Run;
                    case MobileAction.Stand:
                        return (int)ActionIndexAnimal.Stand;
                    case MobileAction.Death:
                        if (dieForwards)
                            return (int)ActionIndexAnimal.Die_Backwards;
                        else
                            return (int)ActionIndexAnimal.Die_Forwards;
                    case MobileAction.MonsterAction:
                        return index;
                    default:
                        return (int)-1;
                }
            }
            else if (body.IsMonster)
            {
                switch (action)
                {
                    case MobileAction.None:
                        return GetActionIndex(entity, MobileAction.Stand, index);
                    case MobileAction.Walk:
                        return (int)ActionIndexMonster.Walk;
                    case MobileAction.Run:
                        return (int)ActionIndexMonster.Walk;
                    case MobileAction.Stand:
                        return (int)ActionIndexMonster.Stand;
                    case MobileAction.Death:
                        if (dieForwards)
                            return (int)ActionIndexMonster.Die_Backwards;
                        else
                            return (int)ActionIndexMonster.Die_Forwards;
                    case MobileAction.MonsterAction:
                        return index;
                    default:
                        return (int)-1;
                }
            }

            return -1;
        }

        public static MobileAction GetActionFromIndex(Body body, int index)
        {
            if (body.IsHumanoid)
            {
                switch ((ActionIndexHumanoid)index)
                {
                    case ActionIndexHumanoid.Walk:
                    case ActionIndexHumanoid.Walk_Armed:
                    case ActionIndexHumanoid.Walk_Warmode:
                    case ActionIndexHumanoid.Mounted_RideSlow:
                        return MobileAction.Walk;

                    case ActionIndexHumanoid.Mounted_RideFast:
                    case ActionIndexHumanoid.Run:
                    case ActionIndexHumanoid.Run_Armed:
                        return MobileAction.Run;

                    case ActionIndexHumanoid.Stand:
                    case ActionIndexHumanoid.Stand_Warmode1H:
                    case ActionIndexHumanoid.Stand_Warmode2H:
                    case ActionIndexHumanoid.Mounted_Stand:
                        return MobileAction.Stand;

                    case ActionIndexHumanoid.Fidget_1:
                        return MobileAction.Emote_Fidget_1;

                    case ActionIndexHumanoid.Fidget_2:
                        return MobileAction.Emote_Fidget_2;

                    case ActionIndexHumanoid.Attack_1H:
                    case ActionIndexHumanoid.Attack_Unarmed1:
                    case ActionIndexHumanoid.Attack_Unarmed2:
                    case ActionIndexHumanoid.Attack_2H_Down:
                    case ActionIndexHumanoid.Attack_2H_Across:
                    case ActionIndexHumanoid.Attack_2H_Jab:
                    case ActionIndexHumanoid.Attack_Bow:
                    case ActionIndexHumanoid.Attack_BowX:
                    case ActionIndexHumanoid.Mounted_Attack_1H:
                    case ActionIndexHumanoid.Mounted_Attack_Bow:
                    case ActionIndexHumanoid.Mounted_Attack_BowX:
                    case ActionIndexHumanoid.Attack_Unarmed3:
                        return MobileAction.Attack;

                    case ActionIndexHumanoid.Cast_Directed:
                        return MobileAction.Cast_Directed;

                    case ActionIndexHumanoid.Cast_Area:
                        return MobileAction.Cast_Area;

                    case ActionIndexHumanoid.Hit:
                        return MobileAction.GetHit;

                    case ActionIndexHumanoid.Die_Backwards:
                    case ActionIndexHumanoid.Die_Forwards:
                        return MobileAction.Death;

                    case ActionIndexHumanoid.Mounted_SlapHorse: // not coded or used?
                        return MobileAction.Stand;

                    case ActionIndexHumanoid.Block_WithShield:
                        return MobileAction.Block;

                    case ActionIndexHumanoid.Emote_Bow:
                        return MobileAction.Emote_Bow;

                    case ActionIndexHumanoid.Emote_Salute:
                        return MobileAction.Emote_Salute;

                    case ActionIndexHumanoid.Emote_Eat:
                        return MobileAction.Emote_Eat;
                }

                // special case animations. When casting a spell, the server will send animation indexes over 200,
                // which all seem to correspond to Cast_Directed. Example indexes are:
                // 200, 201, 203, 206, 209, 212, 215, 218, 221, 227, 230, 239, 245, 260, 266 and doubtless others.
                if (index >= 200)
                    return MobileAction.Cast_Directed;

                Tracer.Warn("Unknown action index {0}", index);
                return MobileAction.None;
            }
            else if (body.IsAnimal)
            {
                switch ((ActionIndexAnimal)index)
                {
                    case ActionIndexAnimal.Stand:
                        return MobileAction.Stand;
                    case ActionIndexAnimal.Walk:
                        return MobileAction.Walk;
                    case ActionIndexAnimal.Run:
                        return MobileAction.Run;
                    default:
                        return MobileAction.MonsterAction;
                }
            }
            else if (body.IsMonster)
            {
                switch ((ActionIndexMonster)index)
                {
                    case ActionIndexMonster.Stand:
                        return MobileAction.Stand;
                    case ActionIndexMonster.Walk:
                        return MobileAction.Walk;
                    case ActionIndexMonster.Run:
                        return MobileAction.Run;
                    default:
                        return MobileAction.MonsterAction;
                }
            }
            return MobileAction.None;
        }
    }
}
