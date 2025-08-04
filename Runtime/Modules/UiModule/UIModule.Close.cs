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
            foreach (string layer in _layerLinkedList.Keys)
            {
                ClearLayer(layer, command.HasEvent);
            }
        }

        [EventSubscriber(typeof(CloseLayerTopCommand)), Preserve]
        private void OnCloseUiTopLayer(CloseLayerTopCommand command)
        {
            if (_layerLinkedList[command.Layer].Count <= 0) return;
            var ui = _layerLinkedList[command.Layer].Last();
            CloseUi(ui);
        }

        [EventSubscriber(typeof(CloseLayerBottomCommand)), Preserve]
        private void OnCloseUiBottomLayer(CloseLayerBottomCommand command)
        {
            if (_layerLinkedList[command.Layer].Count <= 0) return;
            var ui = _layerLinkedList[command.Layer].First();
            CloseUi(ui);
        }

        [EventSubscriber(typeof(CloseUiCommand)), Preserve]
        private void OnCloseUi(CloseUiCommand command)
        {
            CloseUi(command.UiBase);
        }

        [EventSubscriber(typeof(ClearLayerCommand)), Preserve]
        private void OnClearLayer(ClearLayerCommand command)
        {
            ClearLayer(command.Layer, command.HasEvent);
        }

        private void ClearLayer(string layer, bool hasEvent = true)
        {
            if (_layerLinkedList[layer].Count <= 0) return;
            foreach (var ui in _layerLinkedList[layer])
            {
                CloseUi(ui, hasEvent);
            }
        }

        /// <summary>
        ///     关闭指定实例的界面
        /// </summary>
        private void CloseUi(UiBase ui, bool hasEvent = true)
        {
            if (ui == null) return;
            RemoveUiInLinkedList(ui);
            Closing(ui, hasEvent);
        }
        
        private void RemoveUi(UiBase ui)
        {
            ui.OnExit();
            EventManager.Unregister(ui);
            Object.Destroy(ui.gameObject);
        }

        private void Closing(UiBase ui, bool hasEvent = true)
        {
            if (ui.uiAnimation && ui.uiAnimation.hasCloseAnimation)
            {
                App.StartCoroutine(CloseWithAnimation(ui));
                return;
            }

            FinalizeClose(ui, hasEvent);
        }

        /// <summary>
        ///     移除指定界面栈结构管理
        /// </summary>
        /// <param name="ui"></param>
        private void RemoveUiInLinkedList(UiBase ui)
        {
            LinkedList<UiBase> list = _layerLinkedList[ui.uiConfig.layer];
            list.Remove(ui);
        }

        private IEnumerator CloseWithAnimation(UiBase ui, bool hasEvent = true)
        {
            // 禁用交互
            SetPanelInteractable(ui, false);

            // 执行关闭动画
            yield return ui.uiAnimation.PlayExitAnimation();

            // 确保没有被立即关闭
            FinalizeClose(ui, hasEvent);
        }

        private void FinalizeClose(UiBase ui, bool hasEvent = true)
        {
            var layer = ui.uiConfig.layer;

            RemoveUi(ui);

            // 刷新层级
            RefreshUiDepths();

            // 触发关闭ui事件
            if (hasEvent)
            {
                EventManager.Publish(new CloseUiEvent
                {
                    LayerType = ui.uiConfig.layer,
                    LayerCount = _layerLinkedList[ui.uiConfig.layer].Count
                });
            }


            // 恢复栈顶界面
            if (_layerLinkedList[layer].Count > 0)
            {
                var newTop = _layerLinkedList[layer].Last();
                SetPanelInteractable(newTop, true);
            }
        }

        private void SetPanelInteractable(UiBase ui, bool state)
        {
            var canvasGroup = ui.GetComponent<CanvasGroup>();
            if (!canvasGroup) return;
            canvasGroup.interactable = state;
            canvasGroup.blocksRaycasts = state;
        }
    }
}