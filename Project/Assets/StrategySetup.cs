using Assets.ML_Agents.Examples.Soccer.Scripts.SoundSystem;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using Unity.Sentis;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class StrategySetup : MonoBehaviour
{
    public bool Symmetric;

    [Header("Blue Strategy")]
    [Dropdown("_soundSensorNames"), Label("Sound Strategy")]
    public string SoundStrategyBlue;
    [Dropdown("_visionStrategies"), Label("RayStrategy")]
    public RayPerceptionSensorComponent3D RayStrategyBlue;
    [Label("TFModel")]
    public ModelAsset ModelBlue;


    [Header("Purple strategy")]
    [Dropdown("_soundSensorNames"), HideIf("Symmetric"), Label("Sound Strategy")]
    public string SoundStrategyPurple;
    [Dropdown("_visionStrategies"), Label("RayStrategy"), HideIf("Symmetric")]
    public RayPerceptionSensorComponent3D RayStrategyPurple;
    [HideIf("Symmetric"), Label("TFModel")]
    public ModelAsset ModelPurple;


    private List<Type> _soundSensorStrategies;
    private string[] _soundSensorNames;
    private RayPerceptionSensorComponent3D[] _visionStrategies;

#if UNITY_EDITOR
    public void OnValidate()
    {
        RefreshSoundStrategyList();
        RefreshRayStrategyList();
        foreach (GameObject striker in AllStrikers())
        {
            if (IsBlue(striker))
            {
                SetSoundStrategy(striker, SoundStrategyBlue);
                SetRayStrategy(striker, RayStrategyBlue);
                SetModel(striker, ModelBlue);
            }
            else
            {
                SetSoundStrategy(striker, Symmetric ? SoundStrategyBlue : SoundStrategyPurple);
                SetRayStrategy(striker, Symmetric ? RayStrategyBlue : RayStrategyPurple);
                SetModel(striker, Symmetric ? ModelBlue : ModelPurple);
            }
        }
    }
#endif

    private void RefreshSoundStrategyList()
    {
        // A very hacky way to get all classes that implement ISoundSensorStrategy
        Type type = typeof(ISoundSensorStrategy);
        _soundSensorStrategies = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p)).ToList();
        _soundSensorStrategies.Remove(type);
        _soundSensorNames = _soundSensorStrategies.Select(s => s.Name).ToArray();
    }

    private void RefreshRayStrategyList()
    {
        _visionStrategies = GetComponentsInChildren<RayPerceptionSensorComponent3D>();
    }

    private void SetSoundStrategy(GameObject striker, string strategyName)
    {
        SoundSensorComponent soundComponent = striker.GetComponentInChildren<SoundSensorComponent>();
        Type strategyType = _soundSensorStrategies.First(x => x.Name == strategyName);
        soundComponent.Strategy = (ISoundSensorStrategy)Activator.CreateInstance(strategyType);
    }

    private void SetRayStrategy(GameObject striker, RayPerceptionSensorComponent3D rayStrategy)
    {
        RayPerceptionSensorComponent3D rayComponent = GetBackwardsRayComponent(striker);
        rayComponent.RaysPerDirection = rayStrategy.RaysPerDirection;
    }

    private void SetModel(GameObject striker, ModelAsset model)
    {
        striker.GetComponentInChildren<BehaviorParameters>().Model = model;
    }

    private bool IsBlue(GameObject striker)
    {
        return striker.GetComponentInChildren<BehaviorParameters>().TeamId == 0;

    }

    private List<GameObject> AllStrikers()
    {
        return FindObjectsOfType<AgentSoccer>().Select(x => x.gameObject).ToList();
    }

    private RayPerceptionSensorComponent3D GetBackwardsRayComponent(GameObject striker)
    {
        return striker.GetComponentsInChildren<RayPerceptionSensorComponent3D>().Skip(1).First();
    }
}
