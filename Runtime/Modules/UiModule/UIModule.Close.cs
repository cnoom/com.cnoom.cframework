using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CnoomFrameWork.Modules.UiModule.UiPart;
using UnityEngine;

namespace CnoomFrameWork.Modules.UiModule
{
    public partial class UIModule
    {
        // 关闭顶层界面
        public void CloseTop(EUiLayer layer)
        {
            if(layerStack[layer].Count == 0) return;

            BaseUi ui = layerStack[layer].Pop();
            Type panelType = ui.GetType();

            // 调用生命周期
            ui.OnExit();
            ui.gameObject.SetActive(false);

            // 回收入池
            RecyclePanel(ui);
            activePanels.Remove(panelType);

            // 恢复下一层界面
            if(layerStack[layer].Count > 0)
            {
                layerStack[layer].Peek().OnResume();
            }
        }

        /// <summary>
        ///     关闭指定类型的界面
        /// </summary>
        /// <typeparam name="T">要关闭的面板类型</typeparam>
        /// <param name="immediate">是否立即关闭（false则尝试先触发动画）</param>
        public void Close<T>(bool immediate = true) where T : BaseUi
        {
            Type targetType = typeof(T);
            if(!activePanels.ContainsKey(targetType))
            {
                Debug.LogWarning($"尝试关闭未打开的界面: {targetType.Name}");
                return;
            }

            BaseUi ui = activePanels[targetType];
            ClosePanel(ui, immediate);
        }

        public void Close(BaseUi ui, bool immediate = true)
        {
            ClosePanel(ui, immediate);
        }

        /// <summary>
        ///     关闭指定实例的界面
        /// </summary>
        private void ClosePanel(BaseUi ui, bool immediate)
        {
            if(ui == null) return;

            // 处理栈结构
            List<BaseUi> panelList = new List<BaseUi>(layerStack[ui.Layer]);
            panelList.Remove(ui);
            layerStack[ui.Layer] = new Stack<BaseUi>(panelList.Reverse<BaseUi>());

            // 生命周期调用
            if(immediate)
            {
                ui.OnExit();
                FinalizeClose(ui);
            }
            if(ui is AnimationUi animationUi)
            {
                app.StartCoroutine(CloseWithAnimation(animationUi));
            }
        }

        /// <summary>
        ///     关闭所有界面
        /// </summary>
        public void CloseAll()
        {
            foreach (EUiLayer eUiLayer in layerStack.Keys)
            {
                while (layerStack[eUiLayer].Count > 0)
                {
                    ClosePanel(layerStack[eUiLayer].Peek(), true);
                }
            }
        }

        /// <summary>
        ///     关闭到指定界面（保留指定界面及以下的）
        /// </summary>
        public void CloseTo<T>(EUiLayer layer) where T : BaseUi
        {
            Type targetType = typeof(T);
            if(!activePanels.ContainsKey(targetType))
            {
                Debug.LogWarning($"目标界面未打开: {targetType.Name}");
                return;
            }

            List<BaseUi> keepList = new List<BaseUi>();
            var foundTarget = false;

            foreach (BaseUi panel in layerStack[layer].Reverse())
            {
                if(panel.GetType() == targetType)
                {
                    foundTarget = true;
                }

                if(foundTarget)
                {
                    keepList.Add(panel);
                }
                else
                {
                    ClosePanel(panel, true);
                }
            }

            layerStack[layer] = new Stack<BaseUi>(keepList);
            RefreshPanelDepths();
        }

        private IEnumerator CloseWithAnimation(AnimationUi ui)
        {
            // 禁用交互
            SetPanelInteractable(ui, false);

            // 执行关闭动画
            yield return ui.PlayExitAnimation();

            // 确保没有被立即关闭
            if(!activePanels.ContainsValue(ui)) yield break;
            ui.OnExit();
            FinalizeClose(ui);
        }

        private void FinalizeClose(BaseUi ui)
        {
            // 从活动列表中移除
            Type panelType = ui.GetType();
            activePanels.Remove(panelType);

            EUiLayer layer = ui.Layer;
            // 回收处理
            RecyclePanel(ui);

            // 刷新层级
            RefreshPanelDepths();

            // 恢复栈顶界面
            if(layerStack[layer].Count > 0)
            {
                BaseUi newTop = layerStack[layer].Peek();
                SetPanelInteractable(newTop, true);
                newTop.OnResume();
            }
        }

        private void SetPanelInteractable(BaseUi ui, bool state)
        {
            CanvasGroup canvasGroup = ui.GetComponent<CanvasGroup>();
            if(canvasGroup)
            {
                canvasGroup.interactable = state;
                canvasGroup.blocksRaycasts = state;
            }
        }
    }
}