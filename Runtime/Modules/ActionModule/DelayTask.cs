using System;

namespace CnoomFrameWork.Modules.ActionModule
{
    public class DelayTask
    {
        internal Action Callback;
        internal int ExecuteFrame;
        internal float ExecuteTime;
        internal ActionManager Manager;
        internal DelayTask Next;
        internal int NextDelayFrame;
        internal float NextDelayTime;
        internal DelayTask(ActionManager manager)
        {
            Manager = manager;
        }

        public void Execute()
        {
            Callback?.Invoke();
            if(Next == null) return;
            if(NextDelayTime > 0)
                Manager.SetSeconds(Next, NextDelayTime);
            if(NextDelayFrame > 0)
                Manager.SetFrame(Next, NextDelayFrame);
        }

        public void Recycle()
        {
            ExecuteTime = 0;
            ExecuteFrame = 0;
            NextDelayTime = 0;
            NextDelayFrame = 0;
            Callback = null;
            Next = null;
            Manager.PushToPool(this);
        }
    }

    public static class DelayTaskExtension
    {
        public static DelayTask ThenSeconds(this DelayTask task, float seconds, Action callback)
        {
            DelayTask newTask = task.Manager.GetTask();
            newTask.Callback = callback;
            task.NextDelayTime = seconds;
            task.Next = newTask;
            return newTask;
        }

        public static DelayTask ThenFrame(this DelayTask task, int frames, Action callback)
        {
            DelayTask newTask = task.Manager.GetTask();
            newTask.Callback = callback;
            task.NextDelayFrame = frames;
            task.Next = newTask;
            return newTask;
        }
    }
}