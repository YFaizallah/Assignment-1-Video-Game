using UnityEngine;
using TMPro;

public class WeaponSelector : MonoBehaviour
{
    public enum WeaponType { None, Sword, Shield }
    public static WeaponType chosenWeapon = WeaponType.None;

    [Header("World Objects")]
    public GameObject swordObject;    // pf_sword_01
    public GameObject shieldObject;   // pf_shield_03
    public GameObject scrollObject;   // parent object of the scroll (starts disabled)

    [Header("UI")]
    public TextMeshProUGUI hintText;  // HUD_Canvas / InteractHint

    [Header("Texts")]
    [TextArea(2, 4)]
    public string swordDialogue = "You picked the sword.";
    [TextArea(2, 4)]
    public string shieldDialogue = "You picked the shield.";

    private bool playerInside = false;
    private bool hasChosen = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || hasChosen) return;

        playerInside = true;

        if (hintText != null)
        {
            hintText.text = "Press X for Sword, Y for Shield";
            hintText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;

        if (hintText != null)
            hintText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!playerInside || hasChosen) return;

        if (Input.GetKeyDown(KeyCode.X))
        {
            ChooseSword();
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            ChooseShield();
        }
    }

    private void ChooseSword()
    {
        hasChosen = true;
        chosenWeapon = WeaponType.Sword;

        // Show feedback
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowDialogue("System", swordDialogue);

        // Update objective
        if (ObjectiveManager.Instance != null)
            ObjectiveManager.Instance.SetObjective("Find the ancient scroll.");

        // Hide chosen weapon in the world
        if (swordObject != null)
            swordObject.SetActive(false);

        // Reveal the scroll shrine
        if (scrollObject != null)
            scrollObject.SetActive(true);

        FinishSelection();
    }

    private void ChooseShield()
    {
        hasChosen = true;
        chosenWeapon = WeaponType.Shield;

        // Show feedback
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowDialogue("System", shieldDialogue);

        // Update objective
        if (ObjectiveManager.Instance != null)
            ObjectiveManager.Instance.SetObjective("Find the ancient scroll.");

        // Hide chosen weapon in the world
        if (shieldObject != null)
            shieldObject.SetActive(false);

        // Reveal the scroll shrine
        if (scrollObject != null)
            scrollObject.SetActive(true);

        FinishSelection();
    }

    private void FinishSelection()
    {
        if (hintText != null)
            hintText.gameObject.SetActive(false);

        // Stop further triggers
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }
}
