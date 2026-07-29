using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelector : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private bool fadeToBlack = false;
    [SerializeField] private float fadeDuration = 1f;

    public void LoadScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("SceneSelector: No scene assigned.");
            return;
        }

        if (fadeToBlack)
        {
            StartCoroutine(FadeAndLoadScene());
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator FadeAndLoadScene()
    {
        GameObject overlayObject = new GameObject("SceneFadeOverlay");
        Canvas canvas = overlayObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        CanvasGroup canvasGroup = overlayObject.AddComponent<CanvasGroup>();
        Image image = overlayObject.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0f);

        RectTransform rectTransform = overlayObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            image.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        image.color = new Color(0f, 0f, 0f, 1f);
        SceneManager.LoadScene(sceneName);
    }
}