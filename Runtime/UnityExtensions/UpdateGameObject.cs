using System;
using UnityEngine;

namespace CnoomFrameWork.Core.UnityExtensions
{
    /// <summary>
    /// 提供在Unity更新循环中执行自定义操作的GameObject组件
    /// 1. 通过SetAction设置需要在每帧执行的回调方法
    /// 2. 常用于需要在Update中执行的非MonoBehaviour逻辑
    /// </summary>
    public class UpdateGameObject : MonoBehaviour
    {
        private Action _action;
        

        /// <summary>
        /// 设置每帧更新的回调方法
        /// </summary>
        /// <param name="action">需要在每帧执行的无参方法</param>
        public void SetAction(Action action)
        {
            _action = action;   
        }
        
        private void Update()
        {
            _action?.Invoke();
        }
    }
}