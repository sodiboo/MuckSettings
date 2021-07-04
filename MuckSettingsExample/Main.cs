using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;
using MuckSetting = MuckSettings.Settings;

namespace MuckSettingsExample
{
    [BepInPlugin(Guid, Name, Version)]
    public class Main : BaseUnityPlugin
    {
        public const string
            Name = "MuckSettingsExample",
            Author = "Terrain",
            Guid = Author + "." + Name,
            Version = "1.0.0.0";

        internal readonly ManualLogSource log;
        internal readonly Harmony harmony;
        internal readonly Assembly assembly;
        public readonly string modFolder;
        public static string config;

        public static KeyCode bind = KeyCode.X;

        Main()
        {
            log = Logger;
            harmony = new Harmony(Guid);
            assembly = Assembly.GetExecutingAssembly();
            modFolder = Path.GetDirectoryName(assembly.Location);

            config = Path.Combine(modFolder, "config");

            if (File.Exists(config))
            {
                bind = (KeyCode)int.Parse(File.ReadAllText(config));
            }
            else
            {
                File.WriteAllText(config, ((int)bind).ToString());
            }

            harmony.PatchAll(assembly);
        }

        public static void UpdateBind(KeyCode newBind)
        {
            bind = newBind;
            File.WriteAllText(config, ((int)bind).ToString());
        }
    }

    [HarmonyPatch]
    class Patches
    {
        [HarmonyPatch(typeof(MuckSetting), "Controls"), HarmonyPrefix]
        static void Controls(MuckSetting.Page page)
        {
            page.AddControlSetting("PoC", Main.bind, Main.UpdateBind);
        }

        [HarmonyPatch(typeof(PlayerInput), "Update"), HarmonyPostfix]
        static void PlayerInput() {
            if (Input.GetKeyDown(Main.bind)) {
                ChatBox.Instance.AppendMessage(-1, "You pressed the button!", "");
            }
        }
    }
}