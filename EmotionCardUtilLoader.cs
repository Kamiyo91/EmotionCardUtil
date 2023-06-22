using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Mod;
using Debug = UnityEngine.Debug;

namespace EmotionCardUtil
{
    public static class EmotionCardUtilLoader
    {
        private static readonly List<string> IgnoreDll = new List<string>
        {
            "0Harmony", "Mono.Cecil", "MonoMod.RuntimeDetour", "MonoMod.Utils", "1BigDLL4221", "1SMotion-Loader",
            ModParameters.EmotionUtilDLLName
        };

        public static void LoadMods()
        {
            foreach (var modContentInfo in Singleton<ModContentManager>.Instance.GetAllMods().Where(modContentInfo =>
                         modContentInfo.activated &&
                         modContentInfo.invInfo.workshopInfo.uniqueId != ModParameters.EmotionUtilPackageId &&
                         Directory.Exists(modContentInfo.dirInfo.FullName + "/Assemblies/EmotionCardUtil")))
                try
                {
                    var modId = modContentInfo.invInfo.workshopInfo.uniqueId;
                    var path = modContentInfo.dirInfo.FullName + "/Assemblies";
                    ModParameters.Path.Add(modId, path);
                    var stopwatch = new Stopwatch();
                    Debug.Log($"EmotionCardUtil : Start loading mod files {modId} at path {path}");
                    stopwatch.Start();
                    var directoryInfo = new DirectoryInfo(path);
                    var assemblies = (from fileInfo in directoryInfo.GetFiles()
                        where fileInfo.Extension.ToLower() == ".dll" && !IgnoreDll.Contains(fileInfo.FullName)
                        select Assembly.LoadFile(fileInfo.FullName)).ToList();
                    //EmotionCardInternalUtil.LoadEmotionAndEgoCards(modId, path + "/EmotionCardUtil" + "/EmotionCards",
                    //    assemblies);
                    LocalizationUtil.AddGlobalLocalize(modId);
                    LocalizationUtil.InitKeywordsList(assemblies);
                    ModParameters.Assemblies.AddRange(assemblies);
                    stopwatch.Stop();
                    Debug.Log(
                        $"EmotionCardUtil : Loading mod files {modId} at path {path} finished in {stopwatch.ElapsedMilliseconds} ms");
                }
                catch (Exception ex)
                {
                    Debug.LogError(
                        $"Error while loading the mod {modContentInfo.invInfo.workshopInfo.uniqueId} - {ex.Message}");
                }
        }

        public static void LoadModsAfter()
        {
            foreach (var modContentInfo in Singleton<ModContentManager>.Instance.GetAllMods().Where(modContentInfo =>
                         modContentInfo.activated &&
                         modContentInfo.invInfo.workshopInfo.uniqueId != ModParameters.EmotionUtilPackageId &&
                         Directory.Exists(modContentInfo.dirInfo.FullName + "/Assemblies/EmotionCardUtil")))
                try
                {
                    var modId = modContentInfo.invInfo.workshopInfo.uniqueId;
                    var path = modContentInfo.dirInfo.FullName + "/Assemblies";
                    var directoryInfo = new DirectoryInfo(path);
                    var assemblies = (from fileInfo in directoryInfo.GetFiles()
                        where fileInfo.Extension.ToLower() == ".dll" && !IgnoreDll.Contains(fileInfo.FullName)
                        select Assembly.LoadFile(fileInfo.FullName)).ToList();
                    EmotionCardInternalUtil.LoadEmotionAndEgoCards(modId, path + "/EmotionCardUtil" + "/EmotionCards",
                        assemblies);
                }
                catch (Exception)
                {
                    // ignored
                }
        }
    }
}