using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelLoader : MonoBehaviour
{
    public void LoadLevel(string levelName) {
        SceneManager.LoadScene(levelName);
    }

    public void ReturnToMainMenu() {
        CampaignBootstrapper cb = GetComponent<CampaignBootstrapper>();
        if (cb != null) cb.DestroyCampaign();
        //
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitToDesktop() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    //
    public IEnumerator LoadLevelAsync(string levelName) {
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
        yield return new WaitUntil(() => asyncOp.isDone);

        // now that you've loaded, return next frame
        yield return new WaitForEndOfFrame();
    }
}
