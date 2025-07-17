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
    [SerializeField] GameObject _cameraTuto;
    [SerializeField] private AudioSource _UIClick;
    [SerializeField] private AudioSource _UIHover;

    [SerializeField] private AudioSource _ChairSlidingGreen;
    [SerializeField] private AudioSource _ChairSlidingPurple;
    [SerializeField] private AudioSource _CloseBook;
    [SerializeField] private AudioSource _OpenBook;
    [SerializeField] private AudioSource _TurnPage;

    private float minPitch = 0.8f;
    private float maxPitch = 1.2f;

    private Animator animator;

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
        StartCoroutine(ChairMovingSound());
    }

    public void LeaveTutoSwitchCamera()
    {
        _cameraTuto.SetActive(false);
        CameraMenu.SetActive(true);
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
        StartCoroutine(FadeIn(rightPage, 0.5f, 1.25f));
        
        animator.SetTrigger("mainMenu");
        PlayOpenBookSound();
    }
    
    public void OnGoBackToMainPage()
    {
        rightPage.alpha = 0;
        StartCoroutine(FadeOut(leftPage, 0.5f, 0.5f));
        currentPage.SetActive(true);
        Debug.Log("in OnOpenBook");
        StartCoroutine(FadeIn(rightPage, 0.5f, 0.75f));

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
        StartCoroutine(FadeIn(rightPage, 0.5f, 1.25f));
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
        StartCoroutine(ChairMovingSound());
    }

    public void OnOpenOptions()
    {
        rightPage.alpha = 0;
        StartCoroutine(FadeOut(leftPage, 0.5f, 0.5f));
        animator.SetTrigger("menuOptions");
        StartCoroutine(FadeIn(rightPage, 0.5f, 0.75f));
        StartCoroutine(FadeIn(leftPage, 0.5f, 0.75f));
        currentPage.SetActive(false);
        menuOptionsController.SetActive(true);
    }
    public void OnOpenMainMenu()
    {
        currentPage.SetActive(true);
    }
    public void OnOpenTuto()
    {
        _cameraTuto.SetActive(true);
        CameraMenu.SetActive(false);
        rightPage.alpha = 0;
        StartCoroutine(FadeOut(leftPage, 0.5f, 0.5f));
        animator.SetTrigger("menuTuto");
        StartCoroutine(FadeIn(rightPage, 0.5f, 0.75f));
        StartCoroutine(FadeIn(leftPage, 0.5f, 0.75f));
        currentPage.SetActive(false);
        tutoController.SetActive(true);
    }

    public void ChangePageTutoLeft()
    {
        animator.SetTrigger("PreviousTuto");
    }
    public void ChangePageTutoRight()
    {
        animator.SetTrigger("NextTuto");
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
        StartCoroutine(FadeIn(rightPage, 0.5f, 1.25f));
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
        canvasGroup.alpha = endAlpha;
    }

    public IEnumerator FadeOut(CanvasGroup canvasGroup, float duration, float delay)
    {
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

        canvasGroup.alpha = endAlpha;
    }

    public void OnClickSound()
    {
        _UIClick.pitch = Random.Range(minPitch, maxPitch);
        _UIClick.Play();
    }

    public void OnHoverSound(AudioSource audioSource)
    {
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.Play();
    }

    public void PlayCloseBookSound()
    {
        _CloseBook.pitch = Random.Range(minPitch, maxPitch);
        _CloseBook.Play();
    }

    public void PlayOpenBookSound()
    {
        _OpenBook.pitch = Random.Range(minPitch, maxPitch);
        _OpenBook.Play();
    }

    public void PlayTurnPageSound()
    {
        _TurnPage.pitch = Random.Range(minPitch, maxPitch);
        _TurnPage.Play();
    }
    private IEnumerator ChairMovingSound()
    {
        yield return new WaitForSeconds(0.5f);
        _ChairSlidingGreen.pitch = Random.Range(minPitch, maxPitch);
        _ChairSlidingGreen.Play();
        yield return new WaitForSeconds(1.5f);
        _ChairSlidingPurple.pitch = Random.Range(minPitch, maxPitch);
        _ChairSlidingPurple.Play();
    }
}
