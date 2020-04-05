using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardModel
{
    public string cardName = "";
    public int cardID = -1;

    //Special Attributes
    public bool IsSpecial { get { return isSpecial; } set { isSpecial = value; } }
    [SerializeField] private bool isSpecial = false;

    public SpecialCard SpecialCardType { get { return specialCardType; } set { specialCardType = value; } }
    [SerializeField] private SpecialCard specialCardType;

    public bool IsWild { get { return isWild; } set { isWild = value; } }
    [SerializeField] private bool isWild = false;

    public WildCard WildCardType { get { return wildCardType; } set { wildCardType = value; } }
    [SerializeField] private WildCard wildCardType;

    //Normal Card Fields
    public CardColor CardColor { get { return cardColor; } set { cardColor = value; } }
    [SerializeField] private CardColor cardColor;

    public int CardNumber { get { return cardNumber; } set { cardNumber = value; } }
    [SerializeField] private int cardNumber;

    //Image Refrence
    //public Sprite cardImage;

    //public SpriteRenderer cardSpriteRender;
}

//Card Color
public enum CardColor
{
    None,
    Red,
    Yellow,
    Green,
    Blue
}

public enum SpecialCard
{
    None,
    Skip,
    Reverse,
    DrawTwo
}

public enum WildCard
{
    None,
    Wild,
    DrawFour
}
