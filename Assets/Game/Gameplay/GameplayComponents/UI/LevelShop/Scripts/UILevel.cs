using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Control player level UI store.
/// </summary>
public class UILevel : MonoBehaviour
{
    [Header("Game manager")]
    public GameManager gameManager;
    [Header("UI")]
    public TMP_Text priceTxt;
    public TMP_Text levelTxt;
    public Button buyBtn;
    public Button quitBtn;
    [Header("Player levels")]
    public List<LevelData> levels;
    //
    private int currentLevel
    {
        get
        {
            return gameManager.saveData.playerLevel;
        }
    }
    private int currentCoins
    {
        get { return gameManager.saveData.coins; }
    }
    private ButtonInteractionData buttonData
    {
        get
        {
            return gameManager.buttonData;
        }
        set
        {
            gameManager.buttonData = value;
        }
    }


    #region Unity methods

    private void Start()
    {
        buyBtn.onClick.AddListener(() =>
        {
            Buy();
        });

        quitBtn.onClick.AddListener(() =>
        {
            Quit();
        });

    }

    private void OnEnable()
    {
        SetUpNextLevel();
    }

    #endregion

    #region Core

    /// <summary>
    /// Set up UI with new level.
    /// </summary>
    public void SetUpNextLevel()
    {
        // Reached last level.
        if (currentLevel == levels.Count + 1)
        {
            buyBtn.interactable = false;
            levelTxt.text = levels[levels.Count - 1].levelName;
            priceTxt.text = "MAX";
            return;
        }
        else if (currentCoins < levels[currentLevel - 1].price)
            buyBtn.interactable = false;
        else
            buyBtn.interactable = true;

        // Set current level ui properties.
        priceTxt.text = levels[currentLevel - 1].price.ToString();
        levelTxt.text = levels[currentLevel - 1].levelName.ToString();
    }

    /// <summary>
    /// Buy new level.
    /// </summary>
    public void Buy()
    {
        // Remove coins from player.
        gameManager.RemoveCoins(levels[currentLevel - 1].price);

        // Add level to player.
        gameManager.AddLevel();

        // Save game.
        gameManager.Save();

        // Reached last level.
        if (currentLevel == levels.Count + 1)
        {
            buyBtn.interactable = false;
            levelTxt.text = levels[levels.Count - 1].levelName;
            priceTxt.text = "MAX";
            return;
        }
        else
            // Show next level.
            SetUpNextLevel();
    }

    /// <summary>
    /// Quit level shop.
    /// </summary>
    public void Quit()
    {
        // Set the button interaction to false.
        ButtonType cachedButton = buttonData.buttonType;
        buttonData = new ButtonInteractionData
        {
            isInteracting = false,
            buttonType = cachedButton
        };

        // Disable UI.
        gameObject.SetActive(false);
    }

    #endregion

}

/// <summary>
/// Store level properties.
/// </summary>
[System.Serializable]
public struct LevelData
{
    public string levelName;
    public int price;
}
