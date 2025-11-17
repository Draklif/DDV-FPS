using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    [Header("Timing")]
    public float showTime = 4f;
    public float fadeTime = 1.5f;

    void Start()
    {
        canvasGroup.alpha = 1f;

        Time.timeScale = 0f;

        StartCoroutine(FadeRoutine());
    }

    System.Collections.IEnumerator FadeRoutine()
    {
        // Esperar mientras se muestra (tiempo real)
        yield return new WaitForSecondsRealtime(showTime);

        float t = 0f;
        float start = 1f;

        // Fade usando tiempo real porque timeScale está en 0
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, 0f, t / fadeTime);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);

        // Reanudar el juego
        Time.timeScale = 1f;
    }
}
