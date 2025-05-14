# CFramework (v0.8.4)

Unity模块化游戏开发框架，基于控制反转容器实现，提供完整的模块管理、服务集成和资源管理解决方案。

## 帮助文档
https://deepwiki.com/cnoom/com.cnoom.cframework/1-cnoomframework-overview

## 核心功能

### 1. 应用入口 (App.cs)
- 单例管理
- 全局异常处理
- 核心系统初始化
- 模块自动注册

### 2. 模块系统
- 模块生命周期管理
- 自动依赖注入
- 支持链式调用
- 内置常用模块：
    - UI模块：分层管理、动画支持
    - 动作管理：优先级队列、延时任务

### 3. 服务系统
- **资源服务**：基于Addressable的智能资源管理
    - 自动引用计数
    - 实例生命周期追踪
    - 异常处理
- **组件容器服务**：MonoBehaviour组件管理
- **存储服务**：加密存储解决方案

### 4. 编辑器工具
- 框架配置管理
- 一键更新框架包
- 开发日志系统

## 安装指南

1. 添加包依赖到manifest.json:
```json
{
  "dependencies": {
    "com.cnoom.cframework": "https://github.com/cnoom/com.cnoom.cframework.git#0.5.1",
    "com.unity.addressables": "1.22.3"
  }
}
```

2. 初始化框架:
```csharp
// 启动框架
App.Instance.Initialize();

// 注册模块
App.Instance.RegisterModule<UIModule>();
App.Instance.RegisterModule<AssetsService>();
```

## 使用示例

### 资源加载示例
```csharp
// 加载资源
var handle = App.Instance.GetService<AssetsService>().LoadAssetAsync<GameObject>("Prefabs/Character");

// 实例化并自动追踪
handle.Completed += op => {
    var instance = App.Instance.GetService<AssetsService>().Instantiate(op.Result);
};
```

### UI模块示例
```csharp
public class MainPanel : BaseUi {
    [Inject]
    public ILog Logger { get; set; }
    
    protected override void OnShow() {
        Logger.Log("MainPanel shown");
    }
}

// 打开面板
App.Instance.GetModule<UIModule>().Open<MainPanel>();
```

## 最佳实践

1. **资源管理**
- 使用Addressable作为资源加载方案
- 通过服务管理资源生命周期
- 避免直接使用Resources.Load

2. **模块设计**
- 保持单一职责原则
- 通过接口定义契约
- 使用依赖注入而非直接实例化

3. **性能优化**
- 缓存频繁使用的服务引用
- 合理使用Singleton生命周期
- 避免在热路径中使用反射
