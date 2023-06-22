using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using HarmonyLib;
using LOR_XML;
using Mod;
using UnityEngine;

namespace EmotionCardUtil
{
    public static class LocalizationUtil
    {
        internal static OnLanguageChange OnLocalize;

        public static void RemoveError()
        {
            Singleton<ModContentManager>.Instance.GetErrorLogs().RemoveAll(x => new List<string>
            {
                "0Harmony",
                "Mono.Cecil",
                "MonoMod.RuntimeDetour",
                "MonoMod.Utils",
                "MonoMod.Common",
                "CustomMapUtility",
                "NAudio"
            }.Exists(y => x.Contains("The same assembly name already exists. : " + y)));
            Singleton<ModContentManager>.Instance.GetErrorLogs().RemoveAll(x => new List<string>
            {
                "EmotionCardUtil"
            }.Exists(x.Contains));
        }

        public static void AddOnLocalizeAction(Action<string> e)
        {
            Action<string> @object = delegate(string x)
            {
                try
                {
                    e?.Invoke(x);
                }
                catch (Exception ex)
                {
                    Debug.LogError(" - An error occurred while executing localize action:\n" + ex);
                }
            };
            OnLocalize = (OnLanguageChange)Delegate.Combine(OnLocalize, new OnLanguageChange(@object.Invoke));
        }

        public static void AddGlobalLocalize(string packageId = "")
        {
            foreach (var item in ModParameters.Path.Where(x =>
                         string.IsNullOrEmpty(packageId) || x.Key.Equals(packageId)))
            {
                var localizedItem = new LocalizedItem();
                var error = false;
                FileInfo file;
                try
                {
                    var dictionary = Singleton<BattleEffectTextsXmlList>.Instance._dictionary;
                    file = new DirectoryInfo(item.Value + "/EmotionCardUtil/" + "/Localize/" + ModParameters.Language +
                                             "/EffectTexts")
                        .GetFiles().FirstOrDefault();
                    error = true;
                    if (file != null)
                        using (var stringReader = new StringReader(File.ReadAllText(file.FullName)))
                        {
                            var battleEffectTextRoot =
                                (BattleEffectTextRoot)new XmlSerializer(typeof(BattleEffectTextRoot))
                                    .Deserialize(stringReader);
                            foreach (var battleEffectText in battleEffectTextRoot.effectTextList)
                            {
                                dictionary.Remove(battleEffectText.ID);
                                dictionary?.Add(battleEffectText.ID, battleEffectText);
                                localizedItem.EffectTexts.Add(battleEffectText.ID, new EffectText
                                {
                                    Name = battleEffectText.Name,
                                    Desc = battleEffectText.Desc
                                });
                            }
                        }
                }
                catch (Exception ex)
                {
                    if (error)
                        Debug.LogError("Error loading Effect Texts packageId : " + item.Key + " Language : " +
                                       ModParameters.Language + " Error : " + ex.Message);
                }

                try
                {
                    error = false;
                    var dictionary2 = Singleton<AbnormalityCardDescXmlList>.Instance._dictionary;
                    file = new DirectoryInfo(item.Value + "/EmotionCardUtil/" + "/Localize/" + ModParameters.Language +
                                             "/AbnormalityCards")
                        .GetFiles().FirstOrDefault();
                    error = true;
                    if (file != null)
                        using (var stringReader2 = new StringReader(File.ReadAllText(file.FullName)))
                        {
                            foreach (var abnormalityCard in ((AbnormalityCardsRoot)new XmlSerializer(
                                         typeof(AbnormalityCardsRoot)).Deserialize(stringReader2)).sephirahList
                                     .SelectMany(sephirah => sephirah.list))
                            {
                                dictionary2[abnormalityCard.id] = abnormalityCard;
                                localizedItem.AbnormalityCards.Add(abnormalityCard);
                            }
                        }
                }
                catch (Exception ex2)
                {
                    if (error)
                        Debug.LogError(string.Concat("Error loading Abnormality Text packageId : ", packageId,
                            " Language : ", ModParameters.Language, " Error : ", ex2.Message));
                }

                try
                {
                    error = false;
                    file = new DirectoryInfo(item.Value + "/EmotionCardUtil/" + "/Localize/" + ModParameters.Language +
                                             "/BattlesCards")
                        .GetFiles().FirstOrDefault();
                    error = true;
                    if (file != null)
                        using (var stringReader2 = new StringReader(File.ReadAllText(file.FullName)))
                        {
                            var battleCardDescRoot =
                                (BattleCardDescRoot)new XmlSerializer(typeof(BattleCardDescRoot)).Deserialize(
                                    stringReader2);
                            using (var enumerator =
                                   ItemXmlDataList.instance.GetAllWorkshopData()[item.Key].GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    var card = enumerator.Current;
                                    card.workshopName = battleCardDescRoot.cardDescList
                                        .Find(x => x.cardID == card.id.id)
                                        .cardName;
                                    localizedItem.CardNames.Add(card.id.id, card.workshopName);
                                }
                            }

                            using (var enumerator2 = ItemXmlDataList.instance.GetCardList()
                                       .FindAll(x => x.id.packageId == item.Key).GetEnumerator())
                            {
                                while (enumerator2.MoveNext())
                                {
                                    var card = enumerator2.Current;
                                    card.workshopName = battleCardDescRoot.cardDescList
                                        .Find(x => x.cardID == card.id.id)
                                        .cardName;
                                    ItemXmlDataList.instance.GetCardItem(card.id).workshopName = card.workshopName;
                                }
                            }
                        }
                }
                catch (Exception ex)
                {
                    if (error)
                        Debug.LogError("Error loading Battle Cards Texts packageId : " + item.Key + " Language : " +
                                       ModParameters.Language + " Error : " + ex.Message);
                }

                try
                {
                    error = false;
                    var cardAbilityDictionary = Singleton<BattleCardAbilityDescXmlList>.Instance._dictionary;
                    file = new DirectoryInfo(item.Value + "/EmotionCardUtil/" + "/Localize/" + ModParameters.Language +
                                             "/BattleCardAbilities").GetFiles().FirstOrDefault();
                    error = true;
                    if (file != null)
                        using (var stringReader8 = new StringReader(File.ReadAllText(file.FullName)))
                        {
                            foreach (var battleCardAbilityDesc in
                                     ((BattleCardAbilityDescRoot)new XmlSerializer(typeof(BattleCardAbilityDescRoot))
                                         .Deserialize(stringReader8)).cardDescList)
                            {
                                cardAbilityDictionary.Remove(battleCardAbilityDesc.id);
                                cardAbilityDictionary.Add(battleCardAbilityDesc.id, battleCardAbilityDesc);
                                localizedItem.BattleCardAbilitiesText.Add(battleCardAbilityDesc.id,
                                    battleCardAbilityDesc.desc);
                            }
                        }
                }
                catch (Exception ex)
                {
                    if (error)
                        Debug.LogError("Error loading Battle Card Abilities Texts packageId : " + item.Key +
                                       " Language : " + ModParameters.Language + " Error : " + ex.Message);
                }

                ModParameters.LocalizedItems.Remove(item.Key);
                ModParameters.LocalizedItems.Add(item.Key, localizedItem);
            }
        }

        public static void InitKeywordsList(List<Assembly> assemblies)
        {
            var dictionary = BattleCardAbilityDescXmlList.Instance._dictionaryKeywordCache;
            foreach (var assembly in assemblies)
            {
                assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(DiceCardSelfAbilityBase))
                                               && x.Name.StartsWith("DiceCardSelfAbility_"))
                    .Do(x => dictionary[x.Name.Replace("DiceCardSelfAbility_", "")] =
                        new List<string>(((DiceCardSelfAbilityBase)Activator.CreateInstance(x)).Keywords));
                assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(DiceCardAbilityBase))
                                               && x.Name.StartsWith("DiceCardAbility_"))
                    .Do(x => dictionary[x.Name.Replace("DiceCardAbility_", "")] =
                        new List<string>(((DiceCardAbilityBase)Activator.CreateInstance(x)).Keywords));
            }
        }

        public static void LoadLocalization(string language)
        {
            ModParameters.Language = language;
            AddGlobalLocalize();
            InitKeywordsList(ModParameters.Assemblies);
        }

        internal delegate void OnLanguageChange(string language);
    }
}