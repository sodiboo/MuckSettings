using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace MuckSettings
{
    public class Setting : MonoBehaviour
    {
        public Setting.ButtonClickedEvent onClick
        {
            get
            {
                return this.m_OnClick;
            }
            set
            {
                this.m_OnClick = value;
            }
        }

        public int currentSetting;

        public Setting.ButtonClickedEvent m_OnClick = new Setting.ButtonClickedEvent();

        public class ButtonClickedEvent : UnityEvent
        {
        }

        public string settingName;

        public TextMeshProUGUI nameText;

        public void SetName(string name)
        {
            settingName = name;
            nameText.text = name;
        }
    }

    public class ControlSetting : Setting
    {
        public void Awake()
        {
            nameText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            keyText = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            transform.GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(StartListening);
        }

        public void SetSetting(KeyCode k)
        {
            this.currentKey = k;
            MonoBehaviour.print("key: " + k);
            this.UpdateSetting();
        }

        public void UpdateSetting()
        {
            this.keyText.text = (this.currentKey.ToString() ?? "");
        }

        public void SetKey(KeyCode k)
        {
            this.currentKey = k;
            base.onClick.Invoke();
            this.UpdateSetting();
        }

        public void StartListening()
        {
            var listener = KeyListener.Instance;
            listener.alertText.text = "Press any key for\n\"" + settingName + "\"\n\n<i><size=60%>...escape to go back";
            KeyListenerCurrentlyChanging.value = this;
            listener.overlay.SetActive(true);
        }

        public TextMeshProUGUI keyText;

        public KeyCode currentKey;
    }

    public class OneBoolSetting : Setting
    {
        public void Awake()
        {
            nameText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            checkMark = transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
            transform.GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(ToggleSetting);
        }

        public void SetSetting(int s)
        {
            this.currentSetting = s;
            this.UpdateSetting();
        }

        public void SetSetting(bool s)
        {
            if (s)
            {
                this.currentSetting = 1;
            }
            else
            {
                this.currentSetting = 0;
            }
            this.UpdateSetting();
        }

        public void ToggleSetting()
        {
            if (this.currentSetting == 1)
            {
                this.currentSetting = 0;
            }
            else
            {
                this.currentSetting = 1;
            }
            this.UpdateSetting();
        }

        public void UpdateSetting()
        {
            if (this.currentSetting == 1)
            {
                this.checkMark.SetActive(true);
            }
            else
            {
                this.checkMark.SetActive(false);
            }
            this.m_OnClick.Invoke();
        }

        public GameObject checkMark;
    }

    public class TwoBoolSetting : Setting
    {
        public void Awake()
        {
            nameText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            checkMark1 = transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
            checkMark2 = transform.GetChild(1).GetChild(2).GetChild(0).gameObject;
            label1 = transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
            label2 = transform.GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>();
            transform.GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(() => ToggleSetting(0));
            transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(() => ToggleSetting(1));
        }

        public void SetSetting(int s)
        {
            this.currentSetting = s;
            this.UpdateSetting();
        }

        public void SetSetting(bool a, bool b)
        {
            currentSetting = (a ? 1 : 0) | (b ? 2 : 0);
            this.UpdateSetting();
        }

        public void ToggleSetting(int bit)
        {
            currentSetting ^= 1 << bit;
            this.UpdateSetting();
        }

        public void SetLabels(string first, string second)
        {
            label1.text = first;
            label2.text = second;
        }

        public void UpdateSetting()
        {
            this.checkMark1.SetActive((this.currentSetting & 1) != 0);
            this.checkMark2.SetActive((this.currentSetting & 2) != 0);
            this.m_OnClick.Invoke();
        }

        public GameObject checkMark1;
        public GameObject checkMark2;

        public TextMeshProUGUI label1;
        public TextMeshProUGUI label2;
    }

    public class SliderSetting : Setting
    {
        public void Awake()
        {
            nameText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            slider = transform.GetChild(1).GetChild(0).GetComponent<Slider>();
            slider.onValueChanged.AddListener(_ => UpdateSettings());
            value = transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        }

        public void SetSettings(int startVal)
        {
            this.currentSetting = startVal;
            this.slider.value = (float)startVal;
            this.UpdateSettings();
        }

        public void UpdateSettings()
        {
            this.currentSetting = (int)this.slider.value;
            this.value.text = string.Concat(this.currentSetting);
            this.m_OnClick.Invoke();
        }

        public static float Truncate(float value, int digits)
        {
            double num = Math.Pow(10.0, (double)digits);
            return (float)(Math.Truncate(num * (double)value) / num);
        }

        public Slider slider;

        public TextMeshProUGUI value;
    }

    public class ScrollSetting : Setting
    {
        public void Awake()
        {
            nameText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            settingText = transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
            scrollLeft = transform.GetChild(1).GetChild(0).GetComponent<RawImage>();
            scrollLeft.GetComponent<Button>().onClick.AddListener(() => Scroll(-1));
            scrollRight = transform.GetChild(1).GetChild(2).GetComponent<RawImage>();
            scrollRight.GetComponent<Button>().onClick.AddListener(() => Scroll(1));
        }

        public void SetSettings(string[] settings, int startVal)
        {
            this.settingNames = settings;
            this.currentSetting = startVal;
            this.UpdateSetting();
        }

        public void Scroll(int i)
        {
            this.currentSetting += i;
            this.UpdateSetting();
        }

        public void UpdateSetting()
        {
            this.settingText.text = this.settingNames[this.currentSetting];
            if (this.currentSetting == 0)
            {
                this.scrollLeft.enabled = false;
            }
            else if (this.currentSetting > 0)
            {
                this.scrollLeft.enabled = true;
            }
            if (this.currentSetting == this.settingNames.Length - 1)
            {
                this.scrollRight.enabled = false;
            }
            else if (this.currentSetting < this.settingNames.Length - 1)
            {
                this.scrollRight.enabled = true;
            }
            this.m_OnClick.Invoke();
        }

        public TextMeshProUGUI settingText;

        public string[] settingNames;

        public RawImage scrollLeft;

        public RawImage scrollRight;
    }

    public class ResolutionSetting : Setting
    {
        public void Awake()
        {
            nameText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            settingText = transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
            scrollLeft = transform.GetChild(1).GetChild(0).GetComponent<RawImage>();
            scrollLeft.GetComponent<Button>().onClick.AddListener(() => Scroll(-1));
            scrollRight = transform.GetChild(1).GetChild(2).GetComponent<RawImage>();
            scrollRight.GetComponent<Button>().onClick.AddListener(() => Scroll(1));
            transform.GetChild(1).GetChild(3).GetComponent<Button>().onClick.AddListener(ApplySetting);
        }

        public void SetSettings(Resolution[] resolutions, Resolution current)
        {
            this.resolutions = resolutions;
            for (int i = 0; i < resolutions.Length; i++)
            {
                if (current.width == resolutions[i].width && current.height == resolutions[i].height)
                {
                    this.currentSetting = i;
                    MonoBehaviour.print("found current res");
                }
            }
            this.UpdateSetting();
        }

        public void Scroll(int i)
        {
            this.currentSetting += i;
            this.UpdateSetting();
        }

        public void UpdateSetting()
        {
            this.settingText.text = this.ResolutionToText(this.resolutions[this.currentSetting]);
            if (this.currentSetting == 0)
            {
                this.scrollLeft.enabled = false;
            }
            else if (this.currentSetting > 0)
            {
                this.scrollLeft.enabled = true;
            }
            if (this.currentSetting == this.resolutions.Length - 1)
            {
                this.scrollRight.enabled = false;
                return;
            }
            if (this.currentSetting < this.resolutions.Length - 1)
            {
                this.scrollRight.enabled = true;
            }
        }

        public string ResolutionToText(Resolution r)
        {
            return r.ToString();
        }

        public void ApplySetting()
        {
            Resolution resolution = this.resolutions[this.currentSetting];
            CurrentSettings.Instance.UpdateResolution(resolution.width, resolution.height, resolution.refreshRate);
        }

        public RawImage scrollLeft;

        public RawImage scrollRight;

        public TextMeshProUGUI settingText;

        public Resolution[] resolutions;
    }
}