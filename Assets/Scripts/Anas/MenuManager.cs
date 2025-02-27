using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject playPanel;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private Toggle isWithCutsceneToggle;
    [SerializeField] private TMP_Dropdown graphicsDropdown;


    [Header("Audios")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioSource buttonAudioSource;
    [SerializeField] private AudioClip[] randomAudios;
    [SerializeField] private AudioSource randomAudioSource;

    [Header("Animation")]
    [SerializeField] private Animator playerAnimator;


    private int lastPlayedIndex = -1;

    private void Awake()
    {
        Instance = this;
        //if (Instance == null)
        //{
            
        //    DontDestroyOnLoad(gameObject);
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}
    }
    private void Start()
    {
        PlaybackgroundMusic();
        Invoke("PlayRandomSound", 4f);
        

        GameData.FirstPlayerStarts = 0;
        GameData.SecondPlayerStarts = 0;

        audioMixer.SetFloat("MusicVolume", -14);
        SetQuality(GameData.CurrentGraphicsIndex);
        graphicsDropdown.value = GameData.CurrentGraphicsIndex;
    }
    private void Update()
    {
        
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
        GameData.CurrentGraphicsIndex = qualityIndex;
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
    public void PlayMultiplayerScene()
    {
        SceneManager.LoadScene(2);
    }
    public void PlayArcadeScene()
    {
        if (isWithCutsceneToggle.isOn)
        {
            SceneManager.LoadScene(3);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }
    public void PlayRandomSound()
    {
        if (randomAudios == null || randomAudios.Length == 0 || randomAudioSource == null)
        {
            Debug.LogWarning("Random sound cannot be played: Either the audio source or the clips array is empty/null.");
            return;
        }

        int rand;
        do
        {
            rand = Random.Range(0, randomAudios.Length);
        } while (rand == lastPlayedIndex && randomAudios.Length > 1);

        lastPlayedIndex = rand;
        randomAudioSource.clip = randomAudios[rand];
        randomAudioSource.Play();

        Invoke("PlayRandomSound", 5f);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
