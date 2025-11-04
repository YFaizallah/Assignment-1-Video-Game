using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DayNightCycleURP : MonoBehaviour
{
    [Header("Time Settings")]
    public float dayLengthMinutes = 5f;   // full cycle duration
    [Range(0, 24)] public float startHour = 8f;

    [Header("Lights")]
    public Light sun;
    public Light moon;

    [Header("Skybox Material (Duplicate One)")]
    public Material skyboxMaterial;
    public float skyRotationSpeed = 10f;

    [Header("Global Volume (URP)")]
    public Volume globalVolume;           // drag your Global Volume here

    // Internal refs
    private ColorAdjustments colorAdj;
    private Bloom bloom;
    private Vignette vignette;

    private float t01;                    // normalized time 0..1

    void Start()
    {
        t01 = startHour / 24f;

        // Make a unique skybox instance
        if (skyboxMaterial != null)
            RenderSettings.skybox = new Material(skyboxMaterial);

        // Get overrides from Global Volume
        if (globalVolume && globalVolume.profile)
        {
            globalVolume.profile.TryGet(out colorAdj);
            globalVolume.profile.TryGet(out bloom);
            globalVolume.profile.TryGet(out vignette);
        }
    }

    void Update()
    {
        // Advance time
        float secondsPerDay = Mathf.Max(1f, dayLengthMinutes * 60f);
        t01 = (t01 + Time.deltaTime / secondsPerDay) % 1f;

        // 1 = noon, 0 = midnight
        float daylight = Mathf.Clamp01(Mathf.Cos(t01 * Mathf.PI * 2f) * -0.5f + 0.5f);

        // --- Rotate Sun and Moon ---
        float sunAngle = t01 * 360f - 90f;
        if (sun) sun.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);
        if (moon) moon.transform.rotation = Quaternion.Euler(sunAngle + 180f, 170f, 0f);

        // --- Adjust intensities ---
        if (sun)
        {
            sun.intensity = Mathf.Lerp(0f, 1.2f, daylight);
            sun.color = Color.Lerp(new Color(1f, 0.6f, 0.3f), Color.white, daylight);
        }
        if (moon)
        {
            moon.intensity = Mathf.Lerp(0.25f, 0f, daylight);
            moon.enabled = moon.intensity > 0.05f;
        }

        // --- Ambient color ---
        RenderSettings.ambientLight = Color.Lerp(new Color(0.05f, 0.1f, 0.2f), Color.white, daylight);

        // --- Skybox rotation + exposure tint ---
        if (RenderSettings.skybox)
        {
            float rot = RenderSettings.skybox.GetFloat("_Rotation");
            rot = (rot + skyRotationSpeed * Time.deltaTime / 60f) % 360f;
            RenderSettings.skybox.SetFloat("_Rotation", rot);

            // Tint changes from blue night to white noon
            Color tint = Color.Lerp(new Color(0.05f, 0.15f, 0.4f), Color.white, daylight);
            RenderSettings.skybox.SetColor("_Tint", tint);

            // Exposure changes slightly
            RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(0.6f, 1.1f, daylight));
        }

        // --- Post-processing changes (URP) ---
        if (colorAdj != null)
        {
            colorAdj.postExposure.value = Mathf.Lerp(-1f, 0.5f, daylight);
            colorAdj.colorFilter.value = Color.Lerp(
                new Color(0.25f, 0.3f, 0.5f), // blue-night
                Color.white,                  // neutral-day
                daylight
            );
            colorAdj.saturation.value = Mathf.Lerp(-50f, 0f, daylight);
        }

        if (bloom != null)
            bloom.intensity.value = Mathf.Lerp(0.1f, 0.25f, daylight);

        if (vignette != null)
            vignette.intensity.value = Mathf.Lerp(0.35f, 0f, daylight);

        DynamicGI.UpdateEnvironment();
    }
}
