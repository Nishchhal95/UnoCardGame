using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager
{
    public delegate void OnTurnOver(int byHowMuch);
    public static OnTurnOver onTurnOver;

    public delegate void OnCardPlayed(CardController cardPlayed);
    public static OnCardPlayed onCardPlayed;
}
