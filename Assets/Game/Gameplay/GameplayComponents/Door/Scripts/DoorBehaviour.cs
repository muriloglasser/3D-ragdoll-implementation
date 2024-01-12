using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the glass door behaviour.
/// </summary>
public class DoorBehaviour : MonoBehaviour
{
    [Header("Fill image with radial fill")]
    public Image fillImage;
    [Header("Glass door")]
    public GameObject door;
    [Header("Glass door animator")]
    public Animator animator;
    [Header("Game manager")]
    public GameManager gameManager;    
    //
    private bool earningCash
    {
        get
        {
            return gameManager.earningCash;
        }
        set
        {
            gameManager.earningCash = value;
        }
    }
    private int stackedEnemies
    {
        get
        {
            return gameManager.stackedEnemiesCount;
        }
        set
        {
            gameManager.stackedEnemiesCount = value;
        }
    }
    private Coroutine fill;

    #region Unity methods

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            if (stackedEnemies == 0)
                return;

            if (fill != null)
                StopCoroutine(fill);

            fill = StartCoroutine(FillImageIn());
        }
    }

    private void OnTriggerExit(Collider other)
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
    private IEnumerator FillImageIn()
    {
        while (fillImage.fillAmount < 1)
        {
            fillImage.fillAmount += Time.deltaTime;
            yield return null;
        }

        earningCash = true;
        animator.SetBool("OpenDoor", true);
    }

    /// <summary>
    /// Set fill image feedback out and close door.
    /// </summary>
    /// <returns></returns>
    private IEnumerator FillImageOut()
    {
        while (fillImage.fillAmount > 0)
        {
            fillImage.fillAmount -= Time.deltaTime;
            yield return null;
        }

        animator.SetBool("OpenDoor", false);
    }

    #endregion
}
