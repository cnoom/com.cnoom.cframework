using System;
using CnoomFrameWork.Base.Container;
using CnoomFrameWork.Base.Log;
using CnoomFrameWork.Core.Base.DelayManager;
using UnityEngine;

namespace CnoomFrameWork.Core
{
    public static class AppExtensions
    {

        #region Delay

        public static DelayManager.CancellationToken Delay(this App app, float delay, Action action, GameObject bindGameObject = null)
        {
            return DelayManager.Instance.RegisterTimeDelay(delay, action, bindGameObject);
        }

        public static DelayManager.CancellationToken Delay(this App app, int frame, Action action, GameObject bindGameObject = null)
        {
            return DelayManager.Instance.RegisterFrameDelay(frame, action, bindGameObject);
        }

        public static void Cancel(this App app, DelayManager.CancellationToken token)
        {
            DelayManager.Instance.CancelDelay(token);
        }

        #endregion
    }
}