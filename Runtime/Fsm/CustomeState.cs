using System;

namespace CnoomFrameWork.Fsm
{
    public class CommonState<T> : IState<T>
    {
        public Func<bool> ConditionFunc;

        public Action OnEnter, OnUpdate, OnExit;
        public CommonState(T t)
        {
            Onwner = t;
        }

        public T Onwner { get; set; }

        public void Enter()
        {
            OnEnter?.Invoke();
        }

        public void Update()
        {
            OnUpdate?.Invoke();
        }

        public void Exit()
        {
            OnExit?.Invoke();
        }
        public bool Condition()
        {
            return ConditionFunc?.Invoke() ?? true;
        }
    }
}