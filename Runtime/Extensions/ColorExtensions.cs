using UnityEngine;

namespace CnoomFrameWork.Extensions
{
    public static class ColorExtensions
    {
        /// <summary>
        ///     调整颜色透明度
        /// </summary>
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }
    }
}