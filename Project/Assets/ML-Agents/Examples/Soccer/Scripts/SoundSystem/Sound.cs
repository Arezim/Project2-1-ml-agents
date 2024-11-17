using UnityEngine;

public struct Sound
{
    public Vector3 Origin;
    public float Radius;
    public int Priority;

    public Sound(Vector3 origin, float radius,int priority = 1)
    {
        Origin = origin;
        Radius = radius;
        Priority = priority;

    }
}