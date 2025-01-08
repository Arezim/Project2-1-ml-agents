using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class EnvironmentVars : MonoBehaviour
{
    private StatsRecorder m_statsRecorder;
    private int m_nextUpdate;

    // Start is called before the first frame update
    void Start()
    {
        m_statsRecorder = Academy.Instance.StatsRecorder;
        m_nextUpdate = 0;
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
}
