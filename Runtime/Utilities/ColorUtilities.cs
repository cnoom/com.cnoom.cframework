using System;
using CnoomFrameWork.Extensions;
using UnityEngine;

namespace CnoomFrameWork.Core.Utilities
{
    public static class ColorUtilities
    {
        /// <summary>
        /// 将十六进制转化为颜色
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static Color ParseColor(string color)
        {
            if(ColorUtility.TryParseHtmlString("#" + color, out Color result))
            {
                return result;
            }
            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// 颜色线性插值
        /// </summary>
        public static Color LerpColor(Color a, Color b, float t, bool includeAlpha = false)
        {
            return includeAlpha ? 
                new Color(
                    Mathf.Lerp(a.r, b.r, t),
                    Mathf.Lerp(a.g, b.g, t),
                    Mathf.Lerp(a.b, b.b, t),
                    Mathf.Lerp(a.a, b.a, t)) : 
                Color.Lerp(a, b, t);
        }

        /// <summary>
        /// 生成随机颜色
        /// </summary>
        public static Color RandomColor(float alpha = 1f)
        {
            return new Color(
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f),
                alpha);
        }

        /// <summary>
        /// 转换到HSV颜色空间
        /// </summary>
        public static void ToHSV(Color color, out float h, out float s, out float v)
        {
            Color.RGBToHSV(color, out h, out s, out v);
        }

        /// <summary>
        /// 从HSV创建颜色
        /// </summary>
        public static Color FromHSV(float h, float s, float v, float a = 1f)
        {
            return Color.HSVToRGB(h, s, v).WithAlpha(a);
        }

        /// <summary>
        /// 颜色相乘（正片叠底混合模式）
        /// </summary>
        public static Color Multiply(Color a, Color b)
        {
            return new Color(
                a.r * b.r,
                a.g * b.g,
                a.b * b.b,
                a.a * b.a);
        }
    }
}