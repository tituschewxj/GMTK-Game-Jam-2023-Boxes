using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistBetweenScenes : MonoBehaviour
{
    private static GameObject audioSource = null;
    private void Awake() {
        if (audioSource != null && audioSource != this) {
            Destroy(gameObject);
            return;
        } else {
            audioSource = gameObject;
        }
        DontDestroyOnLoad(gameObject);
    }
}
