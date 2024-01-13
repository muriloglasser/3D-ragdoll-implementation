using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Control UI to display player coins.
/// </summary>
public class UICoins : MonoBehaviour
{
    [Header("Game manager")]
    public GameManager gameManager;
    [Header("Coins text")]
    public TMP_Text coinsText;
    //
    private int lastCoinsNumber;
    private int currentCoins
    {
        get { return gameManager.saveData.coins; }
    }

    #region Unity methods

    private void Start()
    {
        lastCoinsNumber = gameManager.saveData.coins;
    }

    private void Update()
    {
        // Update coin UI with new cash values.
        if (lastCoinsNumber != currentCoins)
        {
            coinsText.text = currentCoins.ToString();
            lastCoinsNumber = currentCoins;
        }
    }

    #endregion
}
