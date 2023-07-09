using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is added into door boxes, to toggle their sprites
public class Door : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    [SerializeField]
    Sprite openSprite;
    [SerializeField]
    Sprite closedSprite;
    Box box;
    AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        if (!gameObject.TryGetComponent(out spriteRenderer)) {
            Debug.LogError("No sprite renderer found for door.");
        }
        if (!gameObject.TryGetComponent(out box)) {
            Debug.LogError("No Box found for the Door!");
        }
        box.doorManager = this;

        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null) {
            Debug.LogWarning("No audio manager found!");
        }
    }

    public void OpenDoorTransition() {
        spriteRenderer.sprite = openSprite;

        // Play sound
        audioManager.PlayDooropen();
    }

    public void CloseDoorTransition() {
        spriteRenderer.sprite = closedSprite;

        // Play sound
        audioManager.PlayDoorclose();
    }
}
