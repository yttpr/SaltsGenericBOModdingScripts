using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;

namespace PYMN13
{
    public class MultiTargetting : BaseCombatTargettingSO
    {
        public BaseCombatTargettingSO first;
        public BaseCombatTargettingSO second;
        public override bool AreTargetAllies=> first.AreTargetAllies && second.AreTargetAllies;
        public override bool AreTargetSlots => first.AreTargetSlots && second.AreTargetSlots;
        public override TargetSlotInfo[] GetTargets(SlotsCombat slots, int casterSlotID, bool isCasterCharacter)
        {
            TargetSlotInfo[] targets1 = this.first.GetTargets(slots, casterSlotID, isCasterCharacter);
            TargetSlotInfo[] targets2 = this.second.GetTargets(slots, casterSlotID, isCasterCharacter);
            TargetSlotInfo[] destinationArray = new TargetSlotInfo[targets1.Length + targets2.Length];
            Array.Copy(targets1, destinationArray, targets1.Length);
            Array.Copy(targets2, 0, destinationArray, targets1.Length, targets2.Length);
            return destinationArray;
        }
        public static MultiTargetting Create(BaseCombatTargettingSO first, BaseCombatTargettingSO second)
        {
            MultiTargetting instance = CreateInstance<MultiTargetting>();
            instance.first = first;
            instance.second = second;
            return instance;
        }
    }
    public class TargettingAllSlots : BaseCombatTargettingSO
    {
        public override bool AreTargetAllies => false;

        public override bool AreTargetSlots => false;

        public override TargetSlotInfo[] GetTargets( SlotsCombat slots, int casterSlotID, bool isCasterCharacter)
        {
            List<TargetSlotInfo> targetSlotInfoList = new List<TargetSlotInfo>();
            foreach (CombatSlot characterSlot in slots.CharacterSlots)
            {
                TargetSlotInfo targetSlotInformation = characterSlot.TargetSlotInformation;
                if (targetSlotInformation != null)
                    targetSlotInfoList.Add(targetSlotInformation);
            }
            foreach (CombatSlot enemySlot in slots.EnemySlots)
            {
                TargetSlotInfo targetSlotInformation = enemySlot.TargetSlotInformation;
                if (targetSlotInformation != null)
                    targetSlotInfoList.Add(targetSlotInformation);
            }
            return targetSlotInfoList.ToArray();
        }
    }
    public class TargettingAllUnits : BaseCombatTargettingSO
    {
        public bool ignoreCastSlot = false;

        public override bool AreTargetAllies => false;

        public override bool AreTargetSlots => false;

        public static bool IsUnitAlreadyContained(List<TargetSlotInfo> targets, TargetSlotInfo target)
        {
            foreach (TargetSlotInfo target1 in targets)
            {
                if (target1.Unit == target.Unit)
                    return true;
            }
            return false;
        }

        public bool IsCastSlot(int caster, TargetSlotInfo target)
        {
            return this.ignoreCastSlot && caster == target.SlotID;
        }

        public override TargetSlotInfo[] GetTargets(SlotsCombat slots, int casterSlotID, bool isCasterCharacter)
        {
            List<TargetSlotInfo> targets = new List<TargetSlotInfo>();
            foreach (CombatSlot characterSlot in slots.CharacterSlots)
            {
                TargetSlotInfo targetSlotInformation = characterSlot.TargetSlotInformation;
                if (targetSlotInformation != null && targetSlotInformation.HasUnit && !IsUnitAlreadyContained(targets, targetSlotInformation) && !IsCastSlot(casterSlotID, targetSlotInformation))
                    targets.Add(targetSlotInformation);
            }
            foreach (CombatSlot enemySlot in slots.EnemySlots)
            {
                TargetSlotInfo targetSlotInformation = enemySlot.TargetSlotInformation;
                if (targetSlotInformation != null && targetSlotInformation.HasUnit && !IsUnitAlreadyContained(targets, targetSlotInformation) && !IsCastSlot(casterSlotID, targetSlotInformation))
                    targets.Add(targetSlotInformation);
            }
            return targets.ToArray();
        }
    }
    public class TargettingClosestUnits : BaseCombatTargettingSO
    {
        public bool getAllies;
        public bool ignoreCastSlot = true;

        public override bool AreTargetAllies => getAllies;

        public override bool AreTargetSlots => true;

        public override TargetSlotInfo[] GetTargets(
          SlotsCombat slots,
          int casterSlotID,
          bool isCasterCharacter)
        {
            List<TargetSlotInfo> targetSlotInfoList = new List<TargetSlotInfo>();
            CombatSlot combatSlot1 = null;
            CombatSlot combatSlot2 = null;
            if (isCasterCharacter && getAllies || !isCasterCharacter && !getAllies)
            {
                foreach (CombatSlot characterSlot in slots.CharacterSlots)
                {
                    if (characterSlot.HasUnit && characterSlot.SlotID > casterSlotID && (!ignoreCastSlot || casterSlotID != characterSlot.SlotID))
                    {
                        if (combatSlot1 == null)
                            combatSlot1 = characterSlot;
                        else if (characterSlot.SlotID < combatSlot1.SlotID)
                            combatSlot1 = characterSlot;
                    }
                    else if (characterSlot.HasUnit && characterSlot.SlotID < casterSlotID && (!ignoreCastSlot || casterSlotID != characterSlot.SlotID))
                    {
                        if (combatSlot2 == null)
                            combatSlot2 = characterSlot;
                        else if (characterSlot.SlotID > combatSlot2.SlotID)
                            combatSlot2 = characterSlot;
                    }
                }
            }
            else
            {
                foreach (CombatSlot enemySlot in slots.EnemySlots)
                {
                    if (enemySlot.HasUnit && enemySlot.SlotID > casterSlotID && (!ignoreCastSlot || casterSlotID != enemySlot.SlotID))
                    {
                        if (combatSlot1 == null)
                            combatSlot1 = enemySlot;
                        else if (enemySlot.SlotID < combatSlot1.SlotID)
                            combatSlot1 = enemySlot;
                    }
                    else if (enemySlot.HasUnit && enemySlot.SlotID < casterSlotID && (!ignoreCastSlot || casterSlotID != enemySlot.SlotID))
                    {
                        if (combatSlot2 == null)
                            combatSlot2 = enemySlot;
                        else if (enemySlot.SlotID > combatSlot2.SlotID)
                            combatSlot2 = enemySlot;
                    }
                }
            }
            if (combatSlot1 != null)
                targetSlotInfoList.Add(combatSlot1.TargetSlotInformation);
            if (combatSlot2 != null)
                targetSlotInfoList.Add(combatSlot2.TargetSlotInformation);
            return targetSlotInfoList.ToArray();
        }
    }
    public class TargettingFarthestUnits : BaseCombatTargettingSO
    {
        public bool getAllies;
        public bool ignoreCastSlot = true;
        public bool LeftOnly = false;
        public bool RightOnly = false;
        public bool FarthestOnly = false;

        public override bool AreTargetAllies => getAllies;

        public override bool AreTargetSlots => true;

        public override TargetSlotInfo[] GetTargets(
          SlotsCombat slots,
          int casterSlotID,
          bool isCasterCharacter)
        {
            List<TargetSlotInfo> targetSlotInfoList = new List<TargetSlotInfo>();
            CombatSlot combatSlot1 = null;
            CombatSlot combatSlot2 = null;
            if (isCasterCharacter && getAllies || !isCasterCharacter && !getAllies)
            {
                foreach (CombatSlot characterSlot in slots.CharacterSlots)
                {
                    if (characterSlot.HasUnit && characterSlot.SlotID > casterSlotID && (!ignoreCastSlot || casterSlotID != characterSlot.SlotID))
                    {
                        if (combatSlot1 == null)
                            combatSlot1 = characterSlot;
                        else if (characterSlot.SlotID > combatSlot1.SlotID)
                            combatSlot1 = characterSlot;
                    }
                    else if (characterSlot.HasUnit && characterSlot.SlotID < casterSlotID && (!ignoreCastSlot || casterSlotID != characterSlot.SlotID))
                    {
                        if (combatSlot2 == null)
                            combatSlot2 = characterSlot;
                        else if (characterSlot.SlotID < combatSlot2.SlotID)
                            combatSlot2 = characterSlot;
                    }
                }
            }
            else
            {
                foreach (CombatSlot enemySlot in slots.EnemySlots)
                {
                    if (enemySlot.HasUnit && enemySlot.SlotID > casterSlotID && (!ignoreCastSlot || casterSlotID != enemySlot.SlotID))
                    {
                        if (combatSlot1 == null)
                            combatSlot1 = enemySlot;
                        else if (enemySlot.SlotID > combatSlot1.SlotID)
                            combatSlot1 = enemySlot;
                    }
                    else if (enemySlot.HasUnit && enemySlot.SlotID < casterSlotID && (!ignoreCastSlot || casterSlotID != enemySlot.SlotID))
                    {
                        if (combatSlot2 == null)
                            combatSlot2 = enemySlot;
                        else if (enemySlot.SlotID < combatSlot2.SlotID)
                            combatSlot2 = enemySlot;
                    }
                }
            }
            if (combatSlot1 != null && !LeftOnly)
                targetSlotInfoList.Add(combatSlot1.TargetSlotInformation);
            if (combatSlot2 != null && !RightOnly)
                targetSlotInfoList.Add(combatSlot2.TargetSlotInformation);
            if (FarthestOnly && combatSlot1 != null && combatSlot2 != null)
            {
                int num1 = combatSlot1.SlotID - casterSlotID;
                int num2 = casterSlotID - combatSlot2.SlotID;
                if (num1 != num2)
                    targetSlotInfoList.Clear();
                if (num1 > num2)
                    targetSlotInfoList.Add(combatSlot1.TargetSlotInformation);
                else if (num2 > num1)
                    targetSlotInfoList.Add(combatSlot2.TargetSlotInformation);
            }
            return targetSlotInfoList.ToArray();
        }
    }
    public class TargettingStrongestUnit : Targetting_ByUnit_Side
    {
        public bool OnlyOne;

        public override TargetSlotInfo[] GetTargets(SlotsCombat slots, int casterSlotID, bool isCasterCharacter)
        {
            List<TargetSlotInfo> list = new List<TargetSlotInfo>();
            foreach (TargetSlotInfo target in base.GetTargets(slots, casterSlotID, isCasterCharacter))
            {
                if (target != null && target.HasUnit)
                {
                    if (list.Count <= 0)
                        list.Add(target);
                    else if (list[0].Unit.CurrentHealth < target.Unit.CurrentHealth)
                    {
                        list.Clear();
                        list.Add(target);
                    }
                    else if (list[0].Unit.CurrentHealth == target.Unit.CurrentHealth)
                        list.Add(target);
                }
            }
            if (list.Count <= 0)
                return new TargetSlotInfo[0];
            if (!OnlyOne)
                return list.ToArray();
            return new TargetSlotInfo[1]
            {
                list.GetRandom()
            };
        }
    }
    public class TargettingWeakestUnit : Targetting_ByUnit_Side
    {
        public bool OnlyOne;

        public override TargetSlotInfo[] GetTargets(SlotsCombat slots, int casterSlotID, bool isCasterCharacter)
        {
            List<TargetSlotInfo> list = new List<TargetSlotInfo>();
            foreach (TargetSlotInfo target in base.GetTargets(slots, casterSlotID, isCasterCharacter))
            {
                if (target != null && target.HasUnit)
                {
                    if (list.Count <= 0)
                        list.Add(target);
                    else if (list[0].Unit.CurrentHealth > target.Unit.CurrentHealth)
                    {
                        list.Clear();
                        list.Add(target);
                    }
                    else if (list[0].Unit.CurrentHealth == target.Unit.CurrentHealth)
                        list.Add(target);
                }
            }
            if (list.Count <= 0)
                return new TargetSlotInfo[0];
            if (!OnlyOne)
                return list.ToArray();
            return new TargetSlotInfo[1]
            {
                list.GetRandom()
            };
        }
    }
    public class TargettingUnitsWithStatusEffectAll : BaseCombatTargettingSO
    {
        public bool ignoreCastSlot = false;
        public StatusEffectType targetStatus;

        public override bool AreTargetAllies => false;

        public override bool AreTargetSlots => false;

        public static bool IsUnitAlreadyContained(List<TargetSlotInfo> targets, TargetSlotInfo target)
        {
            foreach (TargetSlotInfo target1 in targets)
            {
                if (target1.Unit == target.Unit)
                    return true;
            }
            return false;
        }

        public bool IsCastSlot(int caster, TargetSlotInfo target)
        {
            return this.ignoreCastSlot && caster == target.SlotID;
        }

        public override TargetSlotInfo[] GetTargets(SlotsCombat slots, int casterSlotID, bool isCasterCharacter)
        {
            List<TargetSlotInfo> targets = new List<TargetSlotInfo>();
            foreach (CombatSlot characterSlot in slots.CharacterSlots)
            {
                TargetSlotInfo targetSlotInformation = characterSlot.TargetSlotInformation;
                if (targetSlotInformation != null && targetSlotInformation.HasUnit && !IsUnitAlreadyContained(targets, targetSlotInformation) && !IsCastSlot(casterSlotID, targetSlotInformation) && targetSlotInformation.Unit.ContainsStatusEffect(targetStatus))
                    targets.Add(targetSlotInformation);
            }
            foreach (CombatSlot enemySlot in slots.EnemySlots)
            {
                TargetSlotInfo targetSlotInformation = enemySlot.TargetSlotInformation;
                if (targetSlotInformation != null && targetSlotInformation.HasUnit && !IsUnitAlreadyContained(targets, targetSlotInformation) && !IsCastSlot(casterSlotID, targetSlotInformation) && targetSlotInformation.Unit.ContainsStatusEffect(targetStatus))
                    targets.Add(targetSlotInformation);
            }
            return targets.ToArray();
        }
    }
    public class TargettingUnitsWithStatusEffectSide : Targetting_ByUnit_Side
    {
        public StatusEffectType targetStatus;

        public static bool IsUnitAlreadyContained(List<TargetSlotInfo> targets, TargetSlotInfo target)
        {
            foreach (TargetSlotInfo target1 in targets)
            {
                if (target1.Unit == target.Unit)
                    return true;
            }
            return false;
        }

        public override TargetSlotInfo[] GetTargets(SlotsCombat slots, int casterSlotID, bool isCasterCharacter)
        {
            List<TargetSlotInfo> targets = new List<TargetSlotInfo>();
            foreach (TargetSlotInfo target in base.GetTargets(slots, casterSlotID, isCasterCharacter))
            {
                if (target != null && target.HasUnit && !IsUnitAlreadyContained(targets, target) && target.Unit.ContainsStatusEffect(targetStatus))
                    targets.Add(target);
            }
            return targets.ToArray();
        }
    }
    public class TargettingByConditionStatus : BaseCombatTargettingSO
    {
        public BaseCombatTargettingSO orig;
        public StatusEffectType status = StatusEffectType.Cursed;
        public bool Has;
        public bool EmptySlots;

        public override bool AreTargetAllies => this.orig.AreTargetAllies;

        public override bool AreTargetSlots => this.orig.AreTargetSlots;

        public override TargetSlotInfo[] GetTargets(SlotsCombat slots, int casterSlotID, bool isCasterCharacter)
        {
            TargetSlotInfo[] targets = orig.GetTargets(slots, casterSlotID, isCasterCharacter);
            List<TargetSlotInfo> targetSlotInfoList = new List<TargetSlotInfo>();
            foreach (TargetSlotInfo targetSlotInfo in targets)
            {
                if (targetSlotInfo.HasUnit && this.Has == targetSlotInfo.Unit.ContainsStatusEffect(status, 0))
                    targetSlotInfoList.Add(targetSlotInfo);
                else if (!targetSlotInfo.HasUnit && EmptySlots)
                    targetSlotInfoList.Add(targetSlotInfo);
            }
            return targetSlotInfoList.ToArray();
        }

        public static TargettingByConditionStatus Create(
          BaseCombatTargettingSO orig,
          StatusEffectType status,
          bool Has = true,
          bool empties = false)
        {
            TargettingByConditionStatus instance = CreateInstance<TargettingByConditionStatus>();
            instance.orig = orig;
            instance.status = status;
            instance.Has = Has;
            instance.EmptySlots = empties;
            return instance;
        }
    }
    public class TargettingByHasUnit : BaseCombatTargettingSO
    {
        public BaseCombatTargettingSO source;

        public override bool AreTargetAllies => source.AreTargetAllies;

        public override bool AreTargetSlots => source.AreTargetSlots;

        public override TargetSlotInfo[] GetTargets(SlotsCombat slots, int casterSlotID, bool isCasterCharacter)
        {
            TargetSlotInfo[] targets = source.GetTargets(slots, casterSlotID, isCasterCharacter);
            List<TargetSlotInfo> targetSlotInfoList = new List<TargetSlotInfo>();
            foreach (TargetSlotInfo targetSlotInfo in targets)
            {
                if (targetSlotInfo.HasUnit)
                    targetSlotInfoList.Add(targetSlotInfo);
            }
            return targetSlotInfoList.ToArray();
        }

        public static TargettingByHasUnit Create(BaseCombatTargettingSO orig)
        {
            TargettingByHasUnit instance = CreateInstance<TargettingByHasUnit>();
            instance.source = orig;
            return instance;
        }
    }
    public class TargettingRandomUnit : BaseCombatTargettingSO
    {
        public bool getAllies;
        public bool ignoreCastSlot = false;
        public static TargetSlotInfo LastRandom;

        public override bool AreTargetAllies => getAllies;

        public override bool AreTargetSlots => false;

        public static bool IsUnitAlreadyContained(List<TargetSlotInfo> targets, TargetSlotInfo target)
        {
            foreach (TargetSlotInfo target1 in targets)
            {
                if (target1.Unit == target.Unit)
                    return true;
            }
            return false;
        }

        public bool IsCastSlot(int caster, TargetSlotInfo target)
        {
            return ignoreCastSlot && caster == target.SlotID;
        }

        public override TargetSlotInfo[] GetTargets(SlotsCombat slots, int casterSlotID, bool isCasterCharacter)
        {
            List<TargetSlotInfo> targets = new List<TargetSlotInfo>();
            if (getAllies & isCasterCharacter || !getAllies && !isCasterCharacter)
            {
                foreach (CombatSlot characterSlot in slots.CharacterSlots)
                {
                    TargetSlotInfo targetSlotInformation = characterSlot.TargetSlotInformation;
                    if (targetSlotInformation != null && targetSlotInformation.HasUnit && !IsUnitAlreadyContained(targets, targetSlotInformation) && !IsCastSlot(casterSlotID, targetSlotInformation))
                        targets.Add(targetSlotInformation);
                }
            }
            else
            {
                foreach (CombatSlot enemySlot in slots.EnemySlots)
                {
                    TargetSlotInfo targetSlotInformation = enemySlot.TargetSlotInformation;
                    if (targetSlotInformation != null && targetSlotInformation.HasUnit && !IsUnitAlreadyContained(targets, targetSlotInformation) && !IsCastSlot(casterSlotID, targetSlotInformation))
                        targets.Add(targetSlotInformation);
                }
            }
            if (targets.Count <= 0)
            {
                LastRandom = null;
                return new TargetSlotInfo[0];
            }
            TargetSlotInfo targetSlotInfo = targets[UnityEngine.Random.Range(0, targets.Count)];
            LastRandom = targetSlotInfo;
            return new TargetSlotInfo[1] { targetSlotInfo };
        }
    }
    public class TargettingByTargetting : BaseCombatTargettingSO
    {
        public BaseCombatTargettingSO first;
        public BaseCombatTargettingSO second;
        public bool OnlyIfUnit;

        public override bool AreTargetSlots => second.AreTargetSlots;

        public override bool AreTargetAllies => (first.AreTargetAllies && second.AreTargetAllies) || (!first.AreTargetAllies && !second.AreTargetAllies);

        public override TargetSlotInfo[] GetTargets(SlotsCombat slots, int casterSlotID, bool isCasterCharacter)
        {
            TargetSlotInfo[] targets = first.GetTargets(slots, casterSlotID, isCasterCharacter);
            List<TargetSlotInfo> targetSlotInfoList = new List<TargetSlotInfo>();
            foreach (TargetSlotInfo targetSlotInfo in targets)
            {
                if (targetSlotInfo.HasUnit || !OnlyIfUnit)
                {
                    foreach (TargetSlotInfo target in second.GetTargets(slots, targetSlotInfo.HasUnit ? targetSlotInfo.Unit.SlotID : targetSlotInfo.SlotID, targetSlotInfo.IsTargetCharacterSlot))
                        targetSlotInfoList.Add(target);
                }
            }
            return targetSlotInfoList.ToArray();
        }

        public static TargettingByTargetting Create(BaseCombatTargettingSO first, BaseCombatTargettingSO second)
        {
            TargettingByTargetting instance = CreateInstance<TargettingByTargetting>();
            instance.first = first;
            instance.second = second;
            return instance;
        }
    }
}
