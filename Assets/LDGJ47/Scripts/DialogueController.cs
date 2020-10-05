using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using TMPro;

[System.Serializable]
public class DialogueFile
{
    [SerializeField] public List<DialogueSequence> sequences;
}

[System.Serializable]
public class DialogueSequence
{
    [SerializeField] public string id;
    [SerializeField] public List<string> sequence;
}

public class DialogueController : MonoBehaviour
{
    public bool hasDialogue;
    public UnityEvent onShowDialogue;
    public UnityEvent onCloseDialogue;
    public TextAsset source;
    public DialogueFile dialogueFile;

    public TextMeshProUGUI dialogueText;
    public DialogueSequence currentSequence;
    public int sequenceIndex;

    public bool IsInSequence { get => currentSequence != null; }

    private void Start() {

    }

    public void Update() {
        if (currentSequence != null) {
            if (Input.GetButtonDown("Jump")) {
                StepSequence();
            }
        }
    }

    public void LoadFile() {
        if (dialogueFile == null) {
            Debug.LogWarning("No dialogue file provided");
        } else {
            dialogueFile = JsonUtility.FromJson<DialogueFile>(source.text);
        }
    }

    public void BeginSequence(DialogueSequence sequence) {
        if (sequence == null) {
            Debug.LogWarning("Attempted to start null dialogue sequence");
        };

        currentSequence = sequence;
        onShowDialogue.Invoke();
        hasDialogue = true;
        sequenceIndex = 0;
        UpdateSequenceText();
    }

    public void StepSequence() {
        if (!hasDialogue) return;
        if (sequenceIndex < currentSequence.sequence.Count - 1) {
            sequenceIndex++;
            UpdateSequenceText();
        } else {
            EndSequence();
        }
    }

    public void UpdateSequenceText() {
        if (!hasDialogue) return;
        dialogueText.text = currentSequence.sequence[sequenceIndex];
    }

    public void EndSequence() {
        hasDialogue = false;
        currentSequence = null;
        sequenceIndex = 0;
        onCloseDialogue.Invoke();
    }

    public DialogueSequence GetSequence(string id) {
        if (dialogueFile.sequences.Count == 0) LoadFile();

        foreach (DialogueSequence sequence in dialogueFile.sequences) {
            if (sequence.id == id) return sequence;
        }

        return null;
    }
}
