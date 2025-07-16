using System.Collections.Generic;
using CnoomFrameWork.Modules.UiModule.UiPart;
using UnityEngine;

namespace CnoomFrameWork.Modules.UiModule
{
    public class UiSettings : ScriptableObject
    {
        public const string FileName = "UiSettings";
        public Canvas canvas;
        public List<UiBase> panels = new();
        public List<string> uiLayers = new();

        public UiBase GetUi(string uiName)
        {
            if (panels == null) throw new System.Exception("ui list is null");

            foreach (var panel in panels)
                if (panel.uiConfig.uiName == uiName)
                    return panel;

            throw new System.Exception("ui not found");
        }
    }
}