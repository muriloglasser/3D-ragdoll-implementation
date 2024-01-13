using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Control player color store.
/// </summary>
public class InteractableColorShopButton : InteractableButton
{    public override void Start()
    {
        onFillOpened = () =>
        {
            gameManager.OpenColorShopUI();
        };
    }
}
