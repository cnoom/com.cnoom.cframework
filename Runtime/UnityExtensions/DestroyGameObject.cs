using System;
using UnityEngine;

namespace CnoomFrameWork.Core.UnityExtensions
{
    public class DestroyGameObject : MonoBehaviour
    {
        private Action _action;

        public void SetAction(Action action)
        {
            _action = action;
        }

        private void OnDestroy()
        {
            _action?.Invoke();
        }
    }
}