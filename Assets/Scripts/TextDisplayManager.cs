using UnityEngine;

public class TextDisplayManager : MonoBehaviour
{
    public static TextDisplayManager Instance { get; private set; }
    public bool IsBusy { get; private set; } = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void SetBusy(bool busy)
    {
        IsBusy = busy;
    }
}