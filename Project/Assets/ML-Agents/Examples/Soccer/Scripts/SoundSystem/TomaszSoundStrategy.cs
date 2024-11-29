using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Assets.ML_Agents.Examples.Soccer.Scripts.SoundSystem
{
    internal class TomaszSoundStrategy : ISoundSensorStrategy
    {
        private List<Sound> sounds = new();
        public IReadOnlyList<Sound> Sounds => sounds;

        private const int VALUES_PER_SOURCE = 3;

        private const int maxObservations = 10;
        private SoccerEnvController envController;
        private readonly int TOTAL_OBSERVATIONS = maxObservations * VALUES_PER_SOURCE;



        public void Clear()
        {
            sounds.Clear();
        }

        public ObservationSpec GetObservationSpec()
        {
            return ObservationSpec.Vector(TOTAL_OBSERVATIONS);
        }

        public void OnHearSound(Sound sound)
        {
            sounds.Add(sound);
        }

        public int Write(ObservationWriter writer, Transform selfTransform)
        {
            float[] observations = new float[TOTAL_OBSERVATIONS];
            int index = 0;

            foreach (Sound sound in sounds)
            {
                if (index >= TOTAL_OBSERVATIONS - VALUES_PER_SOURCE)
                {
                    break;
                }

                // Calculate relative position
                Vector3 relativePos = sound.Origin - selfTransform.position;

                // Calculate distance (normalize by max distance)
                float maxDistance = 40f; // max distance to hear sound !!!!!! TODO: make sure 40f is correct
                float normalizedDistance = Mathf.Clamp01(relativePos.magnitude / maxDistance);

                // Calculate angle between forward direction and sound
                float angle = Vector3.SignedAngle(selfTransform.forward, relativePos, Vector3.up);
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
    }
}
