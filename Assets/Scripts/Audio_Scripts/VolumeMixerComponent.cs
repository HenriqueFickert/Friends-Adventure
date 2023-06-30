using UnityEngine;
using UnityEngine.Audio;

public class VolumeMixerComponent : MonoBehaviour
{
    private AudioSource audioSource;
    public bool isMusic;
    public bool isActivated = true;

    private float actualVolume;
    private float storedVolume;

    public bool isStereonPanActive;
    private Transform playerTransform;

    private float storedMaxVolume;
    private float storedAjustableVolume;

    [Range(0.0f, 1.0f)]
    public float ajustableVolume = 1;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (isStereonPanActive)
        {
            playerTransform = GameObject.Find("Player").transform;
        }

        ChangeVolume();
        UpdateVolume();
    }

    void Update()
    {
        UpdateVolume();

        if (isStereonPanActive)
        {
            audioSource.panStereo = transform.position.x - playerTransform.position.x;
        }
    }

    void UpdateVolume()
    {
        if (actualVolume != (isMusic ? SettingsManager.musicVolume : SettingsManager.sfxVolume) || storedMaxVolume != SettingsManager.maxVolume || ajustableVolume != storedAjustableVolume)
        {
            ChangeVolume();
        }
    }

    void ChangeVolume()
    {
        if (isMusic)
        {
            storedVolume = SettingsManager.musicVolume;
        }
        else
        {
            storedVolume = SettingsManager.sfxVolume;
        }

        if (isActivated)
        {
            actualVolume = storedVolume;
        }// Here, to change audio not based on settings volume

        storedMaxVolume = SettingsManager.maxVolume;
        storedAjustableVolume = ajustableVolume;

        audioSource.volume = actualVolume * SettingsManager.maxVolume * ajustableVolume;
    }
}
