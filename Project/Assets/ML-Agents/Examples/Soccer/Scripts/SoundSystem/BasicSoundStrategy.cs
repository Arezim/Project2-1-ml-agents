using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.MLAgents.Sensors;

namespace Assets.ML_Agents.Examples.Soccer.Scripts.SoundSystem
{
    public class BasicSoundStrategy : ISoundSensorStrategy
    {
        private List<Sound> sounds = new();
        private readonly int maxObservations = 10;
        public IReadOnlyList<Sound> Sounds => sounds;


        public void OnHearSound(Sound sound)
        {
            sounds.Add(sound);
        }

        public int Write(ObservationWriter writer)
        {
            foreach (Sound sound in sounds)
            {
                writer.Add(sound.Origin);
            }

            return sounds.Count;
        }

        public void Clear()
        {
            sounds.Clear();
        }

        public ObservationSpec GetObservationSpec()
        {
            return ObservationSpec.VariableLength(3, maxObservations);
        }
    }
}
