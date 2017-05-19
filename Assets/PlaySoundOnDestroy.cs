using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnDestroy : MonoBehaviour
{
    public GameObject SoundPrefab;
    public AudioClip Clip;

    private void OnDestroy()
    {
        GameObject soundGO = Instantiate<GameObject>(SoundPrefab);
        AudioSource audio = soundGO.GetComponent<AudioSource>();
        audio.clip = Clip;
        audio.Play();
        Destroy(soundGO, audio.clip.length);
    }
}
