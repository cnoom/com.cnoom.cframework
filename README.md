#Cnoom Framework (CFramework)

Unity模块化开发框架，提供模块管理、依赖注入、事件总线等核心功能。

## 功能特性
- 🧩 模块化架构管理
- 📡 事件总线系统
- 🔍 基于配置的模块注册机制
- 📦 控制反转容器支持

## 安装
1. 在manifest.json中添加：
```json
{
  "dependencies": {
    "com.cnoom.cframework": "https://github.com/cnoom/com.cnoom.cframework.git"
  }
}
```
2.通过包管理器->添加来自git URL的包
3.直接在下载并放置于项目/Packages文件夹下

## 功能介绍
### App(框架集成核心)
App是CFramework的核心，将在场景加载后自动初始化。App包含以下功能：
- 模块管理 具有一个模块管理类，用于注册和加载模块。
- 控制反转容器 提供一个简单的控制反转容器，用于管理依赖注入。
- 事件总线 提供一个事件总线系统，用于在模块之间传递事件。
- 日志输出 提供一个日志输出系统，用于输出日志信息。
#### 模块管理
模块管理是CFramework的核心功能之一，用于管理和加载模块。模块管理包含以下功能：
- 模块注册 注册时自动将其加入到App的控制反转容器中并自动注入所需依赖和监听事件
- 模块取消注册 取消注册时自动将其从App的控制反转容器中移除并停止监听事件
- 模块自动注册 通过配置文件在App初始化时自动注册模块(可以在此时用自己的模块替代框架模块)
#### 控制反转容器
控制反转容器是CFramework的核心功能之一，用于管理依赖注入(单例/瞬时)。控制反转容器包含以下功能：
- 依赖注入 可以手动注入依赖也可以通过```InjectAttribute```属性自动注入依赖
- 依赖注册 可以手动注册依赖
- 依赖取消注册 可以手动取消注册依赖
#### 事件总线
事件总线是CFramework的核心功能之一，用于在模块之间传递事件也可以用于一些全局事件的注册监听。事件总线包含以下功能：
- 事件监听 可以监听指定类型的事件
- 事件取消监听 可以取消监听指定类型的事件
- 事件触发 可以触发指定类型的事件
- 通过属性自动注册 可以通过```SubscribeAttribute```属性注册事件监听,同时该属性也负责取消监听
### 现有模块介绍
#### ActionManager
ActionManager是CFramework的一个模块，用于主线程延迟执行的Action。ActionManager包含以下功能：
- 延迟秒执行 可以延迟指定时间执行Action
- 延迟帧执行 可以延迟指定帧执行Action
- 可以自由链式组合
#### AssetsModule
AssetsModule是CFramework的一个模块，通过Addressable加载资源。AssetsModule包含以下特色功能：
- 通过标记文件夹将文件夹下的所有资源一键加入到Addressable中,同时将文件夹作为资源的标签
- 生成资源地址的静态类:以文件名作为字段名,以资源地址作为字段值
- 自动计数引用 加载资源时会自动计数引用,释放资源时会自动计数减一,当引用计数为0时会自动释放缓存资源
### UIModule
UIModule是CFramework的一个模块，用于管理UI。UIModule包含以下功能：
- 分层处理打开的ui
- 自动缓存ui
- 内置ui动画集成
