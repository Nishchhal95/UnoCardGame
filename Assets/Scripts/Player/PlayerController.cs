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

    public void Init(string playerID, string playerName)
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

    public void SetInitialCards(List<CardModel> cards)
    {
        List<CardController> cardControllerList = new List<CardController>();

        for (int i = 0; i < cards.Count; i++)
        {
            CardController cardController = Multi_GameManager.Instance.SpawnCardFromCardModel(cards[i]);
            cardControllerList.Add(cardController);
        }

        playerModel.CardList = cardControllerList;
        playerView.DisplayCards(cardControllerList);
    }

    public void AssignNewCard(CardController pickedNewCard)
    {
        playerModel.CardList.Add(pickedNewCard);
        LeanTween.move(pickedNewCard.gameObject, this.transform.position, .5f).setEaseOutBounce().setOnComplete(() => 
        {
            playerView.DisplayCards(playerModel.CardList);
        });
    }

    public void AssignNewCard(CardModel pickedNewCard)
    {
        CardController cardController = Multi_GameManager.Instance.SpawnCardFromCardModel(pickedNewCard);

        playerModel.CardList.Add(cardController);
        LeanTween.move(cardController.gameObject, this.transform.position, .5f).setEaseOutBounce().setOnComplete(() =>
        {
            playerView.DisplayCards(playerModel.CardList);
        });
    }
}
