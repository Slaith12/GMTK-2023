using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewButton : MonoBehaviour
{
    [SerializeField] RectTransform label;
    private Image button;

    private Coroutine currentCoroutine;

    private void Awake()
    {
        button = GetComponent<Image>();
    }

    public void ButtonHover()
    {
        button.CrossFadeColor(new Color(0.9f, 0.9f, 0.9f), 0.5f, false, false);
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(ExtendLabel());
    }

    private IEnumerator ExtendLabel(float speed = 500)
    {
        float startTime = Time.time;
        label.anchorMin = Vector2.zero;
        float startLength = label.sizeDelta.x;
        float currentLength = startLength;
        float finalLength = ((RectTransform)label.parent).sizeDelta.x;
        while (currentLength < finalLength)
        {
            label.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentLength);
            yield return null;
            currentLength = (Time.time - startTime) * speed + startLength;
        }
        label.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, finalLength);
    }

    public void ButtonUnhover()
    {
        button.CrossFadeColor(Color.white, 0.5f, false, false);
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(UnextendLabel());
    }

    private IEnumerator UnextendLabel(float speed = 500)
    {
        float startTime = Time.time;
        label.anchorMin = new Vector2(1,0);
        float startLength = label.sizeDelta.x + ((RectTransform)label.parent).sizeDelta.x;
        float currentLength = startLength;
        float finalLength = 0;
        while (currentLength > finalLength)
        {
            label.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentLength);
            yield return null;
            currentLength = (Time.time - startTime) * -speed + startLength;
        }
        label.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, finalLength);
    }
}
