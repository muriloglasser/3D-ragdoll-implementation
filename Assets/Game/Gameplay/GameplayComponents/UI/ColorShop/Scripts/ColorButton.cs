using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Control shop color button.
/// </summary>
public class ColorButton : MonoBehaviour
{
    public Image borderImg;
    public Image middleImg;
    public Button button;
    public int slotNumber;
    //
    private UIColor uiColor;

    #region Unity methods

    private void Start()
    {
        button.onClick.AddListener(() =>
        {
            SelectBorderColor();
        });
    }

    #endregion

    #region Core

    /// <summary>
    /// Set Color shop script reference and slot index.
    /// </summary>
    /// <param name="uiColor"> Ui script. </param>
    /// <param name="slotNumber"> Slot index reference. </param>
    public void SetUiScript(UIColor uiColor, int slotNumber)
    {
        this.slotNumber = slotNumber;
        this.uiColor = uiColor;
    }

    /// <summary>
    /// Set middle image color.
    /// </summary>
    /// <param name="color"> Color to set. </param>
    public void SetMiddleColor(Color color)
    {
        middleImg.color = color;
    }

    /// <summary>
    /// Set the border white when selected.
    /// </summary>
    public void SelectBorderColor()
    {
        // Deselect the last selected button.
        uiColor.DeselectAllButtons();
        // Select this slot.
        uiColor.SetSelectedProperties(slotNumber);
        borderImg.color = Color.white;
    }

    /// <summary>
    /// Set the border black when deselected.
    /// </summary>
    public void DeselectColor()
    {
        borderImg.color = Color.black;
    }

    #endregion
}
