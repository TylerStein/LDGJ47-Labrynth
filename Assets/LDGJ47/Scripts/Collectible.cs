using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Collectible : MonoBehaviour
{
    public bool isClone = false;
    public string playerTag = "Player";
    public ECollectible collectible;
    public GameStateController gameStateController;

    private void Start() {
        if (!gameStateController) gameStateController = FindObjectOfType<GameStateController>();
        if (isClone) {
            gameStateController.onCollected.AddListener(OnCollect);
        }
    }

    private void OnCollect() {
        if (gameStateController.HasCollected(collectible)) {
            gameStateController.onCollected.RemoveListener(OnCollect);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (isClone == false && other.tag == playerTag) {
            gameStateController.Collect(collectible);
            Destroy(gameObject);
        }
    }
}
