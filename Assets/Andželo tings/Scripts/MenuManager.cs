using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    [Header("Ui tings")]
    public UnityEngine.UI.Button settingsButton;
    public UnityEngine.UI.Button backToMainMenuButton;
    public UnityEngine.UI.Button backToStartMenuButton;
    public UnityEngine.UI.Button backToGameButton;
    public UnityEngine.UI.Slider fovSlider;
    public UnityEngine.UI.Slider musicSlider;
    public UnityEngine.UI.Slider sfxSlider;
    public Camera fpsCamera;
    public MouseLook cam;

    [Header("Audio")]
    public AudioSource[] musicSources;
    public AudioSource[] sfxSources;

    [Header("Canvas")]
    public GameObject PauseMenu;
    public GameObject SettingsMenu;


    private bool inMenu = false;
    private bool onSettings = false;



    private int currentMusicIndex = 0;
    private int currentSFXIndex = 0;

    void Start()
    {
        
        PauseMenu.SetActive(false);
        SettingsMenu.SetActive(false);

        // ✅ Restore settings from PlayerPrefs
        if (fpsCamera != null)
            fpsCamera.fieldOfView = PlayerPrefs.GetFloat("FOV", 60f);

        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (musicSlider != null)
            musicSlider.value = musicVol;

        if (sfxSlider != null)
            sfxSlider.value = sfxVol;

        foreach (var music in musicSources)
            music.volume = musicVol;

        foreach (var sfx in sfxSources)
            sfx.volume = sfxVol;


        if (backToStartMenuButton != null)
            backToStartMenuButton.onClick.AddListener(BackToStartMenu);

        if (backToMainMenuButton != null)
            backToMainMenuButton.onClick.AddListener(BackToMainMenu);

        if (backToGameButton != null)
            backToGameButton.onClick.AddListener(BackToGame);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettings);

        if (fovSlider != null)
            fovSlider.onValueChanged.AddListener(UpdateFOV);

        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(UpdateMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(UpdateSFXVolume);
    }

    public void OnSettings() { onSettings = true; }

    public void BackToStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }
    public void BackToMainMenu()
    {
        onSettings = false;
    }

    public void UpdateFOV(float value)
    {
        if (fpsCamera != null)
            fpsCamera.fieldOfView = value * 120f;
        PlayerPrefs.SetFloat("FOV", value * 120f);
    }

    public void UpdateMusicVolume(float value)
    {
        float adjustedVolume = value > 0f ? Mathf.Pow(value, 1.5f) : 0f;
        foreach (var music in musicSources)
        {
            music.volume = adjustedVolume;
        }

        PlayerPrefs.SetFloat("MusicVolume", value);
    }


    public void UpdateSFXVolume(float value)
    {
        foreach (var sfx in sfxSources)
        {
            sfx.volume = value;
        }

        PlayerPrefs.SetFloat("SFXVolume", value);

    }
    public void NextMusic()
    {
        if (musicSources.Length > 0)
        {
            musicSources[currentMusicIndex].Stop();
            currentMusicIndex = (currentMusicIndex + 1) % musicSources.Length;
            musicSources[currentMusicIndex].Play();
        }
    }

    public void NextSFX()
    {
        if (sfxSources.Length > 0)
        {
            sfxSources[currentSFXIndex].Stop();
            currentSFXIndex = (currentSFXIndex + 1) % sfxSources.Length;
            sfxSources[currentSFXIndex].Play();
        }
    }

    public void BackToGame()
    {
        inMenu = false;
        
        SettingsMenu.SetActive(false);
        PauseMenu.SetActive(false);
    }

    public void Update()
    {

        if (Input.GetKey(KeyCode.Tab))
        {
            inMenu = true;
            
            
            PauseMenu.SetActive(true);
            SettingsMenu.SetActive(false);
        }
        else { }

        if (onSettings == true)
        {
            SettingsMenu.SetActive(true);
            PauseMenu.SetActive(false);
        }
        else { }

        if (onSettings == false && inMenu == true)
        {
            SettingsMenu.SetActive(false);
            PauseMenu.SetActive(true);
        }
        else { }

        cam.isInMenu = inMenu!;




    }
}
