using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AudioClip[] playermove, boxmove, boxswitch, dooropen, doorclose, buttonpress, cannot, sublevel, goal;
    [SerializeField]
    AudioSource[] audioSources;
    int currentAudioSourceIndex = 0;
    void PlayRandomClip(AudioClip[] clips) {
        if (audioSources[currentAudioSourceIndex].isPlaying) {
            audioSources[currentAudioSourceIndex].Stop();
        }
        audioSources[currentAudioSourceIndex].PlayOneShot(clips[Random.Range(0, clips.Length)]);
        currentAudioSourceIndex = (currentAudioSourceIndex + 1) % audioSources.Length;
    }
    public void PlayPlayermove() {
        PlayRandomClip(playermove);
    }
    public void PlayBoxmove() {
        PlayRandomClip(boxmove);
    }
    public void PlayBoxswitch() {
        PlayRandomClip(boxswitch);
    }
    public void PlayDooropen() {
        PlayRandomClip(dooropen);
    }
    public void PlayDoorclose() {
        PlayRandomClip(doorclose);
    }
    public void PlayButtonpress() {
        PlayRandomClip(buttonpress);
    }
    public void PlayCannot() {
        PlayRandomClip(cannot);
    }
    public void PlaySublevel() {
        PlayRandomClip(sublevel);
    }
    public void PlayGoal() {
        PlayRandomClip(goal);
    }
}
