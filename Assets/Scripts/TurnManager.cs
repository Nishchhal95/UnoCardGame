using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public int currentTurn = 0;

    public void TurnOver(int byHowMuch)
    {
        currentTurn += byHowMuch;
        if(currentTurn < 0)
        {
            currentTurn = _GameManager.Instance.playerCount + currentTurn;
        }
        currentTurn = Mathf.Abs(currentTurn % _GameManager.Instance.playerCount);

        _GameManager.Instance.TurnOverComplete();
    }
}
