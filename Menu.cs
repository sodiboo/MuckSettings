using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MuckSettings
{
    public class Settings
    {
        public class Page : IDisposable
        {
            RectTransform content;

            public Page(GameObject tab)
            {
                MonoBehaviour.Destroy(tab.GetComponent<VerticalLayoutGroup>());
                var scroll = MonoBehaviour.Instantiate(Main.SettingsScroll);
                content = scroll.GetComponent<ScrollRect>().content;
                scroll.transform.SetParent(tab.transform, false);
            }

            class SettingsItem<T> : IDisposable where T : Setting
            {
                public T obj;
                Page page;
                public SettingsItem(Page page, GameObject gameObject, string name)
                {
                    obj = MonoBehaviour.Instantiate(gameObject).GetComponent<T>();
                    obj.SetName(name);
                    this.page = page;
                }

                public void Dispose()
                {
                    var transform = (RectTransform)obj.transform;
                    transform.SetParent(page.content, false);
                    page.height += transform.sizeDelta.y;
                    page.total++;
                }
            }

            float height = 0f;

            int total = 0;

            float maxScrollWidth;

            List<RectTransform> scrollSettings = new List<RectTransform>();

            public void AddControlSetting(string name, KeyCode defaultValue, Action<KeyCode> update)
            {
                using (var setting = new SettingsItem<ControlSetting>(this, Main.ControlSetting, name))
                {
                    setting.obj.SetSetting(defaultValue);
                    setting.obj.onClick.AddListener(() => update(setting.obj.currentKey));
                }
            }

            public void AddSliderSetting(string name, int defaultValue, int min, int max, Action<int> update)
            {
                using (var setting = new SettingsItem<SliderSetting>(this, Main.SliderSetting, name))
                {
                    setting.obj.slider.minValue = min;
                    setting.obj.slider.maxValue = max;
                    setting.obj.SetSettings(defaultValue);
                    setting.obj.onClick.AddListener(() => update(setting.obj.currentSetting));
                }
            }

            public void AddScrollSetting(string name, string[] values, int defaultIndex, Action<int> update)
            {
                using (var setting = new SettingsItem<ScrollSetting>(this, Main.ScrollSetting, name))
                {
                    foreach (var value in values)
                    {
                        setting.obj.settingText.text = value;
                        scrollSettings.Add((RectTransform)setting.obj.settingText.transform);
                        maxScrollWidth = Math.Max(setting.obj.settingText.preferredWidth, maxScrollWidth);
                    }
                    setting.obj.SetSettings(values, defaultIndex);
                    setting.obj.onClick.AddListener(() => update(setting.obj.currentSetting));
                }
            }

            public void AddBoolSetting(string name, bool defaultValue, Action<bool> update)
            {
                using (var setting = new SettingsItem<OneBoolSetting>(this, Main.BoolSetting, name))
                {
                    setting.obj.SetSetting(defaultValue);
                    setting.obj.onClick.AddListener(() => update(IntToBool(setting.obj.currentSetting)));
                }
            }

            public void AddTwoBoolSetting(string name, string label1, string label2, bool defaultValue1, bool defaultValue2, Action<bool, bool> update)
            {
                using (var setting = new SettingsItem<TwoBoolSetting>(this, Main.TwoBoolSetting, name))
                {
                    setting.obj.SetSetting(defaultValue1, defaultValue2);
                    setting.obj.SetLabels(label1, label2);
                    setting.obj.onClick.AddListener(() => update(IntToBool((setting.obj.currentSetting & 1)), IntToBool(setting.obj.currentSetting & 2)));
                }
            }

            public void AddResolutionSetting(string name, Resolution[] resolutions, Resolution current)
            {
                using (var setting = new SettingsItem<ResolutionSetting>(this, Main.ResolutionSetting, name))
                {
                    setting.obj.SetSettings(resolutions, current);
                }
            }

            public void Dispose()
            {
                foreach (var setting in scrollSettings)
                {
                    setting.sizeDelta = new Vector2(maxScrollWidth, setting.sizeDelta.y);
                }
                var group = content.GetComponent<VerticalLayoutGroup>();
                content.sizeDelta = new Vector2(content.sizeDelta.x, height + (total - 1) * group.spacing + group.padding.top + group.padding.bottom);
            }
        }

        public static void Gameplay(Page page)
        {
            page.AddBoolSetting("Camera Shake", SaveManager.Instance.state.cameraShake, UpdateCamShake);
            page.AddSliderSetting("FOV", SaveManager.Instance.state.fov, 50, 120, UpdateFov);
            page.AddSliderSetting("Sensitivity", FloatToInt(SaveManager.Instance.state.sensMultiplier), 0, 500, UpdateSens);
            page.AddTwoBoolSetting("Inverted Mouse", "X", "Y", SaveManager.Instance.state.invertedMouseHor, SaveManager.Instance.state.invertedMouseVert, UpdateInverted);
            page.AddBoolSetting("Grass", SaveManager.Instance.state.grass, UpdateGrass);
            page.AddBoolSetting("Tutorial", SaveManager.Instance.state.tutorial, UpdateTutorial);
        }

        public static void Controls(Page page)
        {
            page.AddControlSetting("Forward", SaveManager.Instance.state.forward, UpdateForwardKey);
            page.AddControlSetting("Backward", SaveManager.Instance.state.backwards, UpdateBackwardKey);
            page.AddControlSetting("Left", SaveManager.Instance.state.left, UpdateLeftKey);
            page.AddControlSetting("Right", SaveManager.Instance.state.right, UpdateRightKey);
            page.AddControlSetting("Jump", SaveManager.Instance.state.jump, UpdateJumpKey);
            page.AddControlSetting("Sprint", SaveManager.Instance.state.sprint, UpdateSprintKey);
            page.AddControlSetting("Interact", SaveManager.Instance.state.interact, UpdateInteractKey);
            page.AddControlSetting("Inventory", SaveManager.Instance.state.inventory, UpdateInventoryKey);
            page.AddControlSetting("Map", SaveManager.Instance.state.map, UpdateMapKey);
            page.AddControlSetting("Attack/Eat", SaveManager.Instance.state.leftClick, UpdateLeftClickKey);
            page.AddControlSetting("Build", SaveManager.Instance.state.rightClick, UpdateRightClickKey);
        }

        public static void Graphics(Page page)
        {
            page.AddScrollSetting("Shadow Quality", Enum.GetNames(typeof(Settings.ShadowQuality)), SaveManager.Instance.state.shadowQuality, UpdateShadowQuality);
            page.AddScrollSetting("Shadow Resolution", Enum.GetNames(typeof(Settings.ShadowResolution)), SaveManager.Instance.state.shadowResolution, UpdateShadowResolution);
            page.AddScrollSetting("Shadow Distance", Enum.GetNames(typeof(Settings.ShadowDistance)), SaveManager.Instance.state.shadowDistance, UpdateShadowDistance);
            page.AddScrollSetting("Shadow Cascades", Enum.GetNames(typeof(Settings.ShadowCascades)), SaveManager.Instance.state.shadowCascade, UpdateShadowCascades);
            page.AddScrollSetting("Texture Resolution", Enum.GetNames(typeof(Settings.TextureResolution)), SaveManager.Instance.state.textureQuality, UpdateTextureRes);
            page.AddScrollSetting("Anti Aliasing", Enum.GetNames(typeof(Settings.AntiAliasing)), SaveManager.Instance.state.antiAliasing, UpdateAntiAliasing);

            page.AddBoolSetting("Soft Particles", SaveManager.Instance.state.softParticles, UpdateSoftParticles);
            page.AddScrollSetting("Bloom", Enum.GetNames(typeof(Settings.Bloom)), SaveManager.Instance.state.bloom, UpdateBloom);

            page.AddBoolSetting("Motion Blur", SaveManager.Instance.state.motionBlur, UpdateMotionBlur);
            page.AddBoolSetting("Ambient Occlusion", SaveManager.Instance.state.ambientOcclusion, UpdateAO);
        }

        public static void Video(Page page)
        {
            page.AddResolutionSetting("Resolution", Screen.resolutions, Screen.currentResolution);
            page.AddBoolSetting("Fullscreen", Screen.fullScreen, UpdateFullscreen);

            page.AddScrollSetting("Fullscreen Mode", Enum.GetNames(typeof(FullScreenMode)), SaveManager.Instance.state.fullscreenMode, UpdateFullscreenMode);
            page.AddScrollSetting("VSync", Enum.GetNames(typeof(Settings.VSync)), SaveManager.Instance.state.vSync, UpdateVSync);
            page.AddSliderSetting("Max FPS", SaveManager.Instance.state.fpsLimit, 30, 500, UpdateMaxFps);
        }

        public static void Audio(Page page)
        {
            page.AddSliderSetting("Master volume", SaveManager.Instance.state.volume, 0, 10, UpdateVolume);
            page.AddSliderSetting("Music volume", SaveManager.Instance.state.music, 0, 10, UpdateMusic);
        }

        public static void UpdateCamShake(bool current)
        {
            CurrentSettings.Instance.UpdateCamShake(current);
        }

        public static void UpdateInverted(bool x, bool y)
        {
            CurrentSettings.Instance.UpdateInverted(x, y);
        }

        public static void UpdateGrass(bool current)
        {
            CurrentSettings.Instance.UpdateGrass(current);
        }

        public static void UpdateTutorial(bool current)
        {
            CurrentSettings.Instance.UpdateTutorial(current);
        }

        public static void UpdateSens(int current)
        {
            CurrentSettings.Instance.UpdateSens(IntToFloat(current));
        }

        public static void UpdateFov(int current)
        {
            CurrentSettings.Instance.UpdateFov(current);
        }

        public static void UpdateForwardKey(KeyCode current)
        {
            SaveManager.Instance.state.forward = current;
            SaveManager.Instance.Save();
            InputManager.forward = current;
        }

        public static void UpdateBackwardKey(KeyCode current)
        {
            SaveManager.Instance.state.backwards = current;
            SaveManager.Instance.Save();
            InputManager.backwards = current;
        }

        public static void UpdateLeftKey(KeyCode current)
        {
            SaveManager.Instance.state.left = current;
            SaveManager.Instance.Save();
            InputManager.left = current;
        }

        public static void UpdateRightKey(KeyCode current)
        {
            SaveManager.Instance.state.right = current;
            SaveManager.Instance.Save();
            InputManager.right = current;
        }

        public static void UpdateJumpKey(KeyCode current)
        {
            SaveManager.Instance.state.jump = current;
            SaveManager.Instance.Save();
            InputManager.jump = current;
        }

        public static void UpdateSprintKey(KeyCode current)
        {
            SaveManager.Instance.state.sprint = current;
            SaveManager.Instance.Save();
            InputManager.sprint = current;
        }

        public static void UpdateInteractKey(KeyCode current)
        {
            SaveManager.Instance.state.interact = current;
            SaveManager.Instance.Save();
            InputManager.interact = current;
        }

        public static void UpdateInventoryKey(KeyCode current)
        {
            SaveManager.Instance.state.inventory = current;
            SaveManager.Instance.Save();
            InputManager.inventory = current;
        }

        public static void UpdateMapKey(KeyCode current)
        {
            SaveManager.Instance.state.map = current;
            SaveManager.Instance.Save();
            InputManager.map = current;
        }

        public static void UpdateLeftClickKey(KeyCode current)
        {
            SaveManager.Instance.state.leftClick = current;
            SaveManager.Instance.Save();
            InputManager.leftClick = current;
        }

        public static void UpdateRightClickKey(KeyCode current)
        {
            SaveManager.Instance.state.rightClick = current;
            SaveManager.Instance.Save();
            InputManager.rightClick = current;
        }

        public static void UpdateShadowQuality(int current)
        {
            CurrentSettings.Instance.UpdateShadowQuality(current);
        }

        public static void UpdateShadowResolution(int current)
        {
            CurrentSettings.Instance.UpdateShadowResolution(current);
        }

        public static void UpdateShadowDistance(int current)
        {
            CurrentSettings.Instance.UpdateShadowDistance(current);
        }

        public static void UpdateShadowCascades(int current)
        {
            CurrentSettings.Instance.UpdateShadowCascades(current);
        }

        public static void UpdateTextureRes(int current)
        {
            CurrentSettings.Instance.UpdateTextureQuality(current);
        }

        public static void UpdateAntiAliasing(int current)
        {
            CurrentSettings.Instance.UpdateAntiAliasing(current);
        }

        public static void UpdateBloom(int current)
        {
            CurrentSettings.Instance.UpdateBloom(current);
        }

        public static void UpdateSoftParticles(bool current)
        {
            CurrentSettings.Instance.UpdateSoftParticles(current);
        }

        public static void UpdateMotionBlur(bool current)
        {
            CurrentSettings.Instance.UpdateMotionBlur(current);
        }

        public static void UpdateAO(bool current)
        {
            CurrentSettings.Instance.UpdateAO(current);
        }

        public static void UpdateFullscreen(bool current)
        {
            CurrentSettings.Instance.UpdateFullscreen(current);
        }

        public static void UpdateFullscreenMode(int current)
        {
            CurrentSettings.Instance.UpdateFullscreenMode(current);
        }

        public static void UpdateVSync(int current)
        {
            CurrentSettings.Instance.UpdateVSync(current);
        }

        public static void UpdateMaxFps(int current)
        {
            CurrentSettings.Instance.UpdateMaxFps(current);
        }

        public static void UpdateVolume(int current)
        {
            CurrentSettings.Instance.UpdateVolume(current);
        }

        public static void UpdateMusic(int current)
        {
            CurrentSettings.Instance.UpdateMusic(current);
        }

        public static float IntToFloat(int i)
        {
            return (float)i / 100f;
        }

        public static int FloatToInt(float f)
        {
            return (int)(f * 100f);
        }

        public static int BoolToInt(bool b)
        {
            if (b)
            {
                return 1;
            }
            return 0;
        }

        public static bool IntToBool(int i)
        {
            return i != 0;
        }

        public Button backBtn;

        public enum BoolSetting
        {
            Off,
            On,
        }

        public enum VSync
        {
            Off,
            Always,
            Half,
        }

        public enum ShadowQuality
        {
            Off,
            Hard,
            Soft,
        }

        public enum ShadowResolution
        {
            Low,
            Medium,
            High,
            Ultra,
        }

        public enum ShadowDistance
        {
            Low,
            Medium,
            High,
            Ultra,
        }

        public enum ShadowCascades
        {
            None,
            Two,
            Four,
        }

        public enum TextureResolution
        {
            Low,
            Medium,
            High,
            Ultra,
        }

        public enum AntiAliasing
        {
            Off,
            x2,
            x4,
            x8,
        }

        public enum Bloom
        {
            Off,
            Fast,
            Fancy,
        }
    }
}