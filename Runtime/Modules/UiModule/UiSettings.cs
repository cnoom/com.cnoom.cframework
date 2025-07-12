using System.Collections.Generic;
using System.Linq;
using CnoomFrameWork.Modules.UiModule.UiPart;
using UnityEngine;

namespace CnoomFrameWork.Modules.UiModule
{
    public class UiSettings : ScriptableObject
    {
        public const string FileName = "UiSettings";
        public Canvas canvas;
        public List<UiBase> panels = new();

        public T GetUi<T>() where T : UiBase
        {
            if (panels == null)
            {
                throw new System.Exception("ui list is null");
            }

            foreach (T panel in panels.OfType<T>())
            {
                return panel;
            }

            throw new System.Exception("ui not found");
        }
    }
}