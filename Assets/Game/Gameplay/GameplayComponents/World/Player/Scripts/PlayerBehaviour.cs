using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controll player movement, rotation, and punch.
/// </summary>
public class PlayerBehaviour : MonoBehaviour
{
    [Header("Game manager")]
    public GameManager gameManager;
    [Header("Base components")]
    public Rigidbody rb;
    public Animator animator;
    [Space(10)]
    [Header("Punch layer mask")]
    public LayerMask punchLayer;
    [Space(10)]
    [Header("Throw point")]
    public Transform throwPoint;
    [Space(10)]
    [Header("Joystick controller")]
    public FloatingJoystick floatingJoy;
    [Space(10)]
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
    private int playerLevel
    {
        get { return gameManager.saveData.playerLevel; }
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
    private AccelerationType acceleration;
    private RagdollData ragdollData;
    private float currentAcceleration;
    private bool isPunching = false;
    private Vector3 inputDirection;
    private Vector3 lastDirection = Vector3.zero;
    private List<EnemyBehaviour> enemies = new List<EnemyBehaviour>();
    private Coroutine throwEnemies = null;

    #region Unity methods

    public virtual void Start()
    {
        acceleration = AccelerationType.Idle;

        // Set up ragdoll data.
        ragdollData = new RagdollData
        {
            punchDirection = Vector3.zero,
            yOffset = 0f,
            inerciaOffset = 0.045f,
        };
    }

    public virtual void Update()
    {
        // Check if is the throw button and then throw enemies.
        if (buttonData.isInteracting && buttonData.buttonType == ButtonType.Throw && throwEnemies == null)
            throwEnemies = StartCoroutine(ThrowEnemies());

        // Calculate input direction.
        inputDirection = new Vector3(floatingJoy.Direction.normalized.x, 0f, floatingJoy.Direction.normalized.y).normalized;

        // Movement acceleration.
        Accelerate();
    }

    public virtual void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
        CheckPunch();
    }

    #endregion

    #region Movement and rotation

    /// <summary>
    /// Move player by floating joystick.
    /// </summary>
    private void MovePlayer()
    {
        if (buttonData.isInteracting)
            return;

        if (isPunching)
            return;

        // Store last player direction.
        if (inputDirection != Vector3.zero)
            lastDirection = inputDirection;

        // Calculate desired velocity.
        Vector3 targetVelocity = lastDirection * currentAcceleration;

        //Apply rb y velocity.
        targetVelocity.y = rb.velocity.y;

        // Apply force to the Rigidbody.
        rb.velocity = targetVelocity;
    }

    /// <summary>
    /// Rotate player.
    /// </summary>
    private void RotatePlayer()
    {
        if (buttonData.isInteracting)
            return;

        if (isPunching)
            return;

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

    #region Punch

    /// <summary>
    /// Check with a box cast the next enemy to recieve a punch.
    /// </summary>
    private void CheckPunch()
    {
        if (playerLevel == enemies.Count)
            return;

        if (buttonData.isInteracting)
            return;

        if (isPunching)
            return;

        // Get the position and direction from the player's torso.
        Vector3 origin = transform.position + new Vector3(0, 2, 0);
        Vector3 boxSize = new Vector3(0.3f, 0.3f, 0.3f);
        Vector3 direction = transform.forward;

        // Set the maximum length of the raycast.
        float maxDistance = 1f;

        // Make a box cast to find the next enemy to punch.
        bool hit = Physics.BoxCast(origin, boxSize * 0.5f, direction, out RaycastHit hitInfo, Quaternion.identity, maxDistance, punchLayer);

        // Set punch coroutine.
        if (hit)
            StartCoroutine(Punch(hitInfo));
    }

    /// <summary>
    /// Make a punch to the raycasted target.
    /// </summary>
    /// <param name="enemy"> Enemy raycasted. </param>
    /// <returns></returns>
    private IEnumerator Punch(RaycastHit enemy)
    {
        isPunching = true;

        rb.isKinematic = true;

        // Enable rootmotion to animation be more realistic.
        animator.applyRootMotion = true;

        // Start punch animation.
        SetPunchAnimation(true);

        bool ragdollActive = false;

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Hook Punch"))
            yield return null;

        // Wait unitl animation end.
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            // Rotate to target to keep player focused on enemy.
            RotatePlayerToTarget((enemy.transform.position - transform.position).normalized);

            // Right frame to set ragdoll.
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.52f && !ragdollActive)
            {
                // Enable ragdoll once.
                ragdollActive = true;

                // Set ragdoll punch direction.
                ragdollData.punchDirection = enemy.transform.position - transform.position;

                // Set punched enemy ragdoll.
                EnemyBehaviour newEnemy = enemy.collider.gameObject.GetComponent<EnemyBehaviour>();
                newEnemy.Die(ragdollData);
                enemies.Add(newEnemy);
                stackedEnemies++;

                // The y offset of ragdoll stack.
                ragdollData.yOffset += 0.8f;

                // The inertia of ragdoll stack.
                ragdollData.inerciaOffset += 0.025f;
            }

            yield return null;
        }

        rb.isKinematic = false;

        // Disable root motion on animation end.
        animator.applyRootMotion = false;

        // Finish punch animation.
        SetPunchAnimation(false);

        isPunching = false;
    }

    /// <summary>
    /// Rotate player to punch target position.
    /// </summary>
    /// <param name="directionToTarget"> Target to rotate. </param>
    private void RotatePlayerToTarget(Vector3 directionToTarget)
    {
        // Set x to zero so player will not look to the ground.
        directionToTarget.y = 0;

        // Calculate the desired rotation.
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

        // Smoothly rotate using Slerp.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    #endregion

    #region Throw enemies

    /// <summary>
    /// Throw away stacked enemies.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ThrowEnemies()
    {
        // Throw enemies.
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            yield return new WaitForSeconds(0.5f);
            enemies[i].StackedThrow((throwPoint.position - transform.position).normalized * 1.4f);
            gameManager.AddCoins(100);
        }

        // Save coins.
        gameManager.Save();

        // Clear stacked enemies.
        enemies.Clear();

        // Set the button interaction to false.
        ButtonType cachedButton = buttonData.buttonType;
        buttonData = new ButtonInteractionData
        {
            isInteracting = false,
            buttonType = cachedButton
        };

        stackedEnemies = 0;
        ragdollData.yOffset = 0f;
        ragdollData.inerciaOffset = 0.025f;
        throwEnemies = null;
    }

    #endregion

    #region Acceleration

    /// <summary>
    /// Accelerate player through 3 different states: idle, walk, run, and set player animations.
    /// </summary>
    private void Accelerate()
    {
        if (buttonData.isInteracting)
        {
            SetAnimatorBools(false, false);
            return;
        }

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
    private void AccelerationTransition(float desiredValue, bool accelerate)
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

    /// <summary>
    /// Set punch animation.  
    /// </summary>
    private void SetPunchAnimation(bool isPunching)
    {
        animator.SetBool("IsPunching", isPunching);
    }

    #endregion

}

/// <summary>
/// Player acceleration states.
/// </summary>
public enum AccelerationType
{
    Idle,
    Walk,
    Run
}

