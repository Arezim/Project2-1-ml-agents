using UnityEngine;

public struct Sound
{
    public Vector3 Origin;
    public float Radius;
    public SoundType Type;
    public SoccerEnvController PlayingField;
    public enum SoundType { Soccer, Player };

    public Sound(Vector3 origin, float radius, SoundType type, SoccerEnvController playingField = null)
    {
        Origin = origin;
        Radius = radius;
        Type = type;
        PlayingField = playingField;
    }
}
