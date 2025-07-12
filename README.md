# CFramework 框架介绍文档
## 框架概述
   CFramework 是一个基于控制反转容器实现的 Unity 模块化游戏开发框架，当前版本为 v0.8.91。它提供了一套完整的模块管理、服务集成和资源管理解决方案，旨在简化 Unity 游戏开发流程，提高代码的可维护性和可扩展性。
## 核心特性
### 2.1 模块化架构
#### 基于控制反转(IoC)容器实现
#### 模块生命周期管理
#### 自动依赖注入
#### 支持链式调用
#### 内置常用模块及服务(UI、资源管理等)
### 2.2 服务系统
#### 资源服务：基于 Addressable 的智能资源管理
##### 自动引用计数
##### 实例生命周期追踪
##### 异常处理机制
#### 组件容器服务：MonoBehaviour 组件管理
#### 存储服务：加密存储解决方案
### 2.3 UI 系统
#### 分层管理
#### 动画支持
#### 生命周期管理(OnEnter/OnExit)
#### 依赖注入支持
```csharp
using CFramework;
using CFramework.Ui;
using UnityEngine;
public class UiBase : MonoBehaviour
{
    public UiConfig uiConfig;
    
    public void Generate()
    {
        Injector.Inject(this);
        OnGenerate();
    }

    public virtual void OnGenerate() {}
    public virtual void OnEnter(UiParameter parameter) {}
    public virtual void OnExit() {}
}
```
### 3. 快速开始
#### 3.1 安装
##### 1.添加包依赖到 manifest.json:
```json
{
  "dependencies": {
    "com.cnoom.cframework": "https://github.com/cnoom/com.cnoom.cframework.git#0.5.1",
    "com.unity.addressables": "1.22.3"
  }
}
```
##### 2.初始化框架:
```csharp
// 启动框架
App.Instance.Initialize();

// 注册模块
App.Instance.RegisterModule<UIModule>();
App.Instance.RegisterModule<AssetsService>();
```
### 3.2 使用示例
#### UI模块示例
```csharp
public class MainPanel : BaseUi {
    [Inject]
    public ILog Logger { get; set; }
    
    protected override void OnEnter() {
        Logger.Log("MainPanel shown");
    }
}

// 打开面板
App.Instance.GetModule<UIModule>().Open<MainPanel>();
```
#### 资源服务示例
```csharp
// 加载资源
yield return App.Instance.GetService<AssetsService>().LoadAssetAsync<GameObject>("Prefabs/Character");
dosmething();
```
### 4. 最佳实践
#### 1.资源管理
##### 使用 Addressable 作为资源加载方案
##### 通过服务管理资源生命周期
##### 避免直接使用 Resources.Load
#### 2.模块设计
##### 保持单一职责原则
##### 通过接口定义契约
##### 使用依赖注入而非直接实例化
#### 3.性能优化
##### 缓存频繁使用的服务引用
##### 合理使用 Singleton 生命周期
##### 避免在热路径中使用反射

### 5. 编辑器工具
#### 框架提供了丰富的编辑器工具支持：
##### 框架配置管理
##### 一键更新框架包
##### 开发日志系统

### 6. 扩展与定制
#### CFramework 设计时考虑了扩展性，开发者可以：
##### 1.自定义模块并注册到框架中
##### 2.扩展服务系统
##### 2.覆盖默认实现
### 7. 技术支持
##### 官方文档与支持[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/cnoom/com.cnoom.cframework)
https://deepwiki.com/cnoom/com.cnoom.cframework/1-cnoomframework-overview

### 8. 版本信息
#### 当前版本: v0.8.92
#### 依赖项: Unity Addressables 1.22.3
#### 作者: cnoom
#### 许可证: 私有(Private)