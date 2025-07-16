using System.Linq;
using UnityEngine;

namespace CnoomFrameWork.Extensions
{
    public static class TransformExtensions
    {
        #region 重置操作

        public static Transform ResetLocalPosition(this Transform transform)
        {
            return transform.SetLocalPosition(Vector3.zero);
        }

        public static Transform ResetLocalScale(this Transform transform)
        {
            return transform.SetLocalScale(Vector3.one);
        }

        #endregion

        #region 本地坐标

        public static Vector3 LocalPosition(this Transform transform)
        {
            return transform.localPosition;
        }

        public static float LocalPositionX(this Transform transform)
        {
            return transform.localPosition.x;
        }

        public static float LocalPositionY(this Transform transform)
        {
            return transform.localPosition.y;
        }

        public static float LocalPositionZ(this Transform transform)
        {
            return transform.localPosition.z;
        }

        public static Transform SetLocalPosition(this Transform transform, Vector3 pos)
        {
            transform.localPosition = pos;
            return transform;
        }

        public static Transform SetLocalPositionX(this Transform transform, float x)
        {
            return transform.SetLocalPosition(new Vector3(x, transform.localPosition.y, transform.localPosition.z));
        }

        public static Transform SetLocalPositionY(this Transform transform, float y)
        {
            return transform.SetLocalPosition(new Vector3(transform.localPosition.x, y, transform.localPosition.z));
        }

        public static Transform SetLocalPositionZ(this Transform transform, float z)
        {
            return transform.SetLocalPosition(new Vector3(transform.localPosition.x, transform.localPosition.y, z));
        }

        public static Transform AddLocalPositionX(this Transform transform, float x)
        {
            return transform.SetLocalPositionX(transform.localPosition.x + x);
        }

        public static Transform AddLocalPositionY(this Transform transform, float y)
        {
            return transform.SetLocalPositionY(transform.localPosition.y + y);
        }

        public static Transform AddLocalPositionZ(this Transform transform, float z)
        {
            return transform.SetLocalPositionZ(transform.localPosition.z + z);
        }

        #endregion

        #region 世界坐标

        public static Vector3 Position(this Transform transform)
        {
            return transform.position;
        }

        public static float PositionX(this Transform transform)
        {
            return transform.position.x;
        }

        public static float PositionY(this Transform transform)
        {
            return transform.position.y;
        }

        public static float PositionZ(this Transform transform)
        {
            return transform.position.z;
        }

        public static Transform SetPosition(this Transform transform, Vector3 pos)
        {
            transform.position = pos;
            return transform;
        }

        public static Transform SetPositionX(this Transform transform, float x)
        {
            return transform.SetPosition(new Vector3(x, transform.position.y, transform.position.z));
        }

        public static Transform SetPositionY(this Transform transform, float y)
        {
            return transform.SetPosition(new Vector3(transform.position.x, y, transform.position.z));
        }

        public static Transform SetPositionZ(this Transform transform, float z)
        {
            return transform.SetPosition(new Vector3(transform.position.x, transform.position.y, z));
        }

        public static Transform AddPositionX(this Transform transform, float x)
        {
            return transform.SetPositionX(transform.position.x + x);
        }

        public static Transform AddPositionY(this Transform transform, float y)
        {
            return transform.SetPositionY(transform.position.y + y);
        }

        public static Transform AddPositionZ(this Transform transform, float z)
        {
            return transform.SetPositionZ(transform.position.z + z);
        }

        #endregion

        #region 缩放设置

        public static Vector3 LocalScale(this Transform transform)
        {
            return transform.localScale;
        }

        public static float LocalScaleX(this Transform transform)
        {
            return transform.localScale.x;
        }

        public static float LocalScaleY(this Transform transform)
        {
            return transform.localScale.y;
        }

        public static float LocalScaleZ(this Transform transform)
        {
            return transform.localScale.z;
        }

        public static Transform SetLocalScaleX(this Transform transform, float x)
        {
            return transform.SetLocalScale(new Vector3(x, transform.localScale.y, transform.localScale.z));
        }

        public static Transform SetLocalScaleY(this Transform transform, float y)
        {
            return transform.SetLocalScale(new Vector3(transform.localScale.x, y, transform.localScale.z));
        }

        public static Transform SetLocalScaleZ(this Transform transform, float z)
        {
            return transform.SetLocalScale(new Vector3(transform.localScale.x, transform.localScale.y, z));
        }

        private static Transform SetLocalScale(this Transform transform, Vector3 scale)
        {
            transform.localScale = scale;
            return transform;
        }

        public static Transform SetLocalEulerAnglesX(this Transform transform, float x)
        {
            return transform.SetLocalEulerAngles(new Vector3(x, transform.localEulerAngles.y,
                transform.localEulerAngles.z));
        }

        public static Transform SetLocalEulerAnglesY(this Transform transform, float y)
        {
            return transform.SetLocalEulerAngles(new Vector3(transform.localEulerAngles.x, y,
                transform.localEulerAngles.z));
        }

        public static Transform SetLocalEulerAnglesZ(this Transform transform, float z)
        {
            return transform.SetLocalEulerAngles(new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y,
                z));
        }

        private static Transform SetLocalEulerAngles(this Transform transform, Vector3 angles)
        {
            transform.localEulerAngles = angles;
            return transform;
        }

        #endregion

        #region 子类操作

        // 递归查找子对象
        public static Transform FindChildRecursive(this Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name) return child;
                var result = child.FindChildRecursive(name);
                if (result) return result;
            }

            return null;
        }

        // 删除所有子对象
        public static Transform DestroyAllChildren(this Transform parent)
        {
            for (var i = parent.childCount - 1; i >= 0; i--)
            {
                var child = parent.GetChild(i);
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    Object.DestroyImmediate(child.gameObject);
                else
#endif
                    Object.Destroy(child.gameObject);
            }

            return parent;
        }

        // 批量设置子对象激活状态
        public static Transform SetChildrenActive(this Transform parent, bool active)
        {
            foreach (Transform child in parent) child.gameObject.SetActive(active);
            return parent;
        }

        // 获取所有子对象的Transform
        public static Transform[] GetAllChildren(this Transform parent)
        {
            var children = new Transform[parent.childCount];
            for (var i = 0; i < parent.childCount; i++) children[i] = parent.GetChild(i);
            return children;
        }

        // 按索引获取子对象
        public static Transform GetChildAtIndex(this Transform parent, int index)
        {
            return index >= 0 && index < parent.childCount ? parent.GetChild(index) : null;
        }

        // 获取第一个子对象
        public static Transform GetFirstChild(this Transform parent)
        {
            return parent.childCount > 0 ? parent.GetChild(0) : null;
        }

        // 获取最后一个子对象
        public static Transform GetLastChild(this Transform parent)
        {
            return parent.childCount > 0 ? parent.GetChild(parent.childCount - 1) : null;
        }

        // 获取子对象组件（仅子对象，不包含自己）
        public static T GetComponentInChildrenOnly<T>(this Transform parent, bool includeInactive = false)
            where T : Component
        {
            foreach (Transform child in parent)
            {
                var component = child.GetComponentInChildren<T>(includeInactive);
                if (component != null) return component;
            }

            return null;
        }

        // 按名称排序子对象（自然排序）
        public static Transform SortChildrenByName(this Transform parent)
        {
            var children = parent.GetAllChildren().OrderBy(t => t.name).ToList();
            for (var i = 0; i < children.Count; i++) children[i].SetSiblingIndex(i);
            return parent;
        }

        // 按X坐标排序子对象
        public static Transform SortChildrenByPositionX(this Transform parent)
        {
            var children = parent.GetAllChildren().OrderBy(t => t.position.x).ToList();
            for (var i = 0; i < children.Count; i++) children[i].SetSiblingIndex(i);
            return parent;
        }

        // 按Y坐标排序子对象
        public static Transform SortChildrenByPositionY(this Transform parent)
        {
            var children = parent.GetAllChildren().OrderBy(t => t.position.y).ToList();
            for (var i = 0; i < children.Count; i++) children[i].SetSiblingIndex(i);
            return parent;
        }

        // 按Z坐标排序子对象
        public static Transform SortChildrenByPositionZ(this Transform parent)
        {
            var children = parent.GetAllChildren().OrderBy(t => t.position.z).ToList();
            for (var i = 0; i < children.Count; i++) children[i].SetSiblingIndex(i);
            return parent;
        }

        #endregion
    }
}