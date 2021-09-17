using System;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gravitron.Settings
{
	public class PlatformUtil : MonoBehaviour
	{
		[SerializeField] private GameObject displayButton, displayMenu;
		
		public static bool mobileMode;

		private void Awake()
		{
			mobileMode = Application.platform == RuntimePlatform.Android;
			if(mobileMode)
			{
				displayMenu.SetActive(false);
				displayButton.SetActive(false);
			}
		} 
			
	}
}