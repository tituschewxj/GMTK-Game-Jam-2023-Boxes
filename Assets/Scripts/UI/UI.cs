using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is attached to the canvas for UI related actions
public class UI : MonoBehaviour
{
    public GameObject levelComplete;
    public GameObject loadingSublevel;
    // Start is called before the first frame update
    void Start()
    {
        if (levelComplete == null) {
            Debug.LogError("levelComplete GameObject not added!");
        }

        // Hide all transitions:
        levelComplete.SetActive(false);
        loadingSublevel.SetActive(false);
        loadingSublevel.transform.localScale = Vector3.zero;
    }

    public void LevelCompleteTransition(Action func) {
        // Slide in from the right.
        levelComplete.SetActive(true);

        Vector3 currentPos = levelComplete.transform.position;
        LeanTween.moveLocal(levelComplete, Vector3.zero, Constants.levelCompleteTransitionTime).
            setOnComplete(func);
    }

    public void LoadingSublevelTransition(Action func) {
        loadingSublevel.SetActive(true);
    
        Vector3 newPosition =  new(loadingSublevel.transform.localPosition.x, -215.9233f);
        // Bounce out from the center
        LeanTween.moveLocal(loadingSublevel, newPosition, Constants.levelCompleteTransitionTime);
        LeanTween.scale(loadingSublevel, Vector3.one, Constants.levelCompleteTransitionTime).
            setOnComplete(func);
    }
}
