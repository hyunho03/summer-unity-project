using UnityEngine;
using System.Collections;

public class BGMManager : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;        // 🔊 실제 오디오 재생기

    [Header("BGM Clips")]
    public AudioClip normalBGM;            
    [Range(0f, 1f)] public float normalBGMVolume = 0.8f;  

    public AudioClip bossBGM;              
    [Range(0f, 1f)] public float bossBGMVolume = 1f;      

    public AudioClip winBGM;
    [Range(0f, 1f)] public float winBGMVolume = 1f;  
    public AudioClip gameoverBGM;
    [Range(0f, 1f)] public float gameoverBGMVolume = 1f;     

    [Header("Fade Settings")]
    public float fadeDuration = 1.5f;      

    void Start()
    {
        PlayNormalBGM();
    }

    public void PlayNormalBGM()
    {
        StartCoroutine(FadeToClip(normalBGM, normalBGMVolume, true)); // 반복
    }
    public void PlayGameOverBGM()
    {
        StartCoroutine(FadeToClip(gameoverBGM, gameoverBGMVolume, false)); // 반복x
    }

    public void PlayBossBGM()
    {
        StartCoroutine(FadeToClip(bossBGM, bossBGMVolume, true)); // 반복
    }

    public void PlayWinBGM()
    {
        StartCoroutine(FadeToClip(winBGM, winBGMVolume, false)); // ❌ 반복 안 함
    }

    private IEnumerator FadeToClip(AudioClip newClip, float targetVolume, bool loop)
    {
        if (audioSource.clip == newClip) yield break;

        float startVolume = audioSource.volume;

        // 🔻 1. 페이드아웃
        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = 0f;

        // 🔻 2. 클립 교체 + 루프 설정
        audioSource.clip = newClip;
        audioSource.loop = loop; // 🎵 여기서 곡마다 루프 여부 결정
        audioSource.Play();

        // 🔻 3. 페이드인
        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            audioSource.volume = Mathf.Lerp(0f, targetVolume, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = targetVolume;
    }
}
