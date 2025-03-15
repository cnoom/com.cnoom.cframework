using System;
using System.Collections.Generic;
using CnoomFrameWork.Core;
using UnityEngine;
using UnityEngine.Scripting;

namespace CnoomFrameWork.Modules.ActionModule
{
    [AutoRegisterModule, Preserve]
    public class ActionManager : Module
    {

        // 帧队列（按帧数排序）
        private readonly PriorityQueue<DelayTask> frameQueue = new PriorityQueue<DelayTask>((a, b) => a.ExecuteFrame.CompareTo(b.ExecuteFrame));

        // 对象池
        private readonly Stack<DelayTask> taskPool = new Stack<DelayTask>(100);

        // 优先队列（按时间排序）
        private readonly PriorityQueue<DelayTask> timeQueue = new PriorityQueue<DelayTask>((a, b) => a.ExecuteTime.CompareTo(b.ExecuteTime));

        private Action update;

        protected override void OnInitialize()
        {
            App app = App.Instance;
            GameObject actionGo = new GameObject("ActionManager");
            actionGo.transform.SetParent(app.transform);

            GameObject timeGo = new GameObject("TimeTimer");
            timeGo.transform.SetParent(actionGo.transform);
            timeGo.AddComponent<UpdateMono>().UpdateAction = UpdateTime;

            GameObject frameGo = new GameObject("FrameTimer");
            frameGo.transform.SetParent(actionGo.transform);
            frameGo.AddComponent<UpdateMono>().UpdateAction = UpdateFrame;

            GameObject updateGo = new GameObject("UpdateTimer");
            updateGo.transform.SetParent(actionGo.transform);
            updateGo.AddComponent<UpdateMono>().UpdateAction = Update;
        }

        private void Update()
        {
            update?.Invoke();
        }

        private void UpdateTime()
        {
            while (timeQueue.Count > 0 && timeQueue.Peek().ExecuteTime <= Time.time)
            {
                ExecuteTask(timeQueue.Dequeue());
            }
        }

        private void UpdateFrame()
        {
            while (frameQueue.Count > 0 && frameQueue.Peek().ExecuteFrame <= Time.frameCount)
            {
                ExecuteTask(frameQueue.Dequeue());
            }
        }

        private void ExecuteTask(DelayTask task)
        {
            task.Execute();
            task.Recycle();
        }

        public void RegisterUpdate(Action callback)
        {
            update += callback;
        }

        public void UnRegisterUpdate(Action callback)
        {
            update -= callback;
        }

        public DelayTask DelaySeconds(float seconds, Action callback)
        {
            DelayTask task = SetSeconds(GetTask(), seconds);
            task.Callback = callback;
            return task;
        }

        public DelayTask DelayFrame(int frames, Action callback)
        {
            DelayTask task = SetFrame(GetTask(), frames);
            task.Callback = callback;
            return task;
        }

        internal DelayTask SetFrame(DelayTask task, int frames)
        {
            task.ExecuteFrame = Time.frameCount + frames;
            frameQueue.Enqueue(task);
            return task;
        }

        internal DelayTask SetSeconds(DelayTask task, float seconds)
        {
            task.ExecuteTime = Time.time + seconds;
            timeQueue.Enqueue(task);
            return task;
        }

        internal DelayTask GetTask()
        {
            return taskPool.Count > 0 ? taskPool.Pop() : new DelayTask(this);
        }

        internal void PushToPool(DelayTask task)
        {
            taskPool.Push(task);
        }

        internal class UpdateMono : MonoBehaviour
        {
            public Action UpdateAction;
            private void Update()
            {
                UpdateAction?.Invoke();
            }
        }
    }
}