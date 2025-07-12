using System;
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
        private readonly Dictionary<EUiLayer, Stack<UiBase>> _layerStack = new();

        private Transform _canvasTransform;
        private UiSettings _uiSettings;

        [Inject] private AssetsService AssetsService { get; set; }

        protected override void OnInitialize()
        {
            _uiSettings = AssetsService.LoadAsset<UiSettings>(UiSettings.FileName);
            _canvasTransform = Object.Instantiate(_uiSettings.canvas, App.transform).transform;

            foreach (EUiLayer layer in Enum.GetValues(typeof(EUiLayer)))
            {
                GameObject go = new GameObject(layer.ToString());
                go.transform.SetParent(_canvasTransform);
                _layerStack[layer] = new Stack<UiBase>();
            }

            EventManager.Publish(this);
        }

        private T CreateUi<T>() where T : UiBase
        {
            GameObject prefab = _uiSettings.GetUi<T>().gameObject;
            GameObject instance = Object.Instantiate(prefab, _canvasTransform);
            T ui = instance.GetComponent<T>();
            ui.Generate();
            return ui;
        }

        private void RemoveUi(UiBase ui)
        {
            Object.Destroy(ui.gameObject);
        }

        /// 刷新层级深度的方法
        private void RefreshUiDepths()
        {
            foreach (EUiLayer layer in _layerStack.Keys)
            {
                Transform layerTransform = _canvasTransform.Find(layer.ToString());
                layerTransform.DetachChildren();
                foreach (UiBase basePanel in _layerStack[layer].Reverse())
                {
                    basePanel.transform.SetParent(layerTransform);
                }
            }
        }

        private bool HasUi(EUiLayer uiLayer, string gameObjectName, out UiBase ui)
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
    }
}