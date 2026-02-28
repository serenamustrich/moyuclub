# Unity2D坦克大战游戏

这是一个使用Unity2D引擎开发的FC风格坦克大战游戏。

## 项目结构

```
UnityTankGame/
├── Assets/
│   ├── Scenes/          # 场景文件
│   ├── Scripts/          # 脚本文件
│   ├── Sprites/          # 精灵资源
│   ├── Audio/            # 音频资源
│   └── Prefabs/          # 预制体
└── README.md            # 项目说明
```

## 核心脚本

- `TankController.cs` - 玩家坦克控制脚本
- `EnemyController.cs` - 敌人坦克AI脚本
- `Bullet.cs` - 子弹逻辑脚本
- `GameManager.cs` - 游戏管理脚本
- `AudioManager.cs` - 音频管理脚本

## 如何设置

1. 使用Unity Hub打开项目
2. 确保安装了Unity 2022.3 LTS或更高版本
3. 导入必要的资源：
   - 坦克精灵
   - 地形精灵
   - 音效文件

## 如何运行

1. 打开 `Assets/Scenes/MainScene.unity` 场景
2. 点击播放按钮开始游戏
3. 使用方向键控制坦克移动，空格键开火

## 游戏功能

- 玩家坦克控制
- 多种敌人类型（普通、快速、智能、重型）
- 地形碰撞检测
- 得分系统
- 游戏结束和重新开始
- 音效系统

## 如何集成到前端项目

1. 在Unity中构建WebGL版本
2. 将构建后的文件复制到前端项目的静态资源目录
3. 在前端页面中嵌入Unity WebGL播放器

## 技术特点

- 使用Unity2D的GameObject和Component系统
- 利用Unity的物理引擎进行碰撞检测
- 实现了状态管理和游戏逻辑
- 支持多种敌人AI行为
- 集成了音效系统

## 注意事项

- 需要添加精灵和音效资源
- 需要在Unity编辑器中设置标签和碰撞层
- 需要配置游戏管理器的引用
