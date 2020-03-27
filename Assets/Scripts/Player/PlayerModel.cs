using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This should have a Player Controller and a Player View
public class PlayerModel
{
    public Transform cardHolder;
    public GameObject cardPrefab;

    public string playerName;
    public int playerID;

    public List<CardController> CardList
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
    [SerializeField] private List<CardController> cardList = new List<CardController>();
}
