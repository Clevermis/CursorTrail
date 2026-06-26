# ✨ CursorTrail

> A lightweight, customizable mouse trail effect tool built with C#.
>
> 一个轻量级、可自定义的 C# 鼠标拖尾特效工具。

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET Version](https://img.shields.io/badge/.NET-6.0%2B-blue)](https://dotnet.microsoft.com/)
[![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)](https://www.microsoft.com/windows)
[![Stars](https://img.shields.io/github/stars/yourusername/CursorTrail?style=social)](https://github.com/yourusername/CursorTrail)

---

## 📸 效果预览

待开发....

---

## ✨ 功能特性

1. 红色鼠标拖尾
  鼠标移动时，在鼠标位置绘制一系列红色圆点，形成拖尾轨迹
  拖尾点的大小和透明度从新到旧渐变
2. 自动淡出消失
  每个拖尾点具有独立生命周期,鼠标停止移动后，已有拖尾点会逐渐淡出直至完全消失，不会永久残留
3. 简单退出
  Alt+F4 退出程序


---

## 🚀 快速开始

### 系统要求

- Windows 10 /11
- [.NET 6.0 Runtime](https://dotnet.microsoft.com/download) 或更高版本
- （或 .NET Framework 4.8 版本，视具体分支而定）

### 下载与运行

1. 前往 [Releases]([https://github.com/yourusername/CursorTrail/releases](https://github.com/Clevermis/CursorTrail/releases/tag/%60%3Cversion%3E%60v1.0.0)) 页面下载最新版本
2. 解压压缩包
3. 双击运行 `CursorTrail.exe`

### 从源码编译

```bash
# 克隆仓库
git clone https://github.com/yourusername/CursorTrail.git

# 进入项目目录
cd CursorTrail

# 还原依赖并编译
dotnet restore
dotnet build -c Release

# 运行
dotnet run
