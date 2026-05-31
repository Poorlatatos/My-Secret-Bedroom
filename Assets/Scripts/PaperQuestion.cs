using UnityEngine;
using TMPro;
using System.Collections;

public class PaperQuestion : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text questionText;
    public TMP_Text yesText;
    public TMP_Text noText;

    [Header("Question")]
    [TextArea]
    public string questionLine = "Do you want to continue?";

    [Header("Answers")]
    [TextArea]
    public string yesAnswer = "You chose YES. This is answer A.";
    [TextArea]
    public string noAnswer = "You chose NO. This is answer B.";

    [Header("Head Gesture Detector")]
    public HeadGestureDetector gestureDetector;

    [Header("Typewriter Effect")]
    public TypewriterEffect typewriterEffect;

    [Header("Option Labels")]
    public string yesLabel = "Yes";
    public string noLabel = "No";

    private bool optionsVisible = false;
    private bool isActive = false;
    private string pendingAnswer = null;

    void Start()
    {
        // Hide everything at start
        if (questionText != null)
        {
            questionText.text = "";
        }
        SetOptionsAlpha(0f);
    }

    void Update()
    {
        if (!isActive || !optionsVisible || gestureDetector == null)
            return;

        if (gestureDetector.DidNod())
        {
            pendingAnswer = yesAnswer;
            StartCoroutine(FadeOutAllUIAndShowAnswer());
            isActive = false;
        }
        else if (gestureDetector.DidShake())
        {
            pendingAnswer = noAnswer;
            StartCoroutine(FadeOutAllUIAndShowAnswer());
            isActive = false;
        }
    }

    IEnumerator FadeOutAllUIAndShowAnswer()
    {
        float duration = 1.0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            if (questionText != null) questionText.alpha = alpha;
            if (yesText != null) yesText.alpha = alpha;
            if (noText != null) noText.alpha = alpha;
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (questionText != null) questionText.alpha = 0f;
        if (yesText != null) yesText.alpha = 0f;
        if (noText != null) noText.alpha = 0f;
        optionsVisible = false;

        // Show the answer with <br> support
        if (questionText != null && !string.IsNullOrEmpty(pendingAnswer))
        {
            questionText.text = "";
            questionText.alpha = 1f;
            questionText.gameObject.SetActive(true);

            string[] segments = pendingAnswer.Split(new string[] { "<br>" }, System.StringSplitOptions.None);
            foreach (string segment in segments)
            {
                if (typewriterEffect != null)
                {
                    typewriterEffect.textComponent = questionText;
                    typewriterEffect.StartTypewriter(segment.Trim());
                    yield return new WaitForSeconds(3f);
                }
                else
                {
                    questionText.text = segment.Trim();
                    yield return new WaitForSeconds(3f);
                }
                questionText.text = ""; // Clear before next segment
            }
        }

        // Optionally, fade out the answer after all segments
        yield return new WaitForSeconds(0.5f);
        if (questionText != null)
        {
            float fadeDuration = 1.0f;
            float fadeElapsed = 0f;
            while (fadeElapsed < fadeDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, fadeElapsed / fadeDuration);
                questionText.alpha = alpha;
                fadeElapsed += Time.deltaTime;
                yield return null;
            }
            questionText.alpha = 1.0f;
            questionText.text = ""; // Clear text after fade out
        }

        ResetQuestionUI();

        if (TextDisplayManager.Instance != null)
        {
            TextDisplayManager.Instance.SetBusy(false);
        }
    }

    public void BeginPaperQuestion()
    {
        if (TextDisplayManager.Instance != null && TextDisplayManager.Instance.IsBusy)
            return; // Another text is playing, so do nothing

        TextDisplayManager.Instance.SetBusy(true);
        isActive = true;
        if (questionText != null)
        {
            questionText.text = "";
            questionText.alpha = 1f;
            questionText.gameObject.SetActive(true);
        }
        ShowOptions();
    }

    void ShowOptions()
    {
        if (gestureDetector != null)
            gestureDetector.ClearGestures();

        if (yesText != null) {
            yesText.gameObject.SetActive(true);
            yesText.text = yesLabel;
            yesText.alpha = 1f;
        }
        if (noText != null) {
            noText.gameObject.SetActive(true);
            noText.text = noLabel;
            noText.alpha = 1f;
        }
        optionsVisible = true;

        // Show the question at the same time as the options
        if (questionText != null)
        {
            questionText.gameObject.SetActive(true);
            questionText.alpha = 1f;
            if (typewriterEffect != null)
            {
                typewriterEffect.textComponent = questionText;
                typewriterEffect.StartTypewriter(questionLine);
            }
            else
            {
                questionText.text = questionLine;
            }
        }
    }
    void ResetQuestionUI()
    {
        if (yesText != null) {
            yesText.alpha = 0f;
            yesText.text = "";
            yesText.gameObject.SetActive(false);
        }
        if (noText != null) {
            noText.alpha = 0f;
            noText.text = "";
            noText.gameObject.SetActive(false);
        }
        optionsVisible = false;
    }

    void SetOptionsAlpha(float alpha)
    {
        if (yesText != null) yesText.alpha = alpha;
        if (noText != null) noText.alpha = alpha;
    }
}