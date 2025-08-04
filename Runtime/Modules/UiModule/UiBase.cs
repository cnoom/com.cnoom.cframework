using CnoomFrameWork.Base.Container;
using CnoomFrameWork.Base.Events;
using Modules.UiModule;
using UnityEngine;

namespace CnoomFrameWork.Modules.UiModule.UiPart
{
    public class UiBase : MonoBehaviour
    {
        public UiConfig uiConfig;

        [HideInInspector] public UiAnimation uiAnimation;

        // 生命周期方法
        private void Awake()
        {
        }

        private void Start()
        {
        }

        public void Generate()
        {
            Injector.Inject(this);
            if (TryGetComponent(out UiAnimation animation))
            {
                uiAnimation = animation;
                uiAnimation.InitAnimationPart();
            }

            OnGenerate();
        }


        public virtual void OnGenerate()
        {
        }

        public virtual void OnEnter(UiParameter parameter)
        {
        }

        public virtual void OnExit()
        {
        }
    }
}