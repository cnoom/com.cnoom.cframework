using System;
using System.Collections;
using CnoomFrameWork.Modules.UiModule.UiPart;
using Transform = UnityEngine.Transform;

namespace CnoomFrameWork.Modules.UiModule
{
    public partial class UIModule
    {
        // 打开界面
        public T Open<T>(UiParameter uiParameter = null) where T : UiBase
        {
            // 已存在检查
            var uiPrefab = _uiSettings.GetUi<T>();
            if (!uiPrefab.uiConfig.allowMultiple && HasUi(uiPrefab.uiConfig.layer, uiPrefab.gameObject.name,out UiBase ui))
            {
                RemoveInStack(ui);
                ui.OnEnter(uiParameter);
                return ui as T;
            }

            // 从缓存池获取或创建新实例
            T panel = CreateUi<T>();
            string layer = panel.uiConfig.layer;
            panel.gameObject.SetActive(true);
            Transform panelParent = _canvasTransform.Find(layer);
            panel.transform.SetParent(panelParent);

            // 加入管理
            _layerStack[layer].Push(panel);
            RefreshUiDepths();

            // 调用生命周期
            App.StartCoroutine(InitializePanel(panel, uiParameter));
            return panel;
        }

        private IEnumerator InitializePanel(UiBase ui, UiParameter param)
        {
            // 初始状态设置
            ui.gameObject.SetActive(true);
            SetPanelInteractable(ui, false);
            
            ui.OnEnter(param);
            if (ui.uiAnimation && ui.uiAnimation.hasOpenAnimation)
            {
                yield return ui.uiAnimation.PlayEnterAnimation();
            }
            // 启用交互
            SetPanelInteractable(ui, true);
        }
    }
}