using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxScript : MonoBehaviour
{
    public float BackgroundSize;

    [Space(10.0f)]
    public float ParalaxSpeed;

    private Transform _transform;
    private Transform _cameraTransform;
    private Transform[] _layers;

    private float _viewZone = 5.0f;

    private int _leftIndex;
    private int _rightIndex;

    private float _lastCameraX;

    private void Start()
    {
        _transform = transform;

        _cameraTransform = Camera.main.transform;
        _layers = new Transform[_transform.childCount];

        _lastCameraX = _cameraTransform.position.x;

        for (int i = 0; i < _transform.childCount; i++)
        {
            _layers[i] = _transform.GetChild(i);

            _leftIndex = 0;
            _rightIndex = _layers.Length - 1;
        }
    }

    private void Update()
    {
        float deltaX = _cameraTransform.position.x - _lastCameraX;
        _transform.position += Vector3.right * deltaX * ParalaxSpeed;

        _lastCameraX = _cameraTransform.position.x;

        if (_cameraTransform.position.x > _layers[_rightIndex].transform.position.x - _viewZone)
        {
            ScrollRight();
        }
    }

    private void ScrollRight()
    {
        int lastLeft = _leftIndex;
        _layers[_leftIndex].position = Vector3.right * (_layers[_rightIndex].position.x + BackgroundSize);
        _rightIndex = _leftIndex;
        _leftIndex++;

        if(_leftIndex == _layers.Length)
        {
            _leftIndex = 0;
        }
    }
}
