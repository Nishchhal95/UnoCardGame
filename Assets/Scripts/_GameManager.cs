using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Game Manager should have all cards Info
//Card Dealer should have an original deck and duplicate deck too for picking cards
public class _GameManager : MonoBehaviour
{
    public int playerCount;
    public int initialCardsCount = 7;

    public CardDealer cardDealer;

    public GameObject playerPrefab;

    public CardModel[] cardsDuplicateArray;

    public List<CardModel> cardList = new List<CardModel>();

    public Sprite[] cardImages;

    private void Start()
    {
        //cardList = cardDealer.cardsDeck.ToList();

        //cardsDuplicateArray = new CardModel[cardDealer.cardsDeck.Length];
        //for (int i = 0; i < cardDealer.cardsDeck.Length; i++)
        //{
        //    CardModel card = new CardModel()
        //    {
        //        IsSpecial = cardDealer.cardsDeck[i].IsSpecial,
        //        SpecialCardType = cardDealer.cardsDeck[i].SpecialCardType,
        //        IsWild = cardDealer.cardsDeck[i].IsWild,
        //        WildCardType = cardDealer.cardsDeck[i].WildCardType,
        //        CardColor = cardDealer.cardsDeck[i].CardColor,
        //        CardNumber = cardDealer.cardsDeck[i].CardNumber,
        //        cardImage = cardDealer.cardsDeck[i].cardImage
        //    };

        //    cardsDuplicateArray[i] = card;
        //}

        //CreatePlayers();
    }

    private void CreatePlayers()
    {
        for (int i = 0; i < playerCount; i++)
        {
            GameObject playerGameObject = Instantiate(playerPrefab, transform);
            PlayerController playerController = playerGameObject.GetComponent<PlayerController>();

            List<CardModel> cards = PickCards(initialCardsCount);
            playerController.SetInitialCards(cards);
        }
    }

    private List<CardModel> PickCards(int count)
    {
        List<CardModel> pickedCards = new List<CardModel>();

        Shuffle(cardList);

        for (int i = 0; i < count; i++)
        {
            pickedCards.Add(cardList[0]);
            cardList.RemoveAt(0);
        }

        return pickedCards;
    }

    private static System.Random rng = new System.Random();
    public static void Shuffle<T>(IList<T> list)
    {
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
}
