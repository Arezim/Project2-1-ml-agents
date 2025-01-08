using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Profiling;

public class EnvironmentVars : MonoBehaviour
{
    private StatsRecorder m_statsRecorder;
    private int m_nextUpdate;

    // Start is called before the first frame update
    void Start()
    {
        m_statsRecorder = Academy.Instance.StatsRecorder;
        m_nextUpdate = 0;

        Debug.Log("Starting logging into file \"soccerLog\"");
        Profiler.logFile = "soccerLog";
        Profiler.enableBinaryLog = true;
        Profiler.enabled = true;
        Profiler.maxUsedMemory = 256 * 1024 * 1024;
        Profiler.BeginSample("Soccer");
    }



    // Update is called once per frame
    void Update()
    {
        if (Academy.Instance.TotalStepCount >= m_nextUpdate)
        {
            m_statsRecorder.Add("Profiler/rnd_test", Random.value);

            // Very dirty and "temporary"
            m_nextUpdate += 10000;
        }
    }

    void onDestroy()
    {
        Profiler.EndSample();
        Profiler.enabled = false;
        Profiler.logFile = "";
        Debug.Log("Ending logging!");
    }
}
