using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI bodyText;
    public TextMeshProUGUI hintText;

    private bool isOpen = false;
    private bool justOpened = false;   // <--- NEW

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!isOpen) return;

        // Skip the first frame after opening so we don't close immediately
        if (justOpened)
        {
            justOpened = false;
            return;
        }

        // Now we allow closing with E
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("DialogueManager: E pressed, closing dialogue.");
            CloseDialogue();
        }
    }

    public void ShowDialogue(string speakerName, string message, string hint = "Press E to continue")
    {
        Debug.Log("DialogueManager: ShowDialogue called.");

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (nameText != null)
            nameText.text = speakerName;

        if (bodyText != null)
            bodyText.text = message;

        if (hintText != null)
            hintText.text = hint;

        isOpen = true;
        justOpened = true;   // <--- NEW: prevents instant close
    }

    public void CloseDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        isOpen = false;
        justOpened = false;
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}
