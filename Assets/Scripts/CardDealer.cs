using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDealer : MonoBehaviour
{
    public Card[] cards;

    public Card[] cardsDeck;

    private void Awake()
    {
        if(cards == null || cards.Length <= 0)
        {
            Debug.Log("No Cards...");
            return;
        }

        InitializeCards();
    }

    private void InitializeCards()
    {
        cardsDeck = new Card[cards.Length * 2];
        int deckIndex = 0;
        for (int i = 0; i < cards.Length; i++)
        {
            cardsDeck[deckIndex] = cards[i];
            deckIndex++;
        }

        for (int i = 0; i < cards.Length; i++)
        {
            cardsDeck[deckIndex] = cards[i];
            deckIndex++;
        }
    }
}
