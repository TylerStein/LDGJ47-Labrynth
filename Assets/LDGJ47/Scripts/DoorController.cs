using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    bool isOpen = false;
    public ECollectible collectible;
    public GameStateController gameStateController;

    // Start is called before the first frame update
    void Start()
    {
        if (!gameStateController) gameStateController = FindObjectOfType<GameStateController>();
        gameStateController.onCollected.AddListener(OnCollected);
    }

    // Update is called once per frame
    void OnCollected() {
        if (isOpen == false) {
            if (gameStateController.HasCollected(collectible)) {
                Open();
            }
        }
    }

    void Open() {
        gameStateController.onCollected.RemoveListener(OnCollected);
        gameObject.SetActive(false);
    }
}
