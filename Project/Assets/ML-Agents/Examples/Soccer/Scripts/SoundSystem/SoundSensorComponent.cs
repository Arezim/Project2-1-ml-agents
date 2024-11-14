using Unity.MLAgents.Sensors;

public class SoundSensorComponent : SensorComponent, ISoundListener
{
    public override ISensor[] CreateSensors()
    {
        throw new System.NotImplementedException();
    }

    public void OnHearSound(Sound soun)
    {
        throw new System.NotImplementedException();
    }
}

public interface SoundSensor : ISensor
{
    
}