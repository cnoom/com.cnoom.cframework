using System;
using System.Collections.Generic;
using CnoomFrameWork.Base.Container;
using UnityEngine;

namespace CnoomFrameWork.Core.UnityExtensions
{
    /// <summary>
    /// UnityContainer 是一个用于管理 Unity 游戏对象的容器类，提供了对游戏对象及其组件的便捷访问和管理功能。
    /// 该类通过字典结构存储游戏对象，并支持按名称获取游戏对象、组件或变换组件。
    /// 提供了添加、移除和清理游戏对象的方法，同时实现了 IDisposable 接口以确保资源正确释放。
    /// 内部使用 GameObjectContainer 类封装游戏对象及其相关组件，优化组件的获取和存储过程。
    /// </summary>
    public class UnityContainer : IDisposable
    {
        private readonly Dictionary<string, GameObjectContainer> _gameObjects = new Dictionary<string, GameObjectContainer>();

        public T GetComponent<T>(string name) where T : Component => _gameObjects[name].GetComponent<T>();

        public Transform GetTransform(string name) => _gameObjects[name].Transform;

        public GameObject GetGameObject(string name) => _gameObjects[name].GameObject;

        public void AddGameObject(GameObject gameObject) => _gameObjects.Add(gameObject.name, new GameObjectContainer
        {
            GameObject = gameObject
        });

        public void AddGameObject(string name, GameObject gameObject) => _gameObjects.Add(name, new GameObjectContainer
        {
            GameObject = gameObject
        });
        /// <summary>
        /// 移除指定名称的游戏对象及其相关资源。
        /// 如果容器中不存在指定名称的游戏对象，则返回 false；否则移除该游戏对象并返回 true。
        /// 在移除过程中，会调用与该游戏对象关联的 GameObjectContainer 的 Dispose 方法以清理资源。
        /// </summary>
        /// <param name="name">要移除的游戏对象的名称。</param>
        /// <returns>如果成功移除游戏对象，则返回 true；如果未找到对应名称的游戏对象，则返回 false。</returns>
        public bool RemoveGameObject(string name)
        {
            if(!_gameObjects.TryGetValue(name, out GameObjectContainer gameObjectContainer)) return false;
            gameObjectContainer.Dispose();
            _gameObjects.Remove(name);
            return true;
        }

        /// <summary>
        /// 释放 UnityContainer 中所有游戏对象及其相关资源。
        /// 该方法会遍历容器中存储的所有 GameObjectContainer 实例，并调用其 Dispose 方法以清理组件缓存。
        /// 调用此方法后，容器中的所有游戏对象及其组件将被清理，确保资源得到正确释放。
        /// </summary>
        public void Dispose()
        {
            foreach (GameObjectContainer gameObjectContainer in _gameObjects.Values)
            {
                gameObjectContainer.Dispose();
            }
        }

        /// <summary>
        /// GameObjectContainer 是一个用于封装 Unity 游戏对象及其相关组件的内部类，提供对游戏对象组件的缓存和便捷访问功能。
        /// 该类通过字典结构存储游戏对象的组件，优化了组件的获取过程，避免重复调用 Unity 的 GetComponent 方法。
        /// 提供了按类型获取组件的方法，并在需要时动态加载和缓存组件。
        /// 实现了 IDisposable 接口，确保在释放资源时能够清空组件缓存，从而避免潜在的内存泄漏问题。
        /// </summary>
        private class GameObjectContainer : IDisposable
        {
            public GameObject GameObject;
            public Dictionary<Type, Component> Components = new Dictionary<Type, Component>();

            public Transform Transform => GameObject.transform;

            /// <summary>
            /// 获取指定类型的游戏对象组件。如果组件已缓存，则直接返回缓存的组件；否则从游戏对象中获取组件并缓存。
            /// 该方法通过内部字典存储组件，避免重复调用 Unity 的 GetComponent 方法，从而提高性能。
            /// </summary>
            /// <typeparam name="T">要获取的组件的类型，必须是 UnityEngine.Component 的子类。</typeparam>
            /// <returns>返回指定类型的游戏对象组件。如果游戏对象中不存在该类型的组件，则返回 null。</returns>
            public T GetComponent<T>() where T : Component
            {
                if(Components.TryGetValue(typeof(T), out Component component))
                {
                    return (T)component;
                }
                T tComponent = GameObject.GetComponent<T>();
                Components.Add(typeof(T), tComponent);
                return tComponent;
            }

            /// <summary>
            /// 释放 UnityContainer 中所有游戏对象及其相关资源。
            /// 该方法会遍历内部存储的所有 GameObjectContainer 实例，并调用其 Dispose 方法以清理组件缓存。
            /// 确保在不再需要 UnityContainer 时调用此方法，以避免潜在的内存泄漏问题。
            /// </summary>
            public void Dispose()
            {
                Components.Clear();
            }
        }
    }
}