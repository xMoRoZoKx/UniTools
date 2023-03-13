using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI
{
    public class WindowBase : MonoBehaviour
    {
        public Button closeButton;
        public WindowAnimator animator;
        public bool isReusableView = true;
        [HideInInspector] public UnityEvent onClose = new UnityEvent();
        [HideInInspector] public bool active = false;
        private void Awake()
        {
            closeButton?.onClick.AddListener(Close);
            animator.Init(transform);
        }
        public void Close()
        {
            WindowManager.instance.Close(this);
        }
        public virtual float ShowAnimation(Action onCompleted = null)
        {
            animator.StartShowAnimation(() =>
            {
                gameObject.SetActive(true);
                onCompleted?.Invoke();
            });
            return animator.duration;
        }
        public virtual float CloseAnimation(Action onCompleted = null)
        {
            animator.StartHideAnimation(onCompleted);
            return animator.duration;
        }
        public virtual void OnOpened()
        {
            active = true;
        }
    }
}