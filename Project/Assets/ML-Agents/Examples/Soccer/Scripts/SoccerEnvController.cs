using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.UI;
using System;


public class SoccerEnvController : MonoBehaviour
{
    [System.Serializable]
    public class PlayerInfo
    {
        public AgentSoccer Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
        [HideInInspector]
        public Rigidbody Rb;
    }


    /// <summary>
    /// Max Academy steps before this platform resets
    /// </summary>
    /// <returns></returns>
    [Tooltip("Max Environment Steps")] public int MaxEnvironmentSteps = 25000;

    /// <summary>
    /// The area bounds.
    /// </summary>

    /// <summary>
    /// We will be changing the ground material based on success/failue
    /// </summary>

    public GameObject ball;
    [HideInInspector]
    public Rigidbody ballRb;
    Vector3 m_BallStartingPos;

    //List of Agents On Platform
    public List<PlayerInfo> AgentsList = new List<PlayerInfo>();

    private SoccerSettings m_SoccerSettings;
    


    private SimpleMultiAgentGroup m_BlueAgentGroup;
    private SimpleMultiAgentGroup m_PurpleAgentGroup;

    private int m_ResetTimer;

    private Dictionary<Team, int> teamScores = new Dictionary<Team, int>();

    void Start()
    {
		this.clearScores();

        m_SoccerSettings = FindObjectOfType<SoccerSettings>();
        // Initialize TeamManager
        m_BlueAgentGroup = new SimpleMultiAgentGroup();
        m_PurpleAgentGroup = new SimpleMultiAgentGroup();
        ballRb = ball.GetComponent<Rigidbody>();
        m_BallStartingPos = new Vector3(ball.transform.position.x, ball.transform.position.y, ball.transform.position.z);
        foreach (var item in AgentsList)
        {
            item.StartingPos = item.Agent.transform.position;
            item.StartingRot = item.Agent.transform.rotation;
            item.Rb = item.Agent.GetComponent<Rigidbody>();
            if (item.Agent.team == Team.Blue)
            {
                m_BlueAgentGroup.RegisterAgent(item.Agent);
            }
            else
            {
                m_PurpleAgentGroup.RegisterAgent(item.Agent);
            }
        }
        ResetScene();
    }

    void FixedUpdate()
    {
        m_ResetTimer += 1;
        if (m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            m_BlueAgentGroup.GroupEpisodeInterrupted();
            m_PurpleAgentGroup.GroupEpisodeInterrupted();
            ResetScene();
        }
    }


    public void ResetBall()
    {
        var randomPosX = UnityEngine.Random.Range(-2.5f, 2.5f);
        var randomPosZ = UnityEngine.Random.Range(-2.5f, 2.5f);

        ball.transform.position = m_BallStartingPos + new Vector3(randomPosX, 0f, randomPosZ);
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

    }

    public void GoalTouched(Team scoredTeam)
    {
        if (scoredTeam == Team.Blue)
        {
            teamScores[Team.Blue] += 1;
            m_BlueAgentGroup.AddGroupReward(1 - (float)m_ResetTimer / MaxEnvironmentSteps);
            m_PurpleAgentGroup.AddGroupReward(-1);
        }
        else
        {
            teamScores[Team.Purple] += 1;
            m_PurpleAgentGroup.AddGroupReward(1 - (float)m_ResetTimer / MaxEnvironmentSteps);
            m_BlueAgentGroup.AddGroupReward(-1);
        }

        if (this.teamScores[Team.Blue] >=1 || this.teamScores[Team.Purple] >=1)
        {
            print("Blue Goals: " + this.teamScores[Team.Blue] + " Purple Goals: " + this.teamScores[Team.Purple]);
            print("Game finished!");
            m_BlueAgentGroup.EndGroupEpisode();
            m_PurpleAgentGroup.EndGroupEpisode();
            StopScene();
            return;
        }

        m_PurpleAgentGroup.EndGroupEpisode();
        m_BlueAgentGroup.EndGroupEpisode();
        ResetScene();

    }

    public void clearScores()
    {
        this.teamScores = new Dictionary<Team, int>();
		this.teamScores[Team.Blue] = 0;
		this.teamScores[Team.Purple] = 0;
    }


    public void ResetScene()
    {
        m_ResetTimer = 0;

        //Reset Agents
        foreach (var item in AgentsList)
        {
            var randomPosX = UnityEngine.Random.Range(-5f, 5f);
            var newStartPos = item.Agent.initialPos + new Vector3(randomPosX, 0f, 0f);
            var rot = item.Agent.rotSign * UnityEngine.Random.Range(80.0f, 100.0f);
            var newRot = Quaternion.Euler(0, rot, 0);
            item.Agent.transform.SetPositionAndRotation(newStartPos, newRot);

            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;
        }

        //Reset Ball
        ResetBall();
    }

    public void StopScene()
    {
        // Reset ball
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        // Stop agents
        m_PurpleAgentGroup.GroupEpisodeInterrupted();
        m_BlueAgentGroup.GroupEpisodeInterrupted();

        foreach (var item in AgentsList)
        {
            // Stop agent physics
            var newStartPos = item.Agent.initialPos;
            var rot = item.Agent.rotSign;
            var newRot = Quaternion.Euler(0, rot, 0);
            item.Agent.transform.SetPositionAndRotation(newStartPos, newRot);
            item.Agent.agentRb.velocity = Vector3.zero;
            item.Agent.agentRb.angularVelocity = Vector3.zero;

            // Disable agent movement
            item.Agent.movementEnabled = false;
            print("movement state:" + item.Agent.movementEnabled);
        }
    }

   public void RestartGame()
{
    clearScores();
    ResetScene();
    foreach (var item in AgentsList)
        {
            
            // Disable agent movement
            item.Agent.movementEnabled = true;
            print("movement state:" + item.Agent.movementEnabled);
        }
} 

}
