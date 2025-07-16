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

        private UiBase CreateUi(string uiName)
        {
            var prefab = _uiSettings.GetUi(uiName).gameObject;
            var instance = Object.Instantiate(prefab, _canvasTransform);
            var ui = instance.GetComponent<UiBase>();
            ui.Generate();
            return ui;
        }

        private void RemoveUi(UiBase ui)
        {
            ui.OnExit();
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

        private bool HasUi(string uiLayer, string gameObjectName, out UiBase ui)
        {
            foreach (var uiBase in _layerStack[uiLayer])
            {
                if (uiBase.gameObject.name != gameObjectName) continue;
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

            public OpenUiCommand(string uiName, UiParameter parameters)
            {
                UiName = uiName;
                Parameters = parameters;
            }
        }
    }
}