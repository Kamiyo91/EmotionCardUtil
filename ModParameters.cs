using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using LOR_XML;

namespace EmotionCardUtil
{
    public static class ModParameters
    {
        public static Harmony Harmony = new Harmony("LOR.EmotionCardUtil_MOD");
        public static string EmotionUtilPackageId = "LOR.EmotionCardUtilMOD";
        public static string EmotionUtilDLLName = "1EmotionCardUtil";
        public static Dictionary<string, string> Path = new Dictionary<string, string>();
        public static string Language = GlobalGameManager.Instance.CurrentOption.language;
        public static List<Assembly> Assemblies = new List<Assembly>();
        public static Dictionary<string, LocalizedItem> LocalizedItems = new Dictionary<string, LocalizedItem>();
        public static Dictionary<string, Type> EmotionCardAbility = new Dictionary<string, Type>();
        public static List<EmotionCardXmlExtension> EmotionCards = new List<EmotionCardXmlExtension>();
        public static List<EmotionEgoCardXmlExtension> EmotionEgoCards = new List<EmotionEgoCardXmlExtension>();
        public static bool UtilLoaderModFound = false;
        public static bool ModLoaded = false;
    }

    public class LocalizedItem
    {
        public Dictionary<int, string> CardNames { get; set; } = new Dictionary<int, string>();

        public Dictionary<string, List<string>> BattleCardAbilitiesText { get; set; } =
            new Dictionary<string, List<string>>();

        public Dictionary<string, EffectText> EffectTexts { get; set; } = new Dictionary<string, EffectText>();
        public List<AbnormalityCard> AbnormalityCards { get; set; } = new List<AbnormalityCard>();
    }

    public class EffectText
    {
        public string Name { get; set; }
        public string Desc { get; set; }
    }
}