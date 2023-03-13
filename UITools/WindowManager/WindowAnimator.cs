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
        public float duration = 0.2f;
        [SerializeField] private float period = 0f;
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
                if (_canvasGroup == null) _canvasGroup = root.GetOrAddCommponent<CanvasGroup>();
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
        public StateData()
        {

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
    }
    [System.Serializable]
    public class AnimatedObject
    {
        public Transform transform;
        public bool useDefaultSettings = true;
        public AnimationSettings settings = new AnimationSettings();
        private StateData startState;
        public void StartShowAnimation(float duration, AnimationSettings defaultSettings = null)
        {
            transform.DOKill();

            if (useDefaultSettings && defaultSettings != null) settings = defaultSettings;

            if (startState == null) startState = new StateData(transform.localScale, transform.position);

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

            transform.localPosition = startState.Position + settings.displacement;
            transform.DOMove(startState.Position, duration);
        }
        public void StartHideAnimation(float duration, AnimationSettings defaultSettings = null)
        {
            transform.DOKill();

            if (startState == null) startState = new StateData(transform.localScale, transform.position);

            if (useDefaultSettings && defaultSettings != null) settings = defaultSettings;

            transform.position = startState.Position;
            transform.localScale = startState.Scale;

            if (settings.scaleForce != 0) transform.DOScale(startState.Scale / settings.scaleForce, duration);//.OnComplete(() => transform.localScale = startState.Scale);
            transform.DOMove(startState.Position + settings.displacement, duration);//.OnComplete(() => transform.position = startState.Position);
        }
    }
}