using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public enum PlayerColor { GREEN, PURPLE };

    private PlayerColor _playerColorTurn;
    private enum _gameState { ADDUPGRADE, ATTACKMOVE };
    private enum _subState { DRAGGING, ATTACKING, NONE };




    public void Start()
    {

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
