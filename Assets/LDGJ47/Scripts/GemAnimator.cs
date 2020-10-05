using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemAnimator : MonoBehaviour
{
    public Transform target;
    public Transform relativeTo;

    public float tick = 0f;
    public float amplitude = 0.15f;
    public float spinSpeed = 180f;
    public float rate = 5.0f;

    // Update is called once per frame
    void Update()
    {
        target.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
        Vector3 offset = Vector3.up * (Mathf.Sin(tick) * amplitude);
        target.position = relativeTo.position + offset;
        tick += Time.deltaTime * rate;
    }
}
