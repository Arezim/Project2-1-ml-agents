using System.Drawing.Printing;
using UnityEngine;

public class SoundManager
{
    public static void PlaySound(Sound sound)
    {
        //Debug.Log($"Sound generated at: {sound.Origin}, Radius: {sound.Radius}");
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