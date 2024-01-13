using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the glass door behaviour.
/// </summary>
public class InteractableDoorButton : InteractableButton
{   
    [Header("Glass door")]
    public GameObject door;
    [Header("Glass door animator")]
    public Animator animator;     
  
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

    #region Unity methods

    public override void Start()
    {
        onFillOpened = () =>
        {
            animator.SetBool("OpenDoor", true);
        };

        onFillClosed = () =>
        {
            animator.SetBool("OpenDoor", false);
        };
    }

    public override void OnTriggerEnter(Collider other)
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

    #endregion
   
}
