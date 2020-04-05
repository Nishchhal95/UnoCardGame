using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine;
using ExitGames.Client.Photon;
using Newtonsoft.Json;

//Game Manager should have all cards Info
//Card Dealer should have an original deck and duplicate deck too for picking cards
public class _GameManager : Singleton<_GameManager>, IOnEventCallback
{
    //GAME CONFIGURATION
    [Header("Game Configuration")]
    public int playerCount;
    [SerializeField] private int initialCardsCount = 7;
    [SerializeField] private int turnMoveDirection = 1;
    [SerializeField] private CardColor currentColor;

    //Manager Refrences
    [Header("Manager Refrences")]
    [SerializeField] private CardDealer cardDealer;
    [SerializeField] private TurnManager turnManager;

    //Spawning Item Refrences
    [Header("Spawning Item Refrences")]
    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private CardController cardPrefab;

    //Game Refrences
    [Header("Game Refrences")]
    [SerializeField] private List<PlayerController> players = new List<PlayerController>();
    [SerializeField] private List<Transform> playerSpawnPoints;
    [SerializeField] private Transform centerDeckTransform;
    [SerializeField] private Transform sideDeckTransform;

    public CardController lastCardController = null;

    [SerializeField] private List<Player> connectedPlayers = new List<Player>();

    private void Start()
    {
        if (NetworkManager.isGameCreated)
        {
            UIManager.Instance.DisplayGameCreatedPage(PhotonNetwork.CurrentRoom.Name);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            OnTurnComplete(1);
        }
    }

    private void OnEnable()
    {
        GameEventManager.onTurnOver += OnTurnComplete;
        GameEventManager.onCardPlayed += OnCardPlayed;
        NetworkManager.onPhotonPlayerEnteredRoom += OnPlayerJoined;
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        GameEventManager.onTurnOver -= OnTurnComplete;
        GameEventManager.onCardPlayed -= OnCardPlayed;
        NetworkManager.onPhotonPlayerEnteredRoom -= OnPlayerJoined;
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void OnPlayerJoined(Player photonPlayer)
    {
        Debug.Log("[GameManager] : OnPlayerJoined");

        //Adding players to a list
        connectedPlayers.Add(photonPlayer);


        //Server Calls this when Server sees a new player has joined and we have recieved enough players or not..
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
        {
            return;
        }
        PhotonNetwork.RaiseEvent(12, null, new RaiseEventOptions { Receivers = ReceiverGroup.All }, new SendOptions() { DeliveryMode = DeliveryMode.Reliable });
    }

    private void StartGame()
    {
        Debug.Log("Start Game: " + PhotonNetwork.CurrentRoom.PlayerCount);
        //Initialize Cards
        cardDealer.Init();
        Debug.Log("Cards are Ready...");

        connectedPlayers = new List<Player>();
        foreach (var playerItem in PhotonNetwork.CurrentRoom.Players.Values)
        {
            connectedPlayers.Add(playerItem);
        }

        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        //Spawn Players
        SpawnPlayers(playerCount);

        //Assign them Cards
        if (PhotonNetwork.IsMasterClient)
        {
            int seed = UnityEngine.Random.Range(1, 999);
            Shuffle(cardDealer.cardsDeck);

            string jsonCardList = JsonConvert.SerializeObject(cardDealer.cardsDeck);
            //string jsonCardList = JsonUtility.ToJson(cardDealer.cardsDeck);
            
            //NEED TO UPDATE SHUFFLED DECK FOR everyone
            PhotonNetwork.RaiseEvent(13, new object[] { jsonCardList }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, new SendOptions() {DeliveryMode = DeliveryMode.Reliable });
        }
    }


    private void StartGAMEAFTERSHUFFLE(string cardJson)
    {
        Debug.Log("StartGAMEAFTERSHUFFLE: " + cardJson.Length);
        List<CardModel> shuffledCardList = JsonConvert.DeserializeObject<List<CardModel>>(cardJson);
        //List<CardModel> shuffledCardList = JsonUtility.FromJson<List<CardModel>>(cardJson);
        Debug.Log("StartGAMEAFTERSHUFFLE: " + shuffledCardList.Count);

        cardDealer.cardsDeck = shuffledCardList;

        AssignCardsToPlayers(initialCardsCount);

        //Play First Card
        PlayFirstCard();

        //Setup TurnManager
        turnManager.gameObject.SetActive(true);
    }

    //Turn Complete
    private void OnTurnComplete(int byHowMuch)
    {
        turnManager.TurnOver(byHowMuch);
    }

    public void OnCardPlayed(CardController cardPlayed)
    {
        int turnMoveCount = 1;
        cardPlayed.gameObject.transform.SetParent(centerDeckTransform);
        if (lastCardController != null)
        {
            cardDealer.cardsDeck.Add(lastCardController.cardModel);
            Destroy(lastCardController.gameObject);
        }
        lastCardController = cardPlayed;

        //If a Special Card...
        if(cardPlayed.cardModel.IsSpecial)
        {
            if(cardPlayed.cardModel.SpecialCardType == SpecialCard.Skip)
            {
                turnMoveCount++;
            }

            if(cardPlayed.cardModel.SpecialCardType == SpecialCard.Reverse)
            {
                turnMoveDirection *= -1;
            }
        }

        //If a Wild Card...
        if(cardPlayed.cardModel.IsWild)
        {
            //TODO: TEMP ADDING DRAW 4 HERE, PLEASE REMOVE!!!!
            if(cardPlayed.cardModel.WildCardType == WildCard.Wild || cardPlayed.cardModel.WildCardType == WildCard.DrawFour)
            {
                //Display Color Select Card Pop Up
                UIManager.Instance.DisplayColorSelectPopUp((CardColor cardColor) => 
                {
                    FinishPlayingCard(turnMoveCount, cardPlayed, cardColor);
                });
                return;
            }
        }

        FinishPlayingCard(turnMoveCount, cardPlayed);
    }

    public void FinishPlayingCard(int turnMoveCount, CardController cardPlayed, CardColor cardColor = CardColor.None)
    {
        if(cardColor != CardColor.None)
        {
            cardPlayed.cardModel.CardColor = cardColor;
        }

        OnTurnComplete(turnMoveCount * turnMoveDirection);
        currentColor = cardPlayed.cardModel.CardColor;
    }


    public bool isItMyTurn(string playerID)
    {
        if(playerID.Equals(connectedPlayers[turnManager.currentTurn % playerCount].UserId))
        {
            return true;
        }
        return false;
        //if(PhotonNetwork.PlayerList[playerID].NickName.Equals(PhotonNetwork.LocalPlayer.NickName))
        //{
        //    return true;
        //}

        //else
        //{
        //    return false;
        //}
        //PlayerController currentTurnPlayer = players[turnManager.currentTurn];
        //if(currentTurnPlayer.playerModel.playerID == playerID)
        //{
        //    return true;
        //}
        //return false;
    }

    public void TurnOverComplete()
    {
        StartCoroutine(CheckIfNextPlayerCanPlayACardRoutine());
    }

    private IEnumerator CheckIfNextPlayerCanPlayACardRoutine()
    {
        yield return new WaitForSecondsRealtime(2f);

        //Check 
        PlayerController player = players[turnManager.currentTurn];

        for (int i = 0; i < player.playerModel.CardList.Count; i++)
        {
            if(GameRules.CanUseCard(player.playerModel.CardList[i]))
            {
                yield break;
            }
        }

        PickCardFromSideDeck(player.playerModel.playerID);
        OnTurnComplete(1 * turnMoveDirection);
    }

    private void PickCardFromSideDeck(string playerID)
    {
        List<CardModel> cardList = cardDealer.cardsDeck;

        //Shuffle(cardList);

        int indexToRemove = 0;
        CardController newCardFromSideDeck = SpawnCardFromCardModel(cardList[0]);

        newCardFromSideDeck.transform.SetParent(sideDeckTransform);
        newCardFromSideDeck.transform.localPosition = new Vector3();

        currentColor = lastCardController.cardModel.CardColor;
        cardList.RemoveAt(indexToRemove);

        //Assign Card to correct Player
        PlayerController player = players.Find(x => x.playerModel.playerID.Equals(playerID));

        if(player == null)
        {
            return;
        }

        player.AssignNewCard(newCardFromSideDeck);
    }



    private void SpawnPlayers(int playerCount)
    {
        for (int i = 0; i < playerCount; i++)
        {
            PlayerController playerController = Instantiate(playerPrefab, playerSpawnPoints[i]);
            playerController.Init(connectedPlayers[i].UserId, connectedPlayers[i].NickName);
            players.Add(playerController);
        }
    }

    private void AssignCardsToPlayers(int numberOfCardsToAssign)
    {
        for (int i = 0; i < players.Count; i++)
        {
            List<CardController> cardControllers = new List<CardController>();

            //Out Picked Cards...
            //PickCards(initialCardsCount, out cardControllers);
            PickCards(initialCardsCount, out cardControllers);

            //Assign These Cards to Players
            players[i].SetInitialCards(cardControllers);
        }
    }

    private void PlayFirstCard()
    {
        List<CardModel> cardList = cardDealer.cardsDeck;

        //Shuffle(cardList);

        int indexToRemove = 0;
        for (int i = 0; i < cardList.Count; i++)
        {
            if(!cardList[i].IsSpecial && !cardList[i].IsWild)
            {
                indexToRemove = i;

                lastCardController = SpawnCardFromCardModel(cardList[i]);

                lastCardController.transform.SetParent(centerDeckTransform);
                lastCardController.transform.localPosition = new Vector3();
                break;
            }
        }

        currentColor = lastCardController.cardModel.CardColor;
        cardList.RemoveAt(indexToRemove);
    }

    private void PickCards(int count, out List<CardController> pickedCards)
    {
        //Using Card Deck From Card Dealer
        List<CardModel> cardList = cardDealer.cardsDeck;
        pickedCards = new List<CardController>();

        //Shuffle(cardList);

        for (int i = 0; i < count; i++)
        {
            //Spawn New Card
            CardController cardController = SpawnCardFromCardModel(cardList[0]);
            pickedCards.Add(cardController);
            cardList.RemoveAt(0);
        }
    }

    private void PickCards(int count, out List<int> pickedCardsIndexes)
    {
        List<CardModel> cardList = cardDealer.cardsDeck;
        pickedCardsIndexes = new List<int>();

        //Shuffle(cardList);

        for (int i = 0; i < count; i++)
        {
            pickedCardsIndexes.Add(i);
            //cardList.RemoveAt(0);
        }
    }

    private CardController SpawnCardFromCardModel(CardModel cardModel)
    {
        CardController cardController = Instantiate(cardPrefab, transform);
        cardController.cardModel = cardModel;
        cardController.gameObject.AddComponent<SpriteRenderer>().sprite = cardDealer.GetCardImageFromCardName(cardModel.cardName);
        return cardController;
    }

    public static void Shuffle<T>(IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        Debug.Log("ON EVENT: " + photonEvent.Code);
        if (photonEvent.Code == 12)
        {
            StartGame();
        }

        else if (photonEvent.Code == 13)
        {
            object[] datas = (object[])photonEvent.CustomData;
            string jsonData = (string)datas[0];

            StartGAMEAFTERSHUFFLE(jsonData);
        }
    }
}

public enum GAME_EVENTS
{
    ShuffleCards
}