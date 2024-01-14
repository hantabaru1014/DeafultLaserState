using FrooxEngine;
using HarmonyLib;
using ResoniteModLoader;

namespace Default_Laser_State
{
    public class Patch : ResoniteMod
    {
        public override string Author => "LeCloutPanda";
        public override string Name => "Default Laser State";
        public override string Version => "1.1.0";

        [AutoRegisterConfigKey]
        public static ModConfigurationKey<bool> ENABLED = new ModConfigurationKey<bool>("Default state", "What state will the lasers have", () => true);
        [AutoRegisterConfigKey]
        public static ModConfigurationKey<bool> ENABLED_LEFT = new ModConfigurationKey<bool>("Default left state", "What state will the left laser have", () => true);
        [AutoRegisterConfigKey]
        public static ModConfigurationKey<bool> ENABLED_RIGHT = new ModConfigurationKey<bool>("Default right state", "What state will the right laser have", () => true);

        public static ModConfiguration? config;

        public override void OnEngineInit()
        {
            config = GetConfiguration();
            config!.Save(true);

            Harmony harmony = new Harmony("dev.lecloutpanda.defaultlaserstate");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(InteractionHandler), "OnAwake")]
        class CommonToolPatch
        {
            [HarmonyPostfix]
            static void Postfix(InteractionHandler __instance, Sync<bool> ____laserEnabled)
            {
                __instance.RunInUpdates(3, () =>
                {
                    if (__instance.Slot.ActiveUser != __instance.LocalUser)
                        return;

                    ____laserEnabled.Value = config!.GetValue(ENABLED) && __instance.Side == Chirality.Left ? config!.GetValue(ENABLED_LEFT) : config!.GetValue(ENABLED_RIGHT);
                });
            }
        }
    }
}