using UnityEngine;

public class MainMenu : MonoBehaviour
{

    [SerializeField] GameObject _tuto;
    [SerializeField] GameObject menuOptions;
    [SerializeField] GameObject mainPage;
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void OnStartGame()
    {
        animator.SetBool("startGame", true);
        GameManager.Instance.ChangeLevel(GameManager.GameLevel.GAME);
    }

    public void OnOpenBook()
    {
        animator.SetBool("open", true);
    }

    public void OnCloseBook()
    {
        animator.SetBool("open", false);
    }

    public void OnOpenOptions()
    {
        animator.SetBool("menuOptions", true);
        mainPage.SetActive(false);
        menuOptions.SetActive(true);
    }


    public void OnCredit()
    {
        animator.SetBool("credit", true);
    }
}
