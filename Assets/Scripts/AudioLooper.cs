using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


// This script is curtsy of ChatGPT (with some modifications to get rid of "saumode" things)
// Also, major edits because that script did not what it was supposed to do. It switched tracks every crossFadeTime instead of crossfading at the end of the track.
public class AudioLooper : MonoBehaviour
{
    [SerializeField] private List<AudioClip> audioClips;
    [SerializeField] private float crossFadeTime = 5.0f;
    [SerializeField] private float volume = 0.7f;

    private AudioSource _audioSourceA;
    private AudioSource _audioSourceB;

    void Start()
    {
        _audioSourceA = gameObject.AddComponent<AudioSource>();
        _audioSourceB = gameObject.AddComponent<AudioSource>();
        _audioSourceA.volume = volume;
        _audioSourceB.volume = volume;

        StartCoroutine(PlayRandomAudio());
    }


    IEnumerator PlayRandomAudio()
    {
        while (true)
        {
            // Shuffle the list
            Shuffle(audioClips);

            foreach (AudioClip clip in audioClips)
            {
                // Assign clip to audioSourceA and start playing
                _audioSourceA.clip = clip;
                _audioSourceA.Play();

                // Crossfade from audioSourceB to audioSourceA
                float startTime = Time.unscaledTime;
                while (Time.unscaledTime < startTime + clip.length - crossFadeTime)
                {
                    float t = (Time.unscaledTime - startTime) / crossFadeTime;
                    _audioSourceA.volume = Mathf.Lerp(0f, volume, t);
                    _audioSourceB.volume = Mathf.Lerp(volume, 0f, t);
                    yield return null;
                }

                // Stop and clear audioSourceB
                _audioSourceB.Stop();
                _audioSourceB.clip = null;

                // Swap audioSourceA and audioSourceB references
                (_audioSourceA, _audioSourceB) = (_audioSourceB, _audioSourceA);
            }

            yield return null;
        }
    }

    // Shuffle the list
    private void Shuffle(List<AudioClip> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
}