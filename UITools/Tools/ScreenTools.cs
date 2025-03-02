using UnityEngine;

namespace UniTools
{
    public static class UITools
    {
        public static float GetWidth(this RectTransform rt, float canvasScaledFactor)
        {
            var w = (rt.anchorMax.x - rt.anchorMin.x) * Screen.width + rt.sizeDelta.x * canvasScaledFactor;
            return w;
        }

        public static float GetHeight(this RectTransform rt, float canvasScaledFactor)
        {
            var h = (rt.anchorMax.y - rt.anchorMin.y) * Screen.height + rt.sizeDelta.y * canvasScaledFactor;
            return h;
        }
    }
}