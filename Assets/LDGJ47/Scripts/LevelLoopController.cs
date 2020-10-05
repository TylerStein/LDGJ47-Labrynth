using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoopController : MonoBehaviour
{
    public Transform loopSubject;
    public LevelGeo levelRoot;
    public LevelGeo levelClone;
    public Transform importantObjects;
    public Transform importantObjectsClone;
    public CameraController cameraController;

    bool isOnMainLevel;
    Transform cloneEdgeStart;
    Transform cloneEdgeEnd;
    Transform levelEdgeStart;
    Transform levelEdgeEnd;

    public float buffer = 1.2f;
    float levelWidth;

    // Start is called before the first frame update
    void Start()
    {
        levelEdgeStart = levelRoot.start;
        levelEdgeEnd = levelRoot.end;

        GameObject levelRootClone = Instantiate(levelRoot.gameObject);
        levelClone = levelRootClone.GetComponent<LevelGeo>();
        cloneEdgeStart = levelClone.start;
        cloneEdgeEnd = levelClone.end;

        levelWidth = levelEdgeEnd.position.x - levelEdgeStart.position.x;

        CalculateMain();
        UpdatePositions();
    }

    // Update is called once per frame
    void FixedUpdate() {
        OffsetLevel();
        CalculateMain();
        UpdatePositions();
    }

    void OffsetLevel() {
        float subjectX = loopSubject.position.x;
        Vector3 offset = Vector3.zero;
        if (isOnMainLevel) {
            if (subjectX > levelEdgeEnd.position.x - buffer) {
                // Exit on Right side of Main level
                offset = Vector3.right * levelWidth * -1f;
            } else if (subjectX < levelEdgeStart.position.x + buffer) {
                // Exit on Left side of Main Level
                offset = Vector3.right * levelWidth * 1f;
            }
        } else {
            if (subjectX > cloneEdgeEnd.position.x - buffer) {
                // Exit on Right side of Clone level
                offset = Vector3.right * levelWidth * -1f;
            } else if (subjectX < cloneEdgeEnd.position.x + buffer) {
                // Exit on Left side of Clone level
                offset = Vector3.right * levelWidth * 1f;
            }
        }

        if (offset.magnitude > 0f) {
            OffsetAll(offset);
        }
    }
    
    void OffsetAll(Vector3 offset) {
        // levelRoot.transform.position += offset;
        // levelClone.transform.position += offset;
        loopSubject.position += offset;
        cameraController.transform.position += offset;
        // cameraController.OffsetPosition(offset);
    }

    void CalculateMain() {
        float subjectX = loopSubject.position.x;
        if (subjectX > levelEdgeStart.position.x && subjectX < levelEdgeEnd.position.x) {
            isOnMainLevel = true;
        } else if (subjectX > cloneEdgeStart.position.x && subjectX < cloneEdgeEnd.position.x) {
            isOnMainLevel = false;
        }
    }

    void UpdatePositions() {
        float subjectX = loopSubject.position.x;
        if (isOnMainLevel) {
            if (subjectX > levelRoot.transform.position.x) {
                Vector3 offset = levelClone.transform.position - cloneEdgeStart.position;
                levelClone.transform.position = levelEdgeEnd.position + offset;
            } else {
                Vector3 offset = levelClone.transform.position - cloneEdgeEnd.position;
                levelClone.transform.position = levelEdgeStart.position + offset;
            }

            importantObjects.position = levelRoot.transform.position;
            importantObjectsClone.position = levelClone.transform.position;
        } else {
            if (subjectX > levelClone.transform.position.x) {
                Vector3 offset = levelRoot.transform.position - levelEdgeStart.position;
                levelRoot.transform.position = cloneEdgeEnd.position + offset;
            } else {
                Vector3 offset = levelRoot.transform.position - levelEdgeEnd.position;
                levelRoot.transform.position = cloneEdgeStart.position + offset;
            }

            importantObjects.position = levelClone.transform.position;
            importantObjectsClone.position = levelRoot.transform.position;
        }
    }

    private void OnDrawGizmos() {
        if (levelEdgeStart && levelEdgeEnd) {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(levelEdgeStart.position + (Vector3.right * buffer), levelEdgeStart.position + (Vector3.right * buffer) + (Vector3.up * 5));
            Gizmos.DrawLine(levelEdgeEnd.position - (Vector3.right * buffer), levelEdgeEnd.position - (Vector3.right * buffer) + (Vector3.up * 5));

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(levelEdgeStart.position, levelEdgeStart.position + (Vector3.up * 5));
            Gizmos.DrawLine(levelEdgeEnd.position, levelEdgeEnd.position + (Vector3.up * 5));
        }

        if (cloneEdgeStart && cloneEdgeEnd) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(cloneEdgeStart.position + (Vector3.right * buffer), cloneEdgeStart.position + (Vector3.right * buffer) + (Vector3.up * 5));
            Gizmos.DrawLine(cloneEdgeEnd.position - (Vector3.right * buffer), cloneEdgeEnd.position - (Vector3.right * buffer) + (Vector3.up * 5));

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(cloneEdgeStart.position, cloneEdgeStart.position + (Vector3.up * 5));
            Gizmos.DrawLine(cloneEdgeEnd.position, cloneEdgeEnd.position + (Vector3.up * 5));
        }
    }
}
