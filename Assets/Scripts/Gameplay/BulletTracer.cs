using System.Collections;
using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float traceDuration = 0.05f;
    [SerializeField] float traceWidth = 0.05f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = traceWidth;
        lineRenderer.endWidth = 0f;
        gameObject.SetActive(false);
    }

    public void Fire(Vector3 from, Vector3 to)
    {
        StopAllCoroutines();
        gameObject.SetActive(true);
        lineRenderer.SetPosition(0, from);
        lineRenderer.SetPosition(1, to);
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(traceDuration);
        gameObject.SetActive(false);
    }
}
