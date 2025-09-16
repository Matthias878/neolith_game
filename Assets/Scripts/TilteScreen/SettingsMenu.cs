using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public Slider slider; //masteraudio slider
    public TMP_InputField inputField;   // or TMP_InputField if using TextMeshPro masteraudio text

    void Awake()
    {
        // Init both UI elements with current volume
        slider.value = AudioListener.volume; // Volume is between 0.0 and 1.0 global unity value
        inputField.text = (AudioListener.volume * 100f).ToString("0");

        // Hook up events
        slider.onValueChanged.AddListener(OnSliderChanged);
        inputField.onEndEdit.AddListener(OnInputChanged);
    }

    void OnSliderChanged(float value)
    {
        AudioListener.volume = value;
        inputField.text = (value * 100f).ToString("0");
    }

    void OnInputChanged(string text)
    {
        if (float.TryParse(text, out float percent))
        {
            float v = Mathf.Clamp01(percent / 100f);
            AudioListener.volume = v;
            slider.value = v;
        }
        else
        {
            // Reset to current value if invalid input
            inputField.text = (AudioListener.volume * 100f).ToString("0");
        }
    }
}
