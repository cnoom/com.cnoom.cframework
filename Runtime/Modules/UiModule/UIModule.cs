using System;
using System.Collections.Generic;
using System.Linq;
using CnoomFrameWork.Base.Container;
using CnoomFrameWork.Base.Events;
using CnoomFrameWork.Core;
using CnoomFrameWork.Core.Base.DelayManager;
using CnoomFrameWork.Modules.AddressableModule;
using CnoomFrameWork.Modules.UiModule.UiPart;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CnoomFrameWork.Modules.UiModule
{
    public partial class UIModule : Module
    {
        // 当前所有已打开的界面
        private readonly Dictionary<Type, BaseUi> activePanels = new Dictionary<Type, BaseUi>();

        // 使用栈管理界面层级
        private readonly Dictionary<EUiLayer, Stack<BaseUi>> layerStack = new Dictionary<EUiLayer, Stack<BaseUi>>();

        // 界面缓存池
        private readonly Dictionary<Type, Queue<BaseUi>> panelPool = new Dictionary<Type, Queue<BaseUi>>();
        private App app;
        private Transform canvasTransform, poolTransform;
        private UiSettings uiSettings;

        [Inject] private AssetsService AssetsService { get; set; }

        protected override void OnInitialize()
        {
            app = App.Instance;
            poolTransform = new GameObject("PanelPool").transform;
            poolTransform.SetParent(app.transform);

            uiSettings = AssetsService.LoadAsset<UiSettings>(UiSettings.FileName);
            canvasTransform = Object.Instantiate(uiSettings.canvas, app.transform).transform;

            foreach (EUiLayer layer in Enum.GetValues(typeof(EUiLayer)))
            {
                GameObject go = new GameObject(layer.ToString());
                go.transform.SetParent(canvasTransform);
                layerStack[layer] = new Stack<BaseUi>();
            }

            DelayManager.Instance.RegisterFrameDelay(1, () => EventManager.PublishAsync(this));
        }

        // 从缓存池获取界面
        private T GetPanelFromPool<T>(bool forceNew) where T : BaseUi
        {
            Type type = typeof(T);
            if (!forceNew && panelPool.ContainsKey(type) && panelPool[type].Count > 0)
            {
                return panelPool[type].Dequeue() as T;
            }

            GameObject prefab = uiSettings.GetPanel<T>().gameObject;
            GameObject instance = Object.Instantiate(prefab, canvasTransform);
            T baseUi = instance.GetComponent<T>();
            baseUi.OnGenerate();
            return instance.GetComponent<T>();
        }

        // 回收界面到缓存池
        private void RecyclePanel(BaseUi ui)
        {
            ui.transform.SetParent(poolTransform);

            Type type = ui.GetType();
            if (!panelPool.ContainsKey(type))
            {
                panelPool[type] = new Queue<BaseUi>();
            }

            // 保持最大缓存数量（可根据需求调整）
            if (panelPool[type].Count < 5)
            {
                panelPool[type].Enqueue(ui);
            }
            else
            {
                Object.Destroy(ui.gameObject);
            }
        }

        private void BringToTop(BaseUi ui)
        {
            if (layerStack[ui.Layer].Count == 0 || layerStack[ui.Layer].Peek() == ui) return;

            // 创建临时列表来处理栈操作
            List<BaseUi> tempList = new List<BaseUi>(layerStack[ui.Layer]);

            if (tempList.Contains(ui))
            {
                // 移除原有位置
                tempList.Remove(ui);
                // 添加到栈顶（列表末尾）
                tempList.Add(ui);

                // 重建栈结构
                layerStack[ui.Layer].Clear();
                foreach (BaseUi p in tempList)
                {
                    layerStack[ui.Layer].Push(p);
                }

                // 反转栈以保持正确顺序
                layerStack[ui.Layer] = new Stack<BaseUi>(tempList.Reverse<BaseUi>());

                // 更新所有面板的层级顺序
                RefreshPanelDepths();

                // 生命周期调用
                layerStack[ui.Layer].Peek().OnResume();
                if (layerStack[ui.Layer].Count > 1)
                {
                    BaseUi previousTop = tempList[tempList.Count - 2];
                    previousTop.OnPause();
                }
            }
        }

        // 新增刷新层级深度的方法
        private void RefreshPanelDepths()
        {
            foreach (EUiLayer layer in layerStack.Keys)
            {
                Transform layerTransform = canvasTransform.Find(layer.ToString());
                layerTransform.DetachChildren();
                foreach (BaseUi basePanel in layerStack[layer].Reverse())
                {
                    basePanel.transform.SetParent(layerTransform);
                }
            }
        }

        // 其他实用方法
        public bool IsPanelOpen<T>()
        {
            return activePanels.ContainsKey(typeof(T));
        }

        public T GetTopPanel<T>(EUiLayer layer) where T : BaseUi
        {
            return layerStack[layer].Count > 0 ? layerStack[layer].Peek() as T : null;
        }
    }
}