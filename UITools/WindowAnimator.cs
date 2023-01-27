using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    //TODO old code base
    [System.Serializable]
    public class WindowAnimator
    {

        public float time = 0.5f;
        public float period = 0f;
        [SerializeField] private List<Transform> animateObject;
        private List<ObjectData> _animateObjectData = new List<ObjectData>();
        private bool _haveState = false;
        [Space]
        [Header("SCALE")]
        [SerializeField] private bool scaleType;
        [SerializeField][Range(0, 1000)] private float size = 1000;
        [SerializeField] private bool boomAnimation;
        [SerializeField] private float boomAnimationForce = 0.1f;
        [SerializeField] private float boomAnimationTime = 0.1f;
        [Header("MOVE")]
        [SerializeField] private bool moveType;
        [SerializeField] private Vector3 displacement;

        [Space]
        private bool _animateStation = true;
        private bool _animationStarted;
        public WindowAnimator StartAnimation(bool isReverse = false, Action onCompleted = null)
        {
            foreach (var obj in animateObject)
            {
                obj.DOKill();
            }
            RememberStartState();
            ResetState();
            StartAnimator(isReverse, onCompleted);
            return this;
        }
        private void RememberStartState()
        {
            if (_haveState) return;
            _haveState = true;
            foreach (var obj in animateObject)
            {
                _animateObjectData.Add(new ObjectData(obj.localScale, obj.localPosition));
            }
        }
        private void ResetState()
        {
            for (int i = 0; i < _animateObjectData.Count; i++)
            {
                animateObject[i].localPosition = _animateObjectData[i].Position;
                animateObject[i].localScale = _animateObjectData[i].Scale;
            }
        }
        private async void StartAnimator(bool isReverse, Action onCompleted)
        {
            for (int i = 0; i < animateObject.Count && !isReverse; i++)
            {
                if (scaleType) animateObject[i].localScale = animateObject[i].localScale / this.size;
                if (moveType) animateObject[i].localPosition += this.displacement;
            }
            var size = this.size;
            var displacement = this.displacement;
            if (isReverse)
            {
                displacement *= -1;
            }

            for (int i = 0; i < animateObject.Count; i++)
            {
                if (scaleType)
                {
                    var scale = animateObject[i].localScale * size;
                    if (isReverse) scale = animateObject[i].localScale / size;
                    if (!boomAnimation || isReverse) animateObject[i].DOScale(scale, time);
                    else
                    {
                        animateObject[i].DOScale(scale * (boomAnimationForce + 1), time + boomAnimationTime);
                        EndBoomAnimation(i);
                    }
                }
                if (moveType)
                    animateObject[i].DOLocalMove(animateObject[i].localPosition - displacement, time);

                if (i < animateObject.Count - 1 && period > 0) await Task.Delay(TimeSpan.FromSeconds(period));
                else if (period > 0) await Task.Delay(TimeSpan.FromSeconds(time + 0.1f));
            }
            onCompleted?.Invoke();
            _animationStarted = false;
        }

        private async void EndBoomAnimation(int id)
        {
            await Task.Delay(TimeSpan.FromSeconds(time + boomAnimationTime));
            animateObject[id].DOScale(animateObject[id].localScale / (boomAnimationForce + 1), boomAnimationTime);
        }

        public bool activeStaticAnimation = false;
        public void StartStaticAnimation()
        {

            activeStaticAnimation = true;
            if (_animateStation)
            {
                for (int i = 0; i < animateObject.Count; i++)
                {
                    if (moveType)
                        animateObject[i].DOLocalMove(animateObject[i].localPosition + displacement, time).OnComplete(() =>
                        {
                            activeStaticAnimation = false;
                        });
                }

                _animateStation = false;
            }
            else
            {
                EndStaticAnimation();
            }
        }

        public void ResetStaticAnimation(Vector3 position)
        {
            for (int i = 0; i < animateObject.Count; i++)
            {
                if (moveType)
                {
                    _animateStation = true;
                    animateObject[i].DOKill();
                    animateObject[i].localPosition = position;
                }
            }
        }
        private void EndStaticAnimation()
        {
            if (!_animateStation)
            {
                for (int i = 0; i < animateObject.Count; i++)
                {
                    if (moveType)
                        animateObject[i].DOLocalMove(animateObject[i].localPosition - displacement, time).OnComplete(() =>
                        {
                            activeStaticAnimation = false;
                        });
                }

                _animateStation = true;
            }
            else
            {
                StartStaticAnimation();
            }
        }

    }
    public class ObjectData
    {
        public ObjectData(Vector3 scale, Vector3 position)
        {
            Scale = scale;
            Position = position;
        }
        public Vector3 Scale;
        public Vector3 Position;
    }
}