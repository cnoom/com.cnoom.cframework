using System;
using System.Collections;
using CnoomFrameWork.Modules.UiModule.UiPart;
using Transform = UnityEngine.Transform;

namespace CnoomFrameWork.Modules.UiModule
{
    public partial class UIModule
    {
        // 打开界面
        public T Open<T>(object param = null, bool forceNewInstance = false) where T : BaseUi
        {
            Type panelType = typeof(T);

            // 已存在检查
            if(activePanels.ContainsKey(panelType) && !forceNewInstance)
            {
                BringToTop(activePanels[panelType]);
                return activePanels[panelType] as T;
            }

            // 从缓存池获取或创建新实例
            T panel = GetPanelFromPool<T>(forceNewInstance);
            EUiLayer layer = panel.Layer;
            panel.gameObject.SetActive(true);
            Transform panelParent = canvasTransform.Find(layer.ToString());
            panel.transform.SetParent(panelParent);
            // 暂停当前顶层界面
            if(layerStack[layer].Count > 0)
            {
                layerStack[layer].Peek().OnPause();
            }

            // 加入管理
            layerStack[layer].Push(panel);
            activePanels[panelType] = panel;
            RefreshPanelDepths();

            // 调用生命周期
            app.StartCoroutine(InitializePanel(panel, param));
            return panel;
        }

        private IEnumerator InitializePanel(BaseUi ui, object param)
        {
            // 初始状态设置
            ui.gameObject.SetActive(true);
            SetPanelInteractable(ui, false);

            // 播放进入动画
            if(ui is AnimationUi animationUi)
            {
                yield return animationUi.PlayEnterAnimation();
            }

            // 启用交互
            SetPanelInteractable(ui, true);

            // 正式调用OnEnter
            ui.OnEnter(param);
        }
    }
}