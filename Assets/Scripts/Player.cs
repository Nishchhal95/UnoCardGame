using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This should have a Player Controller and a Player View
public class Player : MonoBehaviour
{
    public List<Card> CardList
    {
        get
        {
            return cardList;
        }

        set
        {
            cardList = value;

            SetUpCards();
        }
    }
    [SerializeField] private List<Card> cardList = new List<Card>();

    private void SetUpCards()
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            GameObject card = new GameObject("Card: " + i);
            card.transform.SetParent(this.transform);
            card.AddComponent<SpriteRenderer>().sprite = cardList[i].cardImage;
        }
    }
}
