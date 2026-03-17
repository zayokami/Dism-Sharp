<p align="center">
  <h1 align="center">Dism#</h1>
  <p align="center">
    现代化的 Windows 系统管理工具
    <br />
    Sharp and Sharp.
  </p>
</p>

<p align="center">
  <a href="LICENSE-MIT"><img src="https://img.shields.io/badge/shell-MIT-green.svg" alt="MIT License"></a>
  <a href="LICENSE-MPL-2.0"><img src="https://img.shields.io/badge/core-MPL--2.0-orange.svg" alt="MPL-2.0 License"></a>
  <img src="https://img.shields.io/badge/.NET-8.0-purple.svg" alt=".NET 8.0">
  <img src="https://img.shields.io/badge/platform-Windows-blue.svg" alt="Windows">
</p>

---

Dism#（Dism Sharp）是基于Dism的全能系统工具，使用 C# 编写。
本项目与 Dism++ 无任何关系。

## 功能

| 功能 | 状态 | 说明 |
|------|:----:|------|
| 系统信息 | ✅ | 查看 OS、CPU、内存、磁盘等详细信息 |
| 更新包管理 | ✅ | 查看、卸载 Windows 更新包 |
| Windows 功能 | ✅ | 启用 / 禁用 Windows 可选功能 |
| 驱动管理 | ✅ | 查看、备份、安装、卸载系统驱动 |
| 磁盘清理 | ✅ | 临时文件、日志、缩略图缓存、Update 缓存、回收站、预读取 |
| Appx 管理 | 🚧 | UWP / MSIX 应用管理 |
| 启动项管理 | 🚧 | 系统启动配置 |
| 映像操作 | 🚧 | WIM / ESD 映像管理 |

## 快速开始

**系统要求：** Windows 10/11 x64 · [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) · 管理员权限

### 下载

前往 [Releases](https://github.com/zayokami/Dism-Sharp/releases) 下载最新版本（独立发布版无需安装 .NET）。

### 从源码构建

```bash
git clone https://github.com/zayokami/Dism-Sharp.git
cd Dism-Sharp
dotnet run --project src/DismSharp.UI    # 需要管理员终端
```

> 详细的构建命令和开发环境配置请参阅 [开发指南](CONTRIBUTING.md)。

## 技术栈

C# · .NET 8.0 · WPF + [WPF-UI](https://github.com/lepoco/wpfui) · [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) · Windows DISM API (P/Invoke)

## 许可证

本项目采用**双许可证**：

| 组件 | 许可证 | 说明 |
|------|--------|------|
| **DismSharp.Core** | [MPL-2.0](LICENSE-MPL-2.0) | 核心库 — 修改的文件须以 MPL-2.0 开源 |
| **DismSharp.UI / Console** | [MIT](LICENSE-MIT) | 外壳程序 — 可自由使用、修改、分发 |

## 贡献

欢迎参与贡献！请阅读 [开发指南 (CONTRIBUTING.md)](CONTRIBUTING.md) 了解项目架构、编码规范和提交流程。

## 致谢

- [Dism++](https://github.com/Chuyu-Team/Dism-Multi-language)
- [WPF-UI](https://github.com/lepoco/wpfui) · [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet)
