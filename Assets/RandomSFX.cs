using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSFX : MonoBehaviour
{
    AudioSource audioSource;
    public List<AudioClip> audioClips;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRandom()
    {
        if (audioClips.Count != 0)
        {
            int random = Random.Range(0, audioClips.Count);
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
            audioSource.PlayOneShot(audioClips[random]);
        }
    }
}
