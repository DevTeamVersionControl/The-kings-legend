using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    [SerializeField] GameObject menuOptionsController;
    [SerializeField] GameObject tutoController;
    [SerializeField] GameObject mainPage;
    [SerializeField] GameObject pausePage;
    [SerializeField] BoxCollider boxColliderBook;
    [SerializeField] CanvasGroup rightPage;
    [SerializeField] CanvasGroup leftPage;
    [SerializeField] GameObject CameraMenu;
    Animator animator;

    private bool _firstTime = true;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void OnStartGame()
    {
        rightPage.alpha = 0;
        StartCoroutine(FadeOut(leftPage, 0.5f, 0.5f));
        animator.SetTrigger("startGame");
        GameManager.Instance.ChangeLevel(GameManager.GameLevel.GAME);
        mainPage.SetActive(false);
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
        mainPage.SetActive(true);
        Debug.Log("in OnOpenBook");
        StartCoroutine(FadeIn(rightPage, 0.5f, 0.5f));
        
        animator.SetTrigger("mainMenu");
    }
    
    public void OnPause()
    {
        mainPage = pausePage;
        rightPage.alpha = 0;
        StartCoroutine(FadeOut(leftPage, 0.5f, 0.5f));
        boxColliderBook.enabled = false;
        mainPage.SetActive(true);
        Debug.Log("in OnPause");
        StartCoroutine(FadeIn(rightPage, 0.5f, 0.5f));
        animator.SetTrigger("onPause");

    }

    public void OnResume()
    {
        rightPage.alpha = 0;
        StartCoroutine(FadeOut(leftPage, 0.5f, 0.5f));
        animator.SetTrigger("startGame");
        GameManager.Instance.OnPause();
        mainPage.SetActive(false);
    }
    public void OnCloseBook()
    {
        animator.SetTrigger("mainMenu");
    }

    public void OnNewGame()
    {
        rightPage.alpha = 0;
        StartCoroutine(FadeOut(leftPage, 0.5f, 0.5f));
        animator.SetTrigger("startGame");
        GameManager.Instance.PlayAgain();
        GameManager.Instance.OnPause();
        mainPage.SetActive(false);
    }

    public void OnOpenOptions()
    {
        rightPage.alpha = 0;
        StartCoroutine(FadeOut(leftPage, 0.5f, 0.5f));
        animator.SetTrigger("menuOptions");
        StartCoroutine(FadeIn(rightPage, 0.5f, 0.5f));
        StartCoroutine(FadeIn(leftPage, 0.5f, 0.5f));
        mainPage.SetActive(false);
        menuOptionsController.SetActive(true);
    }
    public void OnOpenMainMenu()
    {
        mainPage.SetActive(true);
    }
    public void OnOpenTuto()
    {
        rightPage.alpha = 0;
        StartCoroutine(FadeOut(leftPage, 0.5f, 0.5f));
        animator.SetTrigger("menuTuto");
        StartCoroutine(FadeIn(rightPage, 0.5f, 0.5f));
        StartCoroutine(FadeIn(leftPage, 0.5f, 0.5f));
        mainPage.SetActive(false);
        tutoController.SetActive(true);
    }
    public void OnCredit()
    {
        rightPage.alpha = 0;
        rightPage.interactable = false;
        animator.SetTrigger("credit");
        StartCoroutine(DelayOpenCredit());
    }
    IEnumerator DelayOpenCredit()
    {
        yield return new WaitForSeconds(2);
        boxColliderBook.enabled = true;
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
