using UnityEngine;
using TMPro;
using System.Collections;

public class QuestionToAnswerScript : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text questionText;
    public TMP_Text yesText;
    public TMP_Text noText;

    [Header("Question")]
    [TextArea]
    public string questionLine = "Do you want to continue?";

    [Header("Head Gesture Detector")]
    public HeadGestureDetector gestureDetector;

    [Header("Typewriter Effect")]
    public TypewriterEffect typewriterEffect;

    [Header("Option Labels")]
    public string yesLabel = "Yes";
    public string noLabel = "No";
    private bool optionsVisible = false;
    private bool fading = false;
    public ClothingLaundry clothingLaundry;
    void Start()
    {
        // Only hide Yes/No and clear question at start
        SetOptionsAlpha(0f);
        if (questionText != null)
            questionText.text = "";
    }

    void Update()
    {
        if (!optionsVisible || fading || gestureDetector == null)
            return;

        if (gestureDetector.DidNod())
        {
            StartCoroutine(FadeOutOptions());
        }
        else if (gestureDetector.DidShake())
        {
            StartCoroutine(FadeOutOptions());
        }
    }

    void ShowOptions()
    {
        if (gestureDetector != null)
            gestureDetector.ClearGestures();

        if (yesText != null) {
            yesText.text = yesLabel;
            yesText.alpha = 1f;
        }
        if (noText != null) {
            noText.text = noLabel;
            noText.alpha = 1f;
        }
        optionsVisible = true;
        fading = false;

        if (clothingLaundry != null)
            clothingLaundry.EnableLaundryInteraction();
    }

    public void BeginQuestion()
    {
        SetOptionsAlpha(0f);

        if (typewriterEffect != null && questionText != null)
        {
            questionText.gameObject.SetActive(true);
            typewriterEffect.textComponent = questionText;
            typewriterEffect.OnTypingComplete += ShowOptions;
            typewriterEffect.StartTypewriter(questionLine);
        }
        else if (questionText != null)
        {
            questionText.text = questionLine;
            ShowOptions();
        }
    }

    void SetOptionsAlpha(float alpha)
    {
        if (yesText != null) yesText.alpha = alpha;
        if (noText != null) noText.alpha = alpha;
    }
    void ResetQuestionUI()
    {
        if (questionText != null)
            questionText.text = "";
        SetOptionsAlpha(0f);
        optionsVisible = false;
        fading = false;
    }
    IEnumerator FadeOutOptions()
    {
        fading = true;
        float duration = 1.0f;
        float elapsed = 0f;
        float startAlpha = 1f;

        // Hide the question text immediately
        if (questionText != null)
            questionText.text = "";
            questionText.gameObject.SetActive(false);

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            if (yesText != null) yesText.alpha = alpha;
            if (noText != null) noText.alpha = alpha;
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (yesText != null) yesText.alpha = 0f;
        if (noText != null) noText.alpha = 0f;
        optionsVisible = false;
        ResetQuestionUI();
    }
}