using UnityEngine;
using UnityEngine.SceneManagement;

namespace EmotionCardUtil
{
    public class EmotionCardManager : MonoBehaviour
    {
        private static EmotionCardManager _instance;
        private string _language = string.Empty;

        public static void Init()
        {
            if (_instance != null) return;
            //EmotionCardInternalUtil.PutUtilInTheFirstSlot();
            EmotionCardInternalUtil.OtherModCheck();
            InitGameObject();
            EmotionCardUtilLoader.LoadMods();
            Patch();
            LocalizationUtil.RemoveError();
            SceneManager.sceneLoaded += EmotionCardInternalUtil.OnLoadingScreen;
        }

        private static void InitGameObject()
        {
            var gameObject = new GameObject("LoR.EmotionCardManager");
            DontDestroyOnLoad(gameObject);
            _instance = gameObject.AddComponent<EmotionCardManager>();
        }

        private static void Patch()
        {
            ModParameters.Harmony.CreateClassProcessor(typeof(EmotionCardHarmonyPatch)).Patch();
        }

        private void Update()
        {
            if (SceneManager.GetActiveScene().name != "Stage_Hod_New" ||
                _language == GlobalGameManager.Instance.CurrentOption.language) return;
            _language = GlobalGameManager.Instance.CurrentOption.language;
            LocalizationUtil.LoadLocalization(GlobalGameManager.Instance.CurrentOption.language);
            var onLocalize = LocalizationUtil.OnLocalize;
            onLocalize?.Invoke(GlobalGameManager.Instance.CurrentOption.language);
        }
    }
}