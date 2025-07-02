using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu References")]
    public GameObject mainMenuPanel;
    public GameObject settingsMenuPanel;
    public GameObject creditsMenuPanel;
    
    [Header("Main Menu Panel Buttons")]
    public Button startGameButton;
    public Button settingsButton;
    public Button creditsButton;
    public Button quitButton;
    
    [Header("Settings Panel Buttons")]
    public Button backFromSettingsButton;
    public Button applySettingsButton;
    
    [Header("Credits Panel Buttons")]
    public Button backFromCreditsButton;
    
    [Header("UI Slider References")]
    public Slider mouseSensitivitySlider;
    public Slider volumeSlider;
    
    [Header("Settings")]
    public string gameSceneName = "GameScene";
    
    // Private variables
    private GameObject currentActiveMenu;
    
    void Start()
    {
        // Initialize main menu state
        OpenMainMenu();
        
        // Load saved settings and initialize sliders
        InitializeSettings();
        
        // Connect all button events
        ConnectButtonEvents();
        
        // Connect slider events
        ConnectSliderEvents();
        
        // Ensure proper cursor state for main menu
        SetCursorState(true);
    }
    
    void Update()
    {
        // Handle escape key to navigate back through menus
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentActiveMenu == settingsMenuPanel)
            {
                CloseSettings();
            }
            else if (currentActiveMenu == creditsMenuPanel)
            {
                CloseCredits();
            }
        }
    }
    
    #region Initialization
    void InitializeSettings()
    {
        // Load saved volume
        if (PlayerPrefs.HasKey("Volume"))
        {
            AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        }
        else
        {
            // Set default volume if none saved
            AudioListener.volume = 1f;
            PlayerPrefs.SetFloat("Volume", 1f);
        }
        
        // Initialize slider values
        if (mouseSensitivitySlider != null)
        {
            float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 100f);
            mouseSensitivitySlider.value = savedSensitivity;
        }
        
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
        }
    }
    
    void ConnectButtonEvents()
    {
        // Main menu buttons
        if (startGameButton != null)
            startGameButton.onClick.AddListener(StartGame);
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
        
        if (creditsButton != null)
            creditsButton.onClick.AddListener(OpenCredits);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        
        // Settings menu buttons
        if (backFromSettingsButton != null)
            backFromSettingsButton.onClick.AddListener(CloseSettings);
        
        if (applySettingsButton != null)
            applySettingsButton.onClick.AddListener(ApplySettings);
        
        // Credits menu buttons
        if (backFromCreditsButton != null)
            backFromCreditsButton.onClick.AddListener(CloseCredits);
    }
    
    void ConnectSliderEvents()
    {
        // Connect slider events
        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);
        
        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(SetVolume);
    }
    #endregion
    
    #region Main Menu Functions
    public void OpenMainMenu()
    {
        CloseAllMenus();
        
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
            currentActiveMenu = mainMenuPanel;
        }
        
        // Ensure cursor is visible and unlocked for main menu
        SetCursorState(true);
    }
    
    public void StartGame()
    {
        // Ensure time scale is normal before loading game scene
        Time.timeScale = 1f;
        LoadScene(gameSceneName);
    }
    
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("Scene name is empty or null!");
            return;
        }
        
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
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
        }
    }
    
    public void CloseSettings()
    {
        OpenMainMenu();
    }
    
    public void SetMouseSensitivity(float sensitivity)
    {
        // Save mouse sensitivity to PlayerPrefs for the game scene to use
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
    }
    
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
    }
    
    public void ApplySettings()
    {
        // Save all settings to disk
        PlayerPrefs.Save();
        CloseSettings();
    }
    #endregion
    
    #region Credits Menu Functions
    public void OpenCredits()
    {
        CloseAllMenus();
        
        if (creditsMenuPanel != null)
        {
            creditsMenuPanel.SetActive(true);
            currentActiveMenu = creditsMenuPanel;
        }
    }
    
    public void CloseCredits()
    {
        OpenMainMenu();
    }
    #endregion
    
    #region Utility Functions
    void CloseAllMenus()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
        if (creditsMenuPanel != null) creditsMenuPanel.SetActive(false);
        
        currentActiveMenu = null;
    }
    
    void SetCursorState(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
    
    // Public getters for other scripts
    public GameObject GetCurrentActiveMenu() { return currentActiveMenu; }
    public bool IsInMainMenu() { return currentActiveMenu == mainMenuPanel; }
    public bool IsInSettings() { return currentActiveMenu == settingsMenuPanel; }
    public bool IsInCredits() { return currentActiveMenu == creditsMenuPanel; }
    #endregion
}