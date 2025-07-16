using System;

namespace CnoomFrameWork.Base.Container
{
    public class ChildContainer : RootContainer
    {
        private readonly RootContainer _parent;

        public ChildContainer(RootContainer parent)
        {
            _parent = parent;
        }

        /// <summary>
        ///     解析指定类型的对象实例（非泛型版本）
        /// </summary>
        public override object Resolve(Type type)
        {
            lock (LockObject)
            {
                // 首先检查当前容器的单例字典
                if (Singletons.TryGetValue(type, out var instance)) return instance;

                // 然后检查当前容器的工厂方法字典
                if (Factories.TryGetValue(type, out var factory)) return factory(this);

                // 如果有父容器，递归查找
                if (_parent != null) return _parent.Resolve(type);

                // 未找到注册项，抛出异常
                throw new InvalidOperationException($"类型 {type.FullName} 未在容器中注册");
            }
        }
    }
}