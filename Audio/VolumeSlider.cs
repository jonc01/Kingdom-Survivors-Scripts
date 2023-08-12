using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private bool isMusic;
    [SerializeField] private TextMeshProUGUI sliderTextDisplay;

    void Start()
    {
        if(isMusic) MusicSlider();
        else SFXSlider();
    }

    void MusicSlider()
    {
        slider.onValueChanged.AddListener(val => AudioManager.Instance.ChangeMusicVolume(val));
        slider.onValueChanged.AddListener(val => sliderTextDisplay.text = (val * 100).ToString("N0"));
    }

    void SFXSlider()
    {
        slider.onValueChanged.AddListener(val => GetPercentValue(val));
        slider.onValueChanged.AddListener(val => AudioManager.Instance.ChangeSFXVolume(GetPercentValue(val)));
        slider.onValueChanged.AddListener(val => sliderTextDisplay.text = ((GetPercentValue(val))/.65f * 100).ToString("N0"));
        // (GetPercentValue(val)*2 * 100).ToString("N0"));
        // slider.onValueChanged.AddListener(val => sliderTextDisplay.text = (GetPercentValue(val)*2 * 100).ToString("N0"));

    }

    private float GetPercentValue(float sliderValue)
    {
        // Map the slider value to a volume level between 0 and 0.5 (50%)
        float mappedValue = Mathf.Lerp(0f, 0.65f, (sliderValue - 0.01f) / 0.64f);
        // float mappedValue = Mathf.Lerp(0f, 0.5f, (sliderValue - 0.01f) / 0.49f);
        return mappedValue;
        // Set the audio source volume
        // You need to replace 'audioSource' with your actual AudioSource reference
        // audioSource.volume = mappedValue;
    }
}
