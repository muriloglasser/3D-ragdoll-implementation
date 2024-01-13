using UnityEngine;

/// <summary>
/// The floating joystick behaves in a way that when the screen is pressed,
/// the joystick pops up at the location of the finger and disappears when the finger is lifted.
/// </summary>
public class FloatingJoystick : MonoBehaviour
{
    [Header("Game manager")]
    public GameManager gameManager;
    [Header("Joystick background art")]
    public RectTransform joystickBackground;
    [Header("Joystick handle art")]
    public RectTransform joystickHandle;
    //
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
    public Vector2 Direction
    {
        get
        {
            return direction;
        }
    }
    //
    private Vector2 direction;
    private bool isJoystickActive = false;

    #region Unity metods

    private void Start()
    {
        // Disable joystick on start.
        SetJoystickState(false);
    }

    void Update()
    {
        if (buttonData.isInteracting)
        {
            ResetJoystick();
            return;
        }

        CheckJoystickInput();
    }

    #endregion

    #region Core

    /// <summary>
    /// Check the input from fingers on the device screen.
    /// </summary>
    private void CheckJoystickInput()
    {
        if (Input.touchCount > 0)
        {
            // Get first finger that touched screen.
            Touch touch = Input.GetTouch(0);

            // Finger touch start.
            if (touch.phase == TouchPhase.Began)
            {
                Vector2 touchPos = touch.position;
                joystickBackground.position = touchPos;
                SetJoystickState(true);
                SetJoystickPosition(touchPos);
            }
            // Finger is moving in the screen.
            else if (touch.phase == TouchPhase.Moved && isJoystickActive)
            {
                Vector2 touchPos = touch.position;
                SetJoystickPosition(touchPos);
            }
            // Finger was lifted.
            else if (touch.phase == TouchPhase.Ended)
            {
                ResetJoystick();
            }
        }
    }

    /// <summary>
    /// Set joystick movement by finger position.
    /// </summary>
    /// <param name="touchPosition"> Finger position at screen. </param>
    private void SetJoystickPosition(Vector2 touchPosition)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground, touchPosition, null, out localPoint);

        joystickHandle.localPosition = localPoint;

        // Limit joystick handle within joystick background boundaries
        Vector2 clampedPosition = Vector2.ClampMagnitude(localPoint, joystickBackground.sizeDelta.x * 0.5f);
        joystickHandle.localPosition = clampedPosition;
        direction = clampedPosition;
    }

    /// <summary>
    /// Reset joystick properties when the finger is lifted.
    /// </summary>
    private void ResetJoystick()
    {
        direction = Vector2.zero;
        SetJoystickState(false);
        joystickHandle.localPosition = Vector2.zero;
    }

    /// <summary>
    /// Set joystick enabled or disabled.
    /// </summary>
    /// <param name="state"> State change joystick. </param>
    private void SetJoystickState(bool state)
    {
        joystickBackground.gameObject.SetActive(state);
        isJoystickActive = state;
    }

    #endregion
}