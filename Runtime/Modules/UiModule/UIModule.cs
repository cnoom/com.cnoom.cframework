using System.Collections.Generic;
using System.Linq;
using CnoomFrameWork.Base.Container;
using CnoomFrameWork.Base.Events;
using CnoomFrameWork.Core;
using CnoomFrameWork.Modules.AddressableModule;
using CnoomFrameWork.Modules.UiModule.UiPart;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CnoomFrameWork.Modules.UiModule
{
    public partial class UIModule : Module
    {
        // 使用栈管理界面层级
        private readonly Dictionary<string, Stack<UiBase>> _layerStack = new();

        private Transform _canvasTransform;
        private UiSettings _uiSettings;

        [Inject] private AssetsService AssetsService { get; set; }

        protected override void OnInitialize()
        {
            _uiSettings = AssetsService.LoadAsset<UiSettings>(UiSettings.FileName);
            _canvasTransform = Object.Instantiate(_uiSettings.canvas, App.transform).transform;

            foreach (var layer in _uiSettings.uiLayers)
            {
                var go = new GameObject(layer);
                go.transform.SetParent(_canvasTransform);
                _layerStack[layer] = new Stack<UiBase>();
            }

            EventManager.Publish(this);
        }

        private UiBase CreateUi(string uiName, string objectName)
        {
            GameObject prefab = _uiSettings.GetUi(uiName).gameObject;
            if (prefab == null) return null;
            GameObject instance = Object.Instantiate(prefab, _canvasTransform);
            if (!string.IsNullOrEmpty(objectName)) instance.name = objectName;
            var ui = instance.GetComponent<UiBase>();
            Injector.Inject(ui);
            EventManager.Register(ui);
            ui.Generate();
            return ui;
        }

        private void RemoveUi(UiBase ui)
        {
            ui.OnExit();
            EventManager.Unregister(ui);
            Object.Destroy(ui.gameObject);
        }

        /// 刷新层级深度的方法
        private void RefreshUiDepths()
        {
            foreach (var layer in _layerStack.Keys)
            {
                var layerTransform = _canvasTransform.Find(layer);
                layerTransform.DetachChildren();
                foreach (var basePanel in _layerStack[layer].Reverse()) basePanel.transform.SetParent(layerTransform);
            }
        }

        private bool HasUi(UiConfig uiConfig, out UiBase ui)
        {
            foreach (var uiBase in _layerStack[uiConfig.layer])
            {
                if (uiBase.uiConfig.uiName != uiConfig.uiName) continue;
                ui = uiBase;
                return true;
            }

            ui = null;
            return false;
        }

        /// <summary>
        ///     关闭界面事件
        /// </summary>
        public struct CloseUiEvent
        {
            /// <summary>
            ///     关闭的界面层级
            /// </summary>
            public string LayerType;

            /// <summary>
            ///     该层级剩余的界面数量
            /// </summary>
            public int LayerCount;

            public CloseUiEvent(string layerType, int layerCount)
            {
                LayerType = layerType;
                LayerCount = layerCount;
            }
        }

        /// <summary>
        ///     关闭某层最上方ui命令
        /// </summary>
        public struct CloseLayerTopCommand
        {
            public string Layer;

            public CloseLayerTopCommand(string layer)
            {
                Layer = layer;
            }
        }

        public struct OpenUiCommand
        {
            public string UiName;
            public UiParameter Parameters;
            public string ObjectName;

            public OpenUiCommand(string uiName)
            {
                UiName = uiName;
                Parameters = null;
                ObjectName = null;
            }

            public OpenUiCommand(string uiName, UiParameter parameters = null, string objectName = null)
            {
                UiName = uiName;
                Parameters = parameters;
                ObjectName = objectName;
            }

            public OpenUiCommand(string uiName, string objectName = null, UiParameter parameters = null)
            {
                UiName = uiName;
                Parameters = parameters;
                ObjectName = objectName;
            }
        }

        public struct OpenUiEvent
        {
            /// <summary>
            /// 打开的ui
            /// </summary>
            public UiBase UiBase;

            /// <summary>
            /// 打开的ui层ui数量
            /// </summary>
            public int LayerCount;

            public OpenUiEvent(UiBase uiBase, int layerCount)
            {
                UiBase = uiBase;
                LayerCount = layerCount;
            }
        }
        
        /// <summary>
        /// 清理所有ui,不触发ui移除事件
        /// </summary>
        public struct ClearUiCommand
        {
        }
    }
}