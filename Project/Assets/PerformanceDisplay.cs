using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Profiling;

public class PerformanceDisplay : MonoBehaviour
{
    public double GUIUpdateTime;
    public double TensorBoardUpdateTime;

    private double _guiNextUpdate;
    private double _tbNextUpdate;

    private double _frameTime_ms;
    private long _gcMemoryMB;
    private long _systemMemoryMB;
    private float _cpuUsage;

    private string _statsText;

    private CPUTracker _cpuTracker;
    private PerformanceTracker _performanceTracker;


    // Start is called before the first frame update
    void Start()
    {
        _cpuTracker = GetComponent<CPUTracker>();
        _performanceTracker = GetComponent<PerformanceTracker>();
    }



    // Update is called once per frame
    void Update()
    {
        if (_guiNextUpdate <= Time.realtimeSinceStartup)
        {
            UpdateMeasuredValues();

            StringBuilder sb = new(500);
            sb.AppendLine($"Frame Time: {_frameTime_ms:F1} ms");
            sb.AppendLine($"GC Memory: {_gcMemoryMB} MB");
            sb.AppendLine($"System Memory: {_systemMemoryMB} MB");
            sb.AppendLine($"CPU Usage: {_cpuUsage:F2} %");
            _statsText = sb.ToString();

            _guiNextUpdate = Time.realtimeSinceStartup + GUIUpdateTime;
        }

        if (_tbNextUpdate <= Time.realtimeSinceStartup)
        {
            UpdateMeasuredValues();

            Academy.Instance.StatsRecorder.Add("Profiler/Frame time (ms)", (float)_frameTime_ms);
            Academy.Instance.StatsRecorder.Add("Profiler/GC Memory (MB)", _gcMemoryMB);
            Academy.Instance.StatsRecorder.Add("Profiler/System Memory (MB)", _systemMemoryMB);
            Academy.Instance.StatsRecorder.Add("Profiler/CPU Usage (%)", _cpuUsage);

            _tbNextUpdate = Time.realtimeSinceStartup + TensorBoardUpdateTime;
        }
    }

    private void UpdateMeasuredValues()
    {
        _frameTime_ms = _performanceTracker.FrameTime_ms;
        _gcMemoryMB = _performanceTracker.GCMemoryMB;
        _systemMemoryMB = _performanceTracker.SystemMemoryMB;
        _cpuUsage = _cpuTracker.CPUUsage;
    }

    private void OnGUI()
    {
        GUI.TextArea(new Rect(10, 30, 250, 17 * 4), _statsText);
    }

}
