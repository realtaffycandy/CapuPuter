using Il2CppInterop.Runtime.Injection;
using MelonLoader;

namespace CapuPuterLemon
{
    public class Main : MelonMod
    {
        public static MenuHandler handler;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("CapuPuter Loaded!");
            base.OnInitializeMelon();

            #region Custom Behaviours
            ClassInjector.RegisterTypeInIl2Cpp<MenuButtonKey>();
            #endregion

            HarmonyManager.Init();
        }
    }
}