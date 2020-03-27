using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public PlayerController playerController;
    public Transform cardHolder;
    public TextMeshProUGUI playerNameText;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void UpdatePlayerName(string playerName)
    {
        playerNameText.text = playerName;
    }

    public void DisplayCards(List<CardController> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.SetParent(cardHolder);
            cards[i].transform.localPosition = new Vector3(0 + (i * cards[i].GetComponentInChildren<SpriteRenderer>().bounds.extents.x * 2), 0, 0);
            cards[i].playerController = playerController;
        }
    }
}
