using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour
{
    public UnityEvent OnClick;


    void Start()
    {
        OnClick ??= new UnityEvent();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance._currentLevel == GameManager.GameLevel.GAME) { 
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnClick.Invoke();
            }
        }
    }
}
