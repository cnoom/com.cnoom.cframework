using System;
using System.Collections.Generic;

namespace CnoomFrameWork.Base.Timer
{
    /// <summary>
    /// 定时器管理器，提供更高级的定时器管理功能
    /// </summary>
    public static class TimerManager
    {
        private static readonly Dictionary<string, ITimer> _namedTimers = new Dictionary<string, ITimer>();
        private static readonly Dictionary<string, List<ITimer>> _groupedTimers = new Dictionary<string, List<ITimer>>();

        #region 命名定时器管理

        /// <summary>
        /// 创建或更新命名定时器（秒级）
        /// </summary>
        /// <param name="name">定时器名称</param>
        /// <param name="delay">延迟时间（秒）</param>
        /// <param name="callback">回调函数</param>
        /// <param name="loop">是否循环</param>
        /// <returns>定时器实例</returns>
        public static SecondsTimer SetNamedTimer(string name, float delay, Action callback, bool loop = false)
        {
            // 如果已存在同名定时器，先取消它
            if (_namedTimers.ContainsKey(name))
            {
                _namedTimers[name].Cancel();
            }

            var timer = Timer.Seconds(delay, callback, loop);
            timer.OnCancel += () => _namedTimers.Remove(name);
            _namedTimers[name] = timer;
            
            return timer;
        }

        /// <summary>
        /// 创建或更新命名定时器（帧级）
        /// </summary>
        /// <param name="name">定时器名称</param>
        /// <param name="frames">延迟帧数</param>
        /// <param name="callback">回调函数</param>
        /// <param name="loop">是否循环</param>
        /// <returns>定时器实例</returns>
        public static FrameTimer SetNamedFrameTimer(string name, int frames, Action callback, bool loop = false)
        {
            // 如果已存在同名定时器，先取消它
            if (_namedTimers.ContainsKey(name))
            {
                _namedTimers[name].Cancel();
            }

            var timer = Timer.Frames(frames, callback, loop);
            timer.OnCancel += () => _namedTimers.Remove(name);
            _namedTimers[name] = timer;
            
            return timer;
        }

        /// <summary>
        /// 获取命名定时器
        /// </summary>
        /// <param name="name">定时器名称</param>
        /// <returns>定时器实例，如果不存在返回null</returns>
        public static ITimer GetNamedTimer(string name)
        {
            return _namedTimers.TryGetValue(name, out var timer) ? timer : null;
        }

        /// <summary>
        /// 取消命名定时器
        /// </summary>
        /// <param name="name">定时器名称</param>
        /// <returns>是否成功取消</returns>
        public static bool CancelNamedTimer(string name)
        {
            if (_namedTimers.TryGetValue(name, out var timer))
            {
                timer.Cancel();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查命名定时器是否存在且活跃
        /// </summary>
        /// <param name="name">定时器名称</param>
        /// <returns>是否存在且活跃</returns>
        public static bool HasActiveTimer(string name)
        {
            return _namedTimers.TryGetValue(name, out var timer) && !timer.IsCompleted;
        }

        #endregion

        #region 分组定时器管理

        /// <summary>
        /// 添加定时器到指定组
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="timer">定时器</param>
        public static void AddToGroup(string groupName, ITimer timer)
        {
            if (!_groupedTimers.ContainsKey(groupName))
            {
                _groupedTimers[groupName] = new List<ITimer>();
            }

            _groupedTimers[groupName].Add(timer);
            timer.OnCancel += () => RemoveFromGroup(groupName, timer);
        }

        /// <summary>
        /// 从组中移除定时器
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="timer">定时器</param>
        public static void RemoveFromGroup(string groupName, ITimer timer)
        {
            if (_groupedTimers.TryGetValue(groupName, out var timers))
            {
                timers.Remove(timer);
                if (timers.Count == 0)
                {
                    _groupedTimers.Remove(groupName);
                }
            }
        }

        /// <summary>
        /// 取消指定组的所有定时器
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns>取消的定时器数量</returns>
        public static int CancelGroup(string groupName)
        {
            if (!_groupedTimers.TryGetValue(groupName, out var timers))
                return 0;

            var count = timers.Count;
            foreach (var timer in timers.ToArray())
            {
                timer.Cancel();
            }
            
            return count;
        }

        /// <summary>
        /// 暂停指定组的所有定时器
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns>暂停的定时器数量</returns>
        public static int PauseGroup(string groupName)
        {
            if (!_groupedTimers.TryGetValue(groupName, out var timers))
                return 0;

            var count = 0;
            foreach (var timer in timers)
            {
                if (!timer.IsCompleted)
                {
                    timer.Pause();
                    count++;
                }
            }
            
            return count;
        }

        /// <summary>
        /// 恢复指定组的所有定时器
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns>恢复的定时器数量</returns>
        public static int ResumeGroup(string groupName)
        {
            if (!_groupedTimers.TryGetValue(groupName, out var timers))
                return 0;

            var count = 0;
            foreach (var timer in timers)
            {
                if (!timer.IsCompleted)
                {
                    timer.Resume();
                    count++;
                }
            }
            
            return count;
        }

        /// <summary>
        /// 获取指定组的活跃定时器数量
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns>活跃定时器数量</returns>
        public static int GetActiveTimerCount(string groupName)
        {
            if (!_groupedTimers.TryGetValue(groupName, out var timers))
                return 0;

            var count = 0;
            foreach (var timer in timers)
            {
                if (!timer.IsCompleted)
                    count++;
            }
            
            return count;
        }

        #endregion

        #region 便捷方法

        /// <summary>
        /// 创建带组名的定时器（秒级）
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="delay">延迟时间（秒）</param>
        /// <param name="callback">回调函数</param>
        /// <param name="loop">是否循环</param>
        /// <returns>定时器实例</returns>
        public static SecondsTimer CreateGroupTimer(string groupName, float delay, Action callback, bool loop = false)
        {
            var timer = Timer.Seconds(delay, callback, loop);
            AddToGroup(groupName, timer);
            return timer;
        }

        /// <summary>
        /// 创建带组名的定时器（帧级）
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="frames">延迟帧数</param>
        /// <param name="callback">回调函数</param>
        /// <param name="loop">是否循环</param>
        /// <returns>定时器实例</returns>
        public static FrameTimer CreateGroupFrameTimer(string groupName, int frames, Action callback, bool loop = false)
        {
            var timer = Timer.Frames(frames, callback, loop);
            AddToGroup(groupName, timer);
            return timer;
        }

        /// <summary>
        /// 获取所有活跃的命名定时器
        /// </summary>
        /// <returns>活跃的命名定时器字典</returns>
        public static Dictionary<string, ITimer> GetAllActiveNamedTimers()
        {
            var activeTimers = new Dictionary<string, ITimer>();
            foreach (var kvp in _namedTimers)
            {
                if (!kvp.Value.IsCompleted)
                {
                    activeTimers[kvp.Key] = kvp.Value;
                }
            }
            return activeTimers;
        }

        /// <summary>
        /// 获取所有组名
        /// </summary>
        /// <returns>所有组名列表</returns>
        public static List<string> GetAllGroupNames()
        {
            return new List<string>(_groupedTimers.Keys);
        }

        /// <summary>
        /// 清理所有已完成的定时器引用
        /// </summary>
        public static void CleanupCompletedTimers()
        {
            // 清理命名定时器
            var completedNames = new List<string>();
            foreach (var kvp in _namedTimers)
            {
                if (kvp.Value.IsCompleted)
                {
                    completedNames.Add(kvp.Key);
                }
            }
            foreach (var name in completedNames)
            {
                _namedTimers.Remove(name);
            }

            // 清理分组定时器
            var emptyGroups = new List<string>();
            foreach (var kvp in _groupedTimers)
            {
                kvp.Value.RemoveAll(timer => timer.IsCompleted);
                if (kvp.Value.Count == 0)
                {
                    emptyGroups.Add(kvp.Key);
                }
            }
            foreach (var groupName in emptyGroups)
            {
                _groupedTimers.Remove(groupName);
            }
        }

        #endregion
    }
}