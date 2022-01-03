using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader inst = null;

    void Awake() {
        // only allow one LevelLoader to exist at any time
        // & don't kill when reloading a Scene
        if (inst == null) {
            inst = this;
        } else if (inst != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void LoadLevel(string levelName) => SceneManager.LoadScene(levelName);
    public void ReturnToMainMenu() => SceneManager.LoadScene("MainMenu");

    public void ExitToDesktop() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
