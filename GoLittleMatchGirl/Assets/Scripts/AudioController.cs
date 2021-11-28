using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip[] audioClip;

    public enum AUDIO { 
        MAIN, INGAME, GAMEOVER, GAMECLEAR, BADEND, HAPPYEND, 
        BUTTON, JUMP, WINDOW, ATTACK, POTION, OBSTACLE, SNOW }

    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioClip = new AudioClip[13];
        audioClip = Resources.LoadAll<AudioClip>("Audio");
    }

    private void OnEnable()
    {
        StartCoroutine(AudioCrt());
    }

    public void Play(AUDIO audio)
    {
        if(audioSource.isPlaying) audioSource.Stop();
        audioSource.clip = audioClip[(int)audio];
        audioSource.Play();
    }

    private IEnumerator AudioCrt()
    {
        yield return new WaitForSeconds(1f);
        Play(AUDIO.MAIN);
    }
}
