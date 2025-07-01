using System.Reflection;
using HarmonyLib;
using MelonLoader;
//using Harmony = HarmonyLib.Harmony;

public static class HarmonyManager
{
    private static HarmonyLib.Harmony _harmony;
    private static bool isPatched = false;

    public static void Init()
    {
        if (_harmony == null)
        {
            _harmony = new HarmonyLib.Harmony("taffy.capuputer");
            MelonLogger.Msg("Harmony instance created");
        }

        if (!isPatched)
        {
            try
            {
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
                isPatched = true;
                MelonLogger.Msg("Harmony patches applied successfully");
            }
            catch (System.Exception ex)
            {
                MelonLogger.Error($"Failed to apply Harmony patches: {ex.Message}");
            }
        }
    }

    public static void UnPatch()
    {
        if (isPatched && _harmony != null)
        {
            _harmony.UnpatchSelf();
            isPatched = false;
            MelonLogger.Msg("Harmony patches removed");
        }
    }
}