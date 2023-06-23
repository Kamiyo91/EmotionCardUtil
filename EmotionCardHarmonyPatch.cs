using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UI;

namespace EmotionCardUtil
{
    [HarmonyPatch]
    public class EmotionCardHarmonyPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(EmotionPassiveCardUI), "SetSprites")]
        [HarmonyPatch(typeof(UIEmotionPassiveCardInven), "SetSprites")]
        public static void EmotionPassiveCardUI_SetSprites(object __instance)
        {
            switch (__instance)
            {
                case EmotionPassiveCardUI instance:
                    if (!(instance.Card is EmotionCardXmlExtension cardExtension)) return;
                    EmotionCardInternalUtil.EmotionPassiveCardUISetSprites(instance, cardExtension);
                    break;
                case UIEmotionPassiveCardInven instance:
                    if (!(instance.Card is EmotionCardXmlExtension cardExtension2)) return;
                    EmotionCardInternalUtil.EmotionPassiveCardUISetSprites(instance, cardExtension2);
                    break;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIAbnormalityCardPreviewSlot), "Init")]
        public static void UIAbnormalityCardPreviewSlot_Init(UIAbnormalityCardPreviewSlot __instance, object card)
        {
            if (!(card is EmotionCardXmlExtension cardExtension)) return;
            __instance.artwork.sprite =
                Singleton<CustomizingCardArtworkLoader>.Instance.GetSpecificArtworkSprite(cardExtension.LorId.packageId,
                    cardExtension.Artwork);
        }

        [HarmonyPatch(typeof(BattleEmotionCardModel), MethodType.Constructor, typeof(EmotionCardXmlInfo),
            typeof(BattleUnitModel))]
        [HarmonyPrefix]
        public static void BattleEmotionCardModel_ctor_Pre(BattleEmotionCardModel __instance,
            EmotionCardXmlInfo xmlInfo, ref List<string> __state)
        {
            __state = new List<string>();
            var remove = new List<string>();
            using (var enumerator = xmlInfo.Script.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var text = enumerator.Current;
                    if (string.IsNullOrEmpty(text)) continue;
                    if (!ModParameters.EmotionCardAbility.TryGetValue("EmotionCardAbility_" + text.Trim(),
                            out _)) continue;
                    __state.Add(text);
                    remove.Add(text);
                }
            }

            xmlInfo.Script.RemoveAll(x => remove.Contains(x));
        }

        [HarmonyPatch(typeof(BattleEmotionCardModel), MethodType.Constructor, typeof(EmotionCardXmlInfo),
            typeof(BattleUnitModel))]
        [HarmonyPostfix]
        public static void BattleEmotionCardModel_ctor_Post(BattleEmotionCardModel __instance,
            EmotionCardXmlInfo xmlInfo, ref List<string> __state)
        {
            using (var enumerator = __state.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var text = enumerator.Current;
                    if (string.IsNullOrEmpty(text)) continue;
                    if (!ModParameters.EmotionCardAbility.TryGetValue("EmotionCardAbility_" + text.Trim(),
                            out var abilityType)) continue;
                    var ability = (EmotionCardAbilityBase)Activator.CreateInstance(abilityType);
                    ability.SetEmotionCard(__instance);
                    __instance._abilityList.Add(ability);
                }
            }

            xmlInfo.Script.AddRange(__state);
        }

        [HarmonyPatch(typeof(UIEgoCardPreviewSlot), "Init")]
        [HarmonyPostfix]
        public static void UIEgoCardPreviewSlot_Init(UIEgoCardPreviewSlot __instance, DiceCardItemModel cardModel)
        {
            if (cardModel?.ClassInfo == null) return;
            if (!ModParameters.EmotionEgoCards.Any(x =>
                    x.PackageId == cardModel.GetID().packageId && x.id == cardModel.GetID().id)) return;
            __instance.cardName.text = cardModel.ClassInfo.Name;
            __instance.cardCost.text = cardModel.GetSpec().Cost.ToString();
            __instance.artwork.sprite =
                Singleton<CustomizingCardArtworkLoader>.Instance.GetSpecificArtworkSprite(
                    cardModel.ClassInfo.workshopID, cardModel.GetArtworkSrc());
        }

        [HarmonyPatch(typeof(EmotionEgoXmlInfo), "CardId", MethodType.Getter)]
        [HarmonyPostfix]
        public static void EmotionEgoXmlInfo_get_CardId(object __instance, ref LorId __result)
        {
            if (!(__instance is EmotionEgoCardXmlExtension card)) return;
            __result = new LorId(card.PackageId, card.id);
        }
    }
}