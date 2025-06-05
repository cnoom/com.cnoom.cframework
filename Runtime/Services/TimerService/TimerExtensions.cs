using CnoomFrameWork.Core.UnityExtensions;
using UnityEngine;

namespace CnoomFrameWork.Services.TimerService
{
    public  static class TimerExtensions
    {
        
        /// <summary>
        /// 绑定定时器到GameObject,当GameObject销毁时,定时器会自动取消
        /// </summary>
        /// <param name="timer">定时器</param>
        /// <param name="gameObject">绑定的物体</param>
        public static void Bind(this ITimer timer,GameObject gameObject)
        {
            var destroy = gameObject.AddComponent<DestroyGameObject>();
            destroy.SetAction(timer.Cancel);
            timer.OnCancel += () =>
            {
                UnBind(timer,gameObject);
            };
        }

        public static void UnBind(this ITimer timer, GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out DestroyGameObject destroy))
            {
                destroy.SetAction(null);
                Object.Destroy(destroy);
            }
        }
    }
}