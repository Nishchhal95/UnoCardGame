using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Multi_TurnManager : PunTurnManager, IPunTurnManagerCallbacks
{
    [SerializeField] private Multi_GameManager multi_GameManager;
    [SerializeField] private float MAX_TURNDURATION = 10f;
    [SerializeField] private int currentTurn = 0;

    [SerializeField] public TextMeshProUGUI turnText;
    [SerializeField] public Image turnTimerFill;

    private void Awake()
    {
        TurnManagerListener = this;
    }

    private void Start()
    {
        TurnDuration = MAX_TURNDURATION;
    }

    public void OnTurnComplete(int byHowMuch)
    {
        //if(!multi_GameManager.IsItMyTurn(PhotonNetwork.LocalPlayer.UserId))
        //{
        //    return;
        //}
        //Debug.Log("OnClick : OnTurnComplete");
        currentTurn += byHowMuch;
        if (currentTurn < 0)
        {
            currentTurn = PhotonNetwork.CurrentRoom.PlayerCount + currentTurn;
        }
        currentTurn = Mathf.Abs(currentTurn % PhotonNetwork.CurrentRoom.PlayerCount);

        //_GameManager.Instance.TurnOverComplete();
        //ChangeTurn(currentTurn + byHowMuch);
        ChangeTurn(currentTurn);
    }

    public void ChangeTurn(int value)
    {
        _isOverCallProcessed = false;

        PhotonNetwork.CurrentRoom.SetTurn(value, true);
    }


    //TURN CALLBACKS
    public void OnPlayerFinished(Player player, int turn, object move)
    {
        Debug.Log("OnPlayerFinished");
    }

    public void OnPlayerMove(Player player, int turn, object move)
    {
        Debug.Log("OnPlayerMove");
    }

    public void OnTurnBegins(int turn)
    {
        //For all Client to match TURN
        currentTurn = turn;

        Debug.Log("OnTurnBegins");
        //Only Server is Allowed to Set Room Properties
        if(PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable _customProperties = new ExitGames.Client.Photon.Hashtable();
            if (currentTurn < 0)
            {
                currentTurn = Mathf.Abs(PhotonNetwork.CurrentRoom.PlayerCount + currentTurn);
            }
            int currentTurnPlayerIndex = (currentTurn % PhotonNetwork.CurrentRoom.PlayerCount) + 1;
            _customProperties[Multi_GameManager.CURRENT_TURN_ROOM_PROPERTY] = PhotonNetwork.CurrentRoom.Players[currentTurnPlayerIndex].UserId;
            PhotonNetwork.CurrentRoom.SetCustomProperties(_customProperties);
        }
    }

    public void OnTurnCompleted(int turn)
    {
        Debug.Log("OnTurnCompleted");
    }

    public void OnTurnTimeEnds(int turn)
    {
        Debug.Log("OnTurnTimeEnds");
    }
}
