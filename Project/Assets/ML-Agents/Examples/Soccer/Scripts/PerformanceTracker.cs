using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.MLAgents;
using Unity.Profiling;
using UnityEngine;

public class PerformanceTracker : MonoBehaviour
{
    public double FrameTime_ms => GetRecorderFrameAverage(mainThreadTimeRecorder) * (1e-6f);
    public long GCMemoryMB => gcMemoryRecorder.LastValue / (1024 * 1024);
    public long SystemMemoryMB => systemMemoryRecorder.LastValue / (1024 * 1024);

    ProfilerRecorder systemMemoryRecorder;
    ProfilerRecorder gcMemoryRecorder;
    ProfilerRecorder mainThreadTimeRecorder;

    static double GetRecorderFrameAverage(ProfilerRecorder recorder)
    {
        List<ProfilerRecorderSample> samples = new();
        recorder.CopyTo(samples);
        //Debug.Log("Count: " + samples.Count);
        if (recorder.Count == 0)
        {
            return 0;
        }

        return samples.ConvertAll(x => x.Value).Average();
    }

    void OnEnable()
    {
        systemMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
        gcMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
        mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
    }

    void OnDisable()
    {
        systemMemoryRecorder.Dispose();
        gcMemoryRecorder.Dispose();
        mainThreadTimeRecorder.Dispose();
    }
}
