using UnityEngine;

public class CameraSmoothDampFollow : MonoBehaviour
{
    public Transform target;
    public PlayerController player;
    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (player.IsOrbiting()) {
            return;
        }
        Vector3 targetPosition = target.position;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        smoothedPosition.z = -10;
        transform.position = smoothedPosition;
    }
}