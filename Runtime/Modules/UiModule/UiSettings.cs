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
        public List<BaseUi> panels = new List<BaseUi>();
        public T GetPanel<T>() where T : BaseUi
        {
            if(panels == null)
            {
                throw new System.Exception("panels is null");
            }
            foreach (T panel in panels.OfType<T>())
            {
                return panel;
            }
            throw new System.Exception("panel not found");
        }
    }
}