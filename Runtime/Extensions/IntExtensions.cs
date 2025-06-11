using System.Collections.Generic;

namespace CnoomFrameWork.Extensions
{
    public static class IntExtensions
    {
        /// <summary>
        /// 生成从0到当前整数-1的序列
        /// </summary>
        /// <param name="count">序列长度</param>
        /// <returns>从0开始的整数序列</returns>
        public static IEnumerable<int> ToSequence(this int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return i;
            }
        }
    }
}