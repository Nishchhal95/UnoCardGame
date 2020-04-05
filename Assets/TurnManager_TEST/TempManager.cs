using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TempManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public GameObject roomJoinUI;
    public InputField roomNameInputField;

    public InputField playerNameText;

    public TempTurnManager tempTurnManager;

    public void ConnectClick()
    {
        string playerName = PhotonNetwork.LocalPlayer.NickName = playerNameText.text.Trim();
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom()
    {
        string roomName = roomNameInputField.text;
        roomName = roomName.Trim();

        if(string.IsNullOrEmpty(roomName))
        {
            return;
        }

        PhotonNetwork.CreateRoom(roomName);
    }

    public void JoinRoom()
    {
        string roomName = roomNameInputField.text;
        roomName = roomName.Trim();

        if (string.IsNullOrEmpty(roomName))
        {
            return;
        }

        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
    }

    public override void OnJoinedRoom()
    {
        roomJoinUI.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
        {
            return;
        }

        //tempTurnManager.BeginTurn();
        TurnExtensions.SetTurn(PhotonNetwork.CurrentRoom, -1, true);
        tempTurnManager.BeginTurn();
        PhotonNetwork.RaiseEvent(13, null, new RaiseEventOptions { Receivers = ReceiverGroup.All }, new SendOptions());
    }

    public void StartGame()
    {
        tempTurnManager.gameObject.SetActive(true);
    }

    //Game
    public void OnGameButtonClick()
    {
        tempTurnManager.SendMove(null, false);
    }

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == 13)
        {
            StartGame();
        }
    }
}
