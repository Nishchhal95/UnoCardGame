using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//This should have a Player Controller and a Player View
public class PlayerModel
{
    public Transform cardHolder;
    public GameObject cardPrefab;

    public string playerName;
    public string playerID;

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
