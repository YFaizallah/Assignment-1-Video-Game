using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class AnalyticsInitializer : MonoBehaviour
{
    private async void Start()
    {
        // Initialize Unity Gaming Services (includes Analytics)
        await UnityServices.InitializeAsync();

        // Start collecting analytics data
        AnalyticsService.Instance.StartDataCollection();
    }
}
