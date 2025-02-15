using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Assets.ML_Agents.Examples.Soccer.Scripts.SoundSystem
{
    public interface ISoundSensorStrategy
    {
        public int Write(ObservationWriter writer, Transform selfTransform);
        public void OnHearSound(Sound sound);
        public ObservationSpec GetObservationSpec();
        public void Clear();
    }
}
