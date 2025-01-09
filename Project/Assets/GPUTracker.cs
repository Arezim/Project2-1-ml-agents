using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GPUTracker : MonoBehaviour
{
    public float UpdateInterval = 1.0f;

    private Thread _gpuThread;

    void Start()
    {
        // setup the thread
        _gpuThread = new Thread(UpdateGPUUsage)
        {
            IsBackground = true,
            // we don't want that our measurement thread
            // steals performance
            Priority = System.Threading.ThreadPriority.BelowNormal
        };

        // start the cpu usage thread
        _gpuThread.Start();
    }

    private void UpdateGPUUsage()
    {
        // This is ok since this is executed in a background thread
        while (true)
        {
            List<PerformanceCounter> gpuCounters = GetGPUCounters();
            float gpuUsage = GetGPUUsage(gpuCounters);
            UnityEngine.Debug.Log(gpuUsage);

            Thread.Sleep(Mathf.RoundToInt(UpdateInterval * 1000));
        }
    }

    public static List<PerformanceCounter> GetGPUCounters()
    {
        PerformanceCounterCategory category = new("GPU Engine");
        string[] counterNames = category.GetInstanceNames();

        List<PerformanceCounter> gpuCounters = counterNames
                            .Where(counterName => counterName.EndsWith("engtype_3D"))
                            .SelectMany(counterName => category.GetCounters(counterName))
                            .Where(counter => counter.CounterName.Equals("Utilization Percentage"))
                            .ToList();

        return gpuCounters;
    }

    public static float GetGPUUsage(List<PerformanceCounter> gpuCounters)
    {
        Debug.Log("GPU counters: " + gpuCounters.Count);
        gpuCounters.ForEach(x => x.NextValue());

        Thread.Sleep(1000);

        float result = gpuCounters.Sum(x => x.NextValue());

        return result;
    }
}
