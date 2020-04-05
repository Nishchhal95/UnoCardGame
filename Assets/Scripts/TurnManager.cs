using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : PunTurnManager, IPunTurnManagerCallbacks
{
    public int currentTurn = 0;

    private void Start()
    {
        TurnManagerListener = this;
        TurnDuration = 10f;

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetTurn(-1, true);
            BeginTurn();
        }
    }

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

    public void OnTurnBegins(int turn)
    {
        Debug.Log("OnTurnBegins: " + turn);
    }

    public void OnTurnCompleted(int turn)
    {
        Debug.Log("OnTurnCompleted: " + turn);
    }

    public void OnPlayerMove(Player player, int turn, object move)
    {
        Debug.Log("OnPlayerMove: Player: " + player.NickName +  " TURN: " + turn);

        //TODO : Add player Move Logic
        CardModel cardPlayer = (CardModel)move;


        if(PhotonNetwork.IsMasterClient)
        {
            BeginTurn();
        }
    }

    public void OnPlayerFinished(Player player, int turn, object move)
    {
        Debug.Log("OnPlayerFinished: Player: " + player.NickName + " TURN: " + turn);
    }

    public void OnTurnTimeEnds(int turn)
    {
        currentTurn++;
        Debug.Log("OnPlayerFinished: " + turn);
        //PhotonNetwork.CurrentRoom.SetTurn(currentTurn);
        if (PhotonNetwork.IsMasterClient)
        {
            BeginTurn();
        }
    }
}
