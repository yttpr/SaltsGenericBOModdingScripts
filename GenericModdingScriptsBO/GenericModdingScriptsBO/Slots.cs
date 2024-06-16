using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PYMN13
{
    public static class Slots
    {
        public static BaseCombatTargettingSO Self;

        public static BaseCombatTargettingSO Front;

        public static BaseCombatTargettingSO FrontLeftRight;

        public static BaseCombatTargettingSO LeftRight;

        public static BaseCombatTargettingSO Sides;

        public static BaseCombatTargettingSO Right;

        public static BaseCombatTargettingSO Left;

        public static void Setup()
        {
            Self = LoadedAssetsHandler.GetEnemy("Mung_EN").abilities[1].ability.animationTarget;
            Sides = LoadedAssetsHandler.GetCharacter("Hans_CH").rankedData[0].rankAbilities[2].ability.animationTarget;
            Targetting_BySlot_Index targetting_BySlot_Index = ScriptableObject.CreateInstance(typeof(Targetting_BySlot_Index)) as Targetting_BySlot_Index;
            targetting_BySlot_Index.slotPointerDirections = new int[1];
            Front = targetting_BySlot_Index;
            Targetting_BySlot_Index targetting_BySlot_Index2 = ScriptableObject.CreateInstance(typeof(Targetting_BySlot_Index)) as Targetting_BySlot_Index;
            targetting_BySlot_Index2.slotPointerDirections = new int[2] { -1, 1 };
            LeftRight = targetting_BySlot_Index2;
            Targetting_BySlot_Index targetting_BySlot_Index3 = ScriptableObject.CreateInstance(typeof(Targetting_BySlot_Index)) as Targetting_BySlot_Index;
            targetting_BySlot_Index3.slotPointerDirections = new int[3] { -1, 0, 1 };
            FrontLeftRight = targetting_BySlot_Index3;
            Targetting_BySlot_Index targetting_BySlot_Index4 = ScriptableObject.CreateInstance(typeof(Targetting_BySlot_Index)) as Targetting_BySlot_Index;
            targetting_BySlot_Index4.slotPointerDirections = new int[1] { -1 };
            Left = targetting_BySlot_Index4;
            Targetting_BySlot_Index targetting_BySlot_Index5 = ScriptableObject.CreateInstance(typeof(Targetting_BySlot_Index)) as Targetting_BySlot_Index;
            targetting_BySlot_Index5.slotPointerDirections = new int[1] { 1 };
            Right = targetting_BySlot_Index5;
        }

        public static BaseCombatTargettingSO SlotTarget(int[] slots, bool targetAllies = false)
        {
            Targetting_BySlot_Index targetting_BySlot_Index = ScriptableObject.CreateInstance(typeof(Targetting_BySlot_Index)) as Targetting_BySlot_Index;
            targetting_BySlot_Index.slotPointerDirections = slots;
            targetting_BySlot_Index.getAllies = targetAllies;
            return targetting_BySlot_Index;
        }

        public static BaseCombatTargettingSO LargeSlotTarget(int[] slots)
        {
            CustomOpponentTargetting_BySlot_Index customOpponentTargetting_BySlot_Index = ScriptableObject.CreateInstance(typeof(CustomOpponentTargetting_BySlot_Index)) as CustomOpponentTargetting_BySlot_Index;
            customOpponentTargetting_BySlot_Index._slotPointerDirections = new int[slots.Length];
            for (int i = 0; i < slots.Length; i++)
            {
                customOpponentTargetting_BySlot_Index._slotPointerDirections[i] = 0;
            }
            customOpponentTargetting_BySlot_Index._frontOffsets = slots;
            return customOpponentTargetting_BySlot_Index;
        }
    }
}
