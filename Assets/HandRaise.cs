using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandRaise : MonoBehaviour
{
    public bool _isHandRaised = false;
    public float _duration = 5f;

    private float _currentDuration = 0f;
    private bool _isHandTemporarilyRaised = false;

    private void Update()
    {
        _isHandTemporarilyRaised = IsHandRaised(transform, Camera.main.transform);
        if (_isHandTemporarilyRaised)
        {
            _currentDuration += Time.deltaTime;
            if (_currentDuration >= _duration)
            {
                _isHandRaised = true;
            }
        } else
        {
            _currentDuration = 0f;
            _isHandRaised = false;
        }
    }
    public bool IsHandRaised(Transform handTransform, Transform headTransform, float angleThreshold = 30f)
    {
        if (handTransform == null || headTransform == null)
        {
            return false;
        }
        ////float angle = Vector3.Angle( Vector3.Cross(handTransform.up, -handTransform.up), Vector3.up);
        //float angle = Vector3.Angle(transform.forward, Vector3.up);
        //return angle < angleThreshold;

        bool isHandAboveHead = handTransform.position.y > (headTransform.position.y + 0.1f);
        float handAngle = Vector3.Angle(handTransform.forward, Vector3.up);
        bool isHandRaised = handAngle < angleThreshold;

        return isHandAboveHead && isHandRaised;
    }
}
