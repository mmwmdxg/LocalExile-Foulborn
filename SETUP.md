# LocalExile-Foulborn

LocalExile 本地服务端（PoE 1 / 3.29.3b），在官方主干上加入了以下**已实机验证**的功能。

## 功能清单

| 功能 | 指令 / 说明 |
|---|---|
| 穢生传奇 | `/item foulborn <传奇名>`（256 个传奇有映射，替换最右侧显式词条） |
| 穢生通货 | 穢生增幅石 / 富豪石 / 崇高石，规则同普通对应通货 |
| 首饰催化剂 | 品质数值 + 品质类型（属性/抗性/防御…） |
| 品质指令 | `/item_quality <值> <类型行>`，`/item_mods` 显示类型名 |
| 裁缝宝珠 | 替换胸甲附魔；1/4 概率重铸插槽；附魔颜色约束会改写插槽颜色 |
| 机会石 | 按传奇掉落权重决定是否成为该底材传奇，失败保持普通并消耗 |
| 正火伤害 | 吃增伤（全局/火焰/元素/持续/范围）、DoT 倍率与支持宝石总增 |

---

## 一、环境准备

### 1. 安装 .NET 10 SDK

下载并安装 <https://dotnet.microsoft.com/download/dotnet/10.0>，装完在 PowerShell 里确认：

```powershell
dotnet --version
```

应输出 `10.x.x`。

### 2. 准备客户端（需自行合法持有）

必须是 **3.29.3b 独立客户端（standalone / 带 Content.ggpk）**，Steam 或 Epic 版不适用。

把整个客户端目录的内容复制到仓库的 `Client/` 下，然后：

1. 复制一份 `PathOfExile.exe`，重命名为 **`Game.exe`**；
2. 确认 `Client/Content.ggpk` 存在。

整理完成后应是：

```
LocalExile-Foulborn/
  Client/
    Content.ggpk          ← 必须有
    Game.exe              ← 由 PathOfExile.exe 复制改名
    PathOfExile.exe       ← 原文件保留
    ...客户端其余文件
```

> `Game.patched.exe` 和 `data/` 由下面第 3 步生成，**不需要**手动准备。

---

## 二、首次初始化（只做一次）

在仓库根目录打开 PowerShell：

```powershell
& {
    Set-Location '路径\LocalExile-Foulborn'

    # 1) 生成登录签名密钥
    dotnet run --project src/GameServer.Auth -- keygen .\Client\Game.exe

    # 2) 生成打过补丁的客户端
    dotnet run --project src/GameServer.Auth -- patch .\Client\Game.exe .\Client\Game.patched.exe
}
```

成功后应出现 `data/login-keys.json` 和 `Client/Game.patched.exe`。

---

## 三、启动

```powershell
.\scripts\dev\live.ps1
```

- **第一次**会从 `Client/Content.ggpk` 提取全部游戏数据到 `data/`，**可能几十分钟**；
- 之后启动只需几秒；
- 出现 `servers up` 后客户端会自动打开。

想用繁体中文界面：

```powershell
.\Start-LocalExile-TW.cmd
```

> ⚠️ 只通过 `live.ps1` 启动客户端。直接双击游戏 exe 会连官方补丁服务器，可能把客户端更新掉。

---

## 四、首次登录

1. 邮箱随意填（例：`myaccount`），密码随意；
2. 点 **LOG IN**；
3. 首次进入会创建角色。

如果登录界面显示 “An unexpected disconnection occurred.”，先确认 `live.ps1` 窗口里出现过 `servers up`。

---

## 五、快速验证功能

进入游戏后依次输入：

```text
/item foulborn Cloak of Defiance
/item foulborn Might of the Meek
/item_quality 20 6
/item Intrinsic Catalyst 10
/item Tailoring Orb 10
/item Foulborn Orb of Augmentation 10
```

预期：

- 前两条生成标题带「穢生」的紫色物品；
- `/item_mods` 显示 `quality 20% (Attribute)`；
- 催化剂每次 +1% 品质，到 20% 停止；
- 裁缝石替换附魔，偶尔改变插槽数；
- 穢生增幅石可给魔法物品加词缀。

---

## 六、常见问题

| 现象 | 原因与处理 |
|---|---|
| 脚本被拒绝执行（SecurityError） | 用 `Set-ExecutionPolicy -Scope Process Bypass -Force` 后重试 |
| `Game.patched.exe missing` | 回到第二步执行 `patch` 命令 |
| `Content.ggpk missing` | `Client/` 不是独立客户端，换 standalone 版 |
| `login-keys.json missing` | 回到第二步执行 `keygen` |
| 构建报文件被占用 | 先结束 `GameServer.Gateway` / `dotnet` 进程再 build |
| 模块加载失败、提示 Access denied | 删除 `%LOCALAPPDATA%\Temp\localexile-mod-copy` 后重启 |
| 登录后立刻掉线 | 看 `logs/live-gateway.log` 是否有 “has no Game module” |

---

## 七、更新与重新生成

```powershell
# 拉取上游或本仓库更新
git pull

# 重新生成穢生映射（可选）
python scripts/re/foulborn_map.py            # 用 artifacts/ 缓存
python scripts/re/foulborn_map.py --refresh  # 重新抓取 poedb
```

生成结果写入 `src/GameServer.Gateway/Item/FoulbornMap.cs`。

## 八、测试

```powershell
dotnet build
dotnet test tests/GameServer.Gateway.Tests
```

## 上游与许可

基于 [LocalExile/LocalExile](https://github.com/LocalExile/LocalExile)（GPL-2.0）。本仓库的玩法改动集中在
`foulborn-crafting` 分支，客户端与提取数据不随仓库分发。
