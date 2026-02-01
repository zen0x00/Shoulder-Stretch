using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class FeedbackUIController : MonoBehaviour
{
    [SerializeField] private Image feedbackImage;
    [SerializeField] private Color correctColor = new Color(0, 1, 0, 0.3f);
    [SerializeField] private Color incorrectColor = new Color(1, 0, 0, 0.3f);
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private InputSystem inputSystem;
    private Coroutine flashCoroutine;
    private void Start()
    {
        if (inputSystem == null) inputSystem = FindFirstObjectByType<InputSystem>();
        if (inputSystem != null) inputSystem.OnActionPerformed += HandleAction;
        if (feedbackImage) feedbackImage.enabled = false;
    }
    private void OnDestroy() { if (inputSystem != null) inputSystem.OnActionPerformed -= HandleAction; }
    private void HandleAction(ActionType action, bool success)
    {
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(Flash(success ? correctColor : incorrectColor));
    }
    private IEnumerator Flash(Color targetColor)
    {
        if (!feedbackImage) yield break;
        feedbackImage.enabled = true;
        feedbackImage.color = targetColor;
        yield return new WaitForSeconds(flashDuration);
        feedbackImage.enabled = false;
    }
}