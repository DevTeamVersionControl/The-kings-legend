using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour
{
    public UnityEvent OnClick;
    public UnityEvent OnUnpause;

    void Start()
    {
        OnClick ??= new UnityEvent();
        OnUnpause ??= new UnityEvent();
    }

    // Update is called once per frame
    void Update()
    {
         if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance._currentLevel == GameManager.GameLevel.GAME && !GameManager.Instance.isPaused)
            {
                Debug.Log("Should not be here");
                OnClick.Invoke();
            }
            else if (GameManager.Instance.isPaused)
            {
                Debug.Log("wtfffffff");
                OnUnpause.Invoke();
            }
            
        }
    }
}
