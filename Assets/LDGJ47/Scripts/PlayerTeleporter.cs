using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TeleportKey
{
    public KeyCode keyCode;
    public Transform destination;
}

public class PlayerTeleporter : MonoBehaviour
{
    public PlayerController player;
    public List<TeleportKey> teleportKeys;

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
    foreach (TeleportKey key in teleportKeys) {
            if (Input.GetKeyDown(key.keyCode)) {
                player.TeleportTo(key.destination.position);
                return;
            }
    }
#endif
    }
}
