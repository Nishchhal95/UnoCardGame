using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multi_CardDealer : MonoBehaviour
{
    //Default Cards or Original Cards
    public CardModel[] cards = new CardModel[60];

    //Card Deck To use
    public List<CardModel> cardsDeck;

    public Sprite[] cardSprites;

    public Dictionary<string, Sprite> cardNameToImageDictionary = new Dictionary<string, Sprite>();

    public void Init()
    {
        //Picking up card Images from a SpriteSheet
        GetCardImages();

        //Setting Up Cards from Images
        SetUpBaseCards();

        //Initializing the Card Deck
        InitializeCardDeck();

        //TODO : Add More Game Logic
    }














    private void GetCardImages()
    {
        Sprite[] allCardImages = Resources.LoadAll<Sprite>("Artwork/Uno Cards Deck");

        cardSprites = new Sprite[60];
        int index = 0;
        for (int i = 0; i < allCardImages.Length; i++)
        {
            if (!allCardImages[i].name.StartsWith("Uno"))
            {
                cardSprites[index] = allCardImages[i];
                index++;
            }
        }
    }

    private void SetUpBaseCards()
    {
        cards = new CardModel[cardSprites.Length];
        for (int i = 0; i < cardSprites.Length; i++)
        {
            string cardImageName = cardSprites[i].name;
            cardNameToImageDictionary.Add(cardImageName, cardSprites[i]);

            string cardColor = cardImageName.Split('_')[0];
            string cardNumber = cardImageName.Split('_')[1];


            //Setting Base Values
            cards[i] = new CardModel();
            cards[i].cardName = cardImageName;
            //cards[i].cardImage = cardSprites[i];
            cards[i].IsSpecial = false;
            cards[i].SpecialCardType = SpecialCard.None;
            cards[i].IsWild = false;
            cards[i].WildCardType = WildCard.None;


            //Logic To Parse Cards from names and fill up the Array...
            if (cardColor == "Red" || cardColor == "Yellow" || cardColor == "Green" || cardColor == "Blue")
            {
                if (Enum.TryParse(cardColor, out CardColor _cardColor))
                {
                    cards[i].CardColor = _cardColor;

                    if (cardNumber == "Skip" || cardNumber == "Reverse" || cardNumber == "DrawTwo")
                    {
                        cards[i].CardNumber = -1;
                        cards[i].IsSpecial = true;
                        if (Enum.TryParse(cardNumber, out SpecialCard _specialCard))
                        {
                            cards[i].SpecialCardType = _specialCard;
                        }

                        else
                        {
                            if (int.TryParse(cardNumber, out int _cardNumber))
                            {
                                cards[i].CardNumber = _cardNumber;
                            }
                        }
                    }

                    else
                    {
                        if (int.TryParse(cardNumber, out int _cardNumber))
                        {
                            cards[i].CardNumber = _cardNumber;
                        }
                    }
                }
            }

            else
            {
                if (cardColor == "Wild")
                {
                    cards[i].CardNumber = -1;
                    cards[i].CardColor = CardColor.None;
                    cards[i].IsWild = true;
                    cards[i].WildCardType = WildCard.Wild;
                }

                else if (cardColor == "Draw4")
                {
                    cards[i].CardNumber = -1;
                    cards[i].CardColor = CardColor.None;
                    cards[i].IsWild = true;
                    cards[i].WildCardType = WildCard.DrawFour;
                }
            }
        }
    }

    private void InitializeCardDeck()
    {
        cardsDeck = new List<CardModel>();
        int deckIndex = 0;
        for (int i = 0; i < cards.Length; i++)
        {
            //Deep Copying to retain Original Cards Refrence...
            //TODO : Need to check if there is actually any need to make this or we can
            //       directly refrence the original cards.
            CardModel card = new CardModel()
            {
                cardID = deckIndex,
                IsSpecial = cards[i].IsSpecial,
                SpecialCardType = cards[i].SpecialCardType,
                IsWild = cards[i].IsWild,
                WildCardType = cards[i].WildCardType,
                CardColor = cards[i].CardColor,
                CardNumber = cards[i].CardNumber,
                cardName = cards[i].cardName
            };
            cardsDeck.Add(card);
            deckIndex++;
        }

        for (int i = 0; i < cards.Length; i++)
        {
            //Deep Copying to retain Original Cards Refrence...
            //TODO : Need to check if there is actually any need to make this or we can
            //       directly refrence the original cards.
            CardModel card = new CardModel()
            {
                cardID = deckIndex,
                IsSpecial = cards[i].IsSpecial,
                SpecialCardType = cards[i].SpecialCardType,
                IsWild = cards[i].IsWild,
                WildCardType = cards[i].WildCardType,
                CardColor = cards[i].CardColor,
                CardNumber = cards[i].CardNumber,
                cardName = cards[i].cardName
            };
            cardsDeck.Add(card);
            deckIndex++;
        }
    }


    public Sprite GetCardImageFromCardName(string cardName)
    {
        Sprite sprite = null;
        cardNameToImageDictionary.TryGetValue(cardName, out sprite);

        return sprite;
    }
}
