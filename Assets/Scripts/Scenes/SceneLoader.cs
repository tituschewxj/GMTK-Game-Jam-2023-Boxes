using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    string previousSceneName;
    [SerializeField]
    string[] sublevelNames;
    public IEnumerator LoadPreviousScene() {
        yield return new WaitForSeconds(Constants.levelCompleteNextLevelLoadTime);

        if (previousSceneName == "") {
            Debug.LogWarning("previousScene not set! Defaulting to level selection.");
            LoadDefaultScene();
        } else {
            SceneManager.LoadSceneAsync(previousSceneName);
        }
    
    }
    public IEnumerator LoadSublevelIndex(int index) {
        if (index == -1) {
            Debug.LogWarning("otherProperty of sublevel box not set! Defaulting to 0.");
            index = 0;
        }
        if (sublevelNames.Length == 0) {
            Debug.LogError("No sublevels set in sublevelNames!");
        }

        yield return new WaitForSeconds(Constants.levelCompleteNextLevelLoadTime);

        if (sublevelNames[index] == "") {
            Debug.LogWarning("previousScene not set! Defaulting to level selection.");
            LoadDefaultScene();
        } else {
            SceneManager.LoadSceneAsync(sublevelNames[index]);
        }
    }
    void LoadDefaultScene() {
        SceneManager.LoadSceneAsync("LevelSelection");
    }
}
