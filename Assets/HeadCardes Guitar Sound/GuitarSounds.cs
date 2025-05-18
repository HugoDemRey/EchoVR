using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoundScript
{
    // Main class for playing guitar sounds
    public class GuitarSounds : MonoBehaviour
    {
        // Array of AudioSources for playing sounds
        public AudioSource[] audioList = new AudioSource[6];

                public void PlaySound(int index)
        {
            if (index >= 0 && index < audioList.Length && audioList[index] != null)
            {
                audioList[index].Play();
            }
            else
            {
                Debug.LogWarning("Invalid sound index or unassigned AudioSource.");
            }
        }

        public void Stopaudio()
    {
        foreach (var i in audioList)
        {
            i.Stop();
        }
    }
    }

}