using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempTurnManager : PunTurnManager, IPunTurnManagerCallbacks
{
    public Text currentTurnText;

    // Start is called before the first frame update
    void Start()
    {
        this.TurnManagerListener = this;
        TurnDuration = 10f;
    }

    //TURN MANAGER CALLBACKS
    public void OnTurnBegins(int turn)
    {
        Debug.Log("OnTurnBegins: " + turn);
        currentTurnText.text = "Current Turn: " + Turn.ToString();
    }

    public void OnTurnCompleted(int turn)
    {
        Debug.Log("OnTurnCompleted: " + turn);
    }

    public void OnPlayerMove(Player player, int turn, object move)
    {
        Debug.Log("OnPlayerMove: " + player.NickName + ", Player ID: " + player.UserId + ", Turn: " + turn);
        if (Photon.Pun.PhotonNetwork.IsMasterClient && Turn == turn)
        {
            BeginTurn();
        }
    }

    public void OnPlayerFinished(Player player, int turn, object move)
    {
        Debug.Log("OnPlayerFinished: " + turn);
    }

    public void OnTurnTimeEnds(int turn)
    {
        Debug.Log("OnTurnTimeEnds: " + turn);
        BeginTurn();
    }
}
