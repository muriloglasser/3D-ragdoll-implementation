using UnityEngine;

/// <summary>
/// Safe area controller, need to be added on UI parent content.
/// </summary>
public class SafeArea : MonoBehaviour
{
    private RectTransform rectTransform;

    #region Unity Methods

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        // Update the UI to adjust to the safe area during initialization.
        UpdateUIForSafeArea();
    }

    void Update()
    {
        // Update the UI whenever the screen orientation changes.
        if (Screen.orientation != ScreenOrientation.AutoRotation)
        {
            UpdateUIForSafeArea();
        }
    }

    #endregion

    #region Core

    /// <summary>
    /// Update the UI based on the safe area of the screen.
    /// </summary>
    void UpdateUIForSafeArea()
    {
        // Get the safe area of the screen in pixels.
        Rect safeArea = Screen.safeArea;     

        // Update the size and position of the UI using the proportions of the safe area
        rectTransform.anchorMin = new Vector2(safeArea.xMin / Screen.width, safeArea.yMin / Screen.height);
        rectTransform.anchorMax = new Vector2(safeArea.xMax / Screen.width, safeArea.yMax / Screen.height);
    }

    #endregion
}
