using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;


    [Header("UI")]
    public TextMeshProUGUI objectiveText;

    private string currentObjective = "";

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
        UpdateObjectiveUI();
    }

    public void SetObjective(string newObjective)
    {
        currentObjective = newObjective;
        UpdateObjectiveUI();
    }

    public void ClearObjective()
    {
        currentObjective = "";
        UpdateObjectiveUI();
    }

    private void UpdateObjectiveUI()
    {
        if (objectiveText == null) return;

        if (string.IsNullOrEmpty(currentObjective))
            objectiveText.text = "";
        else
            objectiveText.text = "Current Objective: " + currentObjective;
    }
}
