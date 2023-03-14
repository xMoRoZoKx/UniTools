using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    [System.Serializable]
    public class WindowAnimator
    {
        public float duration = 0.5f;
        [SerializeField] private bool useFade = true;
        [SerializeField] private List<AnimatedObject> animateObjects;
        [SerializeField] private AnimationSettings defaultSettings;
        private CanvasGroup _canvasGroup;
        public CanvasGroup canvasGroup
        {
            get
            {
                if (root == null)
                {
                    Debug.LogError("Need itin animator");
                    return null;
                }
                if (_canvasGroup == null) _canvasGroup = root.GetOrAddComponent<CanvasGroup>();
                return _canvasGroup;
            }
        }
        private Transform root;
        public void Init(Transform root)
        {
            this.root = root;
        }
        public void StartShowAnimation(Action onCompleted = null) => StartAnimation(true, onCompleted);
        public void StartHideAnimation(Action onCompleted = null) => StartAnimation(false, onCompleted);
        private async void StartAnimation(bool isShowing, Action onCompleted = null)
        {
            animateObjects.ForEach(ao =>
            {
                if (isShowing) ao.StartShowAnimation(duration, defaultSettings);
                else ao.StartHideAnimation(duration, defaultSettings);
            });
            canvasGroup.interactable = false;
            if (useFade)
            {
                canvasGroup.alpha = isShowing ? 0 : 1;
                canvasGroup.DOFade(isShowing ? 1 : 0, duration);
            }
            await TaskTools.WaitForSeconds(duration);
            canvasGroup.interactable = true;
            onCompleted?.Invoke();
        }
    }

    public class StateData
    {
        public StateData(Vector3 scale, Vector3 position)
        {
            Scale = scale;
            Position = position;
        }
        public Vector3 Scale;
        public Vector3 Position;
    }
    [System.Serializable]
    public class AnimationSettings
    {
        public Vector3 displacement;
        public float scaleForce = 0;
        public bool useBoomScale;
        public float offset = 0;
    }
    [System.Serializable]
    public class AnimatedObject
    {
        public Transform transform;
        public bool useDefaultSettings = true;
        public AnimationSettings settings = new AnimationSettings();
        private StateData startState;
        public void Preparation(float duration, AnimationSettings defaultSettings)
        {
            transform.DOKill();

            if (startState == null) startState = new StateData(transform.localScale, transform.position);

            if (useDefaultSettings && defaultSettings != null) settings = defaultSettings;

            if (settings.offset > duration)
            {
                Debug.LogError("offset must be less than the duration");
            }

            duration = duration - settings.offset;
        }
        public void Reset()
        {
            if (startState == null) return;
            transform.position = startState.Position;
            transform.localScale = startState.Scale;
        }
        public async void StartShowAnimation(float duration, AnimationSettings defaultSettings = null)
        {
            Preparation(duration, defaultSettings);

            transform.position = startState.Position + settings.displacement;
            if (settings.scaleForce != 0) transform.localScale = startState.Scale / settings.scaleForce;
            await TaskTools.WaitForSeconds(settings.offset);

            if (settings.scaleForce != 0)
            {
                transform.localScale = startState.Scale / settings.scaleForce;
                if (!settings.useBoomScale) transform.DOScale(startState.Scale, duration);
                else
                {
                    const float boomForce = 1.02f;
                    const float boomTime = 0.1f;
                    transform.DOScale(startState.Scale * boomForce, duration - boomTime).OnComplete(() => transform.DOScale(startState.Scale / boomForce, boomTime));
                }
            }

            transform.DOMove(startState.Position, duration);
        }
        public async void StartHideAnimation(float duration, AnimationSettings defaultSettings = null)
        {
            Preparation(duration, defaultSettings);

            transform.position = startState.Position;
            transform.localScale = startState.Scale;

            await TaskTools.WaitForSeconds(settings.offset);

            if (settings.scaleForce != 0) transform.DOScale(startState.Scale / settings.scaleForce, duration);//.OnComplete(() => transform.localScale = startState.Scale);
            transform.DOMove(startState.Position + settings.displacement, duration);//.OnComplete(() => transform.position = startState.Position);
            await TaskTools.WaitForSeconds(duration);
            Reset();
        }
    }
}