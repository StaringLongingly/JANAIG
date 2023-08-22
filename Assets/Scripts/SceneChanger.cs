using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [Header("Scene Name")]
    public string sceneName;

    public IEnumerator SwitchScene()
    {
        Debug.Log("Preparing to load Scene: " + sceneName);
        yield return new WaitForSeconds(0.1f);
        if (sceneName == "Exit") { Application.Quit(); }
        else { SceneManager.LoadScene(sceneName,  LoadSceneMode.Single); }
    }
}
