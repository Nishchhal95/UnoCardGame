using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card
{
    //Special Attributes
    public bool IsSpecial { get; set; }
    [SerializeField] private bool isSpecial = false;

    public SpecialCard SpecialCardType { get; set; }
    [SerializeField] private SpecialCard specialCardType;

    public int IsWild { get; set; }
    [SerializeField] private bool isWild = false;

    public WildCard WildCardType { get; set; }
    [SerializeField] private WildCard wildCardType;

    //Normal Card Fields
    public CardColor CardColor { get; set; }
    [SerializeField] private CardColor cardColor;

    public int CardNumber { get; set; }
    [SerializeField] private int cardNumber;

    //Image Refrence
    public Sprite cardImage; 
}

//Card Color
public enum CardColor
{
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
