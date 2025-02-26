using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject playPanel;
    [SerializeField] private GameObject settingPanel;

    [Header("Audios")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioSource buttonAudioSource;

    [Header("Animation")]
    [SerializeField] private Animator playerAnimator;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        PlaybackgroundMusic();
    }

    public void EnableMainPanel()
    {
        mainPanel.SetActive(true);
        playPanel.SetActive(false);
        settingPanel.SetActive(false);

        PlayMainAnimation();
    }
    public void EnablePlayPanel()
    {
        mainPanel.SetActive(false);
        playPanel.SetActive(true);
        settingPanel.SetActive(false);

        PlayPlayAnimation();
    }
    public void EnableSettingPanel()
    {
        mainPanel.SetActive(false);
        playPanel.SetActive(false);
        settingPanel.SetActive(true);


        PlaySettingAnimation();
    }
    public void PlaybackgroundMusic()
    {
        backgroundMusicSource.Play();
    }
    public void PlayButtionSound()
    {
        buttonAudioSource.Play();
    }

    public void PlayMainAnimation()
    {
        playerAnimator.SetTrigger("Main");
    }
    public void PlayPlayAnimation()
    {
        playerAnimator.SetTrigger("Play");
    }
    public void PlaySettingAnimation()
    {
        playerAnimator.SetTrigger("Setting");
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetMasterVolume(float Volume)
    {
        audioMixer.SetFloat("MasterVolume", Volume);
        if (Volume <= -20)
        {
            audioMixer.SetFloat("MasterVolume", -80);
        }
    }  
     
    public void SetMusicVolume(float Volume)
    {
        audioMixer.SetFloat("MusicVolume", Volume);
        if (Volume <= -20)
        {
            audioMixer.SetFloat("MusicVolume", -80);
        } 
    }

    public void SetSFXVolume(float Volume)
    {
        audioMixer.SetFloat("SFXVolume", Volume);
        if(Volume <= -20)
        {
            audioMixer.SetFloat("SFXVolume", -80);
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}
