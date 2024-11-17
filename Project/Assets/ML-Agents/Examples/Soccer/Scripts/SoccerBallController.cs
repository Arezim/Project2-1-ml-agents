using UnityEngine;

public class SoccerBallController : MonoBehaviour
{
    public GameObject area;
    [HideInInspector]
    public SoccerEnvController envController;
    public string purpleGoalTag; //will be used to check if collided with purple goal
    public string blueGoalTag; //will be used to check if collided with blue goal

    private Rigidbody ball;

    void Start()
    {
        ball = GetComponent<Rigidbody>();
        envController = area.GetComponent<SoccerEnvController>();
    }

    void OnCollisionEnter(Collision col)
    {
        SoundManager.PlaySound(new Sound(transform.position, 15f));

        if (col.gameObject.CompareTag(purpleGoalTag)) //ball touched purple goal
        {
            envController.GoalTouched(Team.Blue);
        }
        if (col.gameObject.CompareTag(blueGoalTag)) //ball touched blue goal
        {
            envController.GoalTouched(Team.Purple);
        }
    }

    void FixedUpdate()
    {
        if (ball.velocity.magnitude > 0.1f) 
        {
            SoundManager.PlaySound(new Sound(transform.position, 10f));
        }
    }
}
