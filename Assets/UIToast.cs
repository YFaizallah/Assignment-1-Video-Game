using UnityEngine;
using TMPro;

public class UIToast : MonoBehaviour
{
    // Singleton reference so other scripts can easily call UIToast.Show("...")
    private static UIToast instance;

    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private float showTime = 1.5f;

    private float timer;

    void Awake()
    {
        instance = this;
        if (label != null)
            label.gameObject.SetActive(false);
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0 && label != null)
                label.gameObject.SetActive(false);
        }
    }

    // Public static method that any script can call
    public static void Show(string message)
    {
        if (instance == null || instance.label == null) return;

        instance.label.text = message;
        instance.label.gameObject.SetActive(true);
        instance.timer = instance.showTime;
    }
}
