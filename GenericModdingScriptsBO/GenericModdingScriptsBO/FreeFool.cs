using BrutalAPI;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tools;
using UnityEngine;

namespace PYMN13
{
    public static class Backrooms
    {
        public static AssetBundle Assets;
        public static YarnProgram Yarn;
        public static Material Mat;
        public const string Path = "Assets/Rooms/";
        public static string[] Hard = new string[3] { "ZoneDB_Hard_01", "ZoneDB_Hard_02", "ZoneDB_Hard_03" };
        public static string[] Easy = new string[3] { "ZoneDB_01", "ZoneDB_02", "ZoneDB_03" };

        public static void Setup()
        {
            IDetour idetour1 = new Hook(typeof(MainMenuController).GetMethod(nameof(MainMenuController.LoadOldRun), ~BindingFlags.Default), typeof(Backrooms).GetMethod(nameof(LoadOldRun), ~BindingFlags.Default));
            IDetour idetour2 = new Hook(typeof(MainMenuController).GetMethod(nameof(MainMenuController.OnEmbarkPressed), ~BindingFlags.Default), typeof(Backrooms).GetMethod(nameof(LoadOldRun), ~BindingFlags.Default));
            Assets = AssetBundle.LoadFromMemory(new byte[0]);
            Yarn = Assets.LoadAsset<YarnProgram>(Path + "example.yarn");
            Mat = ((LoadedAssetsHandler.GetRoomPrefab(CardType.Flavour, LoadedAssetsHandler.GetBasicEncounter("PervertMessiah_Flavour").encounterRoom) as NPCRoomHandler)._npcSelectable as BasicRoomItem)._renderers[0].material;
            Calibrate();
            Add();
        }

        public static void LoadOldRun(Action<MainMenuController> orig, MainMenuController self)
        {
            orig(self);
            Add();
        }

        public static void Calibrate()
        {
            try { ExampleRoom.Setup(); } catch { Debug.LogError("example room fail setup"); }
        }

        public static void Add()
        {
            try { ExampleRoom.Add(); } catch { Debug.LogError("example room fail add"); }
        }

        public static void AddPool(string name, int zone)
        {
            ZoneBGDataBaseSO zoneDb1 = LoadedAssetsHandler.GetZoneDB(Backrooms.Easy[zone]) as ZoneBGDataBaseSO;
            ZoneBGDataBaseSO zoneDb2 = LoadedAssetsHandler.GetZoneDB(Backrooms.Hard[zone]) as ZoneBGDataBaseSO;
            if (!zoneDb2._FreeFoolsPool.Contains(name))
                zoneDb2._FreeFoolsPool = new List<string>(zoneDb2._FreeFoolsPool) { name }.ToArray();
            if (!zoneDb1._FreeFoolsPool.Contains(name))
                zoneDb1._FreeFoolsPool = new List<string>(zoneDb1._FreeFoolsPool) { name }.ToArray();
        }

        public static void MoreFool(string zone)
        {
            CardTypeInfo cardTypeInfo = new CardTypeInfo();
            cardTypeInfo._cardInfo = new CardInfo()
            {
                cardType = CardType.EventFreeFool,
                pilePosition = PilePositionType.First
            };
            cardTypeInfo._minimumAmount = 40;
            cardTypeInfo._maximumAmount = 40;
            ZoneBGDataBaseSO zoneDb = LoadedAssetsHandler.GetZoneDB(zone) as ZoneBGDataBaseSO;
            List<CardTypeInfo> cardTypeInfoList = new List<CardTypeInfo>(zoneDb._deckInfo._possibleCards) { cardTypeInfo };
            zoneDb._deckInfo._possibleCards = cardTypeInfoList.ToArray();
        }

        public static void BoostFoolAll()
        {
            foreach (string zone in Hard)
                MoreFool(zone);
            foreach (string zone in Easy)
                MoreFool(zone);
        }
    }


    public static class ExampleRoom
    {
        static string Name => "Character";
        static string Files => "Character_CH";
        static Character chara => new Character();
        static int Zone => 0;
        static bool Left => false;
        static bool Center => true;
        public static Color32 Color => new Color32((byte)143, (byte)86, (byte)59, byte.MaxValue);
        static string roomName => Name + "Room";
        static string convoName => Name + "Convo";
        static string encounterName => Name + "Encounter";
        static Sprite Talk => chara.frontSprite;
        static Sprite Portal => chara.unlockedSprite;
        static string Audio => chara.dialogueSound;
        static int ID => (int)chara.entityID;

        static GameObject Base;
        static NPCRoomHandler Room;
        static DialogueSO Dialogue;
        static FreeFoolEncounterSO Free;
        static SpeakerBundle bundle;
        static SpeakerData speaker;
        public static void Setup()
        {
            BrutalAPI.BrutalAPI.AddSignType((SignType)ID, Portal);
            Base = Backrooms.Assets.LoadAsset<GameObject>(Backrooms.Path + Name + "Room.prefab");
            Room = Base.AddComponent<NPCRoomHandler>();
            Room._npcSelectable = Room.transform.GetChild(0).gameObject.AddComponent<BasicRoomItem>();
            Room._npcSelectable._renderers = new SpriteRenderer[1]
            {
                Room._npcSelectable.transform.GetChild(0).GetComponent<SpriteRenderer>()
            };
            Room._npcSelectable._renderers[0].material = Backrooms.Mat;
            DialogueSO log = ScriptableObject.CreateInstance<DialogueSO>();
            log.name = convoName;
            log.dialog = Backrooms.Yarn;
            log.startNode = "Example." + Name + ".TryHire";
            Dialogue = log;
            FreeFoolEncounterSO encounter = ScriptableObject.CreateInstance<FreeFoolEncounterSO>();
            encounter.name = encounterName;
            encounter._dialogue = convoName;
            encounter.encounterRoom = roomName;
            encounter._freeFool = Files;
            encounter.signType = (SignType)ID;
            encounter.npcEntityIDs = new EntityIDs[1] { (EntityIDs) ID };
            Free = encounter;
            bundle = new SpeakerBundle()
            {
                dialogueSound = Audio,
                portrait = Talk,
                bundleTextColor = Color
            };
            SpeakerData speaker = ScriptableObject.CreateInstance<SpeakerData>();
            speaker.speakerName = Name + PathUtils.speakerDataSuffix;
            speaker.name = Name + PathUtils.speakerDataSuffix;
            speaker._defaultBundle = bundle;
            speaker.portraitLooksLeft = Left;
            speaker.portraitLooksCenter = Center;
            ExampleRoom.speaker = speaker;
        }

        public static void Add()
        {
            if (!LoadedAssetsHandler.LoadedRoomPrefabs.Keys.Contains(PathUtils.encounterRoomsResPath + roomName))
                LoadedAssetsHandler.LoadedRoomPrefabs.Add(PathUtils.encounterRoomsResPath + roomName, Room);
            else
                LoadedAssetsHandler.LoadedRoomPrefabs[PathUtils.encounterRoomsResPath + roomName] = Room;
            if (!LoadedAssetsHandler.LoadedDialogues.Keys.Contains(convoName))
                LoadedAssetsHandler.LoadedDialogues.Add(convoName, Dialogue);
            else
                LoadedAssetsHandler.LoadedDialogues[convoName] = Dialogue;
            if (!LoadedAssetsHandler.LoadedFreeFoolEncounters.Keys.Contains(encounterName))
                LoadedAssetsHandler.LoadedFreeFoolEncounters.Add(encounterName, Free);
            else
                LoadedAssetsHandler.LoadedFreeFoolEncounters[encounterName] = Free;
            Backrooms.AddPool(encounterName, Zone);
            if (!LoadedAssetsHandler.LoadedSpeakers.Keys.Contains(speaker.speakerName))
                LoadedAssetsHandler.LoadedSpeakers.Add(speaker.speakerName, speaker);
            else
                LoadedAssetsHandler.LoadedSpeakers[speaker.speakerName] = speaker;
        }
    }
}
