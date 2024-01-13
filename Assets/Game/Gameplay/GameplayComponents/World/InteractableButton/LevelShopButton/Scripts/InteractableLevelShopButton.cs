using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Control player level store.
/// </summary>
public class InteractableLevelShopButton : InteractableButton
{
    public override void Start()
    {
        onFillOpened = () =>
        {
            gameManager.OpenLevelUpShopUI();
        };
    }
}
