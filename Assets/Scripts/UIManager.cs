using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject colorSelectPanel;
    private Action<CardColor> onCompleteColorSelectAction;

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
}
