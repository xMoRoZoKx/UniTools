namespace UniTools
{
    public class AnimatedWindowBase : WindowBase
    {
        public WindowAnimator animator;
        public override void OnOpened()
        {
            base.OnOpened();
            animator.Init(transform);
        }

        public override float ShowAnimation()
        {
            animator.StartShowAnimation(() =>
            {
                if(gameObject != null) gameObject.SetActive(true);
            });
            return animator.duration;
        }
        public override float CloseAnimation()
        {
            animator.StartHideAnimation();
            return animator.duration;
        }
        private void OnDestroy()
        {
            animator.DOKill();
        }
    }
}