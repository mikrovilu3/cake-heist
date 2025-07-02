using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Menu References")]
    public GameObject pauseMenuPanel;
    public GameObject settingsMenuPanel;
    
    [Header("Pause Menu Panel Buttons")]
    public Button resumeButton;
    public Button settingsButton;
    public Button restartButton;
    public Button mainMenuButton;
    public Button quitButton;
    
    [Header("Settings Panel Buttons")]
    public Button backButton;
    public Button applySettingsButton;
    
    [Header("Settings Panel Sliders")]
    public Slider mouseSensitivitySlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    
    [Header("Audio References")]
    public AudioSource[] musicSources;
    public AudioSource[] sfxSources;
    
    [Header("Script References")]
    public MouseLookScript mouseLookScript;
    public PlayerMovement playerMovement;
    
    [Header("Settings")]
    public string mainMenuSceneName = "MainMenu";
    
    // Private variables
    private bool isPaused = false;
    private GameObject currentActiveMenu;

    void Start()
    {
        // Initialize game state
        CloseAllMenus();
        
        // Load saved mouse sensitivity
        if (mouseLookScript != null && PlayerPrefs.HasKey("MouseSensitivity"))
        {
            mouseLookScript.mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
        }
        
        // Load saved audio settings
        LoadAudioSettings();
        
        // Initialize slider values
        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.value = mouseLookScript != null ? mouseLookScript.mouseSensitivity : 100f;
        
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        
        // Connect button events
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
        
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        
        if (backButton != null)
            backButton.onClick.AddListener(CloseSettings);
        
        if (applySettingsButton != null)
            applySettingsButton.onClick.AddListener(ApplySettings);
        
        // Connect slider events
        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);
        
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        
        // Ensure game starts in proper state
        SetGameState(true);
    }

    void Update()
    {
        // Toggle pause menu with Tab (keyboard) or Start/Options (controller)
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            if (currentActiveMenu == settingsMenuPanel)
            {
                // If in settings, go back to pause menu
                OpenPauseMenu();
            }
            else if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    #region Pause Menu Functions
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
            currentActiveMenu = pauseMenuPanel;

            // Set focus for controller navigation
            if (resumeButton != null)
                EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
        }
        
        SetGameState(false);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        CloseAllMenus();
        SetGameState(true);
    }

    public void OpenPauseMenu()
    {
        CloseAllMenus();
        
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
            currentActiveMenu = pauseMenuPanel;

            if (resumeButton != null)
                EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    #endregion

    #region Settings Menu Functions
    public void OpenSettings()
    {
        CloseAllMenus();
        
        if (settingsMenuPanel != null)
        {
            settingsMenuPanel.SetActive(true);
            currentActiveMenu = settingsMenuPanel;

            if (backButton != null)
                EventSystem.current.SetSelectedGameObject(backButton.gameObject);
        }
    }

    public void CloseSettings()
    {
        OpenPauseMenu();
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        if (mouseLookScript != null)
        {
            mouseLookScript.mouseSensitivity = sensitivity * 240.0f;
        }

        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity * 240.0f);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float value)
    {
        float adjustedVolume = value > 0f ? Mathf.Pow(value, 1.5f) : 0f;
        
        foreach (var music in musicSources)
        {
            if (music != null)
                music.volume = adjustedVolume;
        }

        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        foreach (var sfx in sfxSources)
        {
            if (sfx != null)
                sfx.volume = value;
        }

        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void ApplySettings()
    {
        PlayerPrefs.Save();
        CloseSettings();
    }

    void LoadAudioSettings()
    {
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SetMusicVolume(savedMusicVolume);

        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        SetSFXVolume(savedSFXVolume);
    }
    #endregion

    #region Utility Functions
    void CloseAllMenus()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);

        currentActiveMenu = null;
    }

    void SetGameState(bool gameActive)
    {
        if (mouseLookScript != null)
        {
            mouseLookScript.inMenu = !gameActive;
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = gameActive;
        }
    }

    public bool IsPaused() { return isPaused; }
    public GameObject GetCurrentActiveMenu() { return currentActiveMenu; }
    #endregion
}
