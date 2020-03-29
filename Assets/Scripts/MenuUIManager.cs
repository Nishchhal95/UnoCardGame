using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameJoiningUI;
    [SerializeField] private GameObject gameJoiningUITweeningBase;
    [SerializeField] private Button gameJoiningUICloseButton;

    [SerializeField] private GameObject roomJoinUI;
    [SerializeField] private GameObject roomJoinUITweeningBase;
    [SerializeField] private GameObject roomJoinUICloseButton;

    [SerializeField] private TMP_InputField roomNameInputField;

    [SerializeField] private float transitionDuration = .5f;

    [SerializeField] private Image photonStatusImage;
    [SerializeField] private TextMeshProUGUI photonStatusText;

    private void Start()
    {
        NetworkManager.isGameCreated = false;
    }

    private void OnEnable()
    {
        NetworkManager.onPhotonConnected += OnPhotonStausUpdate;
        NetworkManager.onPhotonDisconnected += OnPhotonStausUpdate;
        NetworkManager.onPhotonRoomJoined += OnRoomJoined;
    }

    private void OnDisable()
    {
        NetworkManager.onPhotonConnected -= OnPhotonStausUpdate;
        NetworkManager.onPhotonDisconnected -= OnPhotonStausUpdate;
        NetworkManager.onPhotonRoomJoined -= OnRoomJoined;
    }

    private void OnPhotonStausUpdate()
    {
        if(NetworkManager.Instance.IsConnected())
        {
            photonStatusImage.color = Color.green;
            photonStatusText.SetText("Connected");
        }

        else
        {
            photonStatusImage.color = Color.red;
            photonStatusText.SetText("Not Connected");
        }
    }

    public void OnPlayClick()
    {
        if (!NetworkManager.Instance.IsConnected())
        {
            NetworkManager.Instance.ConnectPhoton();
        }
        gameJoiningUITweeningBase.transform.localScale = new Vector3(0, 0, 0);
        gameJoiningUI.SetActive(true);
        LeanTween.scale(gameJoiningUITweeningBase, new Vector3(1, 1, 1), transitionDuration).setEaseInOutCubic();
    }

    public void OnGameJoiningUICloseClick()
    {
        LeanTween.scale(gameJoiningUITweeningBase, new Vector3(0, 0, 0), transitionDuration).setEaseInOutCubic().setOnComplete(() => { gameJoiningUI.SetActive(false); });
    }

    public void OnCreateGameClick()
    {
        if(!NetworkManager.Instance.IsConnected())
        {
            return;
        }

        string randomRoomName = "";
        do
        {
            int randomNumber = Random.Range(1000, 99999);
            randomRoomName = randomNumber.ToString();
        } while (NetworkManager.Instance.RoomExists(randomRoomName));

        NetworkManager.isGameCreated = true;
        NetworkManager.Instance.CreateRoom(randomRoomName);
    }

    public void OnJoinGameClick()
    {
        if (!NetworkManager.Instance.IsConnected())
        {
            return;
        }

        OnGameJoiningUICloseClick();
        roomNameInputField.text = "";
        roomJoinUITweeningBase.transform.localScale = new Vector3(0, 0, 0);
        roomJoinUI.SetActive(true);
        LeanTween.scale(roomJoinUITweeningBase, new Vector3(1, 1, 1), transitionDuration).setEaseInOutCubic();
    }


    public void OnJoinRoomClickAfterFillinRoomName()
    {
        string roomName = roomNameInputField.text;
        roomName = roomName.Trim();

        if(string.IsNullOrEmpty(roomName))
        {
            return;
        }

        NetworkManager.Instance.JoinRoom(roomName);
    }

    public void RoomJoinUICloseClick()
    {
        LeanTween.scale(roomJoinUITweeningBase, new Vector3(0, 0, 0), transitionDuration).setEaseInOutCubic().setOnComplete(() => { roomJoinUI.SetActive(false); });
    }

    private void OnRoomJoined(Photon.Realtime.Room room)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
