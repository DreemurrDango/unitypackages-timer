# Unity 计时器模块 (Timer Module)

## 概述

本模块旨在满足便捷地设置倒计时显示与事件监听，为 Unity 项目提供了两种核心的计时器解决方案：一个用于驱动 UI 和游戏内逻辑的 `CountDown` 组件，以及一个基于 `System.Timers` 的、更灵活的后台 `Timer` 类

## 核心功能

*   **两种计时器类型**:
    *   **`CountDown` (组件)**: 一个 `MonoBehaviour` 组件，非常适合与 Unity 的生命周期和 UI 元素（如文本和进度条）集成
    *   **`Timer` (类)**: 一个独立的 C# 类，不依赖 `MonoBehaviour`，适合在后台执行与游戏对象无关的计时任务
*   **UI 集成**: `CountDown` 组件可以直接驱动 `TextMeshPro` 文本和 `Slider` 进度条，自动更新倒计时显示
*   **灵活的文本格式**: `CountDown` 支持自定义文本格式，可以同时显示剩余时间、总时间、已过时间等多种信息
*   **时间缩放**: `Timer` 类支持时间缩放（Time Scale），可以动态调整计时器的运行速度，而不受 Unity 的全局 `Time.timeScale`影响
*   **事件驱动**: 两种计时器都提供了丰富的事件回调（`UnityEvent` 和 C# `Action`），用于在计时开始、步进和结束时执行逻辑
*   **生命周期控制**: 提供了 `Start`, `Pause`, `Resume`, `Reset`, `Stop` 等完整的控制方法

## 组件选择

| 特性 | `CountDown` (组件) | `Timer` (类) |
| :--- | :--- | :--- |
| **类型** | MonoBehaviour 组件 | 纯 C# 类 |
| **适用场景** | UI 倒计时、技能冷却、与游戏对象生命周期绑定的计时 | 后台逻辑、独立于场景物体的定时任务、需要时间缩放的复杂计时 |
| **运行线程** | Unity 主线程 | 后台线程 |
| **依赖** | `UnityEngine`, `TextMeshPro` (可选) | `System.Timers` |
| **事件模型** | `UnityEvent` (可在 Inspector 中配置) | C# `Action` (仅代码订阅) |

---

## 快速开始

### 1. `CountDown` 组件用法

`CountDown` 组件非常适合用于显示技能冷却、等待时间等与 UI 相关的倒计时

1.  从包的Prefabs目录中拖拽预制件到场景中（推荐做法），或在场景中的任意游戏对象（如一个 UI 面板）上添加 `CountDown` 组件
2.  将 `TMP_Text` 或 `Slider` 组件拖拽到对应的字段上
3.  在其他脚本中获取该组件并调用其方法

```csharp
using DreemurrStudio.CountDownTimer;
using UnityEngine;

public class SkillButton : MonoBehaviour
{
    public CountDown cooldownDisplay;
    public float skillCooldownTime = 10f;

    void Start()
    {
        // 监听倒计时完成事件
        cooldownDisplay.onCountDownCompleted.AddListener((totalTime) =>
        {
            Debug.Log($"时长为 {totalTime} 秒的冷却已结束");
            // 在这里启用按钮交互
        });
    }

    public void UseSkill()
    {
        // 开始一个10秒的倒计时
        cooldownDisplay.Set(skillCooldownTime, true);
        // 在这里禁用按钮交互
    }
}
```

### 2. `Timer` 类用法

`Timer` 类不依赖游戏对象，可以在任何 C# 脚本中创建实例，用于处理后台逻辑

**注意**: `Timer` 的事件在后台线程中触发，不能直接调用大部分 Unity API（如修改 `Transform`）。如果需要与主线程交互，应使用调度器或标志位

```csharp
using DreemurrStudio.CountDownTimer;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Timer resourceTimer;

    void Start()
    {
        // 创建一个 Timer 实例
        resourceTimer = new Timer();

        // 设置一个总时长60秒，每5秒触发一次步进事件的计时器
        resourceTimer.Set(60f, 5f, 1f, true);

        // 订阅步进事件
        resourceTimer.OnTimerStep += (timeLeft) =>
        {
            // 这个回调在后台线程执行
            Debug.Log($"后台计时器：每5秒产生一次资源，剩余时间: {timeLeft}");
            // 可以在这里增加资源计数 (如果变量是线程安全的)
        };

        // 订阅结束事件
        resourceTimer.OnTimerEnd += (totalTime) =>
        {
            Debug.Log("后台计时器：资源生产周期结束");
        };
    }

    // 示例：通过一个方法控制时间缩放
    public void ActivateFrenzyMode(bool isActive)
    {
        if (resourceTimer != null && resourceTimer.InCountDown)
        {
            // 狂热模式下，计时器速度加倍 (timeScale = 2)
            resourceTimer.SetTimeScale(isActive ? 2f : 1f);
        }
    }

    void OnDestroy()
    {
        // 确保在对象销毁时暂停计时器，以停止后台线程
        resourceTimer?.Pause();
    }
}
```