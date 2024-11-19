using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using System.Linq;
using Unity.Sentis.Layers;
using Unity.MLAgents.Sensors;

public enum Team
{
    Blue = 0,
    Purple = 1
}

public class AgentSoccer : Agent
{
    // Note that that the detectable tags are different for the blue and purple teams. The order is
    // * ball
    // * own goal
    // * opposing goal
    // * wall
    // * own teammate
    // * opposing player

    public enum Position
    {
        Striker,
        Goalie,
        Generic
    }

    [HideInInspector]
    public Team team;
    float m_KickPower;
    // The coefficient for the reward for colliding with a ball. Set using curriculum.
    float m_BallTouch;
    public Position position;

    const float k_Power = 2000f;
    float m_Existential;
    float m_LateralSpeed;
    float m_ForwardSpeed;


    [HideInInspector]
    public Rigidbody agentRb;
    SoccerSettings m_SoccerSettings;
    BehaviorParameters m_BehaviorParameters;
    public Vector3 initialPos;
    public float rotSign;

    EnvironmentParameters m_ResetParams;

    public Transform ball; // Reference to the ball
    public Transform goal; // Reference to the team's goal
    public Transform blueGoal; // Reference to the blue team's goal
    public Transform purpleGoal; // Reference to the purple team's goal

    public override void Initialize()
    {
        SoccerEnvController envController = GetComponentInParent<SoccerEnvController>();
        if (envController != null)
        {
            m_Existential = 1f / envController.MaxEnvironmentSteps;
        }
        else
        {
            m_Existential = 1f / MaxStep;
        }

        m_BehaviorParameters = gameObject.GetComponent<BehaviorParameters>();
        if (m_BehaviorParameters.TeamId == (int)Team.Blue)
        {
            team = Team.Blue;
            initialPos = new Vector3(transform.position.x - 5f, .5f, transform.position.z);
            rotSign = 1f;
            goal = purpleGoal; 
        }
        else
        {
            team = Team.Purple;
            initialPos = new Vector3(transform.position.x + 5f, .5f, transform.position.z);
            rotSign = -1f;
            goal = blueGoal; 
        }
        if (position == Position.Goalie)
        {
            m_LateralSpeed = 1.0f;
            m_ForwardSpeed = 1.0f;
        }
        else if (position == Position.Striker)
        {
            m_LateralSpeed = 0.3f;
            m_ForwardSpeed = 1.3f;
        }
        else
        {
            m_LateralSpeed = 0.3f;
            m_ForwardSpeed = 1.0f;
        }
        m_SoccerSettings = FindObjectOfType<SoccerSettings>();
        agentRb = GetComponent<Rigidbody>();
        agentRb.maxAngularVelocity = 500;

        m_ResetParams = Academy.Instance.EnvironmentParameters;
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        m_KickPower = 0f;

        var forwardAxis = act[0];
        var rightAxis = act[1];
        var rotateAxis = act[2];

        switch (forwardAxis)
        {
            case 1:
                dirToGo = transform.forward * m_ForwardSpeed;
                m_KickPower = 1f;
                break;
            case 2:
                dirToGo = transform.forward * -m_ForwardSpeed;
                break;
        }

        switch (rightAxis)
        {
            case 1:
                dirToGo = transform.right * m_LateralSpeed;
                break;
            case 2:
                dirToGo = transform.right * -m_LateralSpeed;
                break;
        }

        switch (rotateAxis)
        {
            case 1:
                rotateDir = transform.up * -1f;
                break;
            case 2:
                rotateDir = transform.up * 1f;
                break;
        }

        transform.Rotate(rotateDir, Time.deltaTime * 100f);
        agentRb.AddForce(dirToGo * m_SoccerSettings.agentRunSpeed,
            ForceMode.VelocityChange);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var soundSensor = transform.Find("Sound Collider").GetComponent<SoundSensorComponent>();
        var sounds = soundSensor.Sounds;
        SoccerEnvController envController = GetComponentInParent<SoccerEnvController>(); 
        GameObject ball = envController.ball; 

        Vector3 directionToBall = (ball.transform.position - transform.position).normalized;
        float distanceToBall = Vector3.Distance(transform.position, ball.transform.position);
        if (sounds.Count == 0)
        {
            AddReward(0.1f); // Reward for moving towards the ball
            MoveTowardsBall(directionToBall);
        }

        if (sounds.Count > 0)
        {
            // Find the closest sound but sort by prioruty first
            var sortedSounds = sounds
               .OrderByDescending(s => s.Priority)  // First, sort by priority
               .ThenBy(s => Vector3.Distance(transform.position, s.Origin))  // Then, sort by distance
               .ToList();

            // Now the closest and highest priority sound is at the top of the list
            Sound closest = sortedSounds.First();

            Debug.Log($"Agent {name} detected sound: Origin = {closest.Origin}, Radius = {closest.Radius}");

        
            Vector3 directionToSound = (closest.Origin - transform.position).normalized;
            float distanceToSound = Vector3.Distance(transform.position, closest.Origin);

            MoveTowardsSound(directionToSound);
            RewardForSoundProximity(distanceToSound);

            // Attenuation based on distance: Inverse Square Law (1 / distance^2)
            float soundIntensity = 1f / (distanceToSound * distanceToSound);
            if (closest.Priority == 10) // Ball sound has a higher priority
            {
                if (distanceToSound < 5f)
                {
                    AddReward(0.1f * soundIntensity); // Reward for moving toward the ball sound
                }
            }

           
            Vector3 nextPosition = transform.position + agentRb.velocity * Time.fixedDeltaTime;
            float nextDistanceToSound = Vector3.Distance(nextPosition, closest.Origin);
            float nextSoundIntensity = 1f / (nextDistanceToSound * nextDistanceToSound);

            if (nextDistanceToSound < distanceToSound)
            {
                // Reward for moving closer to the sound
                AddReward(10f * (distanceToSound - nextDistanceToSound) * soundIntensity);
                Debug.Log($"Agent rewarded for moving closer to sound: {0.1f * (distanceToSound - nextDistanceToSound)}");
            }
            else
            {
                // Penalize for moving away from the sound
                AddReward(-0.5f * (nextDistanceToSound - distanceToSound) * nextSoundIntensity);
                Debug.Log($"Agent penalized for moving away from sound: {-0.05f * (nextDistanceToSound - distanceToSound)}");
            }

            // Determine the direction of the sound (left or right)
            float angleToSound = Vector3.SignedAngle(transform.forward, directionToSound, Vector3.up);
            if (Mathf.Abs(angleToSound) > 30f) // Check if it's significantly to the left or right
            {
                
                float directionPenalty = Mathf.Abs(angleToSound) > 90f ? -0.2f : -0.1f;
                AddReward(directionPenalty);
                Debug.Log($"Agent penalized for moving away from the sound's direction: {directionPenalty}");
            }
        }
        else
        {
            // No sound detected, apply a small penalty for inactivity
            AddReward(-0.1f);
        }

        soundSensor.Reset();


        if (position == Position.Goalie)
        {
            // Existential bonus for Goalies.
            AddReward(m_Existential);
        }
        else if (position == Position.Striker)
        {
            // Existential penalty for Strikers
            AddReward(-m_Existential);
        }

        MoveAgent(actionBuffers.DiscreteActions);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        //forward
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
        //rotate
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[2] = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[2] = 2;
        }
        //right
        if (Input.GetKey(KeyCode.E))
        {
            discreteActionsOut[1] = 1;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            discreteActionsOut[1] = 2;
        }
    }
    /// <summary>
    /// Used to provide a "kick" to the ball.
    /// </summary>
    void OnCollisionEnter(Collision c)
    {
        var force = k_Power * m_KickPower;
        if (position == Position.Goalie)
        {
            force = k_Power;
        }
        if (c.gameObject.CompareTag("ball"))
        {
            AddReward(.2f * m_BallTouch);
            var dir = c.contacts[0].point - transform.position;
            dir = dir.normalized;
            c.gameObject.GetComponent<Rigidbody>().AddForce(dir * force);
        }
    }

    public override void OnEpisodeBegin()
    {
        m_BallTouch = m_ResetParams.GetWithDefault("ball_touch", 0);
    }
    private void MoveTowardsBall(Vector3 directionToBall)
    {
        agentRb.AddForce(directionToBall * m_SoccerSettings.agentRunSpeed, ForceMode.VelocityChange);
    }

    private void MoveTowardsSound(Vector3 directionToSound)
    {
        agentRb.AddForce(directionToSound * m_SoccerSettings.agentRunSpeed, ForceMode.VelocityChange);
    }

    private void RewardForSoundProximity(float distanceToSound)
    {
        float soundIntensity = 1f / (distanceToSound * distanceToSound);  
        AddReward(0.1f * soundIntensity); 
    }

//checks the position of the agent and calls the appropriate movement method based on the agent's role
    void Update()
    {
        if (position == Position.Striker)
        {
            MoveStrikerTowardsGoal();
        }
        else if (position == Position.Goalie)
        {
            MoveGoalieTowardsGoal();
        }
    }

    void MoveStrikerTowardsGoal()
    {
        Vector3 directionToGoal = (goal.position - transform.position).normalized;
        float speed = 5.0f; 
        transform.position += directionToGoal * speed * Time.deltaTime;
    }

    void MoveGoalieTowardsGoal()
    {
        float distanceToBall = Vector3.Distance(transform.position, ball.position);

        
        float thresholdDistance = 10.0f;

        if (distanceToBall < thresholdDistance)
        {
            
            Vector3 directionToGoal = (goal.position - transform.position).normalized;
            float speed = 5.0f; 
            transform.position += directionToGoal * speed * Time.deltaTime;
        }
    }
}