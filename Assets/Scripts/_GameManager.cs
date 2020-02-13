using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Game Manager should have all cards Info
//Card Dealer should have an original deck and duplicate deck too for picking cards
public class _GameManager : MonoBehaviour
{
    public int playerCount;
    public CardDealer cardDealer;

    public Card[] cardsDuplicateArray;

    public List<Card> cardList = new List<Card>();

    private void Start()
    {
        cardList = cardDealer.cardsDeck.ToList();

        cardsDuplicateArray = new Card[cardDealer.cardsDeck.Length];
        for (int i = 0; i < cardDealer.cardsDeck.Length; i++)
        {
            Card card = new Card()
            {
                IsSpecial = cardDealer.cardsDeck[i].IsSpecial,
                SpecialCardType = cardDealer.cardsDeck[i].SpecialCardType,
                IsWild = cardDealer.cardsDeck[i].IsWild,
                WildCardType = cardDealer.cardsDeck[i].WildCardType,
                CardColor = cardDealer.cardsDeck[i].CardColor,
                CardNumber = cardDealer.cardsDeck[i].CardNumber,
                cardImage = cardDealer.cardsDeck[i].cardImage
            };

            cardsDuplicateArray[i] = card;
        }

        CreatePlayers();
    }

    private void CreatePlayers()
    {
        for (int i = 0; i < playerCount; i++)
        {
            GameObject playerGameObject = new GameObject("Player " + (i + 1));
            Player playerComponent = playerGameObject.AddComponent<Player>();

            playerComponent.CardList = PickCards(7);
        }
    }

    private List<Card> PickCards(int count)
    {
        List<Card> pickedCards = new List<Card>();

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
