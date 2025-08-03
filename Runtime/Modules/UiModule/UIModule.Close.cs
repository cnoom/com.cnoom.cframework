using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CnoomFrameWork.Base.Events;
using CnoomFrameWork.Modules.UiModule.UiPart;
using UnityEngine;
using UnityEngine.Scripting;

namespace CnoomFrameWork.Modules.UiModule
{
    public partial class UIModule
    {
        [EventSubscriber(typeof(ClearUiCommand)), Preserve]
        private void OnClearUi(ClearUiCommand command)
        {
            foreach (string stackKey in _layerStack.Keys)
            {
                foreach (UiBase uiBase in _layerStack[stackKey])
                {
                    RemoveUi(uiBase);
                }
            }

            foreach (var key in _layerStack.Keys)
            {
                _layerStack[key].Clear();
            }
        }

        [EventSubscriber(typeof(CloseLayerTopCommand)), Preserve]
        private void OnCloseUiTopLayer(CloseLayerTopCommand command)
        {
            if (_layerStack[command.Layer].Count <= 0) return;
            var ui = _layerStack[command.Layer].Peek();
            CloseUi(ui);
        }

        /// <summary>
        ///     关闭指定实例的界面
        /// </summary>
        private void CloseUi(UiBase ui)
        {
            if (ui == null) return;
            // 处理栈结构
            RemoveInStack(ui);
            Closing(ui);
        }

        private void Closing(UiBase ui)
        {
            if (ui.uiAnimation && ui.uiAnimation.hasCloseAnimation)
            {
                App.StartCoroutine(CloseWithAnimation(ui));
                return;
            }

            FinalizeClose(ui);
        }

        /// <summary>
        ///     移除指定界面栈结构管理
        /// </summary>
        /// <param name="ui"></param>
        private void RemoveInStack(UiBase ui)
        {
            var panelList = new List<UiBase>(_layerStack[ui.uiConfig.layer]);
            panelList.Remove(ui);
            _layerStack[ui.uiConfig.layer] = new Stack<UiBase>(panelList.Reverse<UiBase>());
        }

        private IEnumerator CloseWithAnimation(UiBase ui)
        {
            // 禁用交互
            SetPanelInteractable(ui, false);

            // 执行关闭动画
            yield return ui.uiAnimation.PlayExitAnimation();

            // 确保没有被立即关闭
            FinalizeClose(ui);
        }

        private void FinalizeClose(UiBase ui)
        {
            var layer = ui.uiConfig.layer;

            RemoveUi(ui);

            // 刷新层级
            RefreshUiDepths();

            // 触发关闭ui事件
            EventManager.Publish(new CloseUiEvent
            {
                LayerType = ui.uiConfig.layer,
                LayerCount = _layerStack[ui.uiConfig.layer].Count
            });

            // 恢复栈顶界面
            if (_layerStack[layer].Count > 0)
            {
                var newTop = _layerStack[layer].Peek();
                SetPanelInteractable(newTop, true);
            }
        }

        private void SetPanelInteractable(UiBase ui, bool state)
        {
            var canvasGroup = ui.GetComponent<CanvasGroup>();
            if (canvasGroup)
            {
                canvasGroup.interactable = state;
                canvasGroup.blocksRaycasts = state;
            }
        }
    }
}