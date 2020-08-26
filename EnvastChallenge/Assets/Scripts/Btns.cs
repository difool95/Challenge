using UnityEngine;

public class Btns : MonoBehaviour
{
    public void Startlevel1()
    {
        LevelManager._instance.StartLevel1();
    }

    public void QuitGame()
    {
        LevelManager._instance.QuitGame();
    }
}
