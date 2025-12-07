using UnityEngine;
using TMPro;

public class MonsterInteraction : MonoBehaviour
{
    [Header("World Objects")]
    [Tooltip("Scroll object that should appear after talking to the monster.")]
    public GameObject scrollObject;   // assign ForestScroll here

    [Header("UI")]
    [Tooltip("HUD_Canvas / InteractHint text.")]
    public TextMeshProUGUI hintText;  // assign InteractHint here

    [Header("Dialogue")]
    public string speakerName = "Forest Monster";
    [TextArea(2, 5)]
    public string dialogueText =
        "So you made it this far, warrior...\n" +
        "Take my scroll and learn the path to the witch.";

    [Header("Objective")]
    [TextArea(1, 3)]
    public string newObjectiveText = "Read the monster's scroll.";

    private bool playerInside = false;
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || hasTriggered) return;

        playerInside = true;

        // show 'Press E' hint
        if (hintText != null)
        {
            hintText.text = "Press E to talk to the monster";
            hintText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;

        if (hintText != null)
        {
            hintText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!playerInside || hasTriggered) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TriggerInteraction();
        }
    }

    private void TriggerInteraction()
    {
        hasTriggered = true;

        // hide hint
        if (hintText != null)
        {
            hintText.gameObject.SetActive(false);
        }

        // show dialogue
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowDialogue(speakerName, dialogueText);
        }

        // update objective
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.SetObjective(newObjectiveText);
        }

        // reveal scroll
        if (scrollObject != null)
        {
            scrollObject.SetActive(true);
        }

        // disable this collider so it can't be re-triggered
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }
}
