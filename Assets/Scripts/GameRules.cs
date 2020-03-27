using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameRules
{
    public static bool CanUseCard(CardController cardToPlayController)
    {
        CardModel lastCard = _GameManager.Instance.lastCardController.cardModel;
        CardModel cardToMove = cardToPlayController.cardModel;

        //If Last Card is a Number Card
        if (!lastCard.IsSpecial && !lastCard.IsWild)
        {
            //If Color of Last Card and CardToMove matches we can play that card
            //This covers Number Card + Special Card Case...
            if (lastCard.CardColor == cardToMove.CardColor)
            {
                return true;
            }

            //If Number of Last Card and CardToMove matches we can play that card
            if (lastCard.CardNumber == cardToMove.CardNumber)
            {
                return true;
            }

            //We can always throw a Wild Card
            if (cardToMove.IsWild)
            {
                return true;
            }
        }


        //If Last Card is Special Card
        if(lastCard.IsSpecial && !lastCard.IsWild)
        {
            switch (lastCard.SpecialCardType)
            {
                case SpecialCard.None:
                    break;
                case SpecialCard.Skip:
                    //We can play any Wild Card
                    if(cardToMove.IsWild)
                    {
                        return true;
                    }

                    //Any Color Skip we can play
                    if(cardToMove.IsSpecial && cardToMove.SpecialCardType == SpecialCard.Skip)
                    {
                        return true;
                    }

                    //Same Color Number or Special Cards allowed
                    if(cardToMove.CardColor == lastCard.CardColor)
                    {
                        return true;
                    }
                    break;
                case SpecialCard.Reverse:
                    //We can play any Wild Card
                    if (cardToMove.IsWild)
                    {
                        return true;
                    }

                    //Any Color Reverse we can play
                    if (cardToMove.IsSpecial && cardToMove.SpecialCardType == SpecialCard.Reverse)
                    {
                        return true;
                    }

                    //Same Color Number or Special Cards allowed
                    if (cardToMove.CardColor == lastCard.CardColor)
                    {
                        return true;
                    }
                    break;
                case SpecialCard.DrawTwo:
                    //TODO : Picking Phase vs Playing Phase
                    //TODO : ADDING TEMP DRAW 2 Logic, PLEASE REMOVE IT!!!
                    //We can play any Wild Card
                    if (cardToMove.IsWild)
                    {
                        return true;
                    }

                    //Any Color Reverse we can play
                    if (cardToMove.IsSpecial && cardToMove.SpecialCardType == SpecialCard.DrawTwo)
                    {
                        return true;
                    }

                    //Same Color Number or Special Cards allowed
                    if (cardToMove.CardColor == lastCard.CardColor)
                    {
                        return true;
                    }
                    break;
                    break;
                default:
                    break;
            }
        }


        //If Last Card is a Wild Card
        if(lastCard.IsWild)
        {
            switch (lastCard.WildCardType)
            {
                case WildCard.None:
                    break;
                case WildCard.Wild:
                    //We can play any Wild Card
                    if(cardToMove.IsWild)
                    {
                        return true;
                    }

                    //We can play any Same Color Card (As We update Wild Card Color while playing)
                    if(cardToMove.CardColor == lastCard.CardColor)
                    {
                        return true;
                    }
                    break;
                case WildCard.DrawFour:
                    //TODO : Picking Phase vs Playing Phase..
                    //TODO : ADDING TEM WILD LOGIC, PLEASE REMOVE!!!!
                    //We can play any Wild Card
                    if (cardToMove.IsWild)
                    {
                        return true;
                    }

                    //We can play any Same Color Card (As We update Wild Card Color while playing)
                    if (cardToMove.CardColor == lastCard.CardColor)
                    {
                        return true;
                    }
                    break;
                    break;
                default:
                    break;
            }
        }

        return false;
    }
}
