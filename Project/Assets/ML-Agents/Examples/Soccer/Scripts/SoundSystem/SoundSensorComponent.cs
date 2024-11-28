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
}
