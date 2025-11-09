# SuperOtto 架构文档 | Architecture Documentation

本文档详细说明SuperOtto游戏的技术架构和设计决策。

This document details the technical architecture and design decisions for the SuperOtto game.

## 📐 系统架构 | System Architecture

### 整体架构 | Overall Architecture

SuperOtto采用分层架构设计，将游戏逻辑、系统和表现层分离：

```
┌─────────────────────────────────────┐
│         Game1.cs (Main Loop)        │
│  - Initialize, Update, Draw, Input  │
└──────────────┬──────────────────────┘
               │
    ┌──────────┴──────────┐
    │                     │
┌───▼────┐          ┌─────▼─────┐
│  Core  │          │  Systems  │
└───┬────┘          └─────┬─────┘
    │                     │
    └──────────┬──────────┘
               │
         ┌─────▼──────┐
         │  Graphics  │
         │     &      │
         │    UI      │
         └────────────┘
```

## 🎮 核心模块 | Core Modules

### Core/Player.cs

**职责**: 玩家角色控制

- 处理玩家输入 (WASD/方向键)
- 计算玩家位置和速度
- 提供玩家所在瓦片位置
- 提供面向方向的瓦片位置 (用于互动)

**关键方法**:
- `Update(GameTime)`: 更新玩家位置
- `GetTilePosition()`: 获取玩家当前瓦片坐标
- `GetFacingTilePosition()`: 获取玩家面向的瓦片坐标

### Core/World.cs

**职责**: 游戏世界和瓦片系统

**核心组件**:
- `TileType` 枚举: 定义不同类型的瓦片
- `Crop` 类: 作物生长逻辑
- `World` 类: 世界状态管理

**功能**:
- 程序化世界生成
- 瓦片状态管理 (草地、泥土、耕地等)
- 作物种植和生长系统
- 互动检查 (能否耕地、浇水、种植等)

**关键方法**:
- `TillSoil(x, y)`: 耕地
- `WaterSoil(x, y)`: 浇水
- `PlantCrop(x, y, type)`: 种植作物
- `HarvestCrop(x, y)`: 收获作物
- `OnNewDay()`: 每日重置逻辑

### Core/TimeManager.cs

**职责**: 游戏时间和季节系统

**功能**:
- 时间流逝计算
- 日期和季节管理
- 昼夜循环
- 光照强度计算

**常量**:
- 1游戏分钟 = 0.05真实秒 (加速时间)
- 1游戏天 = 1440分钟 (24小时)
- 1季度 = 28天
- 4个季节: 春、夏、秋、冬

**关键方法**:
- `Update(GameTime)`: 更新时间
- `GetLightIntensity()`: 计算当前光照强度 (0.3-1.0)
- `GetFormattedTime()`: 获取格式化时间字符串

## 🔧 系统模块 | System Modules

### Systems/Camera.cs

**职责**: 相机视角控制

**功能**:
- 跟随玩家
- 视图转换矩阵
- 屏幕坐标转世界坐标

**关键方法**:
- `Follow(targetPosition)`: 相机跟随目标
- `GetTransformMatrix()`: 获取视图变换矩阵
- `ScreenToWorld(screenPosition)`: 坐标转换

### Systems/Inventory.cs

**职责**: 物品栏管理

**功能**:
- 10格快捷栏
- 物品堆叠
- 物品增删

**物品类型**:
- `Seed`: 种子
- `Crop`: 作物
- `Tool`: 工具

**关键方法**:
- `AddItem(item)`: 添加物品
- `RemoveItem(name, quantity)`: 移除物品
- `GetSelectedItem()`: 获取当前选中物品

## 🎨 图形模块 | Graphics Modules

### Graphics/TextureGenerator.cs

**职责**: 程序化纹理生成

这是游戏的核心美术系统，所有视觉资源都通过代码生成。

**设计理念**:
- 无需外部美术资源即可运行
- 提供明确的替换点
- 易于扩展和修改

**主要方法**:

#### `CreateSolidTexture(width, height, color)`
创建单色纹理，用于简单UI元素。

#### `CreateTileTexture(size, baseColor, accentColor)`
创建带随机变化的瓦片纹理，模拟自然地形。

#### `CreateBorderedRectangle(width, height, fillColor, borderColor, borderWidth)`
创建带边框的矩形，用于UI面板和按钮。

#### `CreatePlayerSprite(size)`
创建简单的玩家精灵。
TODO: 替换为完整的角色精灵表。

#### `CreateCropSprite(size, growthStage)`
根据生长阶段创建作物精灵。
- `growthStage`: 0.0 (种子) 到 1.0 (成熟)

#### `CreateItemIcon(size, itemColor)`
创建物品图标。

### 美术资源替换策略 | Asset Replacement Strategy

当需要使用真实美术资源时：

1. **保留程序化生成作为后备**: 如果资源加载失败，使用生成的纹理
2. **资源目录结构**:
   ```
   Content/
   ├── Textures/
   │   ├── Tiles/
   │   ├── Characters/
   │   ├── Crops/
   │   └── Items/
   └── ...
   ```
3. **加载优先级**: 先尝试从Content加载，失败则使用程序化生成

## 🖼️ UI模块 | UI Modules

### UI/HUD.cs

**职责**: 游戏HUD渲染

**显示内容**:
- 时间和日期面板
- 能量条和健康条
- 物品栏快捷栏
- 当前选中物品

**关键方法**:
- `Draw(spriteBatch, timeManager, inventory, ...)`: 绘制完整HUD
- `DrawTimePanel(...)`: 绘制时间信息
- `DrawInventoryHotbar(...)`: 绘制物品栏快捷栏

## 🔄 游戏循环 | Game Loop

### Game1.cs 主循环

```
Initialize()
    ↓
LoadContent()
    ↓
┌─────────────┐
│   Update()  │←─┐
└──────┬──────┘  │
       │         │
       ↓         │
┌─────────────┐  │
│    Draw()   │  │
└──────┬──────┘  │
       └─────────┘
```

#### Update() 阶段

1. **输入处理**
   - 读取键盘状态
   - 检测按键按下/释放

2. **玩家更新**
   - 处理移动输入
   - 更新玩家位置

3. **世界更新**
   - 更新所有作物生长
   - 处理瓦片状态

4. **时间更新**
   - 推进游戏时间
   - 检测新的一天

5. **相机更新**
   - 相机跟随玩家

6. **动作处理**
   - 物品栏选择
   - 工具使用
   - 作物种植/收获

#### Draw() 阶段

1. **清屏** (天空蓝色)

2. **世界渲染** (使用相机变换)
   - 计算可见瓦片范围
   - 绘制瓦片 (应用昼夜光照)
   - 绘制作物
   - 绘制玩家

3. **UI渲染** (无相机变换)
   - 绘制HUD
   - 绘制时间信息
   - 绘制物品栏

## 🎯 设计模式 | Design Patterns

### 单例模式 (隐式)
`Game1` 类作为游戏的中央控制器。

### 组件模式
各个系统 (Player, World, TimeManager等) 作为独立组件，由主游戏类组合。

### 工厂模式
`TextureGenerator` 提供静态工厂方法创建各种纹理。

### 观察者模式 (简化版)
时间管理器通过事件通知新的一天。

## 🔌 扩展性设计 | Extensibility Design

### 添加新瓦片类型

1. 在 `TileType` 枚举中添加类型
2. 在 `Game1.LoadContent()` 中添加纹理
3. 在 `World.GenerateWorld()` 中添加生成逻辑

### 添加新作物

1. 在 `CropType` 枚举中添加类型
2. 在 `Inventory` 中添加对应种子
3. 在 `Game1.HandleFarmingActions()` 中添加种植逻辑

### 添加新系统

1. 在相应目录创建新类
2. 在 `Game1.Initialize()` 中初始化
3. 在 `Game1.Update()` 中调用更新
4. 在 `Game1.Draw()` 中渲染 (如需要)

## ⚡ 性能考虑 | Performance Considerations

### 渲染优化
- **视锥剔除**: 只渲染可见的瓦片
- **纹理重用**: 所有相同类型的瓦片共享纹理
- **批处理**: 使用 SpriteBatch 进行批量绘制

### 内存管理
- 瓦片纹理在 LoadContent 时创建一次
- 作物精灵按需生成 (可优化为预生成池)

### 未来优化方向
- 对象池 (作物、粒子等)
- 区块系统 (大地图)
- 多线程世界更新
- 图集打包 (减少纹理切换)

## 🧪 测试策略 | Testing Strategy

### 单元测试目标
- Core 模块逻辑
- Systems 模块功能
- 时间计算准确性

### 集成测试
- 游戏循环完整性
- 保存/加载功能
- 跨系统交互

## 📚 依赖关系 | Dependencies

```
Game1
├── Core
│   ├── Player
│   ├── World
│   └── TimeManager
├── Systems
│   ├── Camera
│   └── Inventory
├── Graphics
│   └── TextureGenerator
└── UI
    └── HUD
```

## 🔐 数据流 | Data Flow

```
Input → Player → World ← TimeManager
                   ↓
                 Crops
                   ↓
              Inventory
                   ↓
                  HUD
```

## 📝 代码规范 | Coding Standards

- **命名约定**: PascalCase for public members, camelCase for private
- **注释**: XML文档注释用于公共API
- **TODO标记**: 标记需要替换的程序化资源
- **命名空间**: 按功能模块组织

---

**版本**: v0.1
**最后更新**: 2025-11-09

如有架构相关问题，请在GitHub Issues中讨论。

For architecture-related questions, please discuss in GitHub Issues.
