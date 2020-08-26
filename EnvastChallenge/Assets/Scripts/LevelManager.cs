using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    public static LevelManager _instance;

    void Awake()
    {

        if (_instance == null)
        {

            _instance = this;
            DontDestroyOnLoad(gameObject);

            //Rest of your Awake code

        }
        else
        {
            Destroy(this);
        }
    }

    public void StartLevel1()
    {
        SceneManager.LoadScene(1);
    }

    public void StartRetryMenu()
    {
        SceneManager.LoadScene(2);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
