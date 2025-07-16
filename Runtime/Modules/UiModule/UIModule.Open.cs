using System.Collections;
using CnoomFrameWork.Base.Events;
using CnoomFrameWork.Modules.UiModule.UiPart;
using UnityEngine.Scripting;

namespace CnoomFrameWork.Modules.UiModule
{
    public partial class UIModule
    {
        [EventSubscriber(typeof(OpenUiCommand))]
        [Preserve]
        private void OnOpenCommand(OpenUiCommand command)
        {
            // 已存在检查
            var uiPrefab = _uiSettings.GetUi(command.UiName);
            var layer = uiPrefab.uiConfig.layer;
            if (!uiPrefab.uiConfig.allowMultiple &&
                HasUi(uiPrefab.uiConfig.layer, uiPrefab.gameObject.name, out var ui))
            {
                RemoveInStack(ui);
            }
            else
            {
                ui = CreateUi(command.UiName);
                ui.gameObject.SetActive(true);
                var panelParent = _canvasTransform.Find(layer);
                ui.transform.SetParent(panelParent);
            }

            // 加入管理
            _layerStack[layer].Push(ui);
            RefreshUiDepths();

            // 调用生命周期
            App.StartCoroutine(InitializePanel(ui, command.Parameters));
        }
        // 打开界面

        private IEnumerator InitializePanel(UiBase ui, UiParameter param)
        {
            // 初始状态设置
            ui.gameObject.SetActive(true);
            SetPanelInteractable(ui, false);

            ui.OnEnter(param);
            if (ui.uiAnimation && ui.uiAnimation.hasOpenAnimation) yield return ui.uiAnimation.PlayEnterAnimation();

            // 启用交互
            SetPanelInteractable(ui, true);
        }
    }
}