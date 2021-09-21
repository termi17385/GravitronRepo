using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JukeBox : MonoBehaviour
{
    [SerializeField] private AudioSource mainMusic, pauseMusic; 
    [SerializeField] private List<AudioClip> clips = new List<AudioClip>();
    
    // Update is called once per frame
    void Update()
    {
        if(Paused.paused)
        {
            mainMusic.Pause();
            pauseMusic.UnPause();
        }
        else
        {
            mainMusic.UnPause();
            pauseMusic.Pause();
        }
    }
}
