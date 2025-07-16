using System.Collections.Generic;
using UnityEngine;

namespace CnoomFrameWork.Base.Container
{
    public class RootContainer : BaseContainer
    {
        protected Dictionary<string, ChildContainer> ChildContainers = new();

        /// <summary>
        ///     创建一个子容器并将其添加到当前根容器的子容器集合中。
        /// </summary>
        /// <param name="name">子容器的名称，用于唯一标识该子容器。</param>
        /// <returns>返回创建的子容器实例。</returns>
        public virtual ChildContainer CreateChildContainer(string name)
        {
            var childContainer = new ChildContainer(this);
            ChildContainers[name] = childContainer;
            return childContainer;
        }

        /// <summary>
        ///     释放并移除指定名称的子容器。
        /// </summary>
        /// <param name="name">要释放的子容器的名称。</param>
        /// <returns>如果成功找到并释放子容器，则返回 true；否则返回 false。</returns>
        public virtual bool RemoveChildContainer(string name)
        {
            if (ChildContainers.TryGetValue(name, out var childContainer))
            {
                childContainer.Dispose();
                ChildContainers.Remove(name);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     获取指定名称的子容器实例。
        /// </summary>
        /// <param name="name">要获取的子容器的名称。</param>
        /// <returns>如果找到指定名称的子容器，则返回该子容器实例；否则返回 null 并记录错误日志。</returns>
        public ChildContainer GetChildContainer(string name)
        {
            if (!ChildContainers.ContainsKey(name))
            {
                Debug.LogError("ChildContainer not found: " + name);
                return null;
            }

            return ChildContainers[name];
        }
    }
}