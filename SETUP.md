# 中文安装与使用指南

本指南适用于 [LocalExile-Foulborn](https://github.com/mmwmdxg/LocalExile-Foulborn) 的 `main` 源码版本。功能说明及限制见 [首页](README.md)，更完整的开发排错见 [上游环境指南](Agents/GettingStarted.md)。

> 客户端必须为 PoE **3.29.3b 独立版**。始终在仓库根目录执行命令。不要通过官方启动器、Steam 或直接双击游戏程序启动本地客户端，以免自动更新。

## 一、准备环境

安装 Git、[.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) 和 [Node.js 24](https://nodejs.org/)，使用 Windows 10/11 x64 与 PowerShell 5.1+。先确认：

```powershell
dotnet --version
node --version
npm --version
git clone https://github.com/mmwmdxg/LocalExile-Foulborn.git
cd LocalExile-Foulborn
npm --prefix Tools/ooz ci
dotnet restore
dotnet build --no-restore
```

.NET 应为 `10.x`，Node 建议为 `24.x`。首次安装需要联网获取依赖，游戏数据由自己的客户端提取，不从仓库下载。

## 二、复制客户端

先关闭客户端，保留一份原始安装备份。把整个客户端目录复制到仓库下的 `Client`，不要只复制程序文件：

```powershell
# 用自己的安装目录替换示例路径；robocopy 返回值小于 8 通常表示成功。
robocopy "D:\Games\Path of Exile" Client /E
Copy-Item Client/PathOfExile.exe Client/Game.exe
```

目录结构应包含：

```text
LocalExile-Foulborn/
  Client/
    Content.ggpk
    PathOfExile.exe
    Game.exe
    …其余客户端文件
  scripts/
  src/
  Tools/
```

不要覆盖已有的本地客户端或存档来试错；其他版本客户端无法靠改文件名变成 3.29.3b。

## 三、首次初始化

以下命令依次执行，某一步失败时先停止排错，不要继续启动：

```powershell
# 生成自己的登录密钥；已有密钥时保留，不要重复生成。
dotnet run --project src/GameServer.Auth -- keygen Client/Game.exe
# 生成信任本地密钥的客户端副本，不覆盖 Game.exe。
dotnet run --project src/GameServer.Auth -- patch Client/Game.exe Client/Game.patched.exe
# 显式修改客户端网关列表，必须在客户端关闭时执行。
dotnet run --project src/GameServer.Patch -- gateway --login 127.0.0.1:20493
```

网关命令会修改客户端副本中的 GGPK，并保存原网关列表。不要对唯一的原始客户端副本操作。密钥已存在时，跳过 keygen，确保补丁客户端与现有密钥配套。

## 四、创建本地账号

不是随便输入账号密码就能登录。先执行：

```powershell
dotnet run --project src/GameServer.Auth -- register myaccount
```

程序启动后输入密码并回车（从标准输入读取，终端可能回显，请勿使用官方账号密码）。看到账号创建成功后，在游戏登录框输入同一个账号名和密码。账号只保存在本地；已有账号无需重新注册。

## 五、启动与繁体中文

```powershell
.\scripts\dev\live.ps1
```

首次启动会提取游戏数据；耗时取决于机器与磁盘。后续启动会检查提取状态。不要把启动脚本输出接到管道，也不要为了等待界面再开第二个客户端。

繁中入口：

```powershell
.\Start-LocalExile-TW.cmd
```

该入口仍调用标准启动脚本，并设置项目内临时目录。它通过 `fr` 语言入口使用已有繁中资源映射，**不负责下载、安装或转换中文资源**；仅有标准英文／法文资源的客户端不能保证显示中文。

若脚本被执行策略阻止，可在当前 PowerShell 会话中运行：

```powershell
Set-ExecutionPolicy -Scope Process Bypass -Force
.\scripts\dev\live.ps1
```

服务启动后，用第四步创建的本地账号登录，并创建角色。日志位于仓库下的 `logs` 目录；不要因出现某一行启动提示就认定所有服务和玩法都已验证。

## 六、测试物品与品质

进入游戏后可逐条输入：

```text
/item Onyx Amulet
/item Intrinsic Catalyst 10
/item foulborn Mageblood
```

把测试首饰拿到鼠标上，再输入：

```text
/item_quality 20 6
/item_mods
```

常用品质类型：`3` 防御、`4` 生命／魔力、`5` 抗性、`6` 属性、`8` 速度、`9` 暴击、`16` 前缀、`17` 后缀。品质只影响匹配且允许缩放的词缀，不会放大整件物品的所有数值。

催化剂的自动回归测试已通过，但实机属性对照仍待完成。可先记录测试首饰的力量贡献，增加品质，再检查角色面板与取下装备后的恢复值。穢生物品标题或外观出现，不等于其特殊机制全部正确；其他功能边界见首页。

## 七、更新、备份与排错

更新前关闭本地服务与客户端，并备份本地数据目录（含存档和密钥）及客户端副本。之后从本仓库获取更新：

```powershell
git pull --ff-only
npm --prefix Tools/ooz ci
dotnet restore
dotnet build --no-restore
```

有本地改动时先检查 `git status`，不要用强制重置清掉自己的修改。启动器部分更新提示仍来自上游，不要直接用上游发布包覆盖此版本。

| 现象 | 优先检查 |
|---|---|
| 找不到补丁程序 | 第三步是否成功，程序是否被安全软件隔离；不要盲目关闭系统防护 |
| 解压工具报错／缺少 ooz-wasm | Node 是否可用，工具依赖安装是否成功 |
| 数据表缺失 | 查看提取日志，必要时显式运行 `.\scripts\dev\extract-data.ps1` |
| 登录断开 | 核对账号、登录密钥与补丁是否配套，再看 Auth／Gateway 日志 |
| 临时目录拒绝访问 | 使用繁中入口的项目内临时目录，或检查本机临时目录权限；不要删除存档排错 |
| 中文未生效 | 确认客户端是否已有对应中文资源映射；语言参数不是中文补丁 |
| 功能表现与正式服不同 | 先查首页限制及功能状态，不要将本地规则当作官方完整实现 |

开发验证命令：

```powershell
dotnet build
dotnet test tests/GameServer.Gateway.Tests
dotnet test tests/GameServer.Core.Tests
dotnet test tests/GameServer.Data.Tests
```

## 上游与许可

本项目基于 [LocalExile](https://github.com/LocalExile/LocalExile)，与 Grinding Gear Games 无关。许可及分发要求见 [LICENSE](LICENSE) 与 [第三方声明](THIRD-PARTY-NOTICES.md)。不要上传游戏文件、提取数据、存档、账号或密钥。
