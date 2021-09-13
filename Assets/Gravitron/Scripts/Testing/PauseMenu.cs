using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	public void ReloadScene() => SceneManager.LoadScene("SampleScene");
	public void Quit() => Application.Quit();
}
