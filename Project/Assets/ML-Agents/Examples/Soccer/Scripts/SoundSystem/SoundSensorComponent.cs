using System;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents.Sensors;
using Unity.MLAgentsExamples;
using UnityEngine;

public class SoundSensorComponent : SensorComponent, ISoundListener, ISensor
{
    private List<Sound> sounds = new();

    private const int VALUES_PER_SOURCE = 3;
    private int TOTAL_OBSERVATIONS;

    private readonly int maxObservations = 10;
    private SoccerEnvController envController;

    void Start()
    {
        TOTAL_OBSERVATIONS = maxObservations * VALUES_PER_SOURCE;
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
        return ObservationSpec.Vector(TOTAL_OBSERVATIONS);
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
        var observations = new float[TOTAL_OBSERVATIONS];
        int index = 0;
        
        foreach (Sound sound in sounds)
        {
            if (index >= TOTAL_OBSERVATIONS - VALUES_PER_SOURCE)
                break;

            // Calculate relative position
            Vector3 relativePos = sound.Origin - transform.position;
            
            // Calculate distance (normalize by max distance)
            float maxDistance = 40f; // max distance to hear sound !!!!!! TODO: make sure 40f is correct
            float normalizedDistance = Mathf.Clamp01(relativePos.magnitude / maxDistance);
            
            // Calculate angle between forward direction and sound
            float angle = Vector3.SignedAngle(transform.forward, relativePos, Vector3.up);
            // Normalize angle from -180,180 to 0,1
            float normalizedAngle = (angle + 180f) / 360f;
            
            // Write type (0 for ball, 1 for player)
            float soundType = sound.Type == Sound.SoundType.Soccer ? 0f : 1f;

            observations[index] = normalizedDistance;
            observations[index + 1] = normalizedAngle;
            observations[index + 2] = soundType;
            
            index += VALUES_PER_SOURCE;
        }

        // Fill remaining observations with zeros (for safety)
        while (index < TOTAL_OBSERVATIONS)
        {
            observations[index] = 0f;
            observations[index + 1] = 0f;
            observations[index + 2] = 0f;
            index += VALUES_PER_SOURCE;
        }

        // Write all observations at once
        writer.AddList(observations);

        return TOTAL_OBSERVATIONS;
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
