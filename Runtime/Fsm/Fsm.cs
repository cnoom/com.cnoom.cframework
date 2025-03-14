using System.Collections.Generic;
using UnityEngine;

namespace CnoomFrameWork.Fsm
{
    public class Fsm<T>
    {
        private readonly Dictionary<EStateActionType, string> stateActionEvent = new Dictionary<EStateActionType, string>();

        private readonly Dictionary<T, IState<T>> states = new Dictionary<T, IState<T>>();

        private T currentId;
        private T lastId;


        public void AddState(T stateId, IState<T> state)
        {
            states.Add(stateId, state);
        }

        /// <summary>
        ///     以指定状态启动状态机
        /// </summary>
        /// <param name="stateId">初始状态的标识符</param>
        public void Start(T stateId)
        {
            if(!states.TryGetValue(stateId, out IState<T> nextState))
            {
                Debug.unityLogger.LogError(nameof(Fsm<T>), "State not found");
                return;
            }
            currentId = stateId;
            nextState.Enter();
        }

        /// <summary>
        ///     切换状态机的当前状态
        /// </summary>
        /// <param name="stateId">要切换到的目标状态的标识符</param>
        public void ChangeState(T stateId)
        {
            if(!states.TryGetValue(stateId, out IState<T> nextState))
            {
                Debug.unityLogger.LogError(nameof(Fsm<T>), "State not found");
                return;
            }
            IState<T> currentState = states[currentId];
            currentState?.Exit();
            lastId = currentId;
            currentId = stateId;
            nextState.Enter();
        }

        /// <summary>
        ///     更新状态机的当前状态
        /// </summary>
        public void Update()
        {
            IState<T> state = states[currentId];
            state?.Update();
        }

        /// <summary>
        ///     设置状态切换消息传递的key，如果是null则不传递消息
        /// </summary>
        /// <param name="eventName"></param>
        public void SetChangeStateEvent(string eventName)
        {
            if(stateActionEvent.TryAdd(EStateActionType.Change, eventName))
            {
                return;
            }
            stateActionEvent[EStateActionType.Start] = eventName;
        }

        /// <summary>
        ///     设置状态启动消息传递的key，如果是null则不传递消息
        /// </summary>
        /// <param name="eventName">状态启动事件的名称</param>
        public void SetStartStateEvent(string eventName)
        {
            if(stateActionEvent.TryAdd(EStateActionType.Start, eventName))
            {
                return;
            }
            stateActionEvent[EStateActionType.Start] = eventName;
        }

        protected bool HasEvent(EStateActionType type, out string eventName)
        {
            eventName = null;
            if(!stateActionEvent.ContainsKey(type)) return false;
            eventName = stateActionEvent[type];
            return !string.IsNullOrEmpty(eventName);
        }
        protected enum EStateActionType
        {
            /// <summary>
            ///     状态启动
            /// </summary>
            Start,
            /// <summary>
            ///     状态切换
            /// </summary>
            Change
        }
    }
}