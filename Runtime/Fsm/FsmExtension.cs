using System;

namespace CnoomFrameWork.Fsm
{
    public static class FsmExtension
    {
        public static CommonState<T> State<T>(this Fsm<T> fsm, T t)
        {
            CommonState<T> state = new CommonState<T>(t);
            fsm.AddState(t, state);
            return state;
        }

        public static CommonState<T> OnEnter<T>(CommonState<T> state, Action action)
        {
            state.OnEnter += action;
            return state;
        }

        public static CommonState<T> OnUpdate<T>(CommonState<T> state, Action action)
        {
            state.OnUpdate += action;
            return state;
        }

        public static CommonState<T> OnExit<T>(CommonState<T> state, Action action)
        {
            state.OnExit += action;
            return state;
        }

        public static CommonState<T> Condition<T>(CommonState<T> state, Func<bool> condition)
        {
            state.ConditionFunc += condition;
            return state;
        }
    }
}