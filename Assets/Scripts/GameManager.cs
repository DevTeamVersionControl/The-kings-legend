using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

   

    private PlayerColor _playerColorTurn;
    private enum _gameState { ADDUPGRADE, ATTACKMOVE };
    private enum _subState { DRAGGING, ATTACKING, NONE };


    public static GameManager Instance;

    public void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }




    // Update is called once per frame
    public void Update()
    {

    }

    public void OnNextTurn(PlayerColor color)
    {

    }

    public void OnPieceKilled()
    {

    }
    public void OnEndTurn()
    {

    }

    public void OnDrag(Tile tile)
    {

    }

    public void OnClick(Tile tile)
    {

    }

    public void CancelAttack()
    {

    }
    public void PotentialAttack()
    {

    }


}
