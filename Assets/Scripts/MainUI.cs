using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUI : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1);

    }
    public void QuitGame()
    {
        Application.Quit();
    }   
}
