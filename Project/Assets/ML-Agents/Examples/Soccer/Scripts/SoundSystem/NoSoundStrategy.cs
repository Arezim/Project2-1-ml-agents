using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Assets.ML_Agents.Examples.Soccer.Scripts.SoundSystem
{
    public class NoSoundStrategy : ISoundSensorStrategy
    {
        public void Clear()
        {
        }

        public ObservationSpec GetObservationSpec()
        {
            return ObservationSpec.Vector(0);
        }

        public void OnHearSound(Sound sound)
        {
        }

        public int Write(ObservationWriter writer, Transform selfTransform)
        {
            return 0;
        }
    }
}
