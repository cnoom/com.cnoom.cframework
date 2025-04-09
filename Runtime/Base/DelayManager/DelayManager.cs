using System;
using System.Collections.Generic;
using CnoomFrameWork.Singleton;
using UnityEngine;

namespace CnoomFrameWork.Core.Base.DelayManager
{

    public class DelayManager : PersistentMonoSingleton<DelayManager>
    {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            DelayManager manager = Instance;
        }
        #region Data Structures

        private class DelayTaskBase
        {
            public Action Action;
            public bool IsActive;
            public CancellationToken Token;
        }

        private class TimeDelayTask : DelayTaskBase
        {
            public float ExecuteTime;
        }

        private class FrameDelayTask : DelayTaskBase
        {
            public int TargetFrame;
        }

        public class CancellationToken
        {
            public bool IsCancelled;
            public bool IsLinked;
            public GameObject LinkedObject;
        }

        #endregion

        #region Object Pool

        private const int InitialPoolSize = 50;
        private readonly Stack<TimeDelayTask> timeTaskPool = new Stack<TimeDelayTask>(InitialPoolSize);
        private readonly Stack<FrameDelayTask> frameTaskPool = new Stack<FrameDelayTask>(InitialPoolSize);

        private TimeDelayTask GetTimeTask()
        {
            if(timeTaskPool.Count > 0)
            {
                TimeDelayTask task = timeTaskPool.Pop();
                task.IsActive = true;
                return task;
            }
            return new TimeDelayTask
            {
                IsActive = true
            };
        }

        private FrameDelayTask GetFrameTask()
        {
            if(frameTaskPool.Count > 0)
            {
                FrameDelayTask task = frameTaskPool.Pop();
                task.IsActive = true;
                return task;
            }
            return new FrameDelayTask
            {
                IsActive = true
            };
        }

        private void ReturnTask(DelayTaskBase task)
        {
            task.Action = null;
            task.Token = null;
            task.IsActive = false;

            switch(task)
            {
                case TimeDelayTask t:
                    timeTaskPool.Push(t);
                    break;
                case FrameDelayTask f:
                    frameTaskPool.Push(f);
                    break;
            }
        }

        #endregion

        #region Task Lists

        private readonly List<TimeDelayTask> activeTimeTasks = new List<TimeDelayTask>(100);
        private readonly List<FrameDelayTask> activeFrameTasks = new List<FrameDelayTask>(100);
        private readonly Queue<Action> executeQueue = new Queue<Action>(100);

        #endregion

        #region Frame Splitting

        [Header("Performance Settings"), SerializeField]
        private int maxTimeTasksPerFrame = 50;
        [SerializeField]
        private int maxFrameTasksPerFrame = 50;
        private int currentTimeIndex;
        private int currentFrameIndex;

        #endregion

        #region Core Logic

        private void Update()
        {
            ProcessTimeTasks();
            ProcessFrameTasks();
            ExecuteActions();
        }

        private void ProcessTimeTasks()
        {
            var processedCount = 0;
            float currentTime = Time.time;

            while (currentTimeIndex < activeTimeTasks.Count && processedCount < maxTimeTasksPerFrame)
            {
                TimeDelayTask task = activeTimeTasks[currentTimeIndex];

                if(!task.IsActive || CheckCancellation(task))
                {
                    RemoveTimeTask(currentTimeIndex);
                    continue;
                }

                if(currentTime >= task.ExecuteTime)
                {
                    executeQueue.Enqueue(task.Action);
                    RemoveTimeTask(currentTimeIndex);
                    processedCount++;
                    continue;
                }

                currentTimeIndex++;
            }

            if(currentTimeIndex >= activeTimeTasks.Count)
                currentTimeIndex = 0;
        }

        private void ProcessFrameTasks()
        {
            var processedCount = 0;
            int currentFrame = Time.frameCount;

            while (currentFrameIndex < activeFrameTasks.Count && processedCount < maxFrameTasksPerFrame)
            {
                FrameDelayTask task = activeFrameTasks[currentFrameIndex];

                if(!task.IsActive || CheckCancellation(task))
                {
                    RemoveFrameTask(currentFrameIndex);
                    continue;
                }

                if(currentFrame >= task.TargetFrame)
                {
                    executeQueue.Enqueue(task.Action);
                    RemoveFrameTask(currentFrameIndex);
                    processedCount++;
                    continue;
                }

                currentFrameIndex++;
            }

            if(currentFrameIndex >= activeFrameTasks.Count)
                currentFrameIndex = 0;
        }

        private bool CheckCancellation(DelayTaskBase task)
        {
            if(task.Token == null) return false;

            if(task.Token.IsCancelled ||
               task.Token.IsLinked && !task.Token.LinkedObject)
            {
                ReturnTask(task);
                return true;
            }
            return false;
        }

        private void RemoveTimeTask(int index)
        {
            ReturnTask(activeTimeTasks[index]);
            activeTimeTasks.RemoveAt(index);
        }

        private void RemoveFrameTask(int index)
        {
            ReturnTask(activeFrameTasks[index]);
            activeFrameTasks.RemoveAt(index);
        }

        private void ExecuteActions()
        {
            while (executeQueue.Count > 0)
            {
                try
                {
                    executeQueue.Dequeue()?.Invoke();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        #endregion

        #region Public API

        public CancellationToken RegisterTimeDelay(float delay, Action action, GameObject autoCancelTarget = null)
        {
            TimeDelayTask task = GetTimeTask();
            task.ExecuteTime = Time.time + delay;
            task.Action = action;
            task.Token = new CancellationToken
            {
                IsLinked = autoCancelTarget,
                LinkedObject = autoCancelTarget
            };

            activeTimeTasks.Add(task);
            return task.Token;
        }

        public CancellationToken RegisterFrameDelay(int frames, Action action, GameObject autoCancelTarget = null)
        {
            FrameDelayTask task = GetFrameTask();
            task.TargetFrame = Time.frameCount + frames;
            task.Action = action;
            task.Token = new CancellationToken
            {
                IsLinked = autoCancelTarget,
                LinkedObject = autoCancelTarget
            };

            activeFrameTasks.Add(task);
            return task.Token;
        }

        public void CancelDelay(CancellationToken token)
        {
            if(token != null) token.IsCancelled = true;
        }

        #endregion
    }
}