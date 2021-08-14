using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTargetAfterTime : MonoBehaviour
{
    public GameObject TargetToDestroy;
    public float DestroyAfterTime = 0.0f;

    private float _stopwatch = 0.0f;

    private void Update()
    {
        _stopwatch += Time.deltaTime;

        if (_stopwatch >= DestroyAfterTime)
            Destroy(TargetToDestroy);
    }
}
