using FMOD;
using FMOD.Studio;
using FMODUnity;
using MobX.Utilities.Types;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable Unity.PerformanceCriticalCodeInvocation

namespace MobX.Utilities.Fmod
{
    public class AudioAsset : InlinedScriptableObject
    {
        [SerializeField] private EventReference audioEvent;
        [SerializeField] private Optional<float> volume = new(1f, false);

        private readonly Dictionary<string, PARAMETER_ID> _parameterIds = new();

        public EventReference EventReference => audioEvent;


        #region Play

        public void Play()
        {
            audioEvent.PlayOneShot();
        }

        public void Play(Transform target)
        {
            audioEvent.PlayOneShotAttached(target, volume.ValueOrDefault(1f));
        }

        public void Play(Vector3 position)
        {
            audioEvent.PlayOneShot(position);
        }

        public void Play(Vector3 position, in FmodParameter parameter)
        {
            var instance = CreateInstance();
            instance.set3DAttributes(position.To3DAttributes());
            instance.start();
            instance.setParameterByID(parameter.Id, parameter.Value);
            instance.release();
        }

        public void Play(in FmodParameter parameter)
        {
            var instance = CreateInstance();
            instance.start();
            instance.setParameterByID(parameter.Id, parameter.Value);
            instance.release();
        }

        #endregion


        #region Parameter

        public FmodParameter CreateParameter(string parameterName, float defaultValue = 1)
        {
            return new FmodParameter(GetParameterID(parameterName), defaultValue);
        }

        public PARAMETER_ID GetParameterID(string parameterName)
        {
            if (_parameterIds.TryGetValue(parameterName, out var id) is false)
            {
                var description = RuntimeManager.GetEventDescription(audioEvent);
                description.getParameterDescriptionByName(parameterName, out var parameterDescription);
                id = parameterDescription.id;
                _parameterIds.Add(parameterName, id);
            }
            return id;
        }

        #endregion


        #region Instance

        public EventInstance CreateInstance()
        {
            return RuntimeManager.CreateInstance(audioEvent);
        }

        public void StartInstance(out EventInstance audioInstance)
        {
            audioInstance = RuntimeManager.CreateInstance(audioEvent);
            audioInstance.start();
            if (volume.Enabled)
            {
                audioInstance.setVolume(volume.Value);
            }
        }

        public void StartInstance(out EventInstance audioInstance, GameObject target)
        {
            audioInstance = RuntimeManager.CreateInstance(audioEvent);
            RuntimeManager.AttachInstanceToGameObject(audioInstance, target.transform);
            audioInstance.start();
            if (volume.Enabled)
            {
                audioInstance.setVolume(volume.Value);
            }
        }

        public void StartInstance(out EventInstance audioInstance, ATTRIBUTES_3D attributes3D)
        {
            audioInstance = RuntimeManager.CreateInstance(audioEvent);
            audioInstance.set3DAttributes(attributes3D);
            audioInstance.start();
            if (volume.Enabled)
            {
                audioInstance.setVolume(volume.Value);
            }
        }

        public void StopInstance(ref EventInstance instance)
        {
            instance.StopAndRelease();
        }

        #endregion
    }
}
