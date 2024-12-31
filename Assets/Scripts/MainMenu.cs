using UnityEngine;

public class MainMenu : MonoBehaviour
{

    [SerializeField] GameObject menuOptionsController;
    [SerializeField] GameObject tutoController;
    [SerializeField] GameObject mainPage;
    [SerializeField] BoxCollider boxColliderRibbon;
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void OnStartGame()
    {
        animator.SetTrigger("startGame");
        GameManager.Instance.ChangeLevel(GameManager.GameLevel.GAME);
    }

    public void OnOpenBook()
    {
        boxColliderRibbon.enabled = false;
        mainPage.SetActive(true);
        animator.SetTrigger("mainMenu");
    }

    public void OnCloseBook()
    {
        animator.SetTrigger("mainMenu");
    }

    public void OnOpenOptions()
    {
        animator.SetTrigger("menuOptions");
        mainPage.SetActive(false);
        menuOptionsController.SetActive(true);
    }

    public void OnOpenTuto()
    {
        animator.SetTrigger("menuTuto");
        mainPage.SetActive(false);
        tutoController.SetActive(true);
    }
    public void OnCredit()
    {
        boxColliderRibbon.enabled = true;
        animator.SetTrigger("credit");
    }

    public void OnQuit()
    {
        Application.Quit();
    }
        
}
