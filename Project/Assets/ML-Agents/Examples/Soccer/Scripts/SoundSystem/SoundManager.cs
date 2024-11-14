using UnityEngine;

public class SoundManager
{
    public static void PlaySound(Sound sound)
    {
        var collisions = Physics.OverlapSphere(sound.Origin, sound.Radius);
        foreach (var collider in collisions)
        {
            if (collider.gameObject.TryGetComponent<ISoundListener>(out var listener))
            {
                listener.OnHearSound(sound);
            }
        }
    }
}