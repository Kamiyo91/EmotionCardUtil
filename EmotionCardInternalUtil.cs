using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using LOR_DiceSystem;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EmotionCardUtil
{
    public static class EmotionCardInternalUtil
    {
        //private static readonly List<string> LoadingOrder = new List<string>
        //    { "UnityExplorer", "HarmonyLoadOrderFix", "BaseMod" };

        public static void EmotionPassiveCardUISetSprites(EmotionPassiveCardUI instance,
            EmotionCardXmlExtension cardExtension)
        {
            instance._artwork.sprite =
                Singleton<CustomizingCardArtworkLoader>.Instance.GetSpecificArtworkSprite(cardExtension.LorId.packageId,
                    cardExtension.Artwork);
        }

        public static void EmotionPassiveCardUISetSprites(UIEmotionPassiveCardInven instance,
            EmotionCardXmlExtension cardExtension)
        {
            instance._artwork.sprite =
                Singleton<CustomizingCardArtworkLoader>.Instance.GetSpecificArtworkSprite(cardExtension.LorId.packageId,
                    cardExtension.Artwork);
        }

        public static void LoadEmotionAndEgoCards(string packageId, string path, List<Assembly> assemblies)
        {
            var error = false;
            try
            {
                var file = new DirectoryInfo(path).GetFiles().FirstOrDefault();
                error = true;
                var changedCardList = new List<EmotionCardXmlExtension>();
                if (file != null)
                {
                    var list = EmotionCardXmlList.Instance._list;
                    using (var stringReader = new StringReader(File.ReadAllText(file.FullName)))
                    {
                        using (var enumerator =
                               ((EmotionCardXmlRootExtension)new XmlSerializer(typeof(EmotionCardXmlRootExtension))
                                   .Deserialize(
                                       stringReader)).EmotionCardXmlList.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                var a = enumerator.Current;
                                if (a == null) continue;
                                var card = CardXmlConverter(packageId, a);
                                card.id = -1;
                                changedCardList.Add(card);
                                list.Add(card);
                            }
                        }
                    }

                    if (changedCardList.Any())
                        ModParameters.EmotionCards.AddRange(changedCardList);
                }
            }
            catch (Exception ex)
            {
                if (error)
                    Debug.LogError("Error loading Emotion Card packageId : " + packageId + " Error : " + ex.Message +
                                   " ---- " + ex.InnerException);
            }

            var changedEgoCardList = new List<EmotionEgoCardXmlExtension>();
            var cardList = ItemXmlDataList.instance._cardInfoList;
            var egoList = EmotionEgoXmlList.Instance._list;
            foreach (var emotionEgoXmlInfo in cardList.FindAll(x =>
                         x.id.packageId == packageId && x.optionList.Contains(CardOption.EGO)).Select(diceCardXmlInfo =>
                         new EmotionEgoCardXmlExtension
                         {
                             id = diceCardXmlInfo.id.id,
                             _CardId = diceCardXmlInfo.id.id,
                             Sephirah = SephirahType.ETC,
                             isLock = false,
                             PackageId = packageId
                         }))
            {
                egoList?.Add(emotionEgoXmlInfo);
                changedEgoCardList.Add(emotionEgoXmlInfo);
            }

            if (changedEgoCardList.Any())
                ModParameters.EmotionEgoCards.AddRange(changedEgoCardList);
            LoadEmotionAbilities(assemblies);
        }

        public static EmotionCardXmlExtension CardXmlConverter(string packageId, EmotionCardXmlInfo cardXml)
        {
            var newXml = new EmotionCardXmlExtension
            {
                LorId = new LorId(packageId, cardXml.id),
                Sephirah = cardXml.Sephirah,
                TargetType = cardXml.TargetType,
                Script = cardXml.Script,
                EmotionLevel = cardXml.EmotionLevel,
                EmotionRate = cardXml.EmotionRate,
                Level = cardXml.Level,
                Locked = cardXml.Locked,
                Name = cardXml.Name,
                _artwork = cardXml._artwork,
                State = cardXml.State,
                id = cardXml.id
            };
            return newXml;
        }

        public static void LoadEmotionAbilities(List<Assembly> assemblies)
        {
            foreach (var type in from assembly in assemblies
                     from type in assembly.GetTypes()
                     where type.Name.Contains("EmotionCardAbility_")
                     select type)
            {
                if (ModParameters.EmotionCardAbility.ContainsKey(type.Name))
                {
                    Debug.LogError("Emotion Script ability with this name already Exist, being overwritten.");
                    ModParameters.EmotionCardAbility.Remove(type.Name);
                }

                ModParameters.EmotionCardAbility.Add(type.Name, type);
            }
        }

        //public static void PutUtilInTheFirstSlot()
        //{
        //    var modContentInfoList = Singleton<ModContentManager>.Instance._allMods;
        //    var modContentInfo =
        //        modContentInfoList?.FirstOrDefault(x =>
        //            x.invInfo.workshopInfo.uniqueId == ModParameters.EmotionUtilPackageId);
        //    if (modContentInfo == null) return;
        //    var index = modContentInfoList.FindIndex(x => !LoadingOrder.Contains(x.invInfo.workshopInfo.uniqueId));
        //    if (index == -1 || modContentInfoList[index] == modContentInfo) return;
        //    modContentInfoList.Remove(modContentInfo);
        //    modContentInfoList.Insert(index, modContentInfo);
        //}
        public static void OtherModCheck()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            ModParameters.UtilLoaderModFound = assemblies.Any(x => x.GetName().Name == "1UtilLoader21341");
        }

        public static void OnLoadingScreen(Scene scene, LoadSceneMode _)
        {
            if (scene.name != "Stage_Hod_New" || ModParameters.ModLoaded) return;
            ModParameters.ModLoaded = true;
            if (!ModParameters.UtilLoaderModFound) EmotionCardUtilLoader.LoadModsAfter();
        }
    }
}