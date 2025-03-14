using UnityEngine;

namespace CnoomFrameWork.Modules.UiModule.UiPart
{
    public class BaseUi : MonoBehaviour
    {
        [SerializeField]
        private EUiLayer _layer = EUiLayer.Normal;
        public EUiLayer Layer => _layer;

        // 生命周期方法
        public virtual void OnEnter(object param = null) { }
        public virtual void OnPause() { }
        public virtual void OnResume() { }
        public virtual void OnExit() { }
    }
}