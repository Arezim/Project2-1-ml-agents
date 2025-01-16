using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Assets.ML_Agents.Examples.Soccer.Scripts.SoundSystem
{
    internal class MemorySoundStrategy : ISoundSensorStrategy
    {
        //queue to remember the last few sounds
        private Queue<Sound> soundMemory = new();
        private readonly int memorySize = 15;

        public void Clear()
        {
            soundMemory.clear();
        }

        public ObservationSpec GetObservationSpec()
        {
            return ObservationSpec.VariableLength(3, memorySize);
        }

        public void OnHearSound(Sound sound)
        {
            soundMemory.Enqueue(sound);
            if (soundMemory.Count > memorySize)
            {
                soundMemory.Dequeue();
            }
        }

        public int Write(ObservationWriter writer, Transform selfTransform)
        {
            foreach (Sound sound in soundMemory)
            {
                writer.Add(sound.Origin);
            }
            return soundMemory.Count;
        }
    }
}
