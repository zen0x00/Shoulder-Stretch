using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -10);
    [SerializeField] private float smoothSpeed = 5f;
    private void Start() { if (target == null) target = PlayerController.Instance?.transform; }
    private void LateUpdate()
    {
        if (target == null) return;
        transform.position = Vector3.Lerp(transform.position, target.position + offset, smoothSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up);
    }
}