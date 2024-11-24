using Assets.ML_Agents.Examples.Soccer.Scripts.SoundSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents.Sensors;
using Unity.MLAgentsExamples;
using UnityEngine;

public class SoundSensorComponent : SensorComponent, ISoundListener, ISensor
{
    public ISoundSensorStrategy Strategy;

    private SoccerEnvController envController;

    void Awake()
    {
        envController = gameObject.GetComponentInParent<SoccerEnvController>();
        SetStrategy();
    }

    private void SetStrategy()
    {
        Strategy ??= new BasicSoundStrategy();
    }

    public override ISensor[] CreateSensors()
    {
        return new ISensor[] { this };
    }

    public byte[] GetCompressedObservation()
    {
        return new byte[0];
    }

    public CompressionSpec GetCompressionSpec()
    {
        return CompressionSpec.Default();
    }

    public string GetName()
    {
        return gameObject.name;
    }

    public ObservationSpec GetObservationSpec()
    {
        SetStrategy();
        return Strategy.GetObservationSpec();
    }

    public void Reset() => Strategy.Clear();

    void ISensor.Update() => Strategy.Clear();

    public int Write(ObservationWriter writer) => Strategy.Write(writer);

    public void OnHearSound(Sound sound) => Strategy.OnHearSound(sound);

    public SoccerEnvController GetPlayingField()
    {
        return envController;
    }
    
    /* Seperate out
    
    
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents.Sensors;
using Unity.MLAgentsExamples;
using UnityEngine;

public class SoundSensorComponent : SensorComponent, ISoundListener, ISensor
{
    private List<Sound> sounds = new();

    //queue to remember the last few sounds
    private Queue<Sound> soundMemory = new Queue<Sound>();
    //they remember for the next 4 frames
    private readonly int memorySize = 4;

    private readonly int maxObservations = 10;
    private SoccerEnvController envController;

    void Start()
    {
        envController = gameObject.GetComponentInParent<SoccerEnvController>();

    }

    public IReadOnlyList<Sound> Sounds => sounds;




    public override ISensor[] CreateSensors()
    {
        return new ISensor[] { this };
    }

    public byte[] GetCompressedObservation()
    {
        return new byte[0];
    }

    public CompressionSpec GetCompressionSpec()
    {
        return CompressionSpec.Default();
    }

    public string GetName()
    {
        return gameObject.name;
    }

    public ObservationSpec GetObservationSpec()
    {
        return ObservationSpec.VariableLength(4, maxObservations);

    }

    public void Reset()
    {
        sounds.Clear();
    }

    void ISensor.Update()
    {
        sounds.Clear();
    }
    public int Write(ObservationWriter writer)
    {

       foreach (var sound in soundMemory)
        {
            writer.Add(sound.Origin);
        }
        return soundMemory.Count;

        
    }

    public void OnHearSound(Sound sound)
    {
        sounds.Add(sound);
         soundMemory.Enqueue(sound);
        if (soundMemory.Count > memorySize)
        {
            soundMemory.Dequeue();
        }
    }

    public SoccerEnvController GetPlayingField()
    {
        return envController;
    }
}
*/
    
}
