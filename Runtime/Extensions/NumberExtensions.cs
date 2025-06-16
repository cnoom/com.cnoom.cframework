using System;

namespace CnoomFrameWork.Extensions
{
    public static class NumberExtensions
    {
        /// <summary>
        /// 取绝对值
        /// </summary>
        public static int Abs(this int value) => Math.Abs(value);
        /// <summary>
        /// 取绝对值
        /// </summary>
        public static float Abs(this float value) => Math.Abs(value);
        /// <summary>
        /// 取绝对值
        /// </summary>
        public static double Abs(this double value) => Math.Abs(value);
        
        
        /// <summary>
        /// 将单精度浮点数四舍五入到指定小数位数
        /// </summary>
        public static int Round(this float value) => (int)Math.Round(value);
        /// <summary>
        /// 将单精度浮点数四舍五入到指定小数位数
        /// </summary>
        public static int Round(this double value) => (int)Math.Round(value);
        /// <summary>
        /// 将单精度浮点数四舍五入到指定小数位数
        /// </summary>
        public static float Round(this float value, int decimals) => (float)Math.Round(value, decimals);
        /// <summary>
        /// 将双精度浮点数四舍五入到最接近的整数
        /// </summary>
        public static double Round(this double value, int decimals) => Math.Round(value, decimals);
        
        /// <summary>
        /// 将单精度浮点数向下取整到最接近的整数
        /// </summary>
        public static int Floor(this float value) => (int)Math.Floor(value);
        /// <summary>
        /// 将双精度浮点数向下到最接近的整数
        /// </summary>
        public static int Floor(this double value) => (int)Math.Floor(value);
        /// <summary>
        /// 将单精度浮点数向上取整到最接近的整数
        /// </summary>
        public static int Ceil(this float value) => (int)Math.Ceiling(value);
        /// <summary>
        /// 将双精度浮点数向上取整到最接近的整数
        /// </summary>
        public static int Ceil(this double value) => (int)Math.Ceiling(value);
        
        /// <summary>
        /// 将值限制在指定的范围内
        /// </summary>
        public static int Clamp(this int value, int min, int max) => Math.Clamp(value, min, max);
        /// <summary>
        /// 将值限制在指定的范围内
        /// </summary>
        public static float Clamp(this float value, float min, float max) => Math.Clamp(value, min, max);
        /// <summary>
        /// 将值限制在指定的范围内
        /// </summary>
        public static double Clamp(this double value, double min, double max) => Math.Clamp(value, min, max);
        
        /// <summary>
        /// 计算当前值占最大值的百分比
        /// </summary>
        public static float Percentage(this float value, float max) => value / max * 100f;
        /// <summary>
        /// 计算当前值占最大值的百分比
        /// </summary>
        public static double Percentage(this double value, double max) => value / max * 100d;
 
        
    }
}