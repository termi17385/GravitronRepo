using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEditor;

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Paused : MonoBehaviour
{
    public float Mastertext => Mathf.Round((masterVal + 60) / 80 * 100);
    public float Musictext => Mathf.Round((musicVal + 60) / 80 * 100);
    public float SfxText => Mathf.Round((sfxVal + 60) / 80 * 100);
    
    [Header("Audio")] [SerializeField] private AudioMixer mixer;
    [SerializeField] private TextMeshProUGUI masterText, musicText, sfxText;
    [SerializeField] private Slider sMaster, sMusic, sSfx;

    [SerializeField] private GameObject menu;
    
    private float masterVal, musicVal, sfxVal;
    public static bool paused;

    private void Start()
    {
        menu.SetActive(false);
        LoadSettings();
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            
            PauseTheGame();
        }
    }

    public void PauseTheGame()
    {
        paused = !paused;
        if(paused)
        {
            menu.SetActive(true);
            LoadSettings();
            
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            menu.SetActive(false);
            SaveSettings();
            
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void SetUpSliders(Slider _slider, float _val) => _slider.value = _val; 
    private float ReturnMixerVal(string _mixer)
    {
        mixer.GetFloat(_mixer, out float vol);
        return vol;
    }
    
    /// <summary> Handles changing the music volume </summary>
    /// <param name="_vol">the parametre for<br/>changing the volume</param>
    public void MasterSlider(float _vol)
    {
        masterVal = _vol;
        mixer.SetFloat("Master", masterVal);
    }

    /// <summary> Handles changing the music volume </summary>
    /// <param name="_vol">the parametre for<br/>changing the volume</param>
    public void MusicSlider(float _vol)
    {
        musicVal = _vol;
        mixer.SetFloat("Music", musicVal);
    }

    /// <summary> Handles changing the SFX volume </summary>
    /// <param name="_vol">the parametre for<br/>changing the volume</param>
    public void SfxSlider(float _vol)
    {
        sfxVal = _vol; // stores the float for the slider val so i can be converted to text
        mixer.SetFloat("SFX", sfxVal);
    }
    
    /// <summary> Displays the volume
    /// amount as a string </summary>
    private void DisplayVolumeText()
    {
        masterText.text = $"Master: {Mastertext}/100";
        musicText.text = $"Music: {Musictext}/100";
        sfxText.text = $"SFX: {SfxText}/100";
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("Master", ReturnMixerVal("Master"));
        PlayerPrefs.SetFloat("Music", ReturnMixerVal("Music"));
        PlayerPrefs.SetFloat("SFX", ReturnMixerVal("SFX"));
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        float master = PlayerPrefs.GetFloat("Master");
        float music = PlayerPrefs.GetFloat("Music");
        float sfx = PlayerPrefs.GetFloat("SFX");

        SetUpSliders(sMaster, master);
        SetUpSliders(sMusic, music);
        SetUpSliders(sSfx, sfx);
        MasterSlider(master);
        MusicSlider(music);
        SfxSlider(sfx);
    }
    
    public void ChangeScene(string _scene)
    {
        SceneManager.LoadScene(_scene);
    }

    public void QuitGame()
    {
    #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
    #endif
        Application.Quit();
    }
}
