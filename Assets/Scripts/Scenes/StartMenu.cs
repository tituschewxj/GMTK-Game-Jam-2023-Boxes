using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    SceneLoader sceneLoader;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        sceneLoader = gameObject.GetComponent<SceneLoader>();
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown) {
            audioSource.Play();
            StartCoroutine(sceneLoader.LoadSublevelIndex(0));            
        }
    }
}
