using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Interactable button main class.
/// </summary>
public class InteractableButton : MonoBehaviour
{
    [Header("Game manager")]
    public GameManager gameManager;
    [Header("Fill image with radial fill")]
    public Image fillImage;    
    [Header("Button type")]
    public ButtonType buttonType;
    //
    protected Action onFillOpened;
    protected Action onFillClosed;
    protected Coroutine fill;
    protected ButtonInteractionData buttonData
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

    public virtual void Start()
    {
        onFillOpened = () =>
        {
            Debug.Log("Open");
        };

        onFillClosed = () =>
        {
            Debug.Log("Close");
        };
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            if (fill != null)
                StopCoroutine(fill);

            fill = StartCoroutine(FillImageIn());
        }

    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            if (fill != null)
                StopCoroutine(fill);

            fill = StartCoroutine(FillImageOut());
        }
    }

    #endregion

    #region Core

    /// <summary>
    /// Set fill image feedback in and open door.
    /// </summary>
    /// <returns></returns>
    protected IEnumerator FillImageIn()
    {
        while (fillImage.fillAmount < 1)
        {
            fillImage.fillAmount += Time.deltaTime;
            yield return null;
        }

        buttonData = new ButtonInteractionData
        {
            isInteracting = true,
            buttonType = buttonType
        };

        onFillOpened?.Invoke();
    }

    /// <summary>
    /// Set fill image feedback out and close door.
    /// </summary>
    /// <returns></returns>
    protected IEnumerator FillImageOut()
    {
        while (fillImage.fillAmount > 0)
        {
            fillImage.fillAmount -= Time.deltaTime;
            yield return null;
        }

        onFillClosed?.Invoke();
    }

    #endregion
}

/// <summary>
/// Handle current pressed button states.
/// </summary>
public struct ButtonInteractionData
{
    public bool isInteracting;
    public ButtonType buttonType;
}

/// <summary>
/// Button types enum.
/// </summary>
public enum ButtonType
{
    Throw,
    Level,
    Shop
}


