using System.Drawing.Printing;
using UnityEngine;

public class SoundManager
{
    public static void PlaySound(Sound sound)
    {
        //Debug.Log($"Sound generated at: {sound.Origin}, Radius: {sound.Radius}");
        Collider[] collisions = Physics.OverlapSphere(sound.Origin, sound.Radius);
        foreach (Collider collider in collisions)
        {
            if (collider.gameObject.TryGetComponent<ISoundListener>(out ISoundListener listener)
                && listener.GetPlayingField() == sound.PlayingField)
            {
                listener.OnHearSound(sound);
            }
        }
    }
}
