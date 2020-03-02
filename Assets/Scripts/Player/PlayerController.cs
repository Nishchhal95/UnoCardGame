using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerModel playerModel;
    private PlayerView playerView;

    private void Awake()
    {
        playerModel = GetComponent<PlayerModel>();
        playerView = GetComponent<PlayerView>();
    }

    public void SetInitialCards(List<CardModel> cards)
    {
        playerModel.CardList = cards;
        playerView.DisplayCards(cards);
    }
}
