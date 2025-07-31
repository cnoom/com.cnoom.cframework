using CnoomFrameWork.Base.Timer;
using UnityEngine;

/// <summary>
/// TimerService使用示例
/// 展示各种便捷的定时器使用方法
/// </summary>
public class TimerUsageExamples : MonoBehaviour
{
    void Start()
    {
        // ========== 基础使用示例 ==========

        // 1. 简单延迟执行
        Timer.Delay(2f, () => Debug.Log("2秒后执行"));

        // 2. 重复执行
        Timer.Repeat(1f, () => Debug.Log("每秒执行一次"));

        // 3. 下一帧执行
        Timer.NextFrame(() => Debug.Log("下一帧执行"));

        // 4. 延迟指定帧数执行
        Timer.DelayFrames(60, () => Debug.Log("60帧后执行"));

        // ========== 链式调用示例 ==========

        // 5. 链式调用 - 绑定到GameObject并设置取消回调
        Timer.Delay(3f, () => Debug.Log("3秒后执行"))
            .BindTo(gameObject)
            .SetCancel(() => Debug.Log("定时器被取消"));

        // 6. 条件执行
        Timer.Delay(1f, () => Debug.Log("只有当条件为真时才执行"))
            .ExecuteIf(() => Application.isPlaying);

        // 7. 自动取消
        Timer.Repeat(0.5f, () => Debug.Log("重复执行，但5秒后自动取消"))
            .AutoCancel(5f);

        // ========== 高级功能示例 ==========

        // 8. 等待条件满足
        Timer.WaitUntil(
            condition: () => Input.GetKeyDown(KeyCode.Space),
            callback: () => Debug.Log("检测到空格键按下"),
            timeout: 10f
        );

        // 9. 使用真实时间（不受Time.timeScale影响）
        Timer.DelayRealTime(2f, () => Debug.Log("真实时间2秒后执行"), true);

        // 10. 延迟开始的定时器
        Timer.Repeat(1f, () => Debug.Log("重复执行"))
            .StartAfter(3f); // 3秒后开始重复执行

        // ========== 命名定时器示例 ==========

        // 11. 创建命名定时器
        TimerManager.SetNamedTimer("攻击冷却", 2f, () => Debug.Log("攻击冷却结束"));

        // 12. 检查命名定时器是否存在
        if (TimerManager.HasActiveTimer("攻击冷却"))
        {
            Debug.Log("攻击还在冷却中");
        }

        // 13. 取消命名定时器
        TimerManager.CancelNamedTimer("攻击冷却");

        // ========== 分组定时器示例 ==========

        // 14. 创建分组定时器
        TimerManager.CreateGroupTimer("UI动画", 1f, () => Debug.Log("UI动画1"));
        TimerManager.CreateGroupTimer("UI动画", 2f, () => Debug.Log("UI动画2"));
        TimerManager.CreateGroupTimer("UI动画", 3f, () => Debug.Log("UI动画3"));

        // 15. 暂停整个组的定时器
        TimerManager.PauseGroup("UI动画");

        // 16. 恢复整个组的定时器
        Timer.Delay(2f, () => TimerManager.ResumeGroup("UI动画"));

        // 17. 取消整个组的定时器
        Timer.Delay(5f, () => TimerManager.CancelGroup("UI动画"));

        // ========== 实际应用场景示例 ==========

        // 18. 技能冷却系统
        UseSkillWithCooldown("火球术", 3f);

        // 19. UI淡入淡出效果
        FadeInUI();

        // 20. 游戏倒计时
        StartCountdown(10);
    }

    /// <summary>
    /// 技能冷却系统示例
    /// </summary>
    void UseSkillWithCooldown(string skillName, float cooldown)
    {
        // 检查技能是否在冷却中
        if (TimerManager.HasActiveTimer($"{skillName}_冷却"))
        {
            Debug.Log($"{skillName}还在冷却中");
            return;
        }

        // 使用技能
        Debug.Log($"使用了{skillName}");

        // 开始冷却
        TimerManager.SetNamedTimer($"{skillName}_冷却", cooldown, () => { Debug.Log($"{skillName}冷却结束"); });
    }

    /// <summary>
    /// UI淡入效果示例
    /// </summary>
    void FadeInUI()
    {
        var canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) return;

        canvasGroup.alpha = 0f;

        // 使用定时器实现淡入效果
        var fadeTimer = Timer.Repeat(0.02f, () =>
        {
            canvasGroup.alpha += 0.05f;
            if (canvasGroup.alpha >= 1f)
            {
                canvasGroup.alpha = 1f;
            }
        }).BindTo(gameObject);

        // 1秒后停止淡入
        Timer.Delay(1f, () => Timer.Cancel(fadeTimer));
    }

    /// <summary>
    /// 游戏倒计时示例
    /// </summary>
    void StartCountdown(int seconds)
    {
        var remainingTime = seconds;

        TimerManager.SetNamedTimer("游戏倒计时", 1f, () =>
        {
            Debug.Log($"倒计时: {remainingTime}");
            remainingTime--;

            if (remainingTime <= 0)
            {
                Debug.Log("倒计时结束！");
                TimerManager.CancelNamedTimer("游戏倒计时");
            }
        }, loop: true);
    }

    void OnDestroy()
    {
        // 清理所有已完成的定时器引用
        TimerManager.CleanupCompletedTimers();
    }
}