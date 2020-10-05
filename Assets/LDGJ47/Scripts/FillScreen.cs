using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FillScreen : MonoBehaviour
{
    public Camera mainCamera;

    private void Awake() {
        if (!mainCamera) mainCamera = Camera.main;
    }

    private void Update() {
        float pos = (mainCamera.nearClipPlane + 10.0f);

        transform.position = mainCamera.transform.position + mainCamera.transform.forward * pos;
        transform.LookAt(mainCamera.transform);
        float h = (Mathf.Tan(mainCamera.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f) * mainCamera.aspect / 10.0f;
        float w = h * Screen.height / Screen.width;
        transform.localScale = new Vector3(h, w, 1);
    }
}
