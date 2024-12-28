using UnityEngine;

public class MainMenu : MonoBehaviour
{

    [SerializeField] GameObject _tuto;
    [SerializeField] GameObject menuOptions;

    public void OnStartGame()
    {
        GameManager.Instance.ChangeLevel(GameManager.GameLevel.GAME);


    }

    public void OnOpenTuto()
    {
        _tuto.SetActive(true);
    }

    public void OnOpenOptions()
    {
        menuOptions.SetActive(true);
    }
}
