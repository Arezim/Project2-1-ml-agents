using System;
using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class SoundSensorComponent : SensorComponent, ISoundListener, ISensor
{
    private List<Sound> sounds = new List<Sound>();

    private readonly int maxObservations = 10; 

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
        return ObservationSpec.VariableLength(3, maxObservations);
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
        foreach (var sound in sounds)
        {
            writer.Add(sound.Origin);
        }
        return sounds.Count;
    }

    public void OnHearSound(Sound sound)
    {
        sounds.Add(sound);
    }
}