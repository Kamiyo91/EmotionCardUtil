using System.Collections.Generic;
using System.Xml.Serialization;

namespace EmotionCardUtil
{
    public class EmotionCardXmlExtension : EmotionCardXmlInfo
    {
        [XmlElement("UsableByBookIds")] public List<LorIdRoot> UsableByBookIds = new List<LorIdRoot>();
        [XmlIgnore] public LorId LorId { get; set; }
    }


    public class EmotionCardXmlRootExtension
    {
        [XmlElement("EmotionCard")] public List<EmotionCardXmlExtension> EmotionCardXmlList;
    }

    public class LorIdRoot
    {
        [XmlAttribute("Id")] public int Id;
        [XmlAttribute("PackageId")] public string PackageId = "";
    }
}