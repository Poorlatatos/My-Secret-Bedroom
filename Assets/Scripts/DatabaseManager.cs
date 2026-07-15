/// Author : Keanen Lim
/// Date Created : 14/07/2026
/// Description : Handles Firebase authentication and database operations for players.

using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using Firebase.Auth;
using Firebase;
using System;
using System.Collections;

public class DatabaseManager : MonoBehaviour
{
    /// <summary>
    /// Input field for user email during sign up or sign in.
    /// </summary>
    public TMP_InputField EmailInput;

    /// <summary>
    /// Input field for user password during sign up or sign in.
    /// </summary>
    public TMP_InputField PasswordInput;

    /// <summary>
    /// Input field for username during sign up.
    /// </summary>
    public TMP_InputField UsernameInput;

    /// <summary>
    /// Canvas containing the login UI.
    /// </summary>
    public GameObject LoginCanvas;
    public GameObject RegisterCanvas;
    public GameObject MainMenuCanvas;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1.5f;

    private CanvasGroup emailCanvasGroup;
    private CanvasGroup passwordCanvasGroup;
    private CanvasGroup usernameCanvasGroup;

    private void Start()
    {
        // Get or add CanvasGroups to the input fields
        emailCanvasGroup = GetOrAddCanvasGroup(EmailInput.gameObject);
        passwordCanvasGroup = GetOrAddCanvasGroup(PasswordInput.gameObject);

        if (UsernameInput != null)
            usernameCanvasGroup = GetOrAddCanvasGroup(UsernameInput.gameObject);
    }

    /// <summary>
    /// Gets an existing CanvasGroup or adds one if missing.
    /// </summary>
    private CanvasGroup GetOrAddCanvasGroup(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();

        if (cg == null)
            cg = obj.AddComponent<CanvasGroup>();

        return cg;
    }

    public void SignOut()
    {
        FirebaseAuth.DefaultInstance.SignOut();
    }

    /// <summary>
    /// Gets the current timestamp in ISO 8601 format (UTC)
    /// </summary>
    private string GetCurrentTimestamp()
    {
        return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }

    /// <summary>
    /// Handles successful authentication.
    /// </summary>
    private void OnAuthenticationSuccess()
    {
        FadeInputFields();
        StartCoroutine(HideLoginCanvasAfterFade());
        StartCoroutine(HideRegisterCanvasAfterFade());
        StartCoroutine(HideMainMenuCanvasAfterFade());
    }

    /// <summary>
    /// Handles successful login.
    /// </summary>
    private void OnLoginSuccess()
    {
        FadeInputFields();
        StartCoroutine(HideLoginCanvasAfterFade());
        StartCoroutine(HideRegisterCanvasAfterFade());
        StartCoroutine(HideMainMenuCanvasAfterFade());
    }

    /// <summary>
    /// Handles successful registration.
    /// </summary>
    private void OnRegisterSuccess()
    {
        FadeInputFields();
        StartCoroutine(HideRegisterCanvasAfterFade());
        StartCoroutine(HideMainMenuCanvasAfterFade());
    }

    /// <summary>
    /// Fades all input fields.
    /// </summary>
    private void FadeInputFields()
    {
        StartCoroutine(FadeCanvasGroup(emailCanvasGroup));
        StartCoroutine(FadeCanvasGroup(passwordCanvasGroup));

        if (usernameCanvasGroup != null)
            StartCoroutine(FadeCanvasGroup(usernameCanvasGroup));
    }

    /// <summary>
    /// Smoothly fades a CanvasGroup.
    /// </summary>
    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup)
    {
        float timer = 0f;
        float startAlpha = canvasGroup.alpha;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, timer / fadeDuration);

            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Hides the login canvas after the fade finishes.
    /// </summary>
    private IEnumerator HideLoginCanvasAfterFade()
    {
        yield return new WaitForSeconds(fadeDuration);

        if (LoginCanvas != null)
        {
            LoginCanvas.SetActive(false);
            Debug.Log("Login canvas hidden.");
        }
    }

    /// <summary>
    /// Hides the register canvas after the fade finishes.
    /// </summary>
    private IEnumerator HideRegisterCanvasAfterFade()
    {
        yield return new WaitForSeconds(fadeDuration);

        if (RegisterCanvas != null)
        {
            RegisterCanvas.SetActive(false);
            Debug.Log("Register canvas hidden.");
        }
    }

    private IEnumerator HideMainMenuCanvasAfterFade()
    {
        yield return new WaitForSeconds(fadeDuration);

        if (MainMenuCanvas != null)
        {
            MainMenuCanvas.SetActive(true);
            Debug.Log("Main menu canvas unhidden.");
        }
    }

    /// <summary>
    /// Creates a new user with email and password,
    /// then uploads player data to Firebase.
    /// </summary>
    public void SignUp()
    {
        FirebaseAuth.DefaultInstance
            .CreateUserWithEmailAndPasswordAsync(EmailInput.text, PasswordInput.text)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("SignUp Error: " + task.Exception);
                    return;
                }

                FirebaseUser newUser = task.Result.User;
                string uid = newUser.UserId;

                Debug.Log($"User signed up successfully: {uid}");

                UserData userData = new UserData
                {
                    username = UsernameInput.text,
                    email = EmailInput.text,
                    createdAt = GetCurrentTimestamp()
                };

                string json = JsonUtility.ToJson(userData);

                DatabaseReference db = FirebaseDatabase.DefaultInstance.RootReference;

                db.Child("users").Child(uid).SetRawJsonValueAsync(json)
                    .ContinueWithOnMainThread(uploadTask =>
                    {
                        if (uploadTask.IsFaulted)
                        {
                            Debug.LogError("Upload failed: " + uploadTask.Exception);
                        }
                        else
                        {
                            Debug.Log("Player data uploaded successfully.");
                            OnAuthenticationSuccess();
                        }
                    });
            });
    }

    /// <summary>
    /// Signs in an existing user.
    /// </summary>
    public void SignIn()
    {
        FirebaseAuth.DefaultInstance
            .SignInWithEmailAndPasswordAsync(EmailInput.text, PasswordInput.text)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log("Can't sign in due to error!");
                    return;
                }

                FirebaseUser user = task.Result.User;

                Debug.Log($"User signed in successfully, id: {user.UserId}");

                OnAuthenticationSuccess();
            });
    }
}