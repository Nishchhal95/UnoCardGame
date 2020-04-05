using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUpdate : MonoBehaviour
{
    public Text turnRemainingTimeText;
    public Image turnRemainingTimeFillImage;
    public TempTurnManager tempTurnManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        turnRemainingTimeText.text = "Time Remaining: " + (int)tempTurnManager.RemainingSecondsInTurn;
        turnRemainingTimeFillImage.fillAmount = (1 / tempTurnManager.RemainingSecondsInTurn) * tempTurnManager.TurnDuration;
    }
}
