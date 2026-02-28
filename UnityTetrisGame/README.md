# Unity 2D 俄罗斯方块游戏

## 项目结构

```
UnityTetrisGame/
├── Assets/
│   ├── Scenes/          # 场景文件
│   ├── Scripts/         # 脚本文件
│   │   ├── GameManager.cs           # 游戏管理器
│   │   ├── TetrisGrid.cs            # 网格管理
│   │   ├── TetrisPiece.cs           # 方块控制
│   │   └── MobileInputController.cs # 移动端输入控制
└── README.md            # 项目说明
```

## 功能特性

- 标准俄罗斯方块游戏玩法
- 支持PC端键盘控制（方向键或WASD）
- 支持移动端触摸控制
- 游戏状态管理（开始/暂停/继续/重新开始）
- 得分系统和等级提升
- 完整的方块旋转和碰撞检测
- 自动下落和硬降功能

## 如何设置游戏

1. **打开项目**：在Unity编辑器中打开 `UnityTetrisGame` 项目

2. **创建场景**：
   - 在 `Assets/Scenes` 目录下创建一个新场景，命名为 `MainScene`
   - 将场景保存到 `Assets/Scenes/MainScene.unity`

3. **设置游戏对象**：
   
   a. **创建网格管理器**：
   - 创建一个空游戏对象，命名为 `TetrisGrid`
   - 附加 `TetrisGrid.cs` 脚本
   - 设置网格尺寸（默认：宽12，高20）

   b. **创建方块控制器**：
   - 创建一个空游戏对象，命名为 `TetrisPiece`
   - 附加 `TetrisPiece.cs` 脚本

   c. **创建游戏管理器**：
   - 创建一个空游戏对象，命名为 `GameManager`
   - 附加 `GameManager.cs` 脚本

   d. **创建移动端输入控制器**：
   - 创建一个空游戏对象，命名为 `MobileInputController`
   - 附加 `MobileInputController.cs` 脚本

4. **设置UI**：
   - 创建一个 Canvas 游戏对象
   - 添加以下UI元素：
     - Text 组件：ScoreText（显示得分）
     - Text 组件：LevelText（显示等级）
     - Text 组件：LinesText（显示消除行数）
     - Text 组件：FinalScoreText（游戏结束时显示最终得分）
     - Button 组件：StartButton（开始/暂停/继续按钮）
     - Text 组件：StartButtonText（按钮文本）
     - Panel 组件：GameOverPanel（游戏结束面板）

   - 为移动端添加触摸控制按钮：
     - Button 组件：LeftButton（左移）
     - Button 组件：RightButton（右移）
     - Button 组件：DownButton（下移）
     - Button 组件：RotateButton（旋转）
     - Button 组件：HardDropButton（硬降）

5. **设置相机**：
   - 设置主相机的正交模式
   - 调整相机位置和大小，确保游戏区域完整显示

6. **构建游戏**：
   - 选择 `File > Build Settings`
   - 添加 `MainScene` 到构建设置
   - 选择目标平台（PC、WebGL或移动设备）
   - 点击 `Build` 按钮生成游戏

## 游戏控制

### PC端控制
- **左箭头** / **A**：向左移动
- **右箭头** / **D**：向右移动
- **下箭头** / **S**：向下移动（加速下落）
- **上箭头** / **W**：旋转方块
- **空格键**：硬降（直接落到底部）

### 移动端控制
- **左按钮**：向左移动
- **右按钮**：向右移动
- **下按钮**：向下移动
- **旋转按钮**：旋转方块
- **硬降按钮**：直接落到底部

## 游戏规则

- 每次消除一行得100分
- 每次消除两行得300分
- 每次消除三行得500分
- 每次消除四行得800分
- 每消除10行升级一次，等级提升后下落速度加快
- 当方块堆叠到顶部时游戏结束

## 注意事项

- 确保所有脚本都正确附加到相应的游戏对象上
- 确保UI元素的名称与脚本中查找的名称一致
- 在移动设备上测试时，确保触摸按钮的大小和位置适合手指操作
- 可以根据需要调整网格尺寸、方块大小和游戏速度
