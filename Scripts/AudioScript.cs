using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioScript : MonoBehaviour
{

    public AudioClip BackgroundMusic;
    public AudioClip hoverSound;
    public AudioClip takeDamage;
    public AudioClip dealDamage;

    AudioSource MusicSource;
    AudioSource SoundSource;



    void Start()
    {
        AudioSource[] audios = GetComponents<AudioSource>(); //Initialize 2 audiosources, once for music and one for sound effects.
        MusicSource = audios[0];
        SoundSource = audios[1];

        MusicSource.clip = BackgroundMusic;
        MusicSource.Play();
    }

    //Create functions that will be called when sound is needed.


    public void HoverSound()
    {
        SoundSource.clip = hoverSound;
        SoundSource.Play();
    }

    public void TDamageSound() // TDamage = Take damage
    {
        SoundSource.clip = takeDamage;
        SoundSource.Play();
    }

    public void DDamageSound() // DDamage = Deal damage
    {
        SoundSource.clip = dealDamage;
        SoundSource.Play();
    }


}
