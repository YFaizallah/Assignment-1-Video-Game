using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public enum WeaponType { Sword, Shield }
    public WeaponType weaponType;

    public GameObject otherWeapon;   // drag the other weapon here to disable it after picking
    public string pickMessage = "You picked a weapon.";

    private bool playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            PickWeapon();
        }
    }

    private void PickWeapon()
    {
        // Show feedback to the player
        DialogueManager.Instance.ShowDialogue("System", pickMessage);

        // Update objective
        ObjectiveManager.Instance.SetObjective("Find the ancient scroll.");

        // Disable other weapon
        if (otherWeapon != null)
            otherWeapon.SetActive(false);

        // Disable this trigger so it can't be picked again
        gameObject.SetActive(false);
    }
}
