using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Portal : MonoBehaviour
{
    public string playerTag = "Player";
    public bool isActivated = false;

    public MenuController menuController;
    public GameStateController gameStateController;

    // Start is called before the first frame update
    void Start()
    {
        if (!gameStateController) gameStateController = FindObjectOfType<GameStateController>();
        if (!menuController) menuController = FindObjectOfType<MenuController>();

        gameStateController.onCollectedAll.AddListener(OnCollectedAll);
    }

    void OnCollectedAll() {
        isActivated = true;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (isActivated == false) return;

        if (collision.tag == playerTag) {
            DialogueSequence sequence = gameStateController.dialogueController.GetSequence("end");
            gameStateController.dialogueController.BeginSequence(sequence);

            gameStateController.dialogueController.onCloseDialogue.AddListener(() => {
                // is this what you call an ending?
                menuController.LoadScene(1);
            });
        }
    }
}
