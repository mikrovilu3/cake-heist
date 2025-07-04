using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class StartMenuManager : MonoBehaviour
{
    [Header("Ui tings")]
    public UnityEngine.UI.Button startButton;
    public UnityEngine.UI.Button quitButton;
    public UnityEngine.UI.Button settingsButton;
    public UnityEngine.UI.Button backToMainMenuButton; 
    public UnityEngine.UI.Slider fovSlider;
    public UnityEngine.UI.Slider musicSlider;
    public UnityEngine.UI.Slider sfxSlider;
    public Camera fpsCamera;
    public string scene = "Pēteris_gulamistaba";

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
        inMenu = true;

        PauseMenu.SetActive(true);
        SettingsMenu.SetActive(false);

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


        if(startButton != null)
            startButton.onClick.AddListener(PlayGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettings);

        if (backToMainMenuButton != null)
            backToMainMenuButton.onClick.AddListener(BackToMainMenu);


        if (fovSlider != null)
            fovSlider.onValueChanged.AddListener(UpdateFOV);

        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(UpdateMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(UpdateSFXVolume);
    }

    public void OnSettings() { onSettings = true; }

    public void QuitGame()
    {
        Application.Quit();
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

    public void PlayGame()
    {
        SceneManager.LoadScene(scene);
    }

    public void Update()
    {

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





    }
}
