using UnityEngine;
using TMPro;
using System.Collections;

public class TypewriterEffect : MonoBehaviour
{
    public TMP_Text textComponent; // Assign in Inspector
    public float typeSpeed = 0.05f; // Seconds per character

    private Coroutine typingCoroutine;

    public void StartTypewriter(string text)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(text));
    }

    IEnumerator TypeText(string text)
    {
        textComponent.text = "";
        foreach (char c in text)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    public void Clear()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        textComponent.text = "";
    }
}