using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip[] audioClip = new AudioClip[12];
    //private AudioClip[] audioClip;

    public enum AUDIO { 
        MAIN, INGAME, GAMEOVER, GAMECLEAR, BADEND, HAPPYEND, 
        BUTTON, JUMP, WINDOW, ARM, POTION, OBSTACLE, SNOW, SNOW2 }

    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        //audioClip = new AudioClip[13];
        //audioClip = Resources.LoadAll<AudioClip>("Audio");
    }

    public void Play(AUDIO audio)
    {
        if(audioSource.isPlaying) audioSource.Stop();
        audioSource.clip = audioClip[(int)audio];
        audioSource.Play();
    }

    public void PlayAnother(AUDIO audio)
    {
        audioSource.PlayOneShot(audioClip[(int)audio]);
    }

    public void Resume()
    {
        audioSource.Play();
    }

    public void Pause()
    {
        audioSource.Pause();
    }

    public void Stop()
    {
        audioSource.Stop();
    }
}
