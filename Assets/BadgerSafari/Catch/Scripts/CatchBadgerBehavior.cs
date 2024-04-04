using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles badger behavior in the catch scene
/// </summary>
public class CatchBadgerBehavior : MonoBehaviour {
    public AudioClip audioChurr;
    public AudioClip audioCall;
    private AudioSource audioSource;
    private float minAudioTime = 5.0f;
    private float maxAudioTime = 10.0f;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayAudio());
    }

    private IEnumerator PlayAudio() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(minAudioTime, maxAudioTime));

            // choose between two audio types
            if (Random.Range(0, 2) == 0) {
                audioSource.PlayOneShot(audioChurr);
            } else {
                audioSource.PlayOneShot(audioCall);
            }
        }
    }
}
