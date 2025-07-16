using UnityEngine.Events;
using UnityEngine.UI;

namespace CnoomFrameWork.Modules.UiModule.UiPart
{
    public class CButton : Button
    {
        /// <summary>
        ///     所有该类型按钮点击都会触发的事件
        /// </summary>
        public static UnityAction OnAllClick;

        protected override void Awake()
        {
            onClick.AddListener(OnClick);
        }


        private void OnClick()
        {
            OnAllClick?.Invoke();
        }
    }
}