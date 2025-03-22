using System;
using UnityEngine;

public class FollowCamera : MonoBehaviour {
    private enum Mode {
        LookAt,
        LookAtInverse,
        Forward,
        ForwardInverse,
    }


    [SerializeField] private Mode mode = Mode.LookAt;


    private void Update() {
        var mainCamera = Camera.main;
        if (mainCamera is null) return;
        Debug.Log("HERE");
        switch (mode) {
            case Mode.LookAt:
                transform.LookAt(mainCamera.transform);
                break;
            case Mode.LookAtInverse:
                var directionFromCamera = transform.position - mainCamera.transform.position;
                transform.LookAt(transform.position + directionFromCamera);
                break;
            case Mode.Forward:
                transform.forward = mainCamera.transform.forward;
                break;
            case Mode.ForwardInverse:
                transform.forward = -mainCamera.transform.forward;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
