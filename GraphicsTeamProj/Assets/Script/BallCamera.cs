using UnityEngine;

public class FollowCameraNoPivot : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Dead Zone Settings")]
    public float deadZoneHeight = 2f;
    public bool showDeadZoneGizmo = true;

    [Header("Camera Orbit Settings")]
    public float distance = 5f;
    public float minPitch = 10f;
    public float maxPitch = 80f;
    public float rotationSpeed = 100f;
    public float smoothSpeed = 10f;

    private float currentYaw = 0f;
    private float currentPitch = 30f;
    private float pivotY;

    private Vector3 pivotPosition;      

    void Start()
    {
        if (target == null)
            return;

        pivotPosition = target.position;
        pivotY = target.position.y;
    }

    void Update()
    {
        if (target == null) return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        currentYaw   += mouseX * rotationSpeed * Time.deltaTime;
        currentPitch -= mouseY * rotationSpeed * Time.deltaTime;
        currentPitch  = Mathf.Clamp(currentPitch, minPitch, maxPitch);

        float ballY = target.position.y;
        float halfZone = deadZoneHeight * 0.5f;

        if (ballY > pivotY + halfZone)
        {
            pivotY = ballY - halfZone;
        }
        else if (ballY < pivotY - halfZone)
        {
            pivotY = ballY + halfZone;
        }

        Vector3 targetPivotPos = new Vector3(target.position.x, pivotY, target.position.z);

        pivotPosition = Vector3.Lerp(
            pivotPosition,
            targetPivotPos,
            Time.deltaTime * smoothSpeed
        );

        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);

        Vector3 desiredCameraPos = pivotPosition + rotation * (Vector3.back * distance);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredCameraPos,
            Time.deltaTime * smoothSpeed
        );

        transform.LookAt(pivotPosition);
    }

    void OnDrawGizmos()
    {
        if (!showDeadZoneGizmo || target == null) return;

        Vector3 center = (Application.isPlaying) ? pivotPosition : target.position;

        float halfZone = deadZoneHeight * 0.5f;

#if UNITY_EDITOR
        UnityEditor.Handles.color = new Color(0f, 1f, 0f, 0.5f);
        float radius = 0.5f;

        UnityEditor.Handles.DrawWireDisc(center + Vector3.up * halfZone, Vector3.up, radius);
        UnityEditor.Handles.DrawWireDisc(center - Vector3.up * halfZone, Vector3.up, radius);

        UnityEditor.Handles.DrawLine(center + Vector3.up * halfZone + Vector3.right * radius,
                                     center - Vector3.up * halfZone + Vector3.right * radius);
        UnityEditor.Handles.DrawLine(center + Vector3.up * halfZone - Vector3.right * radius,
                                     center - Vector3.up * halfZone - Vector3.right * radius);
        UnityEditor.Handles.DrawLine(center + Vector3.up * halfZone + Vector3.forward * radius,
                                     center - Vector3.up * halfZone + Vector3.forward * radius);
        UnityEditor.Handles.DrawLine(center + Vector3.up * halfZone - Vector3.forward * radius,
                                     center - Vector3.up * halfZone - Vector3.forward * radius);
#endif
    }
}
