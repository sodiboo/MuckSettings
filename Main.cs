using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MuckSettings
{
    [BepInPlugin(Guid, Name, Version)]
    public class Main : BaseUnityPlugin
    {
        public const string
            Name = "MuckSettings",
            Author = "Terrain",
            Guid = Author + "." + Name,
            Version = "1.0.0.0";

        internal readonly ManualLogSource log;
        internal readonly Harmony harmony;
        internal readonly Assembly assembly;
        public readonly string modFolder;

        public static GameObject SettingsScroll;

        public static GameObject BoolSetting;
        public static GameObject ControlSetting;
        public static GameObject ResolutionSetting;
        public static GameObject ScrollSetting;
        public static GameObject SliderSetting;
        public static GameObject TwoBoolSetting;

        Main()
        {
            log = Logger;
            harmony = new Harmony(Guid);
            assembly = Assembly.GetExecutingAssembly();
            modFolder = Path.GetDirectoryName(assembly.Location);

            var bundle = GetAssetBundle("settings");

            SettingsScroll = bundle.LoadAsset<GameObject>("Assets/PrefabInstance/SettingsScroll.prefab");

            BoolSetting = bundle.LoadAsset<GameObject>("Assets/PrefabInstance/BoolSetting.prefab");
            ControlSetting = bundle.LoadAsset<GameObject>("Assets/PrefabInstance/ControlSetting.prefab");
            ResolutionSetting = bundle.LoadAsset<GameObject>("Assets/PrefabInstance/ResolutionSetting.prefab");
            ScrollSetting = bundle.LoadAsset<GameObject>("Assets/PrefabInstance/ScrollSetting.prefab");
            SliderSetting = bundle.LoadAsset<GameObject>("Assets/PrefabInstance/SliderSetting.prefab");
            TwoBoolSetting = bundle.LoadAsset<GameObject>("Assets/PrefabInstance/TwoBoolSetting.prefab");

            BoolSetting.AddComponent<OneBoolSetting>();
            ControlSetting.AddComponent<ControlSetting>();
            ResolutionSetting.AddComponent<ResolutionSetting>();
            ScrollSetting.AddComponent<ScrollSetting>();
            SliderSetting.AddComponent<SliderSetting>();
            TwoBoolSetting.AddComponent<TwoBoolSetting>();
            
            harmony.PatchAll(assembly);
        }

        static readonly OSPlatform[] supportedPlatforms = new[] { OSPlatform.Windows, OSPlatform.Linux, OSPlatform.OSX };

        static AssetBundle GetAssetBundle(string name)
        {
            foreach (var platform in supportedPlatforms) {
                if (RuntimeInformation.IsOSPlatform(platform)) {
                    name = $"{name}-{platform.ToString().ToLower()}";
                    goto load;
                }
            }

            throw new PlatformNotSupportedException("Unsupported platform, cannot load AssetBundles");

            load:
            var execAssembly = Assembly.GetExecutingAssembly();

            var resourceName = execAssembly.GetManifestResourceNames().Single(str => str.EndsWith(name));

            using (var stream = execAssembly.GetManifestResourceStream(resourceName))
            {
                return AssetBundle.LoadFromStream(stream);
            }
        }
    }

    public static class KeyListenerCurrentlyChanging
    {
        public static ControlSetting value;
    }
}