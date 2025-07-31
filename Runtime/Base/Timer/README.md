# TimerService 使用指南

TimerService 已经过全面改进，现在提供了更加便捷和强大的定时器功能。

## 主要改进

### 1. 静态Timer类 - 简化使用
不再需要通过 `App.GetTimerService()` 获取服务，直接使用静态方法：

```csharp
// 旧方式
App.Instance.GetTimerService().AddSecondsTimer(2f, () => Debug.Log("执行"), false);

// 新方式
Timer.Delay(2f, () => Debug.Log("执行"));
```

### 2. 链式调用支持
支持流畅的链式操作：

```csharp
Timer.Delay(3f, () => Debug.Log("执行"))
    .BindTo(gameObject)
    .OnCancel(() => Debug.Log("取消"))
    .AutoCancel(10f);
```

### 3. 命名定时器管理
通过TimerManager管理命名定时器：

```csharp
// 创建命名定时器
TimerManager.SetNamedTimer("攻击冷却", 2f, () => Debug.Log("冷却结束"));

// 检查是否存在
if (TimerManager.HasActiveTimer("攻击冷却"))
{
    Debug.Log("还在冷却中");
}

// 取消定时器
TimerManager.CancelNamedTimer("攻击冷却");
```

### 4. 分组定时器管理
将相关定时器分组管理：

```csharp
// 创建分组定时器
TimerManager.CreateGroupTimer("UI动画", 1f, () => Debug.Log("动画1"));
TimerManager.CreateGroupTimer("UI动画", 2f, () => Debug.Log("动画2"));

// 暂停整个组
TimerManager.PauseGroup("UI动画");

// 恢复整个组
TimerManager.ResumeGroup("UI动画");

// 取消整个组
TimerManager.CancelGroup("UI动画");
```

## 核心API

### Timer 静态类

#### 基础方法
- `Timer.Delay(float delay, Action callback)` - 延迟执行（秒）
- `Timer.Repeat(float interval, Action callback)` - 重复执行（秒）
- `Timer.DelayFrames(int frames, Action callback)` - 延迟执行（帧）
- `Timer.RepeatFrames(int interval, Action callback)` - 重复执行（帧）
- `Timer.NextFrame(Action callback)` - 下一帧执行

#### 高级方法
- `Timer.WaitUntil(Func<bool> condition, Action callback, float timeout)` - 等待条件满足
- `Timer.DelayRealTime(float delay, Action callback, bool useRealTime)` - 真实时间延迟

### 扩展方法

#### 链式调用
- `.BindTo(GameObject gameObject)` - 绑定到GameObject
- `.OnCancel(Action onCancel)` - 设置取消回调
- `.PauseImmediately()` - 立即暂停
- `.StartAfter(float delay)` - 延迟开始
- `.ExecuteIf(Func<bool> condition)` - 条件执行
- `.AutoCancel(float time)` - 自动取消

### TimerManager 管理器

#### 命名定时器
- `SetNamedTimer(string name, float delay, Action callback, bool loop)` - 创建命名定时器
- `GetNamedTimer(string name)` - 获取命名定时器
- `CancelNamedTimer(string name)` - 取消命名定时器
- `HasActiveTimer(string name)` - 检查定时器是否活跃

#### 分组定时器
- `CreateGroupTimer(string groupName, float delay, Action callback, bool loop)` - 创建分组定时器
- `CancelGroup(string groupName)` - 取消整个组
- `PauseGroup(string groupName)` - 暂停整个组
- `ResumeGroup(string groupName)` - 恢复整个组
- `GetActiveTimerCount(string groupName)` - 获取组内活跃定时器数量

## 使用场景示例

### 1. 技能冷却系统
```csharp
void UseSkill(string skillName, float cooldown)
{
    if (TimerManager.HasActiveTimer($"{skillName}_冷却"))
    {
        Debug.Log("技能冷却中");
        return;
    }
    
    // 使用技能
    Debug.Log($"使用{skillName}");
    
    // 开始冷却
    TimerManager.SetNamedTimer($"{skillName}_冷却", cooldown, () => 
    {
        Debug.Log($"{skillName}冷却结束");
    });
}
```

### 2. UI动画管理
```csharp
void StartUIAnimations()
{
    // 将所有UI动画定时器放在同一组
    TimerManager.CreateGroupTimer("UI动画", 0.5f, () => FadeIn());
    TimerManager.CreateGroupTimer("UI动画", 1.0f, () => SlideIn());
    TimerManager.CreateGroupTimer("UI动画", 1.5f, () => ScaleIn());
}

void StopAllUIAnimations()
{
    // 一键停止所有UI动画
    TimerManager.CancelGroup("UI动画");
}
```

### 3. 游戏状态管理
```csharp
void StartGame()
{
    // 游戏开始倒计时
    var countdown = 3;
    TimerManager.SetNamedTimer("开始倒计时", 1f, () =>
    {
        Debug.Log($"游戏开始倒计时: {countdown}");
        countdown--;
        if (countdown <= 0)
        {
            Debug.Log("游戏开始！");
            TimerManager.CancelNamedTimer("开始倒计时");
            StartGameplay();
        }
    }, loop: true);
}
```

### 4. 自动保存系统
```csharp
void EnableAutoSave()
{
    TimerManager.SetNamedTimer("自动保存", 300f, () => // 每5分钟保存一次
    {
        SaveGame();
        Debug.Log("游戏已自动保存");
    }, loop: true);
}

void DisableAutoSave()
{
    TimerManager.CancelNamedTimer("自动保存");
}
```

## 最佳实践

1. **使用命名定时器**：对于重要的、可能需要取消的定时器，使用命名定时器便于管理
2. **合理分组**：将相关的定时器放在同一组，便于批量操作
3. **绑定生命周期**：使用 `.BindTo(gameObject)` 将定时器绑定到GameObject的生命周期
4. **及时清理**：在适当的时候调用 `TimerManager.CleanupCompletedTimers()` 清理已完成的定时器引用
5. **条件执行**：使用 `.ExecuteIf()` 避免不必要的定时器执行

## 性能优化

- 定时器使用对象池，避免频繁的GC
- 支持批量操作，减少遍历开销
- 自动清理已完成的定时器，避免内存泄漏
- 分组管理减少查找时间复杂度

通过这些改进，TimerService现在更加易用、功能更强大，能够满足各种复杂的定时器需求。