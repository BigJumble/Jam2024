using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImageSize : MonoBehaviour
{
    RectTransform rectTransformToResize;
    float duration = 3f;
    public float maxSize = 1.1f;
    private Vector3 initialScale;
    private Vector2 initialX;
    private Coroutine resizeCoroutine;

    private void OnEnable()
    {
        // Start resizing when the script is enabled
        resizeCoroutine = StartCoroutine(ChangeSizeOverTime());
    }

    private void OnDisable()
    {
        // Reset the scale when the script is disabled
        if (resizeCoroutine != null)
        {
            StopCoroutine(resizeCoroutine);
            ResetRectTransformScale();
        }
    }
    private void Awake()
    {
        rectTransformToResize = this.gameObject.GetComponent<RectTransform>();
        initialScale = rectTransformToResize.localScale;
        initialX = rectTransformToResize.anchoredPosition;
    }
    //private void Start()
    //{
    //    // Store the initial scale of the RectTransform
    //    initialScale = rectTransformToResize.localScale;
    //    initialX = rectTransformToResize.anchoredPosition;
    //}

    private IEnumerator ChangeSizeOverTime()
    {

        float elapsedTime = 0f;
        float newX = initialX.x;
        Debug.Log(newX);
        while (elapsedTime < duration)
        {
            // Calculate the new scale based on the random percentage change
            float newScale =  maxSize;

            // Interpolate between the initial scale and the new scale
            float t = elapsedTime / duration;
            float lerped = Mathf.Lerp(1, newScale, t);
            rectTransformToResize.localScale = new Vector3(lerped, lerped, 1);

            rectTransformToResize.anchoredPosition = new Vector2(newX, rectTransformToResize.anchoredPosition.y);
            newX += Time.deltaTime * 20;
            // Update the elapsed time
            elapsedTime += Time.deltaTime;

            yield return null; // Wait for the next frame
        }

        // Ensure the final scale is exactly the target scale
        //rectTransformToResize.localScale = new Vector3(initialScale.x * (1 + randomChangePercentage), initialScale.y * (1 + randomChangePercentage), initialScale.z);
    }

    private void ResetRectTransformScale()
    {
        // Reset the scale of the RectTransform to the initial scale
        rectTransformToResize.localScale = initialScale;
        rectTransformToResize.anchoredPosition = initialX;
    }
}