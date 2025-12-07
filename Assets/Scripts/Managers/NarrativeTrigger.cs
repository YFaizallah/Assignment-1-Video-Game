using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NarrativeTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    public bool triggerOnEnter = false;
    public bool triggerOnInteract = true;
    public bool triggerOnce = true;
    public KeyCode interactKey = KeyCode.E;

    private bool playerInside = false;
    private bool hasTriggered = false;

    [Header("UI Hint")]
    public bool showHint = false;
    public TextMeshProUGUI hintText;
    public string hintMessage = "Press E to interact";

    [Header("Dialogue")]
    public bool showDialogue = false;
    public string speakerName;
    [TextArea(2, 5)]
    public string dialogueText;

    [Header("Objective")]
    public bool setNewObjective = false;
    [TextArea(1, 3)]
    public string newObjectiveText;

    [Header("Scene Transition")]
    public bool loadScene = false;
    public string sceneName;
    public float loadDelay = 1.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;

        // If a dialogue is currently open, don't override its UI / hints
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsOpen())
            return;

        // ALWAYS show hint when player is inside, if enabled
        if (showHint && hintText != null && !hasTriggered)
        {
            hintText.text = hintMessage;
            hintText.gameObject.SetActive(true);
        }

        // Optional auto-trigger on enter
        if (triggerOnEnter)
        {
            TryTrigger();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;

        // Hide hint when player leaves
        if (showHint && hintText != null)
        {
            hintText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!playerInside) return;

        // Only react to key if this trigger uses interaction
        // AND no dialogue is currently open (so we don't double-use E)
        bool dialogueOpen = (DialogueManager.Instance != null && DialogueManager.Instance.IsOpen());

        if (triggerOnInteract && !dialogueOpen && Input.GetKeyDown(interactKey))
        {
            TryTrigger();
        }
    }

    private void TryTrigger()
    {
        if (hasTriggered && triggerOnce) return;

        // Hide hint once used
        if (showHint && hintText != null)
        {
            hintText.gameObject.SetActive(false);
        }

        // Dialogue
        if (showDialogue && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowDialogue(speakerName, dialogueText);
        }

        // Objective
        if (setNewObjective && ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.SetObjective(newObjectiveText);
        }

        // Scene change
        if (loadScene && !string.IsNullOrEmpty(sceneName))
        {
            StartCoroutine(LoadSceneAfterDelay());
        }

        hasTriggered = true;
    }

    private System.Collections.IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(loadDelay);

        if (ScreenFader.Instance != null)
            ScreenFader.Instance.FadeAndLoadScene(sceneName);
        else
            SceneManager.LoadScene(sceneName);

    }
}
