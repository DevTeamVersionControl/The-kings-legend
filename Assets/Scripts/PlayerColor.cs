using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerColor { GREEN, PURPLE };

static class PlayerColorExtensions 
{
    public static PlayerColor GetOpposite(PlayerColor opposite) 
    {
        return opposite == PlayerColor.GREEN ? PlayerColor.PURPLE : PlayerColor.GREEN;
    }
}