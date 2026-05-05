using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // [SerializeField] private Transform target;
    // [SerializeField] private Vector3 offset = new Vector3(0, 5, -10);
    // [SerializeField] private float smoothSpeed = 5f;
    // private void Start() { if (target == null) target = PlayerController.Instance?.transform; }
    // private void LateUpdate()
    // {
    //     if (target == null) return;
    //     transform.position = Vector3.Lerp(transform.position, target.position + offset, smoothSpeed * Time.deltaTime);
    //     transform.LookAt(target.position + Vector3.up);
    // }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float e = 0;

        while (e < duration)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * magnitude;
            e += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}

