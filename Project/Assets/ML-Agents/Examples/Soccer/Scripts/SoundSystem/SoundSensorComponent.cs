using System;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents.Sensors;
using Unity.MLAgentsExamples;
using UnityEngine;

public class SoundSensorComponent : SensorComponent, ISoundListener, ISensor
{
    private List<Sound> sounds = new();

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

    public void Update()
    {

    }

    public int Write(ObservationWriter writer)
    {
        foreach (Sound sound in sounds)
        {
            writer.Add(sound.Origin);
        }
        return sounds.Count;
    }

    public void OnHearSound(Sound sound)
    {
        sounds.Add(sound);
    }

    public SoccerEnvController GetPlayingField()
    {
        return envController;
    }
}
