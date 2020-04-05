using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardController : MonoBehaviour
{
    public CardModel cardModel;
    public bool canMove = true;
    public PlayerController playerController;

    private void OnMouseDown()
    {
        Debug.Log(cardModel.CardColor.ToString() + " " + cardModel.CardNumber + " Card Selected.");
    }

    private void OnMouseDrag()
    {
        if(!canMove)
        {
            return;
        }

        if(!Multi_GameManager.Instance.IsItMyTurn(playerController.playerModel.playerID))
        {
            Debug.Log("Not My Turn...");
            return;
        }

        if(!GameRules.CanUseCard(this))
        {
            Debug.Log("Cannot Play Card " + cardModel.CardColor.ToString() + " " + cardModel.CardNumber);
            return;
        }
        Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);

        Collider2D[] objectsHit = Physics2D.OverlapCircleAll(transform.position, 2f);

        if(objectsHit == null || objectsHit.Length <= 0)
        {
            return;
        }

        for (int i = 0; i < objectsHit.Length; i++)
        {
            if(objectsHit[i].gameObject.name.Equals("CenterDeck"))
            {
                canMove = false;
                Multi_GameManager.Instance.CardPlayed(this);
                //playerController.playerModel.CardList.Remove(this);
                //LeanTween.move(gameObject, objectsHit[i].gameObject.transform.position, .5f).setEaseOutCubic().setOnComplete(() => 
                //{
                //    GameEventManager.onCardPlayed?.Invoke(this);
                //});

                break;
            }
        }
    }
}
