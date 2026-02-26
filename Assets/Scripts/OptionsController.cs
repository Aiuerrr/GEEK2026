using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    [Header("Elementy UI")]
    public Slider musicSlider;
    public Slider soundSlider;
    public Toggle fullscreenToggle;

    void Start()
    {
        // 1. Wczytujemy ustawienia
        if (musicSlider != null) musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        if (soundSlider != null) soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 0.75f);
        if (fullscreenToggle != null) fullscreenToggle.isOn = Screen.fullScreen;

        // 2. Automatyczne nasłuchiwanie zmian
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (soundSlider != null) soundSlider.onValueChanged.AddListener(SetSoundVolume);
        if (fullscreenToggle != null) fullscreenToggle.isOn = Screen.fullScreen;
    }

    // --- MIEJSCE DLA LYSEGO ---
    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        //  Wstaw tu AudioMixer np:
        // myMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
    }

    public void SetSoundVolume(float volume)
    {
        PlayerPrefs.SetFloat("SoundVolume", volume);
        //  Wstaw tu AudioMixer np:
        // myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }
    // -------------------------------------

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    // --- PROSTE WYJŚCIE ---
    public void CloseOptions()
    {
        // Wyłącza tylko ten panel, niczego innego nie dotyka
        gameObject.SetActive(false); 
    }
}