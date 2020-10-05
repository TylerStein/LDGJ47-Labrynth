using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public enum ECollectible
{
    A = 0,
    B = 1,
    C = 2,
    D = 3,
    E = 4,
    F = 5,
}

public class GameStateController : MonoBehaviour
{
    public DialogueController dialogueController;
    public List<ECollectible> collectedItems;

    public bool menuPaused = false;
    public bool IsPaused { get => menuPaused || dialogueController.hasDialogue; }

    public UnityEvent onMenuPause;
    public UnityEvent onMenuResume;

    public UnityEvent onCollected;
    public UnityEvent onCollectedAll;

    public int totalCollectibles = 6;

    public bool HasCollected(ECollectible collectible) {
        return collectedItems.Contains(collectible);
    }

    public void Collect(ECollectible collectible) {
        if (collectedItems.Contains(collectible) == false) {
            collectedItems.Add(collectible);
            onCollected.Invoke();
            if (collectedItems.Count >= totalCollectibles) {
                onCollectedAll.Invoke();
            }
        }
    }

    public void Start() {
        var dialogue = dialogueController.GetSequence("intro");
        dialogueController.BeginSequence(dialogue);
    }

    public void Update() {
        if (Input.GetButtonDown("Cancel")) {
            ToggleMenuPause();
        }
    }

    public void ToggleMenuPause() {
        if (menuPaused) {
            menuPaused = false;
            onMenuResume.Invoke();
        } else {
            menuPaused = true;
            onMenuPause.Invoke();
        }
    }
}
