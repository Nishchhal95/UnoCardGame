using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerModel playerModel;
    public PlayerView playerView;

    private void Awake()
    {
        playerModel = new PlayerModel();
        playerView = GetComponent<PlayerView>();
    }

    public void Init(int playerID, string playerName)
    {
        playerModel.playerID = playerID;
        playerModel.playerName = playerName;
        playerView.UpdatePlayerName(playerModel.playerName);
    }

    public void SetInitialCards(List<CardController> cards)
    {
        playerModel.CardList = cards;
        playerView.DisplayCards(cards);
    }

    public void AssignNewCard(CardController pickedNewCard)
    {
        playerModel.CardList.Add(pickedNewCard);
        LeanTween.move(pickedNewCard.gameObject, this.transform.position, .5f).setEaseOutBounce().setOnComplete(() => 
        {
            playerView.DisplayCards(playerModel.CardList);
        });
    }
}
