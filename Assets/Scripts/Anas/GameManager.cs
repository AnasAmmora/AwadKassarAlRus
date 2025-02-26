using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject controlsUI;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private GameObject[] firstPlayerStars;
    [SerializeField] private GameObject[] secondPlayerStars;
    [SerializeField] private TMPro.TextMeshProUGUI winnerPlayerName;


    [Header("Audio References")]
    [SerializeField] private AudioSource buttonAudioSource;

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

    public void Endlevel()
    {
        controlsUI.SetActive(false);
        UpdatePlayersStars();
        endPanel.SetActive(true);
    }

    public void UpdatePlayersStars()
    {
        for (int i =0;i<GameData.FirstPlayerStarts;i++)
        {
            firstPlayerStars[i].SetActive(true);
        }
        for (int i = 0; i < GameData.SecondPlayerStarts; i++)
        {
            secondPlayerStars[i].SetActive(true);
        }

        if(GameData.FirstPlayerStarts >= 3)
        {
            winnerPlayerName.gameObject.SetActive(true);
            winnerPlayerName.text = "THE WINNER IS First Player";
        }
        else if (GameData.SecondPlayerStarts >= 3)
        {
            winnerPlayerName.gameObject.SetActive(true);
            winnerPlayerName.text = "THE WINNER IS Second Player";
        }
    }

    public void PlayButtionSound()
    {
        buttonAudioSource.Play();
    }
    public void NextLevel()
    {
        if (GameData.FirstPlayerStarts >= 3 || GameData.SecondPlayerStarts >= 3)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }  
    }
}
