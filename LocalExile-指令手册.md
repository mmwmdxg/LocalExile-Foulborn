# LocalExile 游戏内指令手册（中文）

整理日期：2026-11-03。适用：本仓库 LocalExile／PoE 3.29.3b。

本手册核对当前源码中的 **93 个核心及剧情命令**，另含内置帮助命令 /commands（别名 /com）。不是官方在线服命令大全；不是所有命令都经过本次实机验证。实际可用性受当前联赛模块、构建版本及额外 Mod 影响，以游戏内 /commands 为准。

## 1. 使用与安全

- 进入角色场景后按 Enter，在游戏聊天框输入命令，每次一条；不是在 PowerShell 输入。
- 命令保持英文；汉化只改变显示，不保证服务器支持中文物品名。参数里的 <…> 是必填占位，[…] 为可选，不要照抄尖括号。
- 参数按空格分隔；物品名称直接带空格，不要随意加引号。例：/item Mageblood，不是 /item MagebloodHeavy Belt。
- “鼠标上物品”指在背包点击后拿在光标上的物品，不是鼠标悬停。修改前留足背包空间。
- 角色、物品、仓库、任务及全局设置的修改可能持久保存；不要把开发命令当成无副作用的演示。
- 推荐修改前正常停止服务后备份存档。专家模式切勿用伤害／击杀／任务预设做随意测试。
- 不需要修改客户端、重打汉化或重新提取数据来使用这些命令。

## 2. 帮助与查找

~~~text
/commands
/com 2
/com item
/com game_data
~~~

/commands 显示第一页，每页 8 条；/com 2 查看第二页；/com 名称显示精确帮助，或名称包含该文本的命令。额外 Mod 命令随加载状态变化。

## 3. 常用速查

| 功能 | 命令 | 注意 |
|---|---|---|
| 增加十万经验 | /add_xp 100000 | 增量，非总经验 |
| 设为 70 级 | /set_level 70 | 设置目标等级，非增加 70 级 |
| 增加一万金币 | /gold 10000 | 账户金币 |
| 增加 10 天赋点 | /add_passive_points 10 | 改变存档，不等于完成任务 |
| 增加 10 重置点 | /add_refund_points 10 | 与升华点不同 |
| 查询抗性 | /resistances | 只查询 |
| 恢复生命 | /heal | 不能承诺解除专家永久死亡 |
| 添加魔血 | /item Mageblood | 不加底材名称 |
| 添加稀有铁戒指 | /item Iron Ring rare | 随机词缀 |
| 添加火球宝石 | /gem Fireball 20 20 | 等级 20，品质 20 |
| 查看鼠标上词条 | /item_mods | 先拿起物品 |
| 设置品质/类型 | /item_quality 20 6 | 6 = 属性品质 |
| 查看当前位置 | /pos | 查询 |

## 4. 经验倍率（全局，持久化）

~~~text
/game_data setting experience_multiplier 5
/game_data setting experience_multiplier
/game_data clear_setting experience_multiplier
~~~

依次为：设为 5 倍、查询、恢复默认 1 倍。倍率范围 0–1000；0 会停止此路径的击杀经验。此设置自动保存并重新加载，不必重启；作用于服务器全局的怪物击杀经验，不保证所有经验来源同倍变化。等级差惩罚仍然存在。不要把 5 写成 500（500 是 500 倍）。

## 5. 装备、正常满 roll 与品质

### 添加装备

~~~text
/item Limbsplit
/item Springleaf
/item Goldrim
/item Ondar's Clasp
/item Doedre's Tenure
/item Wanderlust
/item Tabula Rasa
/item Pyre
/item Kikazaru
/item Atziri's Foible
/item Hidden Potential
~~~

名称对应之前装备表；并非逐件验证效果。双手斧不能与盾牌同时装备，两副手套是替换选择。角色仍需满足装备要求。

### 断罪（Limbsplit）正常满 roll

先拿起目标斧头，再逐条输入：

~~~text
/item_mods
/item_mod set explicit StrengthUniqueTwoHandAxe3 30
/item_mod set explicit LocalIncreasedPhysicalDamagePercentUniqueTwoHandAxe3 100
/item_mods
~~~

本机提取表中力量范围 15–30，物理伤害提高范围 80–100%；固定附加物理伤害 5–10、力量宝石等级 +1 无需改。set 只替换已有同 ID 词条，不能拿这两条命令套所有装备。其他传奇先查 /item_mods；“最佳值”可能是负面词缀的较小惩罚。

### 品质

~~~text
/item Blacksmith's Whetstone 20
/item Armourer's Scrap 20
/item Glassblower's Bauble 20
~~~

分别是武器、护甲／盾牌、药剂品质通货。把装备放回背包，右键通货再左键装备。当前通货实现每次 +1%，上限 20%；不等于官方所有特殊方法的全局上限。当前核心注册表没有 /quality 命令。

### 孔色与连线

~~~text
/sockets RRGBBB
/sockets RRG B
~~~

对鼠标上物品生效：第一条为六连，第二条为三连加单孔；R 红、G 绿、B 蓝、W 白。受底材孔数容量限制，默认不能给三孔魔杖硬加六孔。

## 6. 风险较高的命令

- /quest_flag、/quest_preset、/league_flag：修改任务状态，可能跳过奖励、制造不一致；不要为了清 NPC 感叹号随意使用。
- /game_data：全局持久化；mod 修改可能影响之后生成的所有装备，不是只改手中一件。
- /item_mod clear、/roll、/reset_passives、/reset_ascendancy：可能删除词缀、重新随机或改变天赋；先读具体帮助。
- /kill 会触发经验、掉落、任务；/despawn 只移除，不可互相代替。
- /hit、/spellhit、/blockhit、/spawn_mob：可能导致角色死亡。
- /god on 是高额强化，不是完全免伤；/god immune 才额外免伤。/god off 关闭。
- /set_speed 会干扰移动测试；怪物卡顿不要靠开高倍人物速度解决。
- /raw、/ui、/death_screen、/wp_state 等是协议／界面测试，不建议日常游玩使用。

## 7. 已确认不应凭空使用的命令

当前核心注册表未发现 /quality、/reveal、/maphack；不要把未知命令当作功能。Tab 只显示地图覆盖层，不揭示未探索区域。Mod 将来可能添加同名命令，以 /commands 为准。

## 8. 全部核心与剧情命令（按名称排序）

下面保留源码英文用法，附中文用途。长命令用代码块方便复制，但参数占位需替换。标为 dev／调试不等于安全，不保证长期保存或官方机制完全实现。

### /accuracy

查看对指定等级怪物的命中率。

~~~text
/accuracy [monster level] (accuracy rating and attack hit chance; default: your level)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1099>)

### /add_passive

按图节点 ID 分配相连天赋。

~~~text
/add_passive <PassiveSkillGraphId> [...] (dev: allocate a connected set of passive nodes)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1030>)

### /add_passive_points

增加额外天赋点，保存。

~~~text
/add_passive_points <n> (dev: add n extra passive skill points, saved)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L995>)

### /add_refund_points

增加天赋重置点，保存。

~~~text
/add_refund_points <n> (dev: add n refund points, saved)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1013>)

### /add_xp

增加角色经验，可升级并保存。

~~~text
/add_xp <amount> (experience as from a kill: levels up, saved)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L901>)

### /adorned

生成 The Adorned 珠宝；正常词条范围 0–100，允许的调试范围更大。

~~~text
/adorned [0-1000] (dev: The Adorned unique jewel into the inventory, corrupted, for passive-jewel testing; the normal item range is 0-100)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L443>)

### /ai

开启或关闭怪物追击 AI；不是修复怪物同步问题。

~~~text
/ai [on|off] (monsters chase you when you come near; emerge monsters rise either way)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1850>)

### /anim

在最近怪物上播放指定动画。

~~~text
/anim <index> [speed] (0x1b2 kind 0: play an animation on the nearest monster)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1821>)

### /ascendancy

列出或选择可用升华，支持 normal／phrecia 分类。

~~~text
/ascendancy [normal|phrecia] [name] (list or choose an available ascendancy)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L913>)

### /ascendancy_points

按试炼阶段发放升华点，每阶段只发一次。

~~~text
/ascendancy_points <normal|cruel|merciless|eternal|all> (dev: grant each Labyrinth's two ascendancy points once)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L959>)

### /atlas

查看或选择通用地图回退布局。

~~~text
/atlas [layout] (dev: show or choose the fallback layout generic maps open)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1333>)

### /atlas_points

查看或设置异界天赋点预算，保存。

~~~text
/atlas_points [<n>] (dev: show or set the Atlas passive point budget, saved)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1357>)

### /atlas_tree

打开异界天赋面板。

~~~text
/atlas_tree (dev: open the Atlas passive tree; S→C 0x7b kind 0x11)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1352>)

### /auras

查看光环和保留。

~~~text
/auras (active auras, their reservation and the character's efficiency)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1139>)

### /base_defence

查看或设置护甲底材防御浮动档位 0–31，不是品质。

~~~text
/base_defence [0 to 31] (show or set the held armour's shared defence base roll)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L855>)

### /block

查看攻击和法术格挡率。

~~~text
/block (attack and spell block chance and caps)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1194>)

### /block_luck

强制幸运格挡，off 关闭。

~~~text
/block_luck <attack|spell|both|off> (dev: force lucky block)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1201>)

### /blockhit

模拟经过格挡和抗性的命中，可能死亡。

~~~text
/blockhit <attack|spell> <fire|cold|lightning|chaos|physical> <damage> (dev: hit through block and resistance)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1215>)

### /cluster_jewel

生成星团珠宝并指定天赋和插槽。

~~~text
/cluster_jewel <small|medium|large> [passives] [sockets] [theme mod] [notable mod ...] (dev: a non-unique cluster jewel with its enchantments, into the inventory; defaults 3/4/8 passives and 0/1/2 sockets)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L621>)

### /corrupt

将鼠标上物品标记为腐化，不等于执行完整瓦尔石随机流程。

~~~text
/corrupt (dev: marks the held item corrupted, e.g. to make a corrupted magic jewel)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L653>)

### /cosmetic

列出、设置或关闭传送门外观。

~~~text
/cosmetic portal <name, number or part of a name> | /cosmetic list [filter] | /cosmetic off (the portal cosmetic: the shop effect replaces the base portal visual)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L732>)

### /crit

查看攻击／法术暴击率与倍率。

~~~text
/crit (attack and spell critical strike chance and multiplier)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1186>)

### /death_screen

显示测试死亡界面，仅调试。

~~~text
/death_screen [hex flags] (show the death screen; default: this league's; 2 hides it)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1269>)

### /defences

查看护甲、闪避等防御。

~~~text
/defences [monster level] (armour, evasion and avoidance; default: your level)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1120>)

### /despawn

移除怪物，不等于正常击杀；不要用于需要击杀的任务目标。

~~~text
/despawn [all | <id>] (default: the last one spawned)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1776>)

### /game_data

查询和持久化修改全局游戏数据，包括经验倍率；立即重新加载。

~~~text
/game_data reload | reference | mod <mod> <stat> <min> <max> | clear_mod <mod> [stat] | limit <key> <value> | clear_limit <key> | setting <key> <value> | clear_setting <key> | unique_weight <name> <weight> [ruthless] | clear_unique_weight <name> | unique_weights [filter] | settings | list (dev: SQLite game-data overrides; no restart)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1504>)

### /gem

生成指定等级、品质的技能宝石；此命令品质限 0–20。

~~~text
/gem <name> [level] [quality] (a gem into the inventory at that level and quality, e.g. /gem Fireball 20 20, /gem Ice Nova of Frostbolts, /gem Vaal Fireball 5)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L662>)

### /gem_xp

设置所有背包和已插槽宝石经验，不是只改一颗。

~~~text
/gem_xp <amount|near> (dev: set the experience of every gem in the inventory and the sockets; near = one below the next level's threshold)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L687>)

### /god

增强角色的开发模式；on 不是完全无敌，immune 才额外免伤；off 关闭。

~~~text
/god [on | off | immune] (toggles, or on/off: 1000% increased attack and cast speed, 100% of life and 10 000 mana regenerated per second, 200% increased area of effect, +10 000 life, +1000 attributes; immune: also no damage taken; kept across area changes)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1243>)

### /gold

增加账户金币；负数扣除，保存。

~~~text
/gold <amount> (dev: add gold, or take it with a negative amount; the account's, saved)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L988>)

### /goto

切换区域，可指定种子；不会替你正常完成沿途任务。

~~~text
/goto <area id or name> [seed] (dev: change area as if through an area transition, e.g. /goto 1_1_3; with a seed, a new instance at that seed, e.g. /goto 1_1_5 1384217569)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1373>)

### /guild_stash_place

临时移动并旋转公会仓库。

~~~text
/guild_stash_place <x> <y> <degrees> (dev: move and turn the guild stash of this area until you leave it)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L315>)

### /heal

恢复生命，也可触发当前死亡画面的复活；不要用它绕过专家死亡规则。

~~~text
/heal (full life; also revives: 0x1b1 from 0 runs the client's revive)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1260>)

### /hideout_keys

设置角色藏身处键值，仅调试。

~~~text
/hideout_keys <a> <b> (dev: the Player component's two Hideouts.dat keys, HASH16 hex or a hideout area id)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1408>)

### /hit

模拟受到敌方伤害，可能死亡。

~~~text
/hit <fire|cold|lightning|chaos|physical> <damage> (dev: incoming enemy test contact through resistances or armour)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1056>)

### /impossible_escape

生成指定基石版本的 Impossible Escape。

~~~text
/impossible_escape <keystone> | list (dev: Impossible Escape for that keystone, into the inventory)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L508>)

### /item

按英文名、底材 ID 或传奇名添加物品到背包。

~~~text
/item <BaseItemTypes row, Id, name or unique> [count] [magic|rare] (into the inventory, e.g. /item Small Life Flask 2, /item Iron Ring rare, /item Mageblood)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L232>)

### /item_give

按 JSON 生成自定义物品，词条列表需要完整填写。

~~~text
/item_give <item as JSON> (dev: an item the launcher's item crafter composed, with its mods, into the inventory; Agents/docs/dev/item-crafter.md)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L294>)

### /item_mod

给鼠标上物品加词条、改已有词条数值或清词条。

~~~text
/item_mod [implicit|explicit|enchant|scourge|hidden] <Mods row, Id or part of it> [values…] | /item_mod set [section] <mod> [values…] | /item_mod clear [section] (the held item; missing values are rolled)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L797>)

### /item_mods

查看鼠标上物品的词条 ID 和数值。

~~~text
/item_mods (the held item's rarity and mods)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L847>)

### /item_quality

查看或设置鼠标上物品的品质；第二个参数是品质类型（AlternateQualityTypes 行号，**6 = 属性**）。

~~~text
/item_quality [0 to 100] [quality type row 0 to 254]
~~~

例：把项链设为 20% 属性品质：

~~~text
/item_quality 20 6
~~~

### /item_state

设置腐化、镜像、分裂、势力等状态；不是完整通货效果。

~~~text
/item_state [corrupted|mirrored|split|synthesised|shaper|elder|crusader|redeemer|hunter|warlord|clear …] (switches them on the held item; without a state, shows them)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L868>)

### /item_tab

批量填充指定仓库页，会大量增加物品。

~~~text
/item_tab <stash tab name> <count> (dev: slotted tabs add up to count, at most 1000; Gem, Flask and Unique add one of each available missing variant, e.g. /item_tab Unique 1)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L372>)

### /kill

走正常死亡逻辑击杀怪物，给予经验掉落并影响任务；会跳过 Boss 阶段。

~~~text
/kill <id> | <name> [n] | boss | near [radius] | all (dev: kill monsters here through the normal death path: experience, drops, quest progress, boss stages skipped)
~~~

[实现位置](<Modules/Core/Player/Commands/Rules.Campaign.cs#L20>)

### /lastpackets

将最近协议消息写入服务日志，反馈时注意隐私。

~~~text
/lastpackets [n] (dev: the connection's last n messages, both directions, to the Gateway log; default 64)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1753>)

### /league_flag

列出、设置或清除联赛任务标志。

~~~text
/league_flag [<id> [off]] (dev: list, set or clear league quest flags by the client's id, e.g. /league_flag 350)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1395>)

### /map_portal

生成测试用地图传送门。

~~~text
/map_portal [type 1-4] [created 0|1] [open after ms, -1 never] [model 0-2] (dev: one MultiplexPortal next to you, listed in the map device's 0x8f)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L713>)

### /mob_mods

查询或替换怪物词缀，可能重建怪物。

~~~text
/mob_mods [id] [mod …|pool] (a monster's mods; with mods: create it again with those affixes, e.g. /mob_mods Quick of_Flames)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1836>)

### /mob_move

发送怪物朝玩家移动的底层测试消息。

~~~text
/mob_move [id] [speed] (0x1ba toward you; default the last monster, speed 20)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1792>)

### /mob_tp

把怪物传送到玩家旁边。

~~~text
/mob_tp [id] (0x1b7 next to you; default the last monster)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1838>)

### /mob_walk

发送怪物行走动作测试消息。

~~~text
/mob_walk [id] [skill hex] (0x1ab Move toward you)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1808>)

### /mobs

列出当前生成怪物。

~~~text
/mobs (list spawned monsters)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1761>)

### /npc

在旁边生成 NPC。

~~~text
/npc <NPCs row, Id or name> [rotation°] (spawns an NPC next to you)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1443>)

### /npc_face

旋转 NPC。

~~~text
/npc_face <name> <degrees> (turn an NPC here)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1469>)

### /npc_place

临时移动 NPC。

~~~text
/npc_place <name> <x> <y> <degrees> (dev: move and turn an NPC here)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1455>)

### /npcs

列出当前区域 NPC。

~~~text
/npcs (list NPCs here)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1492>)

### /passives

查看天赋点与已分配节点。

~~~text
/passives (passive skill points and the allocated passives)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L906>)

### /portal_rotation

旋转地图传送门。

~~~text
/portal_rotation <degrees> (dev: turn the map portals around the vertical axis; rebuilds the ones standing)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L788>)

### /pos

查看当前坐标。

~~~text
/pos (your cell)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1498>)

### /quest_flag

列出、设置或清除任务标志；可能破坏任务进度。

~~~text
/quest_flag [<QuestFlags Id> [off]] (dev: list, set or clear quest flags, e.g. /quest_flag a1q1-KilledHillock)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1307>)

### /quest_preset

加入剧情完成预设和前面章节标志，还可能解锁传送点并发任务物品；高风险。

~~~text
/quest_preset <CharacterStartQuestState Id> (dev: add that preset's quest flags and the earlier acts', e.g. /quest_preset Act1Complete)
~~~

[实现位置](<Modules/Core/Player/Commands/Rules.Campaign.cs#L35>)

### /radius_jewel

列出或生成支持的范围型传奇珠宝。

~~~text
/radius_jewel <name> | list (dev: a radius unique jewel, max-rolled, into the inventory; pilot: the six attribute-conversion jewels)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L484>)

### /rarity

修改鼠标上物品稀有度；仅设 unique 不会自动生成正确传奇。

~~~text
/rarity <normal|magic|rare|unique> [unid] (the held item)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L876>)

### /raw

发送原始协议包，可能导致断线；仅调试。

~~~text
/raw <hex> (send one S→C message)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L50>)

### /reset_ascendancy

重置升华节点，节点／效果通常消耗 5 点重置点。

~~~text
/reset_ascendancy (refund all ascendancy nodes; five refund points per node/effect, choice shells are free)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L982>)

### /reset_passives

退还所有天赋，保存；谨慎使用。

~~~text
/reset_passives (refunds every passive, saved)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1037>)

### /resistances

查看四类抗性及上限前后数值。

~~~text
/resistances (your uncapped and capped fire/cold/lightning/chaos resistance)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1043>)

### /roll

重新随机鼠标上物品的魔法／稀有词缀和名称。

~~~text
/roll <magic|rare> [item level] (new affixes and name for the held item)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L839>)

### /set_aoe

覆盖角色范围扩大数值；off 移除覆盖。

~~~text
/set_aoe <percent> | off (dev: the character's increased area of effect, base_skill_area_of_effect_+%, in place of what passives, items and /god give)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1245>)

### /set_level

设置角色等级 1–100，同时更新经验与属性并保存。

~~~text
/set_level <1 to 100> (saved to the account; the level's experience and stats are sent at once)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L894>)

### /set_speed

设置开发用移动速度参数，100 为正常基准；不建议高倍测试寻路。

~~~text
/set_speed <percent> (100 = normal)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L55>)

### /set_stat

替换角色指定统计值；off 移除该开发覆盖。

~~~text
/set_stat <stat name or client key> <value|off> (off removes the dev replacement)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L66>)

### /sockets

查询或修改鼠标上物品孔色和连线，受底材孔数容量限制。

~~~text
/sockets [colours] (the held item's sockets: R G B W, linked within a word, e.g. /sockets RRGBBB a 6-link, /sockets RRG B a 3-link and a single; without colours, shows them; Game_SOCKET_CAPACITY=1..6 lets any socketable base take that many, e.g. a 6-link wand)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L883>)

### /spawn_mob

生成指定怪物，可指定等级、阵营和稀有度。

~~~text
/spawn_mob <MonsterVarieties row, Id or name>[@level] [friendly] [normal|magic|rare|unique] [name]
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L167>)

### /spellhit

模拟法术命中，经过抗性与压制；可能死亡。

~~~text
/spellhit <fire|cold|lightning|chaos|physical> <damage> (dev: spell hit through resistance and Spell Suppression)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1159>)

### /stash_place

临时移动并旋转当前区域个人仓库。

~~~text
/stash_place <x> <y> <degrees> (dev: move and turn the stash of this area until you leave it; it opens from its new cell)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L308>)

### /stat

查询一个角色统计值。

~~~text
/stat <stat name or client key> (dev: current character stat value)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L157>)

### /state

输出角色、区域、任务等 JSON 状态；可能很长。

~~~text
/state [brief | all] (dev: one STATE {json} reply for the campaign driver: area, character, quest flags, waypoints and this area's monsters, NPCs, objects, ground items and exits; long replies go on in STATE+ lines; brief leaves out the area's things, all lists every monster)
~~~

[实现位置](<Modules/Core/Player/Commands/Rules.Campaign.cs#L13>)

### /statue

生成指定怪物的石像。

~~~text
/statue <MonsterVarieties row, Id or name> [variation] (dev: place that monster's stone statue at the player)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L204>)

### /suppression

查看法术压制情况。

~~~text
/suppression (Spell Suppression chance, roll and damage reduction)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1078>)

### /suppression_luck

强制幸运／不幸法术压制判定，off 关闭。

~~~text
/suppression_luck <lucky|unlucky|off> (dev: force a lucky/unlucky suppression roll)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1085>)

### /timeless

生成永恒珠宝，可指定种子和征服者。

~~~text
/timeless <name> [seed] [conqueror] | list (dev: a Legion timeless jewel into the inventory; Elegant Hubris seeds are the displayed x20 values 2000-160000)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L535>)

### /tour

自动遍历测试区域列表；需要专用采集启动配置。

~~~text
/tour <list file> | stop | status (dev, live.ps1 -Walkmaps: visit every "<area id> <seed>" of the file in a new instance and record it; levelgen tourlist writes one)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1382>)

### /tp

传送到坐标、怪物、NPC、出口等；可查询目标列表。

~~~text
/tp <x> <y> | <+dx> <+dy> | <id> | <name> [n] | list [filter] (dev: teleport to a cell, a monster, NPC, transition, waypoint or captured object, e.g. /tp RhoaChest 2)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1759>)

### /transition

调整区域出口的位置、角度、模型等，仅调试。

~~~text
/transition [pos [here | x y | +dx +dy] | rot <degrees> | z <z | none> | model <path | none> | index <n>] (dev: show or change the first area transition; it is re-created)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1274>)

### /trial_spike_anim

调整试炼尖刺动画时长，仅调试。

~~~text
/trial_spike_anim [<ms>] (dev: how long a Trials of Ascendancy spike trap's Emerge animation (0x1d1) is stretched over, e.g. /trial_spike_anim 2000)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1327>)

### /trial_spike_hit

调整试炼尖刺命中时间，仅调试。

~~~text
/trial_spike_hit [<from ms> <to ms>] (dev: how long after firing the Trials of Ascendancy spike traps hit, up to 3000 ms, e.g. /trial_spike_hit 1000 1250)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1314>)

### /trial_spike_step

调整试炼尖刺波间隔，仅调试。

~~~text
/trial_spike_step [<ms>] (dev: the time between two spike traps of a Trials of Ascendancy ring's wave, from its next wave, e.g. /trial_spike_step 250)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1321>)

### /ui

发送打开界面的测试消息，仅调试。

~~~text
/ui <kind hex> [a] [b] [c] [d] (S→C 0x7b open a UI)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1485>)

### /unique_jewel

列出或生成此命令支持的满 roll 传奇珠宝。

~~~text
/unique_jewel <name> | list (dev: a global unique jewel, max-rolled, into the inventory)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L461>)

### /voices

生成 Voices；参数为无效果小天赋数量。

~~~text
/voices [1|3|5|7] (dev: the Voices unique Large Cluster Jewel with that many empty passives, into the inventory)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L602>)

### /waypoints

查看、全开或重置传送点；会改变解锁状态。

~~~text
/waypoints [all | reset] (list the activated waypoints; dev: activate all and visit every pinned area, or keep only the towns')
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L1416>)

### /wp_place

查看或临时移动传送点。

~~~text
/wp_place [x y degrees] (dev: move and turn this area's waypoint; without arguments shows where it is)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L322>)

### /wp_state

测试传送点模型和激活位；仅调试。

~~~text
/wp_state active <n> | bits all|none|area|<bit> | model <.ao> (dev: recreate this area's waypoint with StateMachine "active" n or another model, or send 0x4c with all, no, this area's or one bit, to find what lights it)
~~~

[实现位置](<Modules/Core/Player/Commands/CommandsFeature.cs#L339>)

## 9. 错误提示

| 提示 | 处理 |
|---|---|
| Unknown command | 用 /com 查询，检查模块与当前运行构建 |
| No item type | 检查英文名，不拼接底材，可能目录不支持 |
| No room in the inventory | 清出背包空间 |
| Hold an item first | 先点击拿起目标装备 |
| has components the server cannot write yet | 当前服务端不支持该物品组件 |
| Usage | 参数格式或范围错误，按返回的实际语法输入 |

## 10. 来源与覆盖范围

- [核心命令源码](<Modules/Core/Player/Commands/CommandsFeature.cs>)
- [剧情命令源码](<Modules/Core/Player/Commands/Rules.Campaign.cs>)
- [帮助与参数解析](<src/GameServer.Gateway/Player/ChatCommands.cs>)
- [品质通货逻辑](<src/GameServer.Gateway/Item/CurrencyEffects.cs>)
- [自定义物品说明](<Agents/docs/dev/item-crafter.md>)

这里只统计上述核心／剧情注册表及内置帮助；其他联赛模块、Mod、客户端原生命令及全局／交易聊天切换不保证穷尽。并未实际运行全部命令，没有修改角色或服务器设置。
