using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Vector3 cameraOffset;
    [SerializeField] Transform targetTransform;
    [SerializeField] float moveSpeed;


    void Update()
    {
        Vector3 targetPosition = targetTransform.position + cameraOffset;
        if (!Manager.ascending) {transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime); }
    }
}
