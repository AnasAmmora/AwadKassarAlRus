using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private float cutsceneDuration = 20f; // Duration before switching scene
    [SerializeField] private string nextSceneName; // Name of the next scene

    private void Start()
    {
        StartCoroutine(WaitAndLoadNextScene());
    }

    private IEnumerator WaitAndLoadNextScene()
    {
        yield return new WaitForSeconds(cutsceneDuration);
        SceneManager.LoadScene(nextSceneName);
    }
}
