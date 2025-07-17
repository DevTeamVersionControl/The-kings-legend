using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialController : MonoBehaviour
{
    [SerializeField] CanvasGroup leftCanvas;
    [SerializeField] CanvasGroup rightCanvas;

    [SerializeField] GameObject[] leftPages;
    [SerializeField] GameObject[] rightPages;

    private int currentPage;

    private void OnEnable()
    {
        currentPage = 0;
        leftPages[0].SetActive(true);
        rightPages[0].SetActive(true);
    }

    public void GoBack()
    {
        for (int i = 0; i < leftPages.Length; i++) {
            leftPages[i].SetActive(false);
            rightPages[i].SetActive(false);
        }
        
        currentPage = 0;
        this.gameObject.SetActive(false);
    }

    public void nextPage()
    {
        Debug.Log("next page bitch");
        StartCoroutine(FadeOut(leftCanvas, 0f, 0.1f, leftPages[currentPage]));
        StartCoroutine(FadeOut(rightCanvas, 0f, 0.1f, rightPages[currentPage]));
        currentPage++;
        StartCoroutine(FadeIn(leftCanvas, 0.5f, 0.75f, leftPages[currentPage]));
        StartCoroutine(FadeIn(rightCanvas, 0.5f, 0.75f, rightPages[currentPage]));
    }
    public void previousPage()
    {
        StartCoroutine(FadeOut(leftCanvas, 0f, 0.1f, leftPages[currentPage]));
        StartCoroutine(FadeOut(rightCanvas, 0f, 0.1f, rightPages[currentPage]));
        currentPage--;
        StartCoroutine(FadeIn(leftCanvas, 0.5f, 0.75f, leftPages[currentPage]));
        StartCoroutine(FadeIn(rightCanvas, 0.5f, 0.75f, rightPages[currentPage]));

    }

    public IEnumerator FadeIn(CanvasGroup canvasGroup, float duration, float delay, GameObject page)
    {
        
        canvasGroup.interactable = false;
        yield return new WaitForSeconds(delay);
        page.SetActive(true);
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

    public IEnumerator FadeOut(CanvasGroup canvasGroup, float duration, float delay, GameObject page)
    {
        canvasGroup.interactable = false;
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
        page.SetActive(false);
        canvasGroup.alpha = endAlpha;
    }
}
