using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;

public class CPUMeasurer : MonoBehaviour
{
    public static float CPU_USAGE = 0;

    private Thread _cpuThread;
    private float _lasCpuUsage;
    private float updateInterval = 1;
    private int processorCount;

    // Start is called before the first frame update
    void Start()
    {
        //Application.runInBackground = true;

        // setup the thread
        _cpuThread = new Thread(UpdateCPUUsage)
        {
            IsBackground = true,
            // we don't want that our measurement thread
            // steals performance
            Priority = System.Threading.ThreadPriority.BelowNormal
        };

        // start the cpu usage thread
        _cpuThread.Start();
    }

    private void OnValidate()
    {
        // We want only the physical cores but usually
        // this returns the twice as many virtual core count
        //
        // if this returns a wrong value for you comment this method out
        // and set the value manually
        processorCount = SystemInfo.processorCount / 2;
    }

    private void UpdateCPUUsage()
    {
        TimeSpan lastCpuTime = new(0);

        // This is ok since this is executed in a background thread
        while (true)
        {
            TimeSpan cpuTime = new(0);

            // Get a list of all running processes in this PC
            Process[] AllProcesses = Process.GetProcesses();

            // Sum up the total processor time of all running processes
            cpuTime = AllProcesses.Aggregate(cpuTime, (current, process) => current + process.TotalProcessorTime);

            // get the difference between the total sum of processor times
            // and the last time we called this
            TimeSpan newCPUTime = cpuTime - lastCpuTime;

            // update the value of _lastCpuTime
            lastCpuTime = cpuTime;

            // The value we look for is the difference, so the processor time all processes together used
            // since the last time we called this divided by the time we waited
            // Then since the performance was optionally spread equally over all physical CPUs
            // we also divide by the physical CPU count
            CPU_USAGE = 100f * (float)newCPUTime.TotalSeconds / updateInterval / processorCount;
            CPU_USAGE = Math.Clamp(CPU_USAGE, 0, 100);

            // Wait for UpdateInterval
            Thread.Sleep(Mathf.RoundToInt(updateInterval * 1000));
        }
    }
}
