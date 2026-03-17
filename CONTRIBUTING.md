# 开发指南

感谢你对 Dism# 的关注！本文档介绍如何搭建开发环境、了解项目架构，以及参与贡献的流程。

## 目录

- [开发环境搭建](#开发环境搭建)
- [项目架构](#项目架构)
- [编码规范](#编码规范)
- [构建与运行](#构建与运行)
- [贡献流程](#贡献流程)
- [许可证说明](#许可证说明)

---

## 开发环境搭建

### 必需工具

| 工具 | 版本 | 用途 |
|------|------|------|
| [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) | 8.0+ | 编译和运行 |
| [Git](https://git-scm.com/) | 最新 | 版本管理 |

### 推荐工具

| 工具 | 用途 |
|------|------|
| [VS Code](https://code.visualstudio.com/) + C# Dev Kit 扩展 | 代码编辑 |
| [Visual Studio 2022](https://visualstudio.microsoft.com/) | 替代编辑器，提供完整 WPF 设计器 |
| [Windows ADK](https://learn.microsoft.com/windows-hardware/get-started/adk-install) | DISM API 开发参考 |

### 克隆仓库

```bash
git clone https://github.com/user/Dism-Sharp.git
cd Dism-Sharp
```

### 验证环境

```bash
dotnet --version   # 应输出 8.0.x
dotnet build       # 确认可以成功编译
```

---

## 项目架构

### 解决方案结构

```
DismSharp.sln
├── DismSharp.Core       # 核心库（MPL-2.0）
├── DismSharp.UI         # WPF 图形界面（MIT）
└── DismSharp.Console    # 控制台调试工具（MIT）
```

### DismSharp.Core — 核心库

核心库封装了所有 Windows 系统管理功能，不依赖任何 UI 框架。

```
DismSharp.Core/
├── Native/              # P/Invoke 声明
│   ├── DismApi.cs       #   Windows DISM API 绑定
│   ├── DismEnums.cs     #   DISM 枚举定义
│   ├── DismStructs.cs   #   DISM 结构体定义
│   └── Win32Api.cs      #   Win32 API 绑定
├── Modules/             # 功能模块
│   ├── SystemInfo.cs    #   系统信息查询
│   ├── PackageManager.cs    # 更新包管理
│   ├── FeatureManager.cs    # Windows 功能管理
│   ├── DriverManager.cs     # 驱动管理
│   └── CleanupEngine.cs     # 磁盘清理引擎
├── CleanupRules/        # 清理规则（可扩展）
│   ├── ICleanupRule.cs      # 清理规则接口
│   ├── FileCleanupRule.cs   # 基于文件的清理规则基类
│   ├── TempFilesRule.cs     # 临时文件清理
│   ├── LogFilesRule.cs      # 日志文件清理
│   └── ...                  # 其他清理规则
├── Helpers/             # 工具类
│   ├── FileHelper.cs        # 文件操作辅助
│   ├── PrivilegeHelper.cs   # 权限提升
│   ├── ServiceHelper.cs     # Windows 服务操作
│   └── WmiHelper.cs         # WMI 查询
├── DismSharpSession.cs  # DISM 会话管理
└── DismSharpException.cs    # 异常定义
```

### DismSharp.UI — 图形界面

基于 WPF + WPF-UI，采用 MVVM 模式。

```
DismSharp.UI/
├── Views/               # 页面（XAML + CodeBehind）
│   ├── DashboardPage    #   仪表盘（系统概况）
│   ├── PackagesPage     #   更新包管理
│   ├── FeaturesPage     #   Windows 功能
│   ├── DriversPage      #   驱动管理
│   ├── CleanupPage      #   磁盘清理
│   ├── AppxPage         #   Appx 管理（计划中）
│   ├── BootPage         #   启动项管理（计划中）
│   ├── ImagePage        #   映像操作（计划中）
│   └── AboutPage        #   关于
├── ViewModels/          # ViewModel（CommunityToolkit.Mvvm）
├── Converters/          # 值转换器
├── MainWindow.xaml      # 主窗口
└── App.xaml             # 应用入口
```

### 依赖关系

```
DismSharp.UI ──────→ DismSharp.Core
DismSharp.Console ──→ DismSharp.Core
```

UI 和 Console 依赖 Core，Core 不依赖任何上层项目。

---

## 编码规范

### 通用规则

- 语言版本：C# 12（.NET 8 默认）
- 启用 Nullable Reference Types（`<Nullable>enable</Nullable>`）
- 优先使用 P/Invoke 调用 Windows API，仅在 P/Invoke 无法实现时才考虑 C++/CLI
- 文件编码：UTF-8 with BOM

### 命名规范

| 类型 | 风格 | 示例 |
|------|------|------|
| 命名空间 | PascalCase | `DismSharp.Core.Modules` |
| 类 / 结构体 | PascalCase | `PackageManager` |
| 公开方法 | PascalCase | `GetInstalledPackages()` |
| 私有字段 | _camelCase | `_dismSession` |
| 参数 / 局部变量 | camelCase | `packageName` |
| 常量 | PascalCase | `MaxRetryCount` |
| 接口 | I + PascalCase | `ICleanupRule` |

### MVVM 规范

- ViewModel 使用 `CommunityToolkit.Mvvm` 的 Source Generator（`[ObservableProperty]`、`[RelayCommand]`）
- View 与 ViewModel 的绑定通过 DataContext 完成
- 避免在 CodeBehind 中编写业务逻辑

### 核心库规范

- `Native/` 目录仅存放 P/Invoke 声明和相关结构体 / 枚举，不包含业务逻辑
- `Modules/` 目录中的每个类负责一个功能域
- 清理规则通过实现 `ICleanupRule` 接口来扩展
- 所有 DISM 操作必须通过 `DismSharpSession` 管理会话生命周期

---

## 构建与运行

### 常用命令

```bash
# 编译
dotnet build

# 运行 UI（需要管理员权限）
dotnet run --project src/DismSharp.UI

# 运行控制台版本（需要管理员权限）
dotnet run --project src/DismSharp.Console

# 清理
dotnet clean

# 发布独立单文件
dotnet publish src/DismSharp.UI -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

### 管理员权限

大部分系统管理功能需要管理员权限。请在**管理员终端**中运行程序，否则相关操作会失败。

UI 项目已配置 `app.manifest` 请求管理员权限提升。

### 管理 NuGet 包

```bash
# 添加包
dotnet add src/DismSharp.Core package <包名>
dotnet add src/DismSharp.UI package <包名>

# 列出已安装的包
dotnet list src/DismSharp.UI package
```

---

## 贡献流程

### 1. Fork 与分支

```bash
# Fork 仓库后克隆
git clone https://github.com/<your-username>/Dism-Sharp.git
cd Dism-Sharp

# 创建功能分支
git checkout -b feature/your-feature-name
```

### 2. 开发

- 确保代码可以通过 `dotnet build` 编译
- 遵循上述编码规范
- 功能性改动应同时更新 UI 和 Console（如适用）

### 3. 提交

提交信息使用以下格式：

```
<type>: <简短描述>

<详细说明（可选）>
```

Type 可选值：

| Type | 说明 |
|------|------|
| `feat` | 新功能 |
| `fix` | Bug 修复 |
| `refactor` | 代码重构（不改变行为） |
| `docs` | 文档更新 |
| `style` | 代码格式调整 |
| `chore` | 构建 / 工具链变更 |

示例：

```
feat: 添加 Windows 服务管理模块

新增 ServiceManager，支持查看、启动、停止系统服务。
```

### 4. 提交 Pull Request

- PR 标题简明扼要
- 描述中说明改动内容和原因
- 如涉及 UI 变更，附上截图

### 添加新的清理规则

这是最常见的扩展场景之一：

1. 在 `src/DismSharp.Core/CleanupRules/` 下新建规则类
2. 实现 `ICleanupRule` 接口（或继承 `FileCleanupRule`）
3. 在 `CleanupEngine` 中注册新规则
4. 在 UI 的 `CleanupPage` / `CleanupViewModel` 中展示

---

## 许可证说明

本项目采用双许可证，贡献代码时请注意：

| 你修改的项目 | 适用许可证 | 你需要做什么 |
|------------|-----------|------------|
| **DismSharp.Core** | MPL-2.0 | 你修改的**文件**必须以 MPL-2.0 开源。你可以在专有项目中引用（不修改）Core 而无需开源你的项目 |
| **DismSharp.UI** | MIT | 几乎无限制，保留版权声明即可 |
| **DismSharp.Console** | MIT | 几乎无限制，保留版权声明即可 |

提交 PR 即表示你同意以上述许可证授权你的贡献。

### MPL-2.0 简要说明

- **文件级 Copyleft**：只有你修改过的源文件需要开源，而非整个项目
- **可与专有代码混合**：你可以在闭源项目中链接 / 引用 DismSharp.Core
- **修改必须回馈**：如果你修改了 Core 的某个文件并分发，修改后的文件必须以 MPL-2.0 公开

### MIT 简要说明

- 可用于任何目的，包括商业用途
- 可自由修改和分发
- 唯一要求是保留版权声明和许可证文本
