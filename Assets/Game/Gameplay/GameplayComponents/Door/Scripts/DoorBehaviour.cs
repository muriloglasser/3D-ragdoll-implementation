using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorBehaviour : MonoBehaviour
{
    public Image fillImage;
    public GameObject door;
    //
    private Coroutine fill;

    #region Unity methods

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
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

    private IEnumerator FillImageIn()
    {
        while (fillImage.fillAmount <= 1)
        {
            fillImage.fillAmount += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FillImageOut()
    {
        while (fillImage.fillAmount > 0)
        {
            fillImage.fillAmount -= Time.deltaTime;
            yield return null;
        }
    }

    #endregion
}
