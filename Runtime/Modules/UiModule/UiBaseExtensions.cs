using CnoomFrameWork.Base.Events;
using CnoomFrameWork.Modules.UiModule;
using CnoomFrameWork.Modules.UiModule.UiPart;

namespace Modules.UiModule
{
    public static class UiBaseExtensions
    {

        public static void CloseLayerTop(this UiBase ui,string layerName = null)
        {
            if (string.IsNullOrEmpty(layerName)) layerName = ui.uiConfig.layer;
            EventManager.Publish(new UIModule.CloseLayerTopCommand(layerName));
        }

        public static void CloseLayerBottom(this UiBase ui,string layerName = null)
        {
            if (string.IsNullOrEmpty(layerName)) layerName = ui.uiConfig.layer;
            EventManager.Publish(new UIModule.CloseLayerBottomCommand(layerName));
        }
        
        public static void ClearLayer(this UiBase ui,string layerName = null)
        {
            if (string.IsNullOrEmpty(layerName)) layerName = ui.uiConfig.layer;
            EventManager.Publish(new UIModule.ClearLayerCommand(layerName));
        }
    }
}