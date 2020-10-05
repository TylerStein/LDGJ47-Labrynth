using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PortalGem : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public bool isActivated = false;
    public ECollectible collectible;
    public GameStateController gameStateController;

    // Start is called before the first frame update
    void Start() {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        if (!gameStateController) gameStateController = FindObjectOfType<GameStateController>();
        gameStateController.onCollected.AddListener(OnCollected);
    }

    // Update is called once per frame
    void OnCollected() {
        if (isActivated == false) {
            if (gameStateController.HasCollected(collectible)) {
                Activate();
            }
        }
    }

    void Activate() {
        gameStateController.onCollected.RemoveListener(OnCollected);
        spriteRenderer.enabled = true;
    }
}
