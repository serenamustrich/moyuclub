# 炸弹人游戏技术规格文档

## 1. 项目概述

本项目是使用Unity3D引擎开发的经典炸弹人游戏，玩家控制角色放置炸弹来消灭敌人并突破关卡。

### 1.1 游戏类型
- 类型：休闲益智/动作
- 平台：PC
- 视角：顶视角

### 1.2 核心玩法
- 玩家控制角色在网格地图中移动
- 放置炸弹摧毁障碍物和敌人
- 收集道具增强能力
- 击败所有敌人并找到出口通关

## 2. 技术架构

### 2.1 开发环境
- Unity版本：2022.3.0f1+
- 编程语言：C#
- 目标平台：PC (Windows/Mac)

### 2.2 项目结构
```
Assets/
├── Scenes/           # 游戏场景
├── Prefabs/          # 预制体
│   ├── Characters/   # 角色预制体
│   ├── Bombs/        # 炸弹预制体
│   ├── Items/        # 道具预制体
│   └── Obstacles/    # 障碍物预制体
├── Scripts/          # 脚本
│   ├── Characters/   # 角色相关脚本
│   ├── Bombs/        # 炸弹相关脚本
│   ├── Items/        # 道具相关脚本
│   ├── Game/         # 游戏逻辑脚本
│   └── UI/           # UI相关脚本
├── Animations/       # 动画
├── Audio/            # 音效和音乐
├── Materials/        # 材质
├── Textures/         # 纹理
└── Tilemaps/         # 瓦片地图
```

## 3. 游戏系统设计

### 3.1 角色系统

#### 3.1.1 玩家角色
- **属性**：
  - 移动速度
  - 炸弹数量上限
  - 炸弹威力
  - 移动速度
  - 生命值

- **控制**：
  - WASD/方向键：移动
  - 空格键：放置炸弹
  - ESC键：暂停游戏

- **状态**：
  - 正常状态
  - 无敌状态（刚复活时）
  - 死亡状态

#### 3.1.2 敌人角色

| 敌人类型 | 移动速度 | 行为模式 | 特殊能力 |
|---------|---------|---------|--------|
| 普通敌人 | 慢 | 随机移动 | 无 |
| 快速敌人 | 快 | 追踪玩家 | 无 |
| 飞行敌人 | 中等 | 穿过障碍物 | 可以飞过障碍物 |
| 爆炸敌人 | 慢 | 随机移动 | 死亡时产生爆炸 |

### 3.2 炸弹系统

- **属性**：
  - 爆炸范围（格子数）
  - 爆炸延迟（秒）
  - 爆炸持续时间（秒）

- **行为**：
  - 放置后倒计时
  - 倒计时结束后产生爆炸
  - 爆炸可以摧毁障碍物和敌人
  - 爆炸可以触发其他炸弹

### 3.3 道具系统

| 道具类型 | 效果 | 持续时间 |
|---------|------|---------|
| 炸弹增加 | 增加可同时放置的炸弹数量 | 永久 |
| 威力增加 | 增加炸弹爆炸范围 | 永久 |
| 速度增加 | 增加移动速度 | 永久 |
| 穿墙 | 可以穿过破坏后的障碍物 | 永久 |
| 无敌 | 临时无敌状态 | 10秒 |
| 火焰免疫 | 临时免疫火焰伤害 | 10秒 |

### 3.4 地图系统

- **地图元素**：
  - 可破坏障碍物（砖块）
  - 不可破坏障碍物（墙壁）
  - 出口（通关点）
  - 出生点

- **地图生成**：
  - 随机生成地图
  - 确保玩家和敌人有足够的移动空间
  - 出口位置随机

### 3.5 游戏逻辑

- **关卡系统**：
  - 多个关卡，难度逐渐增加
  - 每关敌人数量和类型不同

- **胜利条件**：
  - 击败所有敌人
  - 找到并进入出口

- **失败条件**：
  - 玩家生命值为0

## 4. 脚本设计

### 4.1 核心脚本

| 脚本名称 | 功能描述 | 所属目录 |
|---------|---------|---------|
| PlayerController | 玩家控制逻辑 | Scripts/Characters/ |
| EnemyController | 敌人AI逻辑 | Scripts/Characters/ |
| Bomb | 炸弹逻辑 | Scripts/Bombs/ |
| Explosion | 爆炸效果逻辑 | Scripts/Bombs/ |
| Item | 道具逻辑基类 | Scripts/Items/ |
| BombItem | 炸弹增加道具 | Scripts/Items/ |
| PowerItem | 威力增加道具 | Scripts/Items/ |
| SpeedItem | 速度增加道具 | Scripts/Items/ |
| WallPassItem | 穿墙道具 | Scripts/Items/ |
| GameManager | 游戏管理逻辑 | Scripts/Game/ |
| UIManager | UI管理逻辑 | Scripts/UI/ |

### 4.2 关键脚本实现

#### PlayerController.cs
```csharp
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int maxBombs = 1;
    public int bombPower = 1;
    public int currentBombs = 0;
    public bool canPassWalls = false;
    
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update() {
        HandleInput();
        Move();
    }
    
    void HandleInput() {
        // 处理移动输入
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;
        
        // 处理放置炸弹
        if (Input.GetKeyDown(KeyCode.Space) && currentBombs < maxBombs) {
            PlaceBomb();
        }
    }
    
    void Move() {
        rb.velocity = moveDirection * moveSpeed;
    }
    
    void PlaceBomb() {
        // 放置炸弹逻辑
    }
    
    public void CollectItem(Item item) {
        // 处理道具收集
    }
    
    public void TakeDamage() {
        // 处理受伤逻辑
    }
}
```

#### Bomb.cs
```csharp
public class Bomb : MonoBehaviour
{
    public float explosionDelay = 3f;
    public int power = 1;
    public float explosionDuration = 0.5f;
    
    private float countdown;
    private bool exploded = false;
    
    void Start() {
        countdown = explosionDelay;
    }
    
    void Update() {
        countdown -= Time.deltaTime;
        if (countdown <= 0 && !exploded) {
            Explode();
        }
    }
    
    void Explode() {
        exploded = true;
        // 生成爆炸效果
        // 检查爆炸范围内的对象
        // 触发连锁反应
        // 销毁炸弹
    }
}
```

#### GameManager.cs
```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int currentLevel = 1;
    public int playerLives = 3;
    public bool gameOver = false;
    
    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    
    void Start() {
        LoadLevel(currentLevel);
    }
    
    public void LoadLevel(int level) {
        // 加载关卡逻辑
    }
    
    public void PlayerDied() {
        playerLives--;
        if (playerLives <= 0) {
            GameOver();
        } else {
            RespawnPlayer();
        }
    }
    
    public void GameOver() {
        gameOver = true;
        // 游戏结束逻辑
    }
    
    public void LevelComplete() {
        currentLevel++;
        LoadLevel(currentLevel);
    }
}
```

## 5. 资源需求

### 5.1 美术资源

| 资源类型 | 数量 | 规格 |
|---------|------|------|
| 玩家角色精灵 | 1 | 32x32像素，4方向动画 |
| 敌人角色精灵 | 4 | 32x32像素，4方向动画 |
| 炸弹精灵 | 1 | 32x32像素，倒计时动画 |
| 爆炸效果 | 1 | 32x32像素，动画 |
| 道具精灵 | 6 | 32x32像素 |
| 障碍物精灵 | 2 | 32x32像素 |
| 背景瓦片 | 1 | 32x32像素 |

### 5.2 音频资源

| 资源类型 | 数量 | 描述 |
|---------|------|------|
| 背景音乐 | 1 | 游戏主背景音乐 |
| 放置炸弹音效 | 1 | 放置炸弹时的音效 |
| 爆炸音效 | 1 | 炸弹爆炸时的音效 |
| 收集道具音效 | 1 | 收集道具时的音效 |
| 敌人死亡音效 | 1 | 敌人死亡时的音效 |
| 玩家死亡音效 | 1 | 玩家死亡时的音效 |
| 关卡完成音效 | 1 | 关卡完成时的音效 |

## 6. 开发计划

### 6.1 开发阶段

| 阶段 | 任务 | 预计时间 |
|------|------|---------|
| 准备阶段 | 项目初始化，资源收集 | 1天 |
| 核心系统 | 玩家控制，炸弹系统 | 2天 |
| 敌人系统 | 敌人AI，行为模式 | 2天 |
| 道具系统 | 道具生成，效果实现 | 1天 |
| 地图系统 | 地图生成，碰撞检测 | 2天 |
| UI系统 | 游戏界面，菜单 | 1天 |
| 测试阶段 | 功能测试，bug修复 | 2天 |

### 6.2 技术挑战

1. **爆炸范围检测**：需要准确计算炸弹爆炸的范围和影响
2. **敌人AI**：实现不同类型敌人的智能行为
3. **地图生成**：随机生成合理的游戏地图
4. **性能优化**：确保游戏运行流畅

## 7. 游戏特色

1. **经典玩法**：还原经典炸弹人游戏的核心玩法
2. **多种敌人**：不同类型的敌人，增加游戏挑战性
3. **丰富道具**：各种道具增强玩家能力
4. **随机地图**：每次游戏都有不同的地图布局
5. **多关卡**：逐渐增加难度的关卡设计

## 8. 结论

本技术规格文档详细描述了使用Unity3D开发炸弹人游戏的方案，包括游戏系统设计、脚本实现、资源需求和开发计划。通过本方案，可以开发出一款功能完整、玩法丰富的炸弹人游戏。