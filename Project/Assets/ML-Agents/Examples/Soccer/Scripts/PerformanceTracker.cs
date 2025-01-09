using UnityEngine;
using Unity.Profiling;

public class PerformanceTracker : MonoBehaviour
{
    private ProfilerRecorder mainThreadTimeRecorder;
    private ProfilerRecorder renderThreadTimeRecorder;
    private ProfilerRecorder memoryRecorder;

    private float totalMainThreadTime = 0f;
    private int frameCount = 0;

    void OnEnable()
    {
        // Main Thread (CPU time) and Memory Usage Profiler Recorders
        mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread");
        renderThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Render Thread");
        memoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory");
    }

    void Update()
    {
        frameCount++;

        // CPU Time Calculation (Convert nanoseconds to milliseconds)
        if (mainThreadTimeRecorder.Valid)
        {
            float currentMainThreadTimeMs = mainThreadTimeRecorder.LastValue / (1000f * 1000f);
            totalMainThreadTime += currentMainThreadTimeMs;
        }

        // GPU Time (Render Thread - less reliable)
        float currentRenderTimeMs = renderThreadTimeRecorder.LastValue / (1000f * 1000f);

        // RAM Usage (Convert bytes to MB)
        float currentRamUsageMB = memoryRecorder.LastValue / (1024f * 1024f);

        // Display cumulative performance stats in the Console
        Debug.Log($"Average CPU Time: {(totalMainThreadTime / frameCount):F2} ms");
        Debug.Log($"Current Render Thread Time: {currentRenderTimeMs:F2} ms");
        Debug.Log($"Current RAM Usage: {currentRamUsageMB:F2} MB");
    }

    void OnDisable()
    {
        // Dispose of the recorders to avoid memory leaks
        mainThreadTimeRecorder.Dispose();
        renderThreadTimeRecorder.Dispose();
        memoryRecorder.Dispose();
    }
}
