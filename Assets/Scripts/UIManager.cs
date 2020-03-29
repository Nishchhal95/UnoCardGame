using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject colorSelectPanel;
    private Action<CardColor> onCompleteColorSelectAction;

    [SerializeField] private GameObject gameCreatedUI;
    [SerializeField] private GameObject gameCreatedUITweenBase;
    [SerializeField] private TextMeshProUGUI roomNameText;
    private string ROOM_NAME_FORMAT = "Your Room Code is : {0}";

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
}
