using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Target;

    [Space(10.0f)]
    public Vector3 Offset;

    [Space(10.0f)]
    public float FollowSpeed;

    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    private void Update()
    {
        _transform.position = new Vector3(Target.position.x + Offset.x, _transform.position.y, Offset.z);
    }
}
