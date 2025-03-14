using UnityEngine;

namespace CnoomFrameWork.Extensions
{
    public static class Vector2Extensions
    {

        //============== 分量操作 ==============//
        public static Vector2 WithX(this Vector2 v, float x)
        {
            return new Vector2(x, v.y);
        }
        public static Vector2 WithY(this Vector2 v, float y)
        {
            return new Vector2(v.x, y);
        }
        public static Vector2 AddX(this Vector2 v, float x)
        {
            return new Vector2(v.x + x, v.y);
        }
        public static Vector2 AddY(this Vector2 v, float y)
        {
            return new Vector2(v.x, v.y + y);
        }

        //============== 向量运算 ==============//
        public static Vector2 FlipX(this Vector2 v)
        {
            return new Vector2(-v.x, v.y);
        }
        public static Vector2 FlipY(this Vector2 v)
        {
            return new Vector2(v.x, -v.y);
        }

        //============== 几何计算 ==============//
        public static Vector2 MidPoint(this Vector2 a, Vector2 b)
        {
            return (a + b) / 2f;
        }
        public static float AngleTo(this Vector2 from, Vector2 to)
        {
            return Vector2.Angle(from, to);
        }

        //============== 方向判断 ==============//
        public static bool IsZero(this Vector2 v)
        {
            return v == Vector2.zero;
        }
        public static bool IsApproximate(this Vector2 a, Vector2 b, float epsilon = 0.001f)
        {
            return (a - b).sqrMagnitude < epsilon * epsilon;
        }

        //============== 实用转换 ==============//
        public static Vector3 ToXYVector3(this Vector2 v, float z = 0)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector3 ToXZVector3(this Vector2 v, float y = 0)
        {
            return new Vector3(v.x, y, v.y);
        }

        public static Vector3 ToYZVector3(this Vector2 v, float x = 0)
        {
            return new Vector3(x, v.x, v.y);
        }
    }
}