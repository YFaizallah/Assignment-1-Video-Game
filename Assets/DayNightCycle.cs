using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Cycle Length")]
    [Tooltip("Minutes for a full 24h day-night cycle")]
    public float dayLengthMinutes = 5f;   // <-- set duration here
    [Range(0, 24)] public float startHour = 8f;

    [Header("Lights")]
    public Light sun;                     // assign your Sun (Directional)
    public Light moon;                    // assign your Moon (Directional)

    [Header("Colors / Intensity")]
    public Gradient sunColor;             // dawn->day->dusk->night
    public AnimationCurve sunIntensity;   // 0..1 over the 'daylight' amount
    public Gradient ambientColor;         // ambient over 24h
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    float t01; // normalized 0..1 time-of-day

    void Reset()
    {
        // sensible defaults if you click "Reset" on the component
        sunIntensity = AnimationCurve.EaseInOut(0, 0, 1, 1);
        moonIntensity = AnimationCurve.EaseInOut(0, 0.3f, 1, 0.0f);
    }

    void Start()
    {
        t01 = startHour / 24f;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
    }

    void Update()
    {
        float secondsPerDay = Mathf.Max(1f, dayLengthMinutes * 60f);
        t01 = (t01 + Time.deltaTime / secondsPerDay) % 1f;

        // Sun & Moon directions (simple equatorial path)
        float sunAngle = t01 * 360f - 90f; // -90≈dawn, 90≈dusk
        if (sun) sun.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);
        if (moon) moon.transform.rotation = Quaternion.Euler(sunAngle + 180f, 170f, 0f);

        // daylight is 1 at noon, 0 at midnight
        float daylight = Mathf.Clamp01(Mathf.Cos(t01 * Mathf.PI * 2f) * -0.5f + 0.5f);

        if (sun)
        {
            if (sunColor.colorKeys.Length > 0) sun.color = sunColor.Evaluate(t01);
            float i = sunIntensity.Evaluate(daylight);
            sun.intensity = i;
            sun.enabled = i > 0.02f;
        }

        if (moon)
        {
            if (moonColor.colorKeys.Length > 0) moon.color = moonColor.Evaluate(t01);
            float i = moonIntensity.Evaluate(1f - daylight);
            moon.intensity = i;
            moon.enabled = i > 0.02f;
        }

        // Ambient light follows the day
        if (ambientColor.colorKeys.Length > 0)
            RenderSettings.ambientLight = ambientColor.Evaluate(t01);

        DynamicGI.UpdateEnvironment(); // refresh ambient/reflections
    }
}
