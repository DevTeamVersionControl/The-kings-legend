using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public void OnStartGame()
    {
        GameManager.Instance.ChangeLevel(GameManager.GameLevel.GAME);
    }
}
