using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handle a camera that follow a target position.
/// </summary>
public class CameraBehaviour : MonoBehaviour
{
    [Header("Target to follow")]
    public Transform target;
    [Header("Camera offset")]
    public Vector3 offset = new Vector3(0f, 2f, -5f);
    [Header("Camera follow smoothness")]
    public float smoothness = 5f;

    #region Unity methods

    void FixedUpdate()
    {
        FollowTarget();
    }

    #endregion

    #region Core

    /// <summary>
    /// Follow a target, lerping its position with an offset.
    /// </summary>
    private void FollowTarget()
    {
        // Calculate the desired camera position based on the player's position and offset.
        Vector3 desiredPosition = target.position + offset;

        // Move the camera smoothly towards the desired position.
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothness * Time.fixedDeltaTime);
    }

    #endregion
}
