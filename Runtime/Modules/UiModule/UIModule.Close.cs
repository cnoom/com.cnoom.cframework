using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CnoomFrameWork.Base.Events;
using CnoomFrameWork.Modules.UiModule.UiPart;
using UnityEngine;

namespace CnoomFrameWork.Modules.UiModule
{
    public partial class UIModule
    {
        // 关闭顶层界面
        public void CloseTopUi(string layer)
        {
            if (_layerStack[layer].Count == 0) return;

            UiBase ui = _layerStack[layer].Pop();
            Closing(ui);
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

            ui.OnExit();
            FinalizeClose(ui);
        }

        /// <summary>
        ///     移除指定界面栈结构管理
        /// </summary>
        /// <param name="ui"></param>
        private void RemoveInStack(UiBase ui)
        {
            List<UiBase> panelList = new List<UiBase>(_layerStack[ui.uiConfig.layer]);
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
            ui.OnExit();
            FinalizeClose(ui);
        }

        private void FinalizeClose(UiBase ui)
        {
            string layer = ui.uiConfig.layer;
            // 回收处理
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
                UiBase newTop = _layerStack[layer].Peek();
                SetPanelInteractable(newTop, true);
            }
        }

        private void SetPanelInteractable(UiBase ui, bool state)
        {
            CanvasGroup canvasGroup = ui.GetComponent<CanvasGroup>();
            if (canvasGroup)
            {
                canvasGroup.interactable = state;
                canvasGroup.blocksRaycasts = state;
            }
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
            /// 该层级剩余的界面数量
            /// </summary>
            public int LayerCount;
        }
    }
}