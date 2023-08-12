using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioClips : MonoBehaviour
{
    [SerializeField] private AudioClip[] clip;

    public void PlayClip(int index)
    {
        AudioManager.Instance.PlaySound(clip[index]);
    }

    public void PlayRandomClip()
    {
        if(clip.Length == 0) Debug.Log(gameObject + " has no Audio Clips");
        int randIndex = Random.Range(0, clip.Length);
        AudioManager.Instance.PlaySound(clip[randIndex]);
    }
}
