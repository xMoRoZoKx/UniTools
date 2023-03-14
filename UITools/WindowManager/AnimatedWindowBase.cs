using System;
using System.Collections;
using System.Collections.Generic;
using Game.UI;
using Tools;
using UnityEngine;
namespace Game.UI
{
    public class AnimatedWindowBase : WindowBase
    {
        public WindowAnimator animator;
        protected override void Awake()
        {
            base.Awake();
            animator.Init(transform);
        }
        
        public override float ShowAnimation()
        {
            animator.StartShowAnimation(() =>
            {
                gameObject.SetActive(true);
            });
            return animator.duration;
        }
        public override float CloseAnimation()
        {
            animator.StartHideAnimation();
            return animator.duration;
        }
    }
}