using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.MLAgents;
using UnityEngine;

/*
 * Adapted from github https://stackoverflow.com/questions/56684306/how-to-read-system-usage-cpu-ram-etc
 * user derHugo
 */
public class CPUTracker : MonoBehaviour
{

    public float UpdateInterval = 1;

    public float CPUUsage { get => _cpu_usage; }

    private Thread _cpuThread;
    private float _lasCpuUsage;
    private int processorCount;
    private float _cpu_usage;

    // Start is called before the first frame update
    void Start()
    {
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
            _cpu_usage = 100f * (float)newCPUTime.TotalSeconds / UpdateInterval / processorCount;
            _cpu_usage = Math.Clamp(_cpu_usage, 0, 100);

            //Academy.Instance.StatsRecorder.Add("Profiler/cpu_usage", CPU_USAGE);

            // Wait for UpdateInterval
            Thread.Sleep(Mathf.RoundToInt(UpdateInterval * 1000));
        }
    }
}
