using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    private PlayerModel playerModel;
    private void Awake()
    {
        playerModel = GetComponent<PlayerModel>();
    }

    public void DisplayCards(List<CardModel> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject card = Instantiate(playerModel.cardPrefab, playerModel.cardHolder);
            card.AddComponent<SpriteRenderer>().sprite = cards[i].cardImage;
        }
    }
}
