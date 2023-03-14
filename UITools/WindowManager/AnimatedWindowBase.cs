namespace Tools
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