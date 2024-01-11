using UnityEngine;

/// <summary>
/// Controll player movement, rotation, and punch.
/// </summary>
public class PlayerBehaviour : MonoBehaviour
{
    [Header("Base components")]
    public Rigidbody rb;
    public Animator animator;
    [Header("Joystick controller")]
    public FloatingJoystick floatingJoy; 
    [Header("Player rotation speed")]
    public float rotationSpeed = 120f;
    [Header("Walk acceleration")]
    public float minAcceleration = 5f;
    [Header("Run acceleration")]
    public float maxAcceleration = 15f;
    [Header("How fast plarer accelerate")]
    public float accelerationModifier;
    [Header("How fast plarer desaccelerate")]
    public float desaccelerationModifier;
    //
    private float currentAcceleration;
    private Vector3 inputDirection;
    private AccelerationType acceleration;
    private Vector3 lastDirection = Vector3.zero;

    #region Unity methods

    void Start()
    {
        acceleration = AccelerationType.Idle;
    }

    void Update()
    {
        // Calculate input direction.
        inputDirection = new Vector3(floatingJoy.Direction.normalized.x, 0f, floatingJoy.Direction.normalized.y).normalized;

        Accelerate();
    }

    void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
    }

    #endregion   

    #region Movement and rotation

    /// <summary>
    /// Move player by floating joystick.
    /// </summary>
    void MovePlayer()
    {
        // Store last player direction.
        if (inputDirection != Vector3.zero)
            lastDirection = inputDirection;

        // Calculate desired velocity.
        Vector3 targetVelocity = lastDirection * currentAcceleration;

        // Apply force to the Rigidbody.
        rb.velocity = targetVelocity;
    }

    /// <summary>
    /// Rotate player.
    /// </summary>
    void RotatePlayer()
    {
        // Stop rotation when input is equal to 0.
        if (inputDirection == Vector3.zero)
            return;

        // Get the rotation direction.
        Vector3 lookDirection = inputDirection;

        // Calculate the desired rotation.
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);

        // Smoothly rotate using Slerp.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    #endregion

    #region Acceleration

    /// <summary>
    /// Accelerate player through 3 different states: idle, walk, run, and set player animations.
    /// </summary>
    public void Accelerate()
    {
        var joyMagnitude = floatingJoy.Direction.magnitude;

        // Set animations and player acceleration related to idle, walk and run states.
        switch (acceleration)
        {
            case AccelerationType.Idle:

                if (joyMagnitude > 0)
                    acceleration = AccelerationType.Walk;
                
                SetAnimatorBools(false, false);
                AccelerationTransition(0f, false);

                break;
            case AccelerationType.Walk:

                if (joyMagnitude == 0)
                    acceleration = AccelerationType.Idle;
                else if (joyMagnitude > 72f)
                    acceleration = AccelerationType.Run;

                SetAnimatorBools(false, true);
                AccelerationTransition(minAcceleration, true);

                break;
            case AccelerationType.Run:

                if (joyMagnitude == 0)
                    acceleration = AccelerationType.Idle;
                else if (joyMagnitude <= 72f)
                    acceleration = AccelerationType.Walk;
                
                SetAnimatorBools(true, false);
                AccelerationTransition(maxAcceleration, true);

                break;
        }

    }

   /// <summary>
   /// Set acceleration float to desired value.
   /// </summary>
   /// <param name="desiredValue"> Value to accelerate. </param>
   /// <param name="accelerate"> If player is accelertaing. </param>
    void AccelerationTransition(float desiredValue, bool accelerate)
    {
        // Stop acceleration when current acceleration aproximate 0 value.
        if ((currentAcceleration == 0 || currentAcceleration < 0.1f) && !accelerate)
        {          
            currentAcceleration = 0;
            return;
        }

        // Lerp acceleration to desired value.
        currentAcceleration = Mathf.Lerp(currentAcceleration, desiredValue, Time.deltaTime * (accelerate ? accelerationModifier : desaccelerationModifier));
    }

    #endregion

    #region Animation

    /// <summary>
    /// Set animator booleans to walk and run.
    /// </summary>
    /// <param name="isRunning"> Walk state. </param>
    /// <param name="isWalking"> Run state. </param>
    private void SetAnimatorBools(bool isRunning, bool isWalking)
    {
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsWalking", isWalking);
    }

    #endregion


}
public enum AccelerationType
{
    Idle,
    Walk,
    Run
}