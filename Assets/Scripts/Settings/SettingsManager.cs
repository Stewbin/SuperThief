using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{


    public AudioMixer audioMixer; 
    public void SetVolume( float volume)
    {

        audioMixer.SetFloat("Volume", volume); 
        UnityEngine.Debug.Log("The volume is " + volume); 
    }
}
