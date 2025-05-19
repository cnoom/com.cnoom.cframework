using System;
using System.Collections.Generic;

namespace CnoomFrameWork.Base.IoC
{
    /// <summary>
    /// 表示IoC框架中用于依赖注入的作用域容器。  
    /// 该类通过引入限定作用域的生命周期管理及可释放实例，扩展了基础IoC容器的功能。  
    /// 支持在当前作用域内解析依赖项，并将未解析的请求委托给父级容器处理。
    /// </summary>
    public class ScopeContainer : IoCContainer, IDisposable
    {
        public IIoCContainer ParentContainer { get; }

        private readonly List<IDisposable> _disposables = new();

        public ScopeContainer(IIoCContainer parentContainer)
        {
            ParentContainer = parentContainer;
        }

        public override object Resolve(Type type)
        {
            if(Registrations.TryGetValue(type, out Registration registration))
            {
                object instance = CreateInstance(registration);
                if(instance is IDisposable disposable) _disposables.Add(disposable);
                return instance;
            }
            return ParentContainer.Resolve(type);
        }

        public void Dispose()
        {
            foreach (IDisposable disposable in _disposables) disposable.Dispose();
            _disposables.Clear();
        }
    }
}