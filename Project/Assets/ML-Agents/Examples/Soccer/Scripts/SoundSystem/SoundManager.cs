public class SoundManager
{
    public static void PlaySound(Sound sound)
    {
        var collisions = Physics.OverlapSphere(sound.Origin, sound.Position);
        foreach (var collider : collisions)
        {
            if (collider.gameObject.TryGetComponent<ISoundListener>(out var listener))
            {
                listener.OnHearSound(sound);
            }
        }
    }
}