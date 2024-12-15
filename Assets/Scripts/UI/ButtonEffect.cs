using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonEffect : MonoBehaviour
{
    public void OnResume()
    {
        PauseUI.Instance.Unshow();

    }
	public void OnButtonClick()
    {
        PlayerPrefs.DeleteAll();
        EventCenter.Instance.TriggerEvent("ClearRecord");
    }

    public void OnQuitGame()
    {
        Application.Quit();
    }

    public void OnBackToMenu()
    {
        Time.timeScale=1;
        SceneManager.LoadScene("Initial");
    }

    public void OnReStart()
    {
        Time.timeScale=1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
