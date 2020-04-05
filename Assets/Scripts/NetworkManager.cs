using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance = null;

    public delegate void OnPhotonDisconnected();
    public static OnPhotonDisconnected onPhotonDisconnected;

    public delegate void OnPhotonConnected();
    public static OnPhotonConnected onPhotonConnected;

    public delegate void OnPhotonRoomCreated(Room currentRoom);
    public static OnPhotonRoomCreated onPhotonRoomCreated;

    public delegate void OnPhotonRoomJoined(Room currentRoom);
    public static OnPhotonRoomJoined onPhotonRoomJoined;

    public delegate void OnPhotonPlayerEnteredRoom(Player player);
    public static OnPhotonPlayerEnteredRoom onPhotonPlayerEnteredRoom;

    public delegate void OnPhotonRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable changedProps);
    public static OnPhotonRoomPropertiesUpdate onPhotonRoomPropertiesUpdate;

    public static bool isGameCreated = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        
    }

    public override void OnEnable()
    {
        base.OnEnable();
        
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.Disconnect();
    }

    #region Photon Functions

    public void ConnectPhoton()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { PublishUserId = true, PlayerTtl = 10000, BroadcastPropsChangeToAll = true });
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public bool RoomExists(string roomName)
    {
        if(!IsConnected())
        {
            return true;
        }

        return PhotonNetwork.GetCustomRoomList(TypedLobby.Default, "C0=" + roomName);
    }


    public bool IsConnected()
    {
        return PhotonNetwork.IsConnected;
    }
    #endregion


    #region CallBacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("On Photon Connected to Master");
        PhotonNetwork.LocalPlayer.NickName = "Player " + Random.Range(0, 100);
        onPhotonConnected?.Invoke();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconnected: " + cause.ToString());
        onPhotonDisconnected?.Invoke();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom RoomName: " + PhotonNetwork.CurrentRoom.Name);
        onPhotonRoomCreated?.Invoke(PhotonNetwork.CurrentRoom);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom: " + PhotonNetwork.CurrentRoom + " & Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount);
        onPhotonRoomJoined?.Invoke(PhotonNetwork.CurrentRoom);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        Debug.Log("OnRoomPropertiesUpdate");
        onPhotonRoomPropertiesUpdate?.Invoke(propertiesThatChanged);
    }

    //This callback is recieved on Master Client as this acts as Server...
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom: " + newPlayer.NickName);
        onPhotonPlayerEnteredRoom?.Invoke(newPlayer);
    }

    #endregion
}
