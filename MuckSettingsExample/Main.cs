using BepInEx;
using BepInEx.Configuration;
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

        public static ConfigFile config = new ConfigFile(Path.Combine(Paths.ConfigPath, "example.cfg"), true);
        public static ConfigEntry<KeyCode> example = config.Bind<KeyCode>("Example", "example", KeyCode.X, "Example button that sends a chat message.");

        Main()
        {
            log = Logger;
            harmony = new Harmony(Guid);
            assembly = Assembly.GetExecutingAssembly();
            modFolder = Path.GetDirectoryName(assembly.Location);

            // this line is very important, anyone using this as an example shouldn't forget to copy-paste this as well!
            config.SaveOnConfigSet = true;

            harmony.PatchAll(assembly);
        }
    }

    [HarmonyPatch]
    class Patches
    {
        [HarmonyPatch(typeof(MuckSetting), "Controls"), HarmonyPrefix]
        static void Controls(MuckSetting.Page page)
        {
            page.AddControlSetting("Example", Main.example);
        }

        [HarmonyPatch(typeof(PlayerInput), "Update"), HarmonyPostfix]
        static void PlayerInput() {
            if (Input.GetKeyDown(Main.example.Value)) {
                ChatBox.Instance.AppendMessage(-1, "You pressed the button!", "");
            }
        }
    }
}