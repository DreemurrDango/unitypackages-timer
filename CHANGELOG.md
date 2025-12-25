# 更新日志

> 此文件记录了该软件包所有重要的变更
> 文件格式基于 [Keep a Changelog](http://keepachangelog.com/en/1.0.0/) 更新日志规范，且此项目版本号遵循 [语义化版本](http://semver.org/spec/v2.0.0.html) 规范

## [2.0.0] - 2025-07-14
### 新增
- **`CountDown`**: 添加了 `resetOnAnyInput` 选项，允许在接收到任意输入时重置倒计时
- **`CountDown`**: 文本格式化功能 (`contentFormat`) 增加了更多可用参数，如 `{1:完整时间}`、`{2:已经过时间}` 和 `{3:剩余时间百分比}`
- **`Timer`**: 添加了 `SetTimeScale` 方法和 `timeScale` 参数，使计时器可以受时间缩放控制

### 更改
- 将 `Timer` 类和 `CountDown` 组件拆分到不同的脚本文件中，以实现更好的模块化
- 将插件脚本和资源文件统一整理到 `Plugins/DreemurrStudio` 目录下，并遵循包管理规范

## [1.1.0] - 2025-04-16
### 新增
- **`CountDown`**: 添加了 `contentFormat` 字段，支持以特定格式显示倒计时文本
- **`Timer`**: 添加了基于 `System.Timers.Timer` 的 `Timer` 类，提供了一个更灵活、独立于 `MonoBehaviour` 的计时器解决方案

### 修复
- **`CountDown`**: 修复了当倒计时物件被禁用时，仍会因调用结束事件而引发报错的问题

## [1.0.0] - 2025-04-16
### 新增
- **`CountDown`**: 初始版本发布，提供了一个通用的倒计时组件
- **`CountDown`**: 支持在启用时自动进行倒计时，并更新关联的 `TMP_Text` 和 `Slider` UI 组件
- **`CountDown`**: 提供了 `onCountDownCompleted` 事件，用于在倒计时结束时执行回调