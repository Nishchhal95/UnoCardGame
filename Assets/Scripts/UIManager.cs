using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TurnManager turnManager;
    [SerializeField] private GameObject colorSelectPanel;
    private Action<CardColor> onCompleteColorSelectAction;

    [SerializeField] private GameObject colorChangeDialogGO;
    [SerializeField] private TextMeshProUGUI colorChangeDialogText;

    [SerializeField] private GameObject gameCreatedUI;
    [SerializeField] private GameObject gameCreatedUITweenBase;
    [SerializeField] private TextMeshProUGUI roomNameText;
    private string ROOM_NAME_FORMAT = "Your Room Code is : {0}";

    [SerializeField] private Image turnTextFillImage;
    [SerializeField] private TextMeshProUGUI turnText;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if(turnManager == null)
        {
            return;
        }
        turnText.SetText("Turn: " + turnManager.Turn);
        turnTextFillImage.fillAmount = (1 / turnManager.RemainingSecondsInTurn) * turnManager.TurnDuration;
    }

    public void DisplayColorSelectPopUp(Action<CardColor> onCompleteAction)
    {
        colorSelectPanel.SetActive(true);
        LeanTween.scale(colorSelectPanel.transform.GetChild(0).gameObject, new Vector3(1, 1, 1), .5f).setEaseOutBounce();
        onCompleteColorSelectAction = onCompleteAction;
    }

    //On Color Select Button Click
    public void OnColorSelectButtonClick(string cardColor)
    {
        Enum.TryParse(cardColor, out CardColor cardColorToEnum);
        onCompleteColorSelectAction?.Invoke(cardColorToEnum);
        LeanTween.scale(colorSelectPanel.transform.GetChild(0).gameObject, new Vector3(0, 0, 0), .5f).setEaseInCubic().setOnComplete(() => { colorSelectPanel.SetActive(false); });

    }

    public void OnShareClick()
    {
        //TODO : ADD SHARING OPTION

    }

    public void DisplayGameCreatedPage(string roomName)
    {
        roomNameText.SetText(string.Format(ROOM_NAME_FORMAT, roomName));
        gameCreatedUITweenBase.transform.localScale = new Vector3(0, 0, 0);
        gameCreatedUI.SetActive(true);
        LeanTween.scale(gameCreatedUITweenBase, new Vector3(1, 1, 1), .25f).setEaseInOutCubic();
    }

    public void OnDisplayGameCreatePageCloseClick()
    {
        LeanTween.scale(gameCreatedUITweenBase, new Vector3(0, 0, 0), .25f).setEaseInOutCubic().setOnComplete(() => { gameCreatedUI.SetActive(false); });
    }

    public void ShowColorChangeDialog(string playerName, CardColor cardColor)
    {
        colorChangeDialogText.SetText(playerName + " changed color to " + cardColor.ToString());
        colorChangeDialogGO.transform.localScale = new Vector3();
        colorChangeDialogGO.SetActive(true);
        LeanTween.scale(colorChangeDialogGO, new Vector3(1, 1, 1), .25f).setEaseInOutCubic().setOnComplete(() => 
        {
            LeanTween.scale(colorChangeDialogGO, new Vector3(1, 1, 1), 1f).setEaseInOutCubic().setOnComplete(() =>
            {
                LeanTween.scale(colorChangeDialogGO, new Vector3(0, 0, 0), .25f).setEaseInOutCubic().setOnComplete(() =>
                {
                    colorChangeDialogGO.SetActive(false);
                });
            });
        });
    }
}
