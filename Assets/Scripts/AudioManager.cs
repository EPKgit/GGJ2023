using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoSingleton<AudioManager>
{
    public AudioSource pickupAudio;
    public AudioSource dropAudio;
    public AudioSource growAudio;
    public AudioSource dieAudio;
    public AudioSource growSuccessAudio;
    public AudioSource hoverAudio;
    public AudioSource cannotAudio;
    public AudioSource harvestAudio;
}