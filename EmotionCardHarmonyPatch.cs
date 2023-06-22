using System;
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
        [HarmonyPostfix]
        public static void BattleEmotionCardModel_ctor_Post(BattleEmotionCardModel __instance,
            EmotionCardXmlInfo xmlInfo)
        {
            using (var enumerator = xmlInfo.Script.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var text = enumerator.Current;
                    if (string.IsNullOrEmpty(text)) continue;
                    if (!ModParameters.EmotionCardAbility.TryGetValue("EmotionCardAbility_" + text.Trim(),
                            out var abilityType)) continue;
                    var ability = (EmotionCardAbilityBase)Activator.CreateInstance(abilityType);
                    ability.SetEmotionCard(__instance);
                    __instance._abilityList.RemoveAll(x =>
                        x.GetType().Name.Substring("EmotionCardAbility_".Length).Trim() == text);
                    __instance._abilityList.Add(ability);
                }
            }
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