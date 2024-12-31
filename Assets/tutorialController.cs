using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialController : MonoBehaviour
{

    [SerializeField] GameObject leftPage;
    [SerializeField] GameObject rightPage;
    // Start is called before the first frame update

    private void OnEnable()
    {
        leftPage.SetActive(true);
        rightPage.SetActive(true);
    }

    public void GoBack()
    {
        leftPage.SetActive(false);
        rightPage.SetActive(false);
        this.gameObject.SetActive(false);
    }
}
