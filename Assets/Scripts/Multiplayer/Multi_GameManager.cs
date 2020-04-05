// Decompiled with JetBrains decompiler
// Type: Multi_GameManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 09BA8776-0E72-4ECB-AED4-36697251DEEE
// Assembly location: D:\Unity Projects\Uno\Builds\Build Raise Event\Uno_Data\Managed\Assembly-CSharp.dll

using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multi_GameManager : MonoBehaviour, IOnEventCallback
{
    public static Multi_GameManager Instance = null;
    private static byte INIT_GAME_ON_ALL_CLIENTS = 10;
    private static byte CARD_PLAYED = 11;
    private static byte CARD_PICK_FROM_SIDE_DECK = 12;
    public static string CURRENT_TURN_ROOM_PROPERTY = "CurrentPlayerTurn";
    [SerializeField]
    private int playerRequired = 2;
    [SerializeField]
    private int cardRequiredToStartGame = 7;
    [SerializeField]
    public List<PlayerController> spawnedPlayerControllerList = new List<PlayerController>();
    [SerializeField]
    private int turnMoveDirection = 1;
    [SerializeField]
    private Multi_CardDealer multi_CardDealer;
    [SerializeField]
    private Multi_TurnManager multi_TurnManager;
    [SerializeField]
    private PlayerController playerPrefab;
    [SerializeField]
    private CardController cardPrefab;
    [SerializeField]
    private Transform[] playerSpawnPoints;
    [SerializeField]
    private Transform centerTableTrnasform;
    [SerializeField]
    private Transform sideDeckTransform;
    [SerializeField]
    private string currentTurnPlayerID;
    [SerializeField]
    public CardController lastCardController;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
        NetworkManager.onPhotonPlayerEnteredRoom += new NetworkManager.OnPhotonPlayerEnteredRoom(OnPlayerConnectedToMasterClientServer);
        NetworkManager.onPhotonRoomPropertiesUpdate += new NetworkManager.OnPhotonRoomPropertiesUpdate(OnPropertiesChanged);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        NetworkManager.onPhotonPlayerEnteredRoom -= new NetworkManager.OnPhotonPlayerEnteredRoom(this.OnPlayerConnectedToMasterClientServer);
        NetworkManager.onPhotonRoomPropertiesUpdate -= new NetworkManager.OnPhotonRoomPropertiesUpdate(this.OnPropertiesChanged);
    }

    private void OnPlayerConnectedToMasterClientServer(Player player)
    {
        if (!PhotonNetwork.IsMasterClient || (int)PhotonNetwork.CurrentRoom.PlayerCount != this.playerRequired)
            return;
        this.SetupGame();
        this.multi_TurnManager.ChangeTurn(0);
        UIManager.Instance.OnDisplayGameCreatePageCloseClick();
    }

    private void OnPropertiesChanged(ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (!changedProps.ContainsKey((object)Multi_GameManager.CURRENT_TURN_ROOM_PROPERTY))
            return;
        this.currentTurnPlayerID = (string)changedProps[(object)Multi_GameManager.CURRENT_TURN_ROOM_PROPERTY];
        if (!PhotonNetwork.IsMasterClient)
            return;
        this.StartCoroutine(this.CheckIfPlayerCanPlayACardRoutine(this.currentTurnPlayerID));
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient && (int)PhotonNetwork.CurrentRoom.PlayerCount != this.playerRequired)
            UIManager.Instance.DisplayGameCreatedPage(PhotonNetwork.CurrentRoom.Name);
        this.multi_CardDealer.Init();
    }

    private void Update()
    {
        if (this.spawnedPlayerControllerList != null && this.spawnedPlayerControllerList.Count > 0)
        {
            PlayerController playerController = this.spawnedPlayerControllerList.Find((Predicate<PlayerController>)(x => x.playerModel.playerID.Equals(this.currentTurnPlayerID)));
            if ((UnityEngine.Object)playerController != (UnityEngine.Object)null)
                this.multi_TurnManager.turnText.text = "Turn: " + playerController.playerModel.playerName;
        }
        this.multi_TurnManager.turnTimerFill.fillAmount = 1f / this.multi_TurnManager.TurnDuration * this.multi_TurnManager.RemainingSecondsInTurn;
    }

    private void SetupGame()
    {
        Multi_GameManager.Shuffle<CardModel>((IList<CardModel>)this.multi_CardDealer.cardsDeck);
        Dictionary<string, List<CardModel>> dictionary = new Dictionary<string, List<CardModel>>();
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            List<CardModel> pickedCards = new List<CardModel>();
            this.PickCards(this.cardRequiredToStartGame, out pickedCards);
            dictionary.Add(player.UserId, pickedCards);
        }
        CardModel cardModel = (CardModel)null;
        List<CardModel> cardsDeck = this.multi_CardDealer.cardsDeck;
        int index1 = 0;
        for (int index2 = 0; index2 < cardsDeck.Count; ++index2)
        {
            if (!cardsDeck[index2].IsSpecial && !cardsDeck[index2].IsWild)
            {
                index1 = index2;
                cardModel = cardsDeck[index2];
                break;
            }
        }
        cardsDeck.RemoveAt(index1);
        string str1 = JsonConvert.SerializeObject((object)this.multi_CardDealer.cardsDeck);
        string str2 = JsonConvert.SerializeObject((object)dictionary);
        string str3 = JsonConvert.SerializeObject((object)cardModel);
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.All
        };
        SendOptions sendOptions = new SendOptions()
        {
            DeliveryMode = DeliveryMode.Reliable
        };
        PhotonNetwork.RaiseEvent(Multi_GameManager.INIT_GAME_ON_ALL_CLIENTS, (object)new object[3]
        {
      (object) str1,
      (object) str2,
      (object) str3
        }, raiseEventOptions, sendOptions);
    }

    private void InitGame(
      List<CardModel> serverCardsDeck,
      Dictionary<string, List<CardModel>> playerIDtoCardsDictionary,
      CardModel firstCard)
    {
        this.multi_CardDealer.cardsDeck = serverCardsDeck;
        for (int index1 = 0; index1 < PhotonNetwork.CurrentRoom.Players.Count; ++index1)
        {
            int index2 = index1 == 0 ? index1 + 1 : index1;
            if (PhotonNetwork.CurrentRoom.Players[index1 + 1].UserId.Equals(PhotonNetwork.LocalPlayer.UserId))
                index2 = 0;
            PlayerController playerController = UnityEngine.Object.Instantiate<PlayerController>(this.playerPrefab, this.playerSpawnPoints[index2]);
            playerController.Init(PhotonNetwork.CurrentRoom.Players[index1 + 1].UserId, PhotonNetwork.CurrentRoom.Players[index1 + 1].NickName);
            List<CardModel> cards = (List<CardModel>)null;
            playerIDtoCardsDictionary.TryGetValue(PhotonNetwork.CurrentRoom.Players[index1 + 1].UserId, out cards);
            playerController.SetInitialCards(cards);
            this.spawnedPlayerControllerList.Add(playerController);
        }
        CardController cardController = this.SpawnCardFromCardModel(firstCard);
        cardController.transform.SetParent(this.centerTableTrnasform);
        cardController.transform.localPosition = new Vector3();
        this.lastCardController = cardController;
    }

    public bool IsItMyTurn(string playerID)
    {
        return playerID.Equals(PhotonNetwork.LocalPlayer.UserId) && playerID.Equals(this.currentTurnPlayerID);
    }

    public void CardPlayed(CardController cardPlayed)
    {
        int turnMoveCount = 1;
        if (cardPlayed.cardModel.IsSpecial)
        {
            if (cardPlayed.cardModel.SpecialCardType == SpecialCard.Skip)
                turnMoveCount++;
            if (cardPlayed.cardModel.SpecialCardType == SpecialCard.Reverse)
                this.turnMoveDirection *= -1;
        }
        if (cardPlayed.cardModel.IsWild && (cardPlayed.cardModel.WildCardType == WildCard.Wild || cardPlayed.cardModel.WildCardType == WildCard.DrawFour))
            UIManager.Instance.DisplayColorSelectPopUp((Action<CardColor>)(cardColor => this.FinishPlayingCard(turnMoveCount, cardPlayed, cardColor)));
        else
            this.FinishPlayingCard(turnMoveCount, cardPlayed, CardColor.None);
    }

    private void FinishPlayingCard(int turnMoveCount, CardController cardPlayed, CardColor cardColor = CardColor.None)
    {
        int cardId = cardPlayed.cardModel.cardID;
        string playerId = cardPlayed.playerController.playerModel.playerID;
        CardColor cardColor1 = cardColor;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.All
        };
        SendOptions sendOptions = new SendOptions()
        {
            DeliveryMode = DeliveryMode.Reliable
        };
        PhotonNetwork.RaiseEvent(Multi_GameManager.CARD_PLAYED, (object)new object[3]
        {
      (object) playerId,
      (object) cardId,
      (object) cardColor1
        }, raiseEventOptions, sendOptions);
        this.multi_TurnManager.OnTurnComplete(turnMoveCount * this.turnMoveDirection);
    }

    private void OnPlayerPlayedCard(string playerID, int cardID, CardColor wildCardColorChangedTo)
    {
        PlayerController playerController = this.spawnedPlayerControllerList.Find((Predicate<PlayerController>)(x => x.playerModel.playerID.Equals(playerID)));
        CardController cardController = playerController.playerModel.CardList.Find((Predicate<CardController>)(x => x.cardModel.cardID == cardID));
        if (wildCardColorChangedTo != CardColor.None)
        {
            cardController.cardModel.CardColor = wildCardColorChangedTo;
            UIManager.Instance.ShowColorChangeDialog(playerController.playerModel.playerName, wildCardColorChangedTo);
        }
        playerController.playerModel.CardList.Remove(cardController);
        if ((UnityEngine.Object)this.lastCardController != (UnityEngine.Object)null)
        {
            this.multi_CardDealer.cardsDeck.Add(this.lastCardController.cardModel);
            UnityEngine.Object.Destroy((UnityEngine.Object)this.lastCardController.gameObject);
        }
        this.lastCardController = cardController;
        LeanTween.move(cardController.gameObject, this.centerTableTrnasform.position, 0.5f).setEaseOutCubic();
        cardController.gameObject.transform.SetParent(this.centerTableTrnasform);
    }

    private IEnumerator CheckIfPlayerCanPlayACardRoutine(string playerID)
    {
        yield return (object)new WaitForSecondsRealtime(2f);
        PlayerController playerController = this.spawnedPlayerControllerList.Find((Predicate<PlayerController>)(x => x.playerModel.playerID.Equals(playerID)));
        for (int index = 0; index < playerController.playerModel.CardList.Count; ++index)
        {
            if (GameRules.CanUseCard(playerController.playerModel.CardList[index]))
                yield break;
        }
        Debug.Log((object)"CheckIfPlayerCanPlayACardRoutine: PickCardFromSideDeck");
        this.PickCardFromSideDeck(playerController.playerModel.playerID);
    }

    private void PickCardFromSideDeck(string playerID)
    {
        CardModel cardModel = this.multi_CardDealer.cardsDeck[0];
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.All
        };
        SendOptions sendOptions = new SendOptions()
        {
            DeliveryMode = DeliveryMode.Reliable
        };
        PhotonNetwork.RaiseEvent(Multi_GameManager.CARD_PICK_FROM_SIDE_DECK, (object)new object[2]
        {
      (object) playerID,
      (object) cardModel.cardID
        }, raiseEventOptions, sendOptions);
        this.multi_TurnManager.OnTurnComplete(this.turnMoveDirection);
    }

    private void OnCardPickedFromSideDeck(string playerID, int cardID)
    {
        List<CardModel> cardsDeck = this.multi_CardDealer.cardsDeck;
        CardModel cardModel = cardsDeck.Find((Predicate<CardModel>)(x => x.cardID == cardID));
        int index = 0;
        CardController pickedNewCard = this.SpawnCardFromCardModel(cardModel);
        pickedNewCard.transform.SetParent(this.sideDeckTransform);
        pickedNewCard.transform.localPosition = new Vector3();
        cardsDeck.RemoveAt(index);
        PlayerController playerController = this.spawnedPlayerControllerList.Find((Predicate<PlayerController>)(x => x.playerModel.playerID.Equals(playerID)));
        if ((UnityEngine.Object)playerController == (UnityEngine.Object)null)
            return;
        playerController.AssignNewCard(pickedNewCard);
    }

    private void PickCards(int count, out List<CardModel> pickedCards)
    {
        List<CardModel> cardsDeck = this.multi_CardDealer.cardsDeck;
        pickedCards = new List<CardModel>();
        for (int index = 0; index < count; ++index)
        {
            pickedCards.Add(cardsDeck[0]);
            cardsDeck.RemoveAt(0);
        }
    }

    public CardController SpawnCardFromCardModel(CardModel cardModel)
    {
        CardController cardController = UnityEngine.Object.Instantiate<CardController>(this.cardPrefab, this.transform);
        cardController.cardModel = cardModel;
        cardController.gameObject.AddComponent<SpriteRenderer>().sprite = this.multi_CardDealer.GetCardImageFromCardName(cardModel.cardName);
        return cardController;
    }

    public void OnEvent(EventData photonEvent)
    {
        if ((int)photonEvent.Code == (int)Multi_GameManager.INIT_GAME_ON_ALL_CLIENTS)
        {
            object[] customData = (object[])photonEvent.CustomData;
            string str1 = (string)customData[0];
            string str2 = (string)customData[1];
            string str3 = (string)customData[2];
            Debug.Log((object)"OnEvent: INIT_GAME_ON_ALL_CLIENTS");
            Debug.Log((object)("OnEvent: cardJson: " + (object)str1.Length));
            Debug.Log((object)("OnEvent: dictionaryJson: " + (object)str2.Length));
            List<CardModel> serverCardsDeck = JsonConvert.DeserializeObject<List<CardModel>>(str1);
            Dictionary<string, List<CardModel>> playerIDtoCardsDictionary = JsonConvert.DeserializeObject<Dictionary<string, List<CardModel>>>(str2);
            Debug.Log((object)("OnEvent: serverCardsList: " + (object)serverCardsDeck.Count));
            Debug.Log((object)("OnEvent: playerIDToCardListDictionary: " + (object)playerIDtoCardsDictionary.Count));
            CardModel firstCard = JsonConvert.DeserializeObject<CardModel>(str3);
            this.InitGame(serverCardsDeck, playerIDtoCardsDictionary, firstCard);
        }
        else if ((int)photonEvent.Code == (int)Multi_GameManager.CARD_PLAYED)
        {
            object[] customData = (object[])photonEvent.CustomData;
            this.OnPlayerPlayedCard((string)customData[0], (int)customData[1], (CardColor)customData[2]);
        }
        else
        {
            if ((int)photonEvent.Code != (int)Multi_GameManager.CARD_PICK_FROM_SIDE_DECK)
                return;
            object[] customData = (object[])photonEvent.CustomData;
            this.OnCardPickedFromSideDeck((string)customData[0], (int)customData[1]);
        }
    }

    public static void Shuffle<T>(IList<T> list)
    {
        System.Random random = new System.Random();
        int count = list.Count;
        while (count > 1)
        {
            --count;
            int index = random.Next(count + 1);
            T obj = list[index];
            list[index] = list[count];
            list[count] = obj;
        }
    }
}
