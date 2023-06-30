using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider audioMasterSlider;

    public Image[] muteButtons;
    public Sprite[] muteIcon;

    private float storedMusicVolume = 0;
    private float storedSFXVolume = 0;
    private float storedMaxVolume = 0;

    public static bool isMusicMuted;
    public static bool isSfxMuted;
    public static bool isMasterMuted;

    [Range(0f,1f)]
    public static float maxVolume;

    public static float currentMaxVolume;

    public static float musicVolume;
    public static float sfxVolume;

    public Dropdown resolutionDropdown;
    Resolution[] resolutions;

    public Toggle fullScreenToggle;
    private bool lockFullScreenStart = false;

    public static string languegeFileName = "pt_br";
    public static int languegeDropdownValue;

    private void Awake()
    {
        InitializePlayerPrefsVariables();
        MusicStart();
        currentMaxVolume = maxVolume;
    }

    private void Start()
    {
        ResolucaoStart();

        lockFullScreenStart = false;
        fullScreenToggle.isOn = Screen.fullScreen;
        lockFullScreenStart = true;
    }

    public void InitializePlayerPrefsVariables()
    {
        isMasterMuted = PlayerPrefs.GetInt("IsMasterMuted") == 1 ? true : false;
        isMusicMuted = PlayerPrefs.GetInt("IsMusicMuted") == 1 ? true : false;
        isSfxMuted = PlayerPrefs.GetInt("IsSfxMuted") == 1 ? true : false;
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        maxVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        languegeFileName = PlayerPrefs.GetString("Languege", "pt_br");
        languegeDropdownValue = PlayerPrefs.GetInt("DropdownValue", 0);
    }

    #region RESOLUTIONS
    public void ResolucaoStart()
    {
        resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
        resolutionDropdown.ClearOptions();

        List<string> resolutionText = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resolutionText.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        string resolutionx = Screen.currentResolution.ToString();

        resolutionDropdown.AddOptions(resolutionText);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen, resolution.refreshRate);
    }
    #endregion

    #region MUSIC
    public void MusicStart()
    {
        if (musicVolume == 0 || isMusicMuted)
        {
            musicVolume = 0f;
            musicSlider.interactable = false;
            muteButtons[0].sprite = muteIcon[1];
        }
        else
        {
            musicSlider.interactable = true;
            muteButtons[0].sprite = muteIcon[0];
        }

        if (sfxVolume == 0 || isSfxMuted)
        {
            sfxVolume = 0f;
            sfxSlider.interactable = false;
            muteButtons[1].sprite = muteIcon[1];
        }
        else
        {
            sfxSlider.interactable = true;
            muteButtons[1].sprite = muteIcon[0];
        }

        if (maxVolume == 0 || isMasterMuted)
        {
            maxVolume = 0f;
            audioMasterSlider.interactable = false;
            muteButtons[2].sprite = muteIcon[1];
        }
        else
        {
            audioMasterSlider.interactable = true;
            muteButtons[2].sprite = muteIcon[0];
        }

        audioMasterSlider.value = maxVolume;
        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
    }

    public void MasterAudioSlider()
    {
        maxVolume = audioMasterSlider.value;

        if (audioMasterSlider.value == 0)
        {
            isMasterMuted = !isMasterMuted;
            muteButtons[2].sprite = muteIcon[1];
        }
        else
        {
            muteButtons[2].sprite = muteIcon[0];
        }

        storedMaxVolume = maxVolume;
        PlayerPrefs.SetInt("IsMasterMuted", isMasterMuted ? 1 : 0);
        PlayerPrefs.SetFloat("MasterVolume", maxVolume);
    }

    public void MusicSlider()
    {
        musicVolume = musicSlider.value;

        if (musicSlider.value == 0)
        {
            isMusicMuted = !isMusicMuted;
            muteButtons[0].sprite = muteIcon[1];
        }
        else
        {
            muteButtons[0].sprite = muteIcon[0];
        }

        storedMusicVolume = musicVolume;
        PlayerPrefs.SetInt("IsMusicMuted", isMusicMuted ? 1 : 0);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }

    public void SFXSlider()
    {
        sfxVolume = sfxSlider.value;

        if (sfxSlider.value == 0)
        {
            isSfxMuted = !isSfxMuted;
            muteButtons[1].sprite = muteIcon[1];
        }
        else
        {
            muteButtons[1].sprite = muteIcon[0];
        }

        storedSFXVolume = sfxVolume;
        PlayerPrefs.SetInt("IsSfxMuted", isSfxMuted ? 1 : 0);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    public void MuteMusicButton()
    {
        isMusicMuted = !isMusicMuted;
        if (isMusicMuted)
        {
            if (musicVolume != 0)
            {
                muteButtons[0].sprite = muteIcon[1];
            }
            storedMusicVolume = musicVolume;
            musicSlider.interactable = false;
            musicVolume = 0f;
        }
        else
        {

            if (storedMusicVolume == 0)
            {
                musicVolume = 0.1f;
            }
            else
            {
                musicVolume = storedMusicVolume;
            }


            musicSlider.interactable = true;
            musicSlider.value = musicVolume;
            muteButtons[0].sprite = muteIcon[0];
        }
        PlayerPrefs.SetInt("IsMusicMuted", isMusicMuted ? 1 : 0);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }

    public void MuteSFXButton()
    {
        isSfxMuted = !isSfxMuted;
        if (isSfxMuted)
        {
            if (sfxVolume != 0)
            {
                muteButtons[1].sprite = muteIcon[1];
            }

            storedSFXVolume = sfxVolume;
            sfxSlider.interactable = false;
            sfxVolume = 0f;
        }
        else
        {

            if (storedSFXVolume == 0)
            {
                sfxVolume = 0.1f;
            }
            else
            {
                sfxVolume = storedSFXVolume;
            }


            sfxSlider.interactable = true;
            sfxSlider.value = sfxVolume;
            muteButtons[1].sprite = muteIcon[0];
        }
        PlayerPrefs.SetInt("IsSfxMuted", isSfxMuted ? 1 : 0);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    public void MuteMasterAudioButton()
    {
        isMasterMuted = !isMasterMuted;
        if (isMasterMuted)
        {
            if (maxVolume != 0)
            {
                muteButtons[2].sprite = muteIcon[1];
            }

            storedMaxVolume = maxVolume;
            audioMasterSlider.interactable = false;
            maxVolume = 0f;
        }
        else
        {
            if (storedMaxVolume == 0)
            {
                maxVolume = 0.1f;
            }
            else
            {
                maxVolume = storedMaxVolume;
            }

            audioMasterSlider.interactable = true;
            audioMasterSlider.value = maxVolume;
            muteButtons[2].sprite = muteIcon[0];
        }

        PlayerPrefs.SetInt("IsMasterMuted", isMasterMuted ? 1 : 0);
        PlayerPrefs.SetFloat("MasterVolume", maxVolume);
    }
    #endregion

    #region FULL SCREEN
    public void SetFullScreen()
    {
        if (lockFullScreenStart)
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }
    #endregion
}
