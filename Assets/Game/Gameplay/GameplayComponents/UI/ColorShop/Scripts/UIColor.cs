using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIColor : MonoBehaviour
{
    [Header("Game manager")]
    public GameManager gameManager;
    [Header("UI")]
    public TMP_Text priceTxt;
    public TMP_Text colorTxt;
    public TMP_Text buttonTxt;
    public Button buyBtn;
    public Button quitBtn;
    [Header("List of buttons")]
    public List<ColorButton> buttons;
    [Header("List of colors")]
    public ColorDataScriptable colorData;
    [Header("Player material")]
    public Material skinColor;
    //
    private int currentColor
    {
        get
        {
            return gameManager.saveData.currentColor;
        }
        set
        {
            gameManager.saveData.currentColor = value;
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
    private int selectedColorIndex;

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
        // Set initial screen state.
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetUiScript(this, i);
            buttons[i].SetMiddleColor(colorData.colors[i].color);
            buttons[i].DeselectColor();
        }

        // Set current color selected.
        buttons[currentColor].SelectBorderColor();

        selectedColorIndex = currentColor;
    }

    #endregion

    #region Core   

    /// <summary>
    /// Buy new level.
    /// </summary>
    public void Buy()
    {
        buyBtn.interactable = false;
        priceTxt.text = "000";
        buttonTxt.text = "Equiped";
        currentColor = selectedColorIndex;

        // Unlock color on save and remove it from player.
        if (!gameManager.saveData.unlockedColors.Contains(selectedColorIndex))
        {
            gameManager.RemoveCoins(colorData.colors[selectedColorIndex].price);
            gameManager.UnlockColor(selectedColorIndex);
        }

        gameManager.Save();

        skinColor.SetColor("_Color", colorData.colors[selectedColorIndex].color);
    }

    /// <summary>
    /// Quit color shop.
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

    /// <summary>
    /// Deselect all shop buttons.
    /// </summary>
    public void DeselectAllButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
            buttons[i].DeselectColor();
    }

    /// <summary>
    /// Set selected color properties on ui.
    /// </summary>
    /// <param name="colorIndex"> Color index. </param>
    public void SetSelectedProperties(int colorIndex)
    {
        selectedColorIndex = colorIndex;

        // Color already unlocked.
        if (gameManager.saveData.unlockedColors.Contains(colorIndex) && currentColor != colorIndex)
        {
            buyBtn.interactable = true;
            priceTxt.text = "000";
            colorTxt.text = colorData.colors[colorIndex].name;
            buttonTxt.text = "Equip";
        }
        // Color already selected.
        else if (gameManager.saveData.unlockedColors.Contains(colorIndex) && currentColor == colorIndex)
        {
            buyBtn.interactable = false;
            priceTxt.text = "000";
            colorTxt.text = colorData.colors[colorIndex].name;
            buttonTxt.text = "Equiped";
        }
        // Color locked.
        else
        {
            buyBtn.interactable = currentCoins >= colorData.colors[colorIndex].price ? true : false;
            priceTxt.text = colorData.colors[colorIndex].price.ToString();
            colorTxt.text = colorData.colors[colorIndex].name;
            buttonTxt.text = "Buy";
        }
    }

    #endregion
}

/// <summary>
/// Handle shop color, prices and names.
/// </summary>
[System.Serializable]
public struct ColorData
{
    public string name;
    public int price;
    public Color color;
}
