using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This should have a Player Controller and a Player View
public class PlayerModel
{
    public Transform cardHolder;
    public GameObject cardPrefab;

    public List<CardModel> CardList
    {
        get
        {
            return cardList;
        }

        set
        {
            cardList = value;
        }
    }
    [SerializeField] private List<CardModel> cardList = new List<CardModel>();
}
