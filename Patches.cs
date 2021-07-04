using System;
using HarmonyLib;
using UnityEngine;

namespace MuckSettings
{
    [HarmonyPatch(typeof(KeyListener), nameof(KeyListener.Update))]
    class UpdateKeyListener
    {
        static bool Prefix(KeyListener __instance)
        {
            if (!__instance.overlay.activeInHierarchy)
            {
                return false;
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                __instance.CloseListener();
                return false;
            }
            foreach (object obj in Enum.GetValues(typeof(KeyCode)))
            {
                KeyCode key = (KeyCode)obj;
                if (Input.GetKey(key))
                {
                    KeyListenerCurrentlyChanging.value.SetKey(key);
                    __instance.CloseListener();
                    break;
                }
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(KeyListener), nameof(KeyListener.CloseListener))]
    class CloseKeyListener
    {
        static void Postfix()
        {
            KeyListenerCurrentlyChanging.value = null;
        }
    }

    [HarmonyPatch(typeof(global::Settings), nameof(global::Settings.UpdateSave))]
    class SettingsMenu
    {
        static bool Prefix(global::Settings __instance)
        {
            var nav = __instance.GetComponentInChildren<TopNavigate>();

            foreach (var menu in nav.settingMenus)
            {
                for (var i = menu.transform.childCount; i --> 0;) MonoBehaviour.Destroy(menu.transform.GetChild(i).gameObject);
            }

            using (var page = new Settings.Page(nav.settingMenus[0])) Settings.Gameplay(page);
            using (var page = new Settings.Page(nav.settingMenus[1])) Settings.Controls(page);
            using (var page = new Settings.Page(nav.settingMenus[2])) Settings.Graphics(page);
            using (var page = new Settings.Page(nav.settingMenus[3])) Settings.Video(page);
            using (var page = new Settings.Page(nav.settingMenus[4])) Settings.Audio(page);

            return false;
        }
    }
}