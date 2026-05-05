using System.Collections;
using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;

    [SerializeField]
    float traceDuration = 0.05f;

    [SerializeField]
    float traceWidth = 0.05f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = traceWidth;
        lineRenderer.endWidth = 0f;
        lineRenderer.enabled = false;
    }

    public void Fire(Vector3 from, Vector3 to)
    {
        StopAllCoroutines();
        lineRenderer.SetPosition(1, to);
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, from);
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(traceDuration);
        lineRenderer.enabled = false;
    }
}
