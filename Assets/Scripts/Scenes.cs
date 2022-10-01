using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    [SerializeField] float DelayAfterGameOver = 3f;

    void Start()
    {
        
    }

    public void LoadStartMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(1);
        //GameManager.Instance.ResetGame();
    }

    public void LoadGameOver()
    {
        StartCoroutine(LoadGameOverCoroutine());
    }

    private IEnumerator LoadGameOverCoroutine()
    {
        yield return new WaitForSeconds(DelayAfterGameOver);
        SceneManager.LoadScene("Game Over");
    }

    public void LoadSuccess()
    {
        StartCoroutine(LoadSuccessCoroutine());
    }

    private IEnumerator LoadSuccessCoroutine()
    {
        yield return new WaitForSeconds(DelayAfterGameOver);
        SceneManager.LoadScene("Success");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
