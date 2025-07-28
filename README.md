# CnoomFrameWork

CnoomFrameWork是一个为Unity游戏开发设计的轻量级框架，提供模块化架构、依赖注入和事件系统，帮助开发者构建可维护、可扩展的游戏项目。

## 功能特性

- **依赖注入系统**：通过容器管理对象创建和依赖关系，实现松耦合组件设计
- **类型安全的事件系统**：支持普通事件、引用类型事件和回调事件，带优先级和过滤器功能
- **模块化架构**：将游戏功能划分为独立模块，提供统一的生命周期管理
- **服务定位器**：管理全局服务的注册和访问，支持自动注册
- **实用工具集**：包含单例实现、对象池、扩展方法等常用工具

## 快速开始

### 安装要求

- Unity 2020.3 或更高版本
- 依赖包：
  - com.unity.addressables (1.22.3+)
  - com.unity.nuget.newtonsoft-json (3.2.1+)

### 安装步骤

1. 通过Unity Package Manager安装：
   - 打开 Window > Package Manager
   - 点击 "+" 按钮 > Add package from git URL
   - 输入 `https://github.com/cnoom/cframework.git`

2. 或者直接修改项目的 `manifest.json` 文件：
   ```json
   {
     "dependencies": {
       "com.cnoom.cframework": "https://github.com/cnoom/cframework.git",
     }
   }
   ```

### 基础用法

1. **初始化框架**

创建启动场景，添加一个空的GameObject并挂载以下脚本：

```csharp
using CnoomFrameWork.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    private void Awake()
    {
        // 初始化App
        var app = App.Instance;
        
        // 加载主场景
        SceneManager.LoadScene("MainMenu");
    }
}
```

2. **使用依赖注入**

```csharp
// 定义服务接口
public interface IDataService : IService
{
    void SaveData(string key, string value);
    string LoadData(string key);
}

// 实现服务
public class PlayerPrefsDataService : IDataService
{
    public void Initialize() { }
    
    public void Dispose() { }
    
    public void SaveData(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }
    
    public string LoadData(string key)
    {
        return PlayerPrefs.GetString(key);
    }
}

// 注册服务
App.Instance.ServiceLocator.RegisterService<IDataService, PlayerPrefsDataService>();

// 在其他类中使用依赖注入
public class GameManager
{
    [Inject]
    private IDataService _dataService;
    
    [PostConstruct]
    private void Initialize()
    {
        _dataService.SaveData("lastLogin", DateTime.Now.ToString());
    }
}
```

3. **使用事件系统**

```csharp
// 定义事件数据
public class PlayerScoreEvent
{
    public string PlayerId { get; set; }
    public int Score { get; set; }
}

// 订阅事件
EventManager.Subscribe<PlayerScoreEvent>(OnPlayerScore);

// 发布事件
EventManager.Publish(new PlayerScoreEvent { 
    PlayerId = "player1", 
    Score = 100 
});

// 事件处理方法
private void OnPlayerScore(PlayerScoreEvent evt)
{
    Debug.Log($"玩家 {evt.PlayerId} 获得了 {evt.Score} 分");
}

// 使用特性订阅事件
public class ScoreManager
{
    [EventSubscriber(typeof(PlayerScoreEvent), Priority = 10)]
    public void OnPlayerScore(PlayerScoreEvent evt)
    {
        // 处理玩家得分事件
    }
}

// 注册带有特性的订阅者
EventManager.Register(scoreManager);
```

## 进阶配置

### 模块系统配置

创建自定义模块并配置加载顺序：

```csharp
// 定义模块
public class GameplayModule : Module
{
    [Inject]
    private IDataService _dataService;
    
    public override void Initialize()
    {
        Debug.Log("GameplayModule 已初始化");
    }
    
    public override void Dispose()
    {
        Debug.Log("GameplayModule 已销毁");
    }
}

// 创建模块加载顺序配置
public class GameModuleConfig : ModuleOrderConfig
{
    public GameModuleConfig()
    {
        // 按顺序添加模块注册器
        Registers.Add(new ModuleRegister<DataModule>());
        Registers.Add(new ModuleRegister<UIModule>());
        Registers.Add(new ModuleRegister<GameplayModule>());
    }
}
```

### 日志系统配置

自定义日志配置：

```csharp
// 创建日志配置
public class CustomLogConfig : LogConfig
{
    public CustomLogConfig()
    {
        // 设置日志级别
        MinLevel = LogLevel.Info;
        
        // 添加日志格式化器
        Formatters.Add(new TextFormatter());
        
        // 添加日志输出器
        Appenders.Add(new ConsoleAppender());
        Appenders.Add(new FileAppender("Logs/game.log"));
        
        // 设置日志类别
        Categories.Add(LogCategory.Default, LogLevel.Info);
        Categories.Add(LogCategory.Network, LogLevel.Warning);
    }
}

// 在App初始化前设置日志配置
ConfigManager.Instance.SetConfig(new CustomLogConfig());
```

### 容器高级用法

```csharp
// 创建子容器
var childContainer = App.Instance.RootContainer.CreateChildContainer();

// 在子容器中注册对象
childContainer.BindTransient<IWeaponFactory, WeaponFactory>();

// 使用工厂方法注册
childContainer.BindSingleton<IEnemySpawner>(c => {
    var dataService = c.Resolve<IDataService>();
    return new EnemySpawner(dataService);
});

// 条件绑定
childContainer.BindSingleton<IRenderer, MobileRenderer>()
    .When(c => Application.platform == RuntimePlatform.Android || 
               Application.platform == RuntimePlatform.IOS);
```

## 贡献指南

### PR提交规范

1. **分支命名**：使用 `feature/功能名`、`bugfix/问题描述` 或 `improvement/改进描述` 格式
2. **提交信息**：使用 `类型(范围): 描述` 格式，例如 `feat(event): 添加事件过滤器功能`
3. **代码风格**：遵循项目现有的代码风格和命名约定
4. **文档**：为新功能或API更改提供相应的文档更新

### 测试要求

1. 所有新功能必须包含单元测试
2. 修复bug时应添加能重现问题的测试用例
3. 测试覆盖率不应低于现有水平
4. 在提交PR前确保所有测试通过

## 许可证

本项目采用MIT许可证。详情请参阅[LICENSE](LICENSE)文件。

```
MIT License

Copyright (c) 2023 cnoom

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.