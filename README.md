# CnoomFrameWork 文档

CnoomFrameWork 是一个为Unity游戏开发设计的轻量级框架，提供了模块化、依赖注入、事件系统等核心功能，帮助开发者构建可维护、可扩展的游戏项目。

## 目录

1. [框架概述](#框架概述)
2. [核心组件](#核心组件)
   - [App 应用程序入口](#app-应用程序入口)
   - [依赖注入系统](#依赖注入系统)
   - [事件系统](#事件系统)
   - [单例模式](#单例模式)
3. [服务系统](#服务系统)
4. [模块系统](#模块系统)
5. [配置系统](#配置系统)
6. [工具与扩展](#工具与扩展)
   - [扩展方法](#扩展方法)
   - [对象池](#对象池)
7. [编辑器工具](#编辑器工具)
   - [Addressable管理](#addressable管理)
   - [表格导入工具](#表格导入工具)
8. [最佳实践](#最佳实践)
9. [API参考](#api参考)

## 框架概述

CnoomFrameWork 是一个为Unity游戏开发设计的框架，专注于提供清晰的架构和实用的工具，帮助独立游戏开发者更高效地开发游戏。框架的主要特点包括：

- **模块化设计**：将游戏功能划分为独立的模块，便于管理和扩展
- **依赖注入**：通过容器系统实现松耦合的组件关系
- **事件系统**：提供强类型的事件发布/订阅机制
- **服务定位器**：管理全局服务的注册和访问
- **单例模式**：提供多种单例实现方式
- **实用工具集**：包含常用的扩展方法和工具类
- **编辑器增强**：提供表格导入、Addressable管理等编辑器工具

## 核心组件

### App 应用程序入口

`App` 类是框架的核心入口点，负责初始化和管理框架的核心系统。它是一个持久化的MonoBehaviour单例，在游戏启动时需要被实例化。

```csharp
// 在游戏启动场景中创建App实例
var app = App.Instance;
```

App类提供以下核心组件：

- **ModuleManager**：模块管理器，负责注册和管理游戏模块
- **ServiceLocator**：服务定位器，负责注册和获取全局服务
- **Log**：日志系统，提供统一的日志记录接口
- **RootContainer**：根依赖注入容器，管理对象的创建和依赖关系

### 依赖注入系统

依赖注入系统是框架的核心功能之一，它通过容器管理对象的创建和依赖关系，实现松耦合的组件设计。

#### 容器体系

- **BaseContainer**：容器基类，提供依赖注入的基本功能
- **RootContainer**：根容器，管理全局单例和子容器
- **ChildContainer**：子容器，用于管理特定场景或模块的对象

#### 注册和解析

```csharp
// 注册单例
container.BindSingleton<IMyService, MyServiceImpl>(new MyServiceImpl());

// 注册瞬态对象
container.BindTransient<IMyFactory, MyFactoryImpl>(c => new MyFactoryImpl());

// 解析对象
var service = container.Resolve<IMyService>();
```

#### 特性注入

框架支持通过特性进行依赖注入：

```csharp
public class MyComponent
{
    [Inject] // 标记需要注入的字段
    private IMyService _myService;
    
    [Inject] // 标记需要注入的属性
    public IMyLogger Logger { get; set; }
    
    [PostConstruct] // 标记在依赖注入完成后调用的方法
    private void Initialize()
    {
        // 初始化代码
    }
}
```

### 事件系统

事件系统提供了类型安全的事件发布/订阅机制，支持优先级、一次性订阅和过滤器等功能。

#### 事件发布和订阅

```csharp
// 定义事件数据类
public class PlayerDeathEvent
{
    public string PlayerId { get; set; }
    public Vector3 Position { get; set; }
}

// 订阅事件
EventManager.Subscribe<PlayerDeathEvent>(OnPlayerDeath);

// 发布事件
EventManager.Publish(new PlayerDeathEvent { PlayerId = "player1", Position = transform.position });

// 取消订阅
EventManager.Unsubscribe<PlayerDeathEvent>(OnPlayerDeath);

// 事件处理方法
private void OnPlayerDeath(PlayerDeathEvent evt)
{
    Debug.Log($"Player {evt.PlayerId} died at {evt.Position}");
}
```

#### 使用特性订阅事件

```csharp
public class EnemyManager
{
    [EventSubscriber(typeof(PlayerDeathEvent),Priority = 10)]
    public void OnPlayerDeath(PlayerDeathEvent evt)
    {
        // 处理玩家死亡事件
    }
}

// 注册带有特性的订阅者
EventManager.Register(enemyManager);

// 注销订阅者
EventManager.Unregister(enemyManager);
```

#### 引用类型事件

框架还支持引用类型事件，可以在事件处理过程中修改事件数据：

```csharp
// 定义引用类型事件
public struct DamageEvent
{
    public float Damage;
    public bool Handled;
}

// 订阅引用类型事件
EventManager.SubscribeRef<DamageEvent>(OnDamage);

// 发布引用类型事件
var evt = new DamageEvent { Damage = 10 };
EventManager.RefPublish(ref evt);

// 引用类型事件处理方法
private void OnDamage(ref DamageEvent evt)
{
    // 可以修改事件数据
    evt.Damage *= 0.5f;
    evt.Handled = true;
}
```

### 单例模式

框架提供了多种单例实现方式，满足不同场景的需求：

#### Singleton

纯C#单例，适用于非MonoBehaviour类：

```csharp
public class GameManager : Singleton<GameManager>
{
    // 单例实现
}

// 使用
GameManager.Instance.DoSomething();
```

#### MonoSingleton

MonoBehaviour单例，随场景销毁：

```csharp
public class UIManager : MonoSingleton<UIManager>
{
    // 单例实现
    
    protected override void OnInitialized()
    {
        // 初始化代码
    }
}

// 使用
UIManager.Instance.ShowDialog();
```

#### PersistentMonoSingleton

持久化的MonoBehaviour单例，在场景切换时不会被销毁：

```csharp
public class GameController : PersistentMonoSingleton<GameController>
{
    // 单例实现
    
    protected override void OnInitialized()
    {
        // 初始化代码
    }
}

// 使用
GameController.Instance.StartGame();
```

## 服务系统

服务系统用于管理全局服务，提供统一的注册和访问接口。

### 服务定义

```csharp
// 定义服务接口
public interface IStorageService : IService
{
    void Save(string key, string value);
    string Load(string key);
}

// 实现服务
public class PlayerPrefsStorageService : IStorageService
{
    public void Initialize()
    {
        // 服务初始化
    }
    
    public void Dispose()
    {
        // 服务清理
    }
    
    public void Save(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }
    
    public string Load(string key)
    {
        return PlayerPrefs.GetString(key);
    }
}
```

### 服务注册和获取

```csharp
// 注册服务
App.Instance.ServiceLocator.RegisterService<IStorageService, PlayerPrefsStorageService>();

// 获取服务
var storageService = App.Instance.ServiceLocator.GetService<IStorageService>();
storageService.Save("highScore", "100");

// 注销服务
App.Instance.ServiceLocator.UnRegisterService<IStorageService>();
```

### 自动注册服务

框架支持通过配置自动注册服务：

```csharp
// 创建服务配置
public class MyServiceConfig : ServiceConfig
{
    public MyServiceConfig()
    {
        // 添加服务注册器
        Registers.Add(new ServiceRegister<IStorageService, PlayerPrefsStorageService>());
        Registers.Add(new ServiceRegister<INetworkService, WebRequestNetworkService>());
    }
}

// 在App初始化时会自动注册配置中的服务
```

## 模块系统

模块系统用于管理游戏的功能模块，提供统一的生命周期管理。

### 模块定义

```csharp
// 定义模块
public class UIModule : Module
{
    [Inject]
    private IStorageService _storageService;
    
    public override void Initialize()
    {
        // 模块初始化
        Debug.Log("UIModule initialized");
    }
    
    public override void Dispose()
    {
        // 模块清理
        Debug.Log("UIModule disposed");
    }
    
    // 模块特定功能
    public void ShowMainMenu()
    {
        // 显示主菜单
    }
}
```

### 模块注册和获取

```csharp
// 注册模块
App.Instance.ModuleManager.RegisterModule<UIModule>();

// 获取模块
var uiModule = App.Instance.ModuleManager.GetModule<UIModule>();
uiModule.ShowMainMenu();

// 注销模块
App.Instance.ModuleManager.UnRegisterModule<UIModule>();
```

### 自动注册模块

框架支持通过配置自动注册模块：

```csharp
// 创建模块配置
public class MyModuleOrderConfig : ModuleOrderConfig
{
    public MyModuleOrderConfig()
    {
        // 按顺序添加模块注册器
        Registers.Add(new ModuleRegister<DataModule>());
        Registers.Add(new ModuleRegister<UIModule>());
        Registers.Add(new ModuleRegister<GameplayModule>());
    }
}

// 在App初始化时会自动按顺序注册配置中的模块
```

## 配置系统

配置系统用于管理游戏的配置数据，提供统一的访问接口。

### 配置定义

```csharp
// 定义配置
public class GameConfig : IConfig
{
    public string GameName { get; set; } = "My Game";
    public float MasterVolume { get; set; } = 0.8f;
    public bool EnableTutorial { get; set; } = true;
}

// 定义日志配置
public class LogConfig : IConfig
{
    public ILog Log { get; set; } = new UnityDebugLog();
}
```

### 配置访问

```csharp
// 获取配置
var gameConfig = ConfigManager.Instance.GetConfig<GameConfig>();
Debug.Log($"Game name: {gameConfig.GameName}");

// 修改配置
gameConfig.MasterVolume = 0.5f;
```

## 工具与扩展

### 扩展方法

框架提供了多种实用的扩展方法：

#### 颜色扩展

```csharp
// 设置透明度
Color color = Color.red.WithAlpha(0.5f);

// 转换为十六进制
string hex = Color.blue.ToHexString();
```

#### 集合扩展

```csharp
// 随机获取元素
var randomItem = myList.GetRandom();

// 洗牌
myList.Shuffle();
```

#### Transform扩展

```csharp
// 重置变换
transform.Reset();

// 设置本地位置的X坐标
transform.SetLocalX(5f);
```

#### 向量扩展

```csharp
// 设置向量的X坐标
Vector3 newPos = myVector.WithX(10f);

// 获取XZ平面上的向量
Vector2 xzVector = myVector.ToXZ();
```

### 对象池

框架提供了对象池系统，用于减少对象创建和销毁的开销：

```csharp
// 获取字典对象
var dict = DictionaryPool<string, int>.Get();

// 使用字典
dict["score"] = 100;

// 释放字典对象
DictionaryPool<string, int>.Release(dict);
```

## 编辑器工具

### Addressable管理

框架提供了Addressable资源管理工具，简化Addressable资源的配置和使用：

- **AddressableManagerWindow**：可视化管理Addressable资源
- **AddressableCodeGenerator**：自动生成Addressable资源的代码引用
- **AddressableNameSimplifier**：简化Addressable资源的命名
- **AddressableFolderMarker**：标记文件夹为Addressable资源组

### 表格导入工具

框架提供了表格导入工具，支持从Excel和CSV文件导入数据：

- **TableImporterWindow**：可视化导入表格数据
- **TableToClassGenerator**：自动生成表格数据的C#类
- **TableToJsonExporter**：将表格数据导出为JSON文件

使用方法：

1. 打开表格导入工具窗口（菜单：CnoomFrameWork > Table Importer）
2. 选择Excel或CSV文件
3. 配置导入选项
4. 点击导入按钮

## 最佳实践

### 项目结构

推荐的项目结构：

```
Assets/
  ├── Scripts/
  │   ├── Configs/        # 配置类
  │   ├── Events/         # 事件定义
  │   ├── Modules/        # 游戏模块
  │   ├── Services/       # 服务实现
  │   └── Bootstrap.cs    # 启动脚本
  ├── Resources/          # 资源文件
  ├── Scenes/             # 场景文件
  └── Packages/
      └── com.cnoom.cframework/  # 框架包
```

### 启动流程

推荐的启动流程：

1. 创建启动场景，包含一个空的GameObject
2. 添加Bootstrap脚本到GameObject
3. 在Bootstrap中初始化App和必要的模块

```csharp
public class Bootstrap : MonoBehaviour
{
    private void Awake()
    {
        // 初始化App
        var app = App.Instance;
        
        // 手动注册额外的模块（如果需要）
        app.ModuleManager.RegisterModule<MyCustomModule>();
        
        // 加载主场景
        SceneManager.LoadScene("MainMenu");
    }
}
```

### 依赖注入最佳实践

- 使用接口而非具体类型
- 保持服务和模块的单一职责
- 避免循环依赖
- 使用[Inject]特性标记依赖项

### 事件系统最佳实践

- 事件数据类应该是不可变的
- 避免在事件处理器中执行耗时操作
- 使用优先级控制事件处理顺序
- 及时注销不再需要的事件订阅

## API参考

### 核心类

- **App**：应用程序入口
- **ModuleManager**：模块管理器
- **ServiceLocator**：服务定位器
- **EventManager**：事件管理器
- **ConfigManager**：配置管理器

### 容器类

- **BaseContainer**：容器基类
- **RootContainer**：根容器
- **ChildContainer**：子容器
- **Injector**：依赖注入器
- **InstanceFactory**：实例工厂

### 特性类

- **InjectAttribute**：标记需要注入的成员
- **PostConstructAttribute**：标记初始化方法
- **EventSubscriberAttribute**：标记事件订阅方法

### 单例类

- **Singleton<T>**：纯C#单例基类
- **MonoSingleton<T>**：MonoBehaviour单例基类
- **PersistentMonoSingleton<T>**：持久化MonoBehaviour单例基类

### 接口

- **IService**：服务接口
- **IModule**：模块接口
- **IConfig**：配置接口
- **ILog**：日志接口
- **ISingleton**：单例接口