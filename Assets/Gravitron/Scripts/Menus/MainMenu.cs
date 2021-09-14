using Gravitron.Settings;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	private void Start()
	{
		if(PlatformUtil.mobileMode)
		{
			Screen.SetResolution(1920, 1080, true);
		}
	}

	public void ChangeScene(string _scene)
	{
		SceneManager.LoadScene(_scene);
	}
}
