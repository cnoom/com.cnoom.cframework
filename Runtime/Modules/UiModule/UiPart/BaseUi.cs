﻿using CnoomFrameWork.Base.IoC;
using CnoomFrameWork.Core;
using UnityEngine;
using UnityEngine.Scripting;

namespace CnoomFrameWork.Modules.UiModule.UiPart
{
    public class BaseUi : MonoBehaviour
    {
        [SerializeField]
        private EUiLayer _layer = EUiLayer.Normal;
        public EUiLayer Layer => _layer;

        [Inject, Preserve] protected UIModule UIModule { get; set; }
        // 生命周期方法
        private void Awake() { }
        private void Start() { }
        public virtual void OnGenerate()
        {
            App.Instance.Inject(this);
        }
        public virtual void OnEnter(object param = null) { }
        public virtual void OnPause() { }
        public virtual void OnResume() { }
        public virtual void OnExit() { }
    }
}