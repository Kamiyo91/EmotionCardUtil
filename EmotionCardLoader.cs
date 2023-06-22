using System.Linq;

namespace EmotionCardUtil
{
    public static class EmotionCardLoader
    {
        public static EmotionCardXmlInfo GetEmotionCard(string packageId, int cardId)
        {
            return ModParameters.EmotionCards.FirstOrDefault(
                x => x.LorId.packageId == packageId && x.LorId.id == cardId);
        }

        public static EmotionEgoXmlInfo GetEmotionEgoCard(string packageId, int cardId)
        {
            return ModParameters.EmotionEgoCards.FirstOrDefault(x => x.PackageId == packageId && x.id == cardId);
        }

        public static bool GetActivatedCustomEmotionCard(this BattleUnitModel owner, string packageId, int id,
            out EmotionCardXmlExtension xmlInfo)
        {
            var cards = owner.emotionDetail.GetSelectedCardList().Where(x => x.XmlInfo is EmotionCardXmlExtension)
                .Select(x => x.XmlInfo as EmotionCardXmlExtension);
            xmlInfo = cards.FirstOrDefault(x => x != null && x.LorId == new LorId(packageId, id));
            return xmlInfo != null;
        }
    }
}