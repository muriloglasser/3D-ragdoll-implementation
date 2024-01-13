using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls enemy punch and ragdoll behaviour.
/// </summary>
public class EnemyBehaviour : MonoBehaviour
{
    [Header("Enemy rb")]
    public Rigidbody rb;
    [Header("Enemy animator")]
    public Animator animator;
    [Header("Ragdoll rb and colliders")]
    public Rigidbody[] rigidbodies;
    public Collider[] colliders;
    [Header("Enemy capsule collider")]
    public CapsuleCollider capsuleCollider;
    [Header("Stacked enemy rotation offset")]
    public Quaternion offsetRotationLeft;
    //
    private GameObject ragdollFollower;
    private GameObject player;
    private bool ragdollActive = false;
    private RagdollData ragdollData;
    private Vector3 currentVelocity;

    #region Unity methods

    public void Start()
    {
        //Find player and ragdoll slot to follow.
        ragdollFollower = GameObject.Find("RagdollSlot");
        player = GameObject.Find("Player");

        // Set ragdoll disabled on start
        ToggleRagdoll(false);
    }

    public void FixedUpdate()
    {
        SetRagdollRotation();
        SetRagdollPosition();
    }

    #endregion

    #region Core

    /// <summary>
    /// Set ragdoll when enemy is punched.
    /// </summary>
    /// <param name="ragdollData"> Ragdoll data from player. </param>
    public void Die(RagdollData ragdollData)
    {
        // Store ragdoll data from player.
        this.ragdollData = ragdollData;

        // Set main rb as kinematic to enable ragdoll.
        capsuleCollider.enabled = false;
        animator.enabled = false;

        // Enable ragdoll.
        ToggleRagdoll(true);

        // Apply punch force.
        ApplyForceToRagdoll(ragdollData.punchDirection, 2);

        // Wait 3 secs and go to player head.
        StartCoroutine(WaitToFloat());
    }

    /// <summary>
    /// Enable or disable ragdoll system.
    /// </summary>
    /// <param name="isRagdollActive"> Ragdoll enable state. </param>
    private void ToggleRagdoll(bool isRagdollActive)
    {

        // Set rigidbody list enabled or disabled.
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = !isRagdollActive;
        }

        // Set collider list enabled or disabled.
        foreach (Collider col in colliders)
        {
            col.enabled = isRagdollActive;
        }
    }

    /// <summary>
    /// Apply punch force to ragdoll.
    /// </summary>
    /// <param name="forceDirection"> Force direction. </param>
    private void ApplyForceToRagdoll(Vector3 forceDirection, float multiplier = 1)
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.AddForce(forceDirection * 13 * multiplier, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// Wait to float on player head.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitToFloat()
    {
        yield return new WaitForSeconds(3f);
        //Set pelvis and spine as kinematic to better controll ragdoll on player head.
        rigidbodies[0].isKinematic = true;
        rigidbodies[5].isKinematic = true;
        ragdollActive = true;
    }

    /// <summary>
    /// Set ragdoll rotation onplayer head.
    /// </summary>
    private void SetRagdollRotation()
    {
        if (!ragdollActive)
            return;

        Quaternion playerRotation = player.transform.rotation;
        rigidbodies[0].transform.rotation = playerRotation * offsetRotationLeft;
    }

    /// <summary>
    /// Set ragdoll position onplayer head.
    /// </summary>
    private void SetRagdollPosition()
    {
        if (!ragdollActive)
            return;

        rigidbodies[0].transform.position = Vector3.SmoothDamp(rigidbodies[0].transform.position, ragdollFollower.transform.position + new Vector3(0, ragdollData.yOffset, 0), ref currentVelocity, ragdollData.inerciaOffset);

    }

    /// <summary>
    /// Throw enemie when It's stacked.
    /// </summary>
    /// <param name="forceDirection"> Direction to throw .</param>
    public void StackedThrow(Vector3 forceDirection)
    {
        ragdollActive = false;
        rigidbodies[0].isKinematic = false;
        rigidbodies[5].isKinematic = false;
        ApplyForceToRagdoll(forceDirection);
        StartCoroutine(ResetEnemy());
    }

    /// <summary>
    /// Reset enemy behavior after being thrown.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetEnemy()
    {
        yield return new WaitForSeconds(4f);
        ToggleRagdoll(false);
        animator.enabled = true;
        capsuleCollider.enabled = true;
    }

    #endregion
}

/// <summary>
/// Store ragdoll data comming from player.
/// </summary>
public struct RagdollData
{
    public Vector3 punchDirection;
    public float yOffset;
    public float inerciaOffset;
}