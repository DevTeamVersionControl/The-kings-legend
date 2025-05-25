using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    [SerializeField] GameObject menuOptionsController;
    [SerializeField] GameObject tutoController;
    [SerializeField] GameObject mainPage;
    [SerializeField] GameObject pausePage;
    [SerializeField] GameObject currentPage;
    [SerializeField] BoxCollider boxColliderBook;
    [SerializeField] CanvasGroup rightPage;
    [SerializeField] CanvasGroup leftPage;
    [SerializeField] GameObject CameraMenu;
    Animator animator;

    private bool _firstTime = true;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        currentPage = mainPage;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && currentPage.gameObject.activeInHierarchy && GameManager.Instance._currentLevel == GameManager.GameLevel.MAINMENU)
        {
            rightPage.alpha = 0;
            StartCoroutine(FadeOut(leftPage, 0.5f, 0.5f));
            animator.SetTrigger("onCover");
            currentPage.SetActive(false);
            boxColliderBook.enabled = true;
        } 
    }
    public void OnStartGame()
    {
        rightPage.alpha = 0;
        StartCoroutine(FadeOut(leftPage, 0.5f, 0.5f));
        animator.SetTrigger("startGame");
        GameManager.Instance.ChangeLevel(GameManager.GameLevel.GAME);
        currentPage.SetActive(false);
    }

    public void OnOpenBook()
    {

        if (_firstTime) {
            _firstTime = false;
            CameraMenu.GetComponent<Animation>().Play("OpenBook");
        }
        rightPage.alpha = 0;
        StartCoroutine(FadeOut(leftPage, 0.5f, 0.5f));
        boxColliderBook.enabled = false;
        currentPage.SetActive(true);
        Debug.Log("in OnOpenBook");
        StartCoroutine(FadeIn(rightPage, 0.5f, 0.5f));
        
        animator.SetTrigger("mainMenu");
    }
    
    public void OnPause()
    {
        currentPage = pausePage;
        rightPage.alpha = 0;
        StartCoroutine(FadeOut(leftPage, 0.5f, 0.5f));
        boxColliderBook.enabled = false;
        currentPage.SetActive(true);
        Debug.Log("in OnPause");
        StartCoroutine(FadeIn(rightPage, 0.5f, 0.5f));
        animator.SetTrigger("onPause");

    }

    public void OnResume()
    {
        rightPage.alpha = 0;
        StartCoroutine(FadeOut(leftPage, 0.5f, 0.5f));
        StartCoroutine(BackToGameDelay());
        GameManager.Instance.OnPause();
        currentPage.SetActive(false);
    }
    
    public void OnCloseBook()
    {
        animator.SetTrigger("mainMenu");
    }

    public void OnNewGame()
    {
        rightPage.alpha = 0;
        StartCoroutine(FadeOut(leftPage, 0.5f, 0.5f));
        StartCoroutine(BackToGameDelay());
        GameManager.Instance.PlayAgain();
        GameManager.Instance.OnPause();
        currentPage.SetActive(false);
    }

    public void OnOpenOptions()
    {
        rightPage.alpha = 0;
        StartCoroutine(FadeOut(leftPage, 0.5f, 0.5f));
        animator.SetTrigger("menuOptions");
        StartCoroutine(FadeIn(rightPage, 0.5f, 0.5f));
        StartCoroutine(FadeIn(leftPage, 0.5f, 0.5f));
        currentPage.SetActive(false);
        menuOptionsController.SetActive(true);
    }
    public void OnOpenMainMenu()
    {
        currentPage.SetActive(true);
    }
    public void OnOpenTuto()
    {
        rightPage.alpha = 0;
        StartCoroutine(FadeOut(leftPage, 0.5f, 0.5f));
        animator.SetTrigger("menuTuto");
        StartCoroutine(FadeIn(rightPage, 0.5f, 0.5f));
        StartCoroutine(FadeIn(leftPage, 0.5f, 0.5f));
        currentPage.SetActive(false);
        tutoController.SetActive(true);
    }
    public void OnCredit()
    {
        rightPage.alpha = 0;
        currentPage.SetActive(false);
        rightPage.interactable = false;
        animator.SetTrigger("credit");
        StartCoroutine(DelayOpenCredit());
    }

    public void OnEndGame()
    {
        currentPage = mainPage;
        rightPage.alpha = 0;
        StartCoroutine(FadeOut(leftPage, 0.5f, 0.5f));
        boxColliderBook.enabled = false;
        currentPage.SetActive(true);
        Debug.Log("in OnEndGame");
        StartCoroutine(FadeIn(rightPage, 0.5f, 0.5f));
        animator.SetTrigger("onPause");
    }
    IEnumerator DelayOpenCredit()
    {
        yield return new WaitForSeconds(2);
        boxColliderBook.enabled = true;
    }

    IEnumerator BackToGameDelay()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("onCover");
    }
    public void OnQuit()
    {
        Application.Quit();
    }

    public IEnumerator FadeIn(CanvasGroup canvasGroup, float duration, float delay)
    {
        canvasGroup.interactable = false;
        // Wait for the specified delay before starting the fade
        yield return new WaitForSeconds(delay);
        
        float startAlpha = canvasGroup.alpha;
        float endAlpha = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);

            yield return null;
        }

        canvasGroup.interactable = true;
        // Ensure alpha is set to exactly 1 at the end
        canvasGroup.alpha = endAlpha;
    }

    public IEnumerator FadeOut(CanvasGroup canvasGroup, float duration, float delay)
    {
        canvasGroup.interactable = false;
        // Wait for the specified delay before starting the fade
        yield return new WaitForSeconds(delay);

        float startAlpha = canvasGroup.alpha;
        float endAlpha = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);

            yield return null;
        }

        // Ensure alpha is set to exactly 0 at the end
        canvasGroup.interactable = true;
        canvasGroup.alpha = endAlpha;
    }

}
