# LocalExile-Foulborn

本仓库是 [LocalExile](https://github.com/LocalExile/LocalExile)（PoE 1 / 3.29.3b 本地服务端）的派生版本，
在保留上游代码的基础上，**额外加入并在本机实机验证**了以下功能：

| 功能 | 用法 |
|---|---|
| 穢生（Foulborn）传奇生成 | `/item foulborn <传奇名>` |
| 穢生增幅石 / 富豪石 / 崇高石 | 与普通对应通货同规则 |
| 首饰催化剂（品质 + 品质类型） | 右键催化剂点首饰 |
| 品质指令与显示 | `/item_quality <值> <类型行>`、`/item_mods` |
| 裁缝宝珠（附魔替换 + 插槽重铸 + 颜色约束） | 右键宝珠点胸甲 |
| 机会石按传奇掉落权重升级 | 右键点普通物品 |
| 正火伤害接入增伤与支持宝石总增 | 无需指令，自动生效 |

安装与首次启动步骤见 **[SETUP.md](SETUP.md)**。

## 说明

- 本仓库**不包含**游戏客户端、提取的游戏数据或作者的多媒体资料；
- 上游的完整历史与文档（`Agents/`、`src/` 等）保留在原处，仅作代码与说明使用；
- 上游作者与项目地址：<https://github.com/LocalExile/LocalExile>；
- 许可与第三方组件见 `LICENSE` 与 `THIRD-PARTY-NOTICES.md`。

本项目仅供本地游玩与学习，与 Grinding Gear Games 无关，也不支持任何公开服务器。
