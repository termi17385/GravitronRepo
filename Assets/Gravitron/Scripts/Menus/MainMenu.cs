using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;
using System;
using TMPro;

using static System.Boolean;

namespace Gravitron.Settings
{
	public class MainMenu : MonoBehaviour
	{
		public float Mastertext => Mathf.Round((masterVal + 80) / 120 * 100);
		public float Musictext => Mathf.Round((musicVal + 80) / 100 * 100);
		public float SfxText => Mathf.Round((sfxVal + 80) / 100 * 100);

		[Header("Display")] [SerializeField] private List<string> options = new List<string>();
		[SerializeField] private TMP_Dropdown resDown;
		[SerializeField] private bool fullscreen;
		[SerializeField] private Toggle fullscreenToggle;
		private Resolution[] resolutionArray;
		private int resolutionIndex;

		[Header("Graphics")] [SerializeField] private List<string> graphicalNames = new List<string>();
		[SerializeField] private int graphicsIndex;
		[SerializeField] private TMP_Dropdown grapDown;

		[Header("Audio")] [SerializeField] private AudioMixer mixer;
		[SerializeField] private TextMeshProUGUI masterText, musicText, sfxText;
		[SerializeField] private Slider sMaster, sMusic, sSfx;
		private float masterVal, musicVal, sfxVal;

		private void SetUpSliders(Slider _slider, float _val) => _slider.value = _val;

		private float ReturnMixerVal(string _mixer)
		{
			mixer.GetFloat(_mixer, out float vol);
			return vol;
		}

		private void Start()
		{
			fullscreen = true;
			Time.timeScale = 1f;
			GetResolutions();
			GetGraphics();
			LoadSettings();
			if(PlatformUtil.mobileMode) Screen.SetResolution(1920, 1080, true);
		}

		private void LateUpdate() => DisplayVolumeText();

		/// <summary>
		/// Handles the set up of the resolutions drop down
		/// making sure that it is filled with the correct values
		/// </summary>
		public void GetResolutions()
		{
			options.Clear(); // makes sure the list is cleared beforehand
			// ReSharper disable once CollectionNeverQueried.Local
			List<Resolution> resolutions = new List<Resolution>(); // a list for storing the resolutions
			foreach(Resolution res in Screen.resolutions)
			{
				// checks if the refreshrate is 60 then adds resolution to list
				if(res.refreshRate == 60.0f || res.refreshRate == 120.0f)
					resolutions.Add(res);
			}

			//stores the resolutions in an array and filters out the duplicates
			resolutionArray = Screen.resolutions.Select(_resolutions => new Resolution { width = _resolutions.width, height = _resolutions.height }).Distinct().ToArray();

			// loop to assign all resolutions to a string 
			// set the current resolution
			for(int i = 0; i < resolutionArray.Length; i++)
			{
				// formats the resolution into a user friendly format then adds that the the list of options
				string option = resolutionArray[i].width + "x" + resolutionArray[i].height;
				options.Add(option);

				// checks to see if we are at the current resolution
				if(resolutionArray[i].width == Screen.currentResolution.width && resolutionArray[i].height == Screen.currentResolution.height)
				{
					// then sets the index to that value
					resolutionIndex = i;
				}
			}

			resDown.ClearOptions();
			resDown.AddOptions(options);
		}

		/// <summary> Sets the resolution based on the index given </summary>
		/// <param name="_resolutionIndex">A value for changing the resolution</param>
		public void SetResolution(int _resolutionIndex)
		{
			Resolution res = resolutionArray[resolutionIndex = _resolutionIndex];
			Screen.SetResolution(res.width, res.height, fullscreen);
		}

		/// <summary> sets whether the game
		/// is in fullscreen or not </summary>
		public void SetFullScreen(bool _fullScreen)
		{
			Screen.fullScreen = (fullscreen = _fullScreen);
		}

		private void GetGraphics()
		{
			string[] names = QualitySettings.names; // assigns the names to an array of string
			graphicalNames.Clear();                 // makes sure the list is clean
			foreach(string item in names)
				graphicalNames.Add(item); // gets all the names from the array and converts to list

			grapDown.ClearOptions();             // makes sure the drop down is cleaned first
			grapDown.AddOptions(graphicalNames); // assigns the names to the dropDown
		}

		/// <summary> Sets the graphics settings of the game </summary>
		/// <param name="_i">the index</param>
		public void SetGraphics(int _i)
		{
			QualitySettings.SetQualityLevel((graphicsIndex = _i), true);
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
			var screenData = $"{fullscreen}".ToLower();
			PlayerPrefs.SetInt("resolution", resolutionIndex);
			PlayerPrefs.SetString("fullscreen", screenData);
			PlayerPrefs.SetInt("quality", graphicsIndex);						
			PlayerPrefs.SetFloat("Master", ReturnMixerVal("Master"));
			PlayerPrefs.SetFloat("Music", ReturnMixerVal("Music"));
			PlayerPrefs.SetFloat("SFX", ReturnMixerVal("SFX"));
			PlayerPrefs.Save();
		}

		public void LoadSettings()
		{
			string screenData = PlayerPrefs.GetString("fullscreen");
			int fIndex = PlayerPrefs.GetInt("resolution");
			float master = PlayerPrefs.GetFloat("Master");
			float music = PlayerPrefs.GetFloat("Music");
			int gIndex = PlayerPrefs.GetInt("quality");
			float sfx = PlayerPrefs.GetFloat("SFX");

			TryParse(screenData, out bool fullscreenData);
			SetFullScreen(fullscreenToggle.isOn = fullscreenData);
			SetResolution(resDown.value = fIndex);
			SetGraphics(grapDown.value = gIndex);
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
	}
}