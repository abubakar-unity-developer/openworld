using UnityEngine;

public class HelicopterCamera : MonoBehaviour
{
    public Transform target;   // helicopter ka Transform
    public Vector3 offset = new Vector3(0, 5, -10);
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desiredPosition = target.position + target.rotation * offset;
        Vector3 smoothed = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothed;
        transform.LookAt(target);
    }
}
