namespace EmotionCardUtil
{
    public class ModInit : ModInitializer
    {
        public override void OnInitializeMod()
        {
            EmotionCardManager.Init();
        }
    }
}