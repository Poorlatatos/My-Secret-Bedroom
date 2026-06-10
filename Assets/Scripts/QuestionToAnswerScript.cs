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

    public GameObject backgroundUI;
    void Start()
    {
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

        if (yesText != null)
        {
            yesText.gameObject.SetActive(true);
            yesText.text = yesLabel;
            yesText.alpha = 1f;
        }
        if (noText != null)
        {
            noText.gameObject.SetActive(true);
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
        if (TextDisplayManager.Instance != null && TextDisplayManager.Instance.IsBusy)
            return;

        if (TextDisplayManager.Instance != null)
            TextDisplayManager.Instance.SetBusy(true);

        if (backgroundUI != null)
            backgroundUI.SetActive(true);

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
        {
            questionText.text = "";
        }
        if (yesText != null)
        {
            yesText.alpha = 0f;
            yesText.text = "";
            yesText.gameObject.SetActive(false);
        }
        if (noText != null)
        {
            noText.alpha = 0f;
            noText.text = "";
            noText.gameObject.SetActive(false);
        }
        if (backgroundUI != null)
        {
            backgroundUI.SetActive(false);
        }
        optionsVisible = false;
        fading = false;
    }

    IEnumerator FadeOutOptions()
    {
        fading = true;
        float duration = 1.0f;
        float elapsed = 0f;
        float startAlpha = 1f;

        // Clear the question text immediately
        if (questionText != null)
            questionText.text = "";

        // Fade out the yes/no options only
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
        fading = false;

        yield return new WaitForSeconds(0.5f);

        ResetQuestionUI();

        if (questionText != null)
        {
            questionText.text = "";
        }
        if (TextDisplayManager.Instance != null)
            TextDisplayManager.Instance.SetBusy(false);
    }
}