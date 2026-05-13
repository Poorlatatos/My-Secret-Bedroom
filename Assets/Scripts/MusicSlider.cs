using UnityEngine;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour
{
    public AudioSource audioSource; 
    public Slider musicSlider;

    void Start()
    {
        if (musicSlider != null)
        {
            musicSlider.minValue = 0f;
            musicSlider.maxValue = 1f;
            musicSlider.value = 0f;
        }
    }

    void Update()
    {
        if (audioSource != null && musicSlider != null && audioSource.clip != null && audioSource.isPlaying)
        {
            float progress = audioSource.time / audioSource.clip.length;
            musicSlider.value = progress;
        }
    }
}