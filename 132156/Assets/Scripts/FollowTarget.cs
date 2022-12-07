using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public bool ObjectIsCamera;

    public Transform TargetToFollow;
    public float Smoothing = 5.0f;

    private void LateUpdate()
    {
        SmoothFollow();
    }

    private void SmoothFollow()
    {
        if (TargetToFollow == null)
            return;

        Vector3 targetPosition = new Vector3();

        if (ObjectIsCamera)
        {
            if (PlayerStats.Instance.IsDead)
                return;

            targetPosition[0] = TargetToFollow.position.x;
            targetPosition[1] = TargetToFollow.position.y;
            targetPosition[2] = gameObject.transform.position.z;

            transform.position = Vector3.Lerp(transform.position, targetPosition, Smoothing * Time.deltaTime);
        }
        else
        {
            targetPosition = TargetToFollow.position;

            transform.position = Vector2.Lerp(transform.position, targetPosition, Smoothing * Time.deltaTime);
        }
    }

    public void SetTargetToFollow(Transform target)
    {
        TargetToFollow = target;
    }
}
