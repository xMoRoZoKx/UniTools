using System;
using System.Collections.Generic;
using DG.Tweening;
using UniTools;
using UnityEngine;

namespace UniTools
{
    [System.Serializable]
    public class WindowAnimator
    {
        public float duration = 0.2f;
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
                    Debug.LogWarning("Need itin animator");
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
            DOKill();
            animateObjects.ForEach(ao =>
            {
                if (isShowing) ao.StartShowAnimation(duration, defaultSettings);
                else ao.StartHideAnimation(duration, defaultSettings);
            });
            //canvasGroup.interactable = false;
            if (useFade)
            {
                canvasGroup.alpha = isShowing ? 0 : 1;
                canvasGroup.DOFade(isShowing ? 1 : 0, duration);
            }
            await TaskTools.WaitForSeconds(duration);
            //if (canvasGroup != null) canvasGroup.interactable = true;
            onCompleted?.Invoke();
        }
        public void DOKill()
        {
            animateObjects.ForEach(animateObject => animateObject.transform.DOKill());
            canvasGroup.DOKill();
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
        public ScaleVector scaleForce;
        [Range(0, 100)] public int offset = 0;
    }
    [System.Serializable]
    public class ScaleVector
    {
        [Range(0, 10)] public float x = 1.4f;
        [Range(0, 10)] public float y = 1.4f;
        [Range(0, 10)] public float z = 1.4f;
        [Range(0, 10)] public float punchScaleForce = .1f;
        public Vector3 GetVector() => new Vector3(x, y, z);
        public bool isZeroVector => x == default && y == default && z == default;
        public static Vector3 operator /(Vector3 uniVector, ScaleVector scaleVector)
        {
            float x = scaleVector.x == 0 ? uniVector.x : uniVector.x / scaleVector.x;
            float y = scaleVector.y == 0 ? uniVector.y : uniVector.y / scaleVector.y;
            float z = scaleVector.z == 0 ? uniVector.z : uniVector.z / scaleVector.z;
            return new Vector3(x, y, z);
        }
    }
    [System.Serializable]
    public class AnimatedObject
    {
        public AnimatedObject() { }
        public AnimatedObject(Transform transform, AnimationSettings settings)
        {
            this.transform = transform;
            this.settings = settings;
        }
        [field: SerializeField] public Transform transform { get; private set; }
        [SerializeField] private bool useDefaultSettings = true;
        [SerializeField] private AnimationSettings settings = new AnimationSettings();
        private StateData startState;
        public void Preparation(float duration, AnimationSettings defaultSettings)
        {
            transform.DOKill();

            if (startState == null) startState = new StateData(transform.localScale, transform.position);

            if (useDefaultSettings && defaultSettings != null) settings = defaultSettings;

            duration = duration - duration / 100 * settings.offset;
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
            if (!settings.scaleForce.isZeroVector) transform.localScale = startState.Scale / settings.scaleForce;
            await TaskTools.WaitForSeconds(duration / 100 * settings.offset);

            if (!settings.scaleForce.isZeroVector)
            {
                transform.localScale = startState.Scale / settings.scaleForce;
                if (settings.scaleForce.punchScaleForce == 0) transform.DOScale(startState.Scale, duration);
                else
                {
                    float punchForce = 1 + settings.scaleForce.punchScaleForce;
                    const float boomTime = 0.1f;
                    transform.DOScale(startState.Scale * punchForce, duration - boomTime).OnComplete(() => transform.DOScale(startState.Scale, boomTime));
                }
            }

            if (settings.displacement != Vector3.zero) transform.DOMove(startState.Position, duration);
        }
        public async void StartHideAnimation(float duration, AnimationSettings defaultSettings = null)
        {
            Preparation(duration, defaultSettings);

            transform.position = startState.Position;
            transform.localScale = startState.Scale;

            await TaskTools.WaitForSeconds(duration / 100 * settings.offset);

            if (transform == null) return;

            if (!settings.scaleForce.isZeroVector) transform.DOScale(startState.Scale / settings.scaleForce, duration);//.OnComplete(() => transform.localScale = startState.Scale);
            if (settings.displacement != Vector3.zero) transform.DOMove(startState.Position + settings.displacement, duration);//.OnComplete(() => transform.position = startState.Position);

            await TaskTools.WaitForSeconds(duration);

            if (transform != null) Reset();
        }
    }
}