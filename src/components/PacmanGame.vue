<template>
  <div class="pacman-game">
    <div class="header">
      <h1>吃豆豆</h1>
      <div class="scoreboard">
        得分: <span id="score">{{ score }}</span>
      </div>
      <div class="scoreboard">
        用时: <span id="timeUsed">{{ timeUsed }}</span>秒
      </div>
    </div>
    <div class="game-container">
      <canvas id="gameCanvas" ref="gameCanvas"></canvas>
      <div class="game-over" id="gameOver" v-if="isGameOver">
        <h2>游戏结束</h2>
        <p>最终得分: <span>{{ score }}</span></p>
        <p>用时: <span>{{ timeUsed }}秒</span></p>
        <button class="btn" @click="resetGame">重新开始</button>
        <button class="btn" @click="$router.push('/leaderboard')">查看排行榜</button>
      </div>
    </div>
    <div class="controls">
      <p>使用方向键或WASD控制移动</p>
      <button class="btn" @click="$router.push('/')">返回首页</button>
      <button class="btn" id="startPauseBtn" @click="toggleGame">{{ gameButtonText }}</button>
      <button class="btn" @click="resetGame">重置</button>
      <button class="btn" @click="$router.push('/leaderboard')">排行榜</button>
    </div>
    
    <!-- 移动端控制按钮 -->
    <div class="mobile-controls">
      <div class="control-pad">
        <button class="control-btn up-btn" @touchstart="handleMobileDirection('up')" @touchend="handleMobileDirectionUp">↑</button>
        <div class="control-row">
          <button class="control-btn left-btn" @touchstart="handleMobileDirection('left')" @touchend="handleMobileDirectionUp">←</button>
          <button class="control-btn down-btn" @touchstart="handleMobileDirection('down')" @touchend="handleMobileDirectionUp">↓</button>
          <button class="control-btn right-btn" @touchstart="handleMobileDirection('right')" @touchend="handleMobileDirectionUp">→</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'PacmanGame',
  data() {
    return {
      // 游戏常量
      GRID_SIZE: 20,
      CELL_SIZE: 20,
      GAME_SPEED: 130,
      SAFE_TIME: 3000,
      LONG_PRESS_DELAY: 100,
      
      // 游戏计时
      gameStartTime: 0,
      timeUsed: 0,
      timeInterval: null,
      
      // 用户信息
      currentUser: null,
      
      // 游戏状态
      canvas: null,
      ctx: null,
      gameGrid: [],
      pacman: {
        x: 1, y: 18, direction: 'up', nextDirection: 'up',
        width: 16, height: 16, angle: 0
      },
      ghosts: [],
      score: 0,
      gameInterval: null,
      isGameRunning: false,
      isPaused: false,
      isGameOver: false,
      
      // 键盘状态和长按相关变量
      keyState: {},
      longPressTimer: null,
      longPressInterval: null,
      mobileLongPressTimer: null,
      mobileLongPressInterval: null,
      touchState: {
        up: false,
        left: false,
        down: false,
        right: false
      },
      
      // 游戏地图
      map: [
        '####################',
        '#........#........#',
        '#o##.###.#.###.##o#',
        '#.................#',
        '#.##.#.#####.#.##.#',
        '#....#...#...#....#',
        '####.### # ###.####',
        '   #.#       #.#   ',
        '####.# ##=## #.####',
        '     .  # #  .     ',
        '####.# ##### #.####',
        '   #.#       #.#   ',
        '####.#.#####.#.####',
        '#........#........#',
        '#.##.###.#.###.##.#',
        '#o..#........#..o#',
        '##.#.#.#####.#.#.##',
        '#.................#',
        '#.##.###.#.###.##.#',
        '####################'
      ]
    }
  },
  computed: {
    gameButtonText() {
      if (!this.isGameRunning) {
        return '开始游戏'
      } else if (this.isPaused) {
        return '继续游戏'
      } else {
        return '暂停游戏'
      }
    }
  },
  mounted() {
    this.canvas = this.$refs.gameCanvas
    this.ctx = this.canvas.getContext('2d')
    this.setCanvasSize()
    this.initGame()
    this.checkLoginStatus()
    this.addEventListeners()
  },
  beforeUnmount() {
    this.clearIntervals()
    this.removeEventListeners()
  },
  methods: {
    setCanvasSize() {
      const gameContainer = this.canvas.parentElement
      this.canvas.width = gameContainer.clientWidth
      this.canvas.height = gameContainer.clientHeight
    },
    initGame() {
      this.createGameGrid()
      this.createGhosts()
      this.drawGame()
    },
    createGameGrid() {
      this.gameGrid = []
      for (let y = 0; y < this.GRID_SIZE; y++) {
        this.gameGrid[y] = []
        for (let x = 0; x < this.GRID_SIZE; x++) {
          const cellType = this.map[y][x]
          this.gameGrid[y][x] = {
            type: cellType,
            visited: false
          }
        }
      }
    },
    createGhosts() {
      this.ghosts = [
        { x: 8, y: 6, direction: 'left', color: '#ff6666', isAfraid: false, afraidTimer: 0 },
        { x: 10, y: 6, direction: 'right', color: '#66ccff', isAfraid: false, afraidTimer: 0 },
        { x: 12, y: 6, direction: 'left', color: '#ffcc66', isAfraid: false, afraidTimer: 0 },
        { x: 10, y: 4, direction: 'down', color: '#cc66ff', isAfraid: false, afraidTimer: 0 }
      ]
    },
    drawGame() {
      // 清空画布
      this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height)
      
      // 计算实际单元格大小
      const actualCellSize = Math.min(this.canvas.width, this.canvas.height) / this.GRID_SIZE
      
      // 绘制网格
      for (let y = 0; y < this.GRID_SIZE; y++) {
        for (let x = 0; x < this.GRID_SIZE; x++) {
          const cell = this.gameGrid[y][x]
          
          // 绘制墙壁
          if (cell.type === '#' || cell.type === '=') {
            this.ctx.fillStyle = '#0033cc'
            this.ctx.fillRect(x * actualCellSize, y * actualCellSize, actualCellSize, actualCellSize)
            this.ctx.strokeStyle = '#0066ff'
            this.ctx.strokeRect(x * actualCellSize, y * actualCellSize, actualCellSize, actualCellSize)
          }
          // 绘制食物
          else if (cell.type === '.') {
            this.ctx.fillStyle = '#fff'
            this.ctx.beginPath()
            this.ctx.arc(x * actualCellSize + actualCellSize / 2, y * actualCellSize + actualCellSize / 2, actualCellSize * 0.1, 0, Math.PI * 2)
            this.ctx.fill()
          }
          // 绘制能量食物
          else if (cell.type === 'o') {
            this.ctx.fillStyle = '#ffcc00'
            this.ctx.beginPath()
            this.ctx.arc(x * actualCellSize + actualCellSize / 2, y * actualCellSize + actualCellSize / 2, actualCellSize * 0.2, 0, Math.PI * 2)
            this.ctx.fill()
            // 添加发光效果
            this.ctx.shadowColor = '#ffcc00'
            this.ctx.shadowBlur = 10
            this.ctx.beginPath()
            this.ctx.arc(x * actualCellSize + actualCellSize / 2, y * actualCellSize + actualCellSize / 2, actualCellSize * 0.2, 0, Math.PI * 2)
            this.ctx.fill()
            this.ctx.shadowBlur = 0
          }
        }
      }
      
      // 绘制吃豆豆
      this.drawPacman(actualCellSize)
      
      // 绘制幽灵
      this.drawGhosts(actualCellSize)
    },
    drawPacman(actualCellSize) {
      const pacmanSize = actualCellSize * 0.8
      const x = this.pacman.x * actualCellSize + (actualCellSize - pacmanSize) / 2
      const y = this.pacman.y * actualCellSize + (actualCellSize - pacmanSize) / 2
      
      // 更新角度
      this.pacman.angle = (this.pacman.angle + 0.1) % (Math.PI * 2)
      const openAngle = Math.sin(this.pacman.angle) * 0.3 + 0.4
      
      this.ctx.save()
      this.ctx.translate(x + pacmanSize / 2, y + pacmanSize / 2)
      
      // 根据方向旋转
      switch (this.pacman.direction) {
        case 'up':
          this.ctx.rotate(-Math.PI / 2)
          break
        case 'down':
          this.ctx.rotate(Math.PI / 2)
          break
        case 'left':
          this.ctx.rotate(Math.PI)
          break
        case 'right':
          this.ctx.rotate(0)
          break
      }
      
      // 绘制吃豆豆
      this.ctx.fillStyle = '#ffcc00'
      this.ctx.beginPath()
      this.ctx.moveTo(0, 0)
      this.ctx.arc(0, 0, pacmanSize / 2, openAngle, Math.PI * 2 - openAngle)
      this.ctx.closePath()
      this.ctx.fill()
      
      // 绘制眼睛
      this.ctx.fillStyle = '#000'
      this.ctx.beginPath()
      this.ctx.arc(pacmanSize * 0.25, -pacmanSize * 0.25, pacmanSize * 0.1, 0, Math.PI * 2)
      this.ctx.fill()
      
      this.ctx.restore()
    },
    drawGhosts(actualCellSize) {
      const ghostSize = actualCellSize * 0.8
      this.ghosts.forEach(ghost => {
        const x = ghost.x * actualCellSize + (actualCellSize - ghostSize) / 2
        const y = ghost.y * actualCellSize + (actualCellSize - ghostSize) / 2
        
        this.ctx.save()
        
        // 绘制幽灵身体
        this.ctx.fillStyle = ghost.isAfraid ? '#6666ff' : ghost.color
        this.ctx.beginPath()
        this.ctx.moveTo(x, y)
        this.ctx.lineTo(x + ghostSize, y)
        this.ctx.quadraticCurveTo(x + ghostSize, y + ghostSize, x + ghostSize * 0.75, y + ghostSize * 0.75)
        this.ctx.lineTo(x + ghostSize * 0.5, y + ghostSize)
        this.ctx.lineTo(x + ghostSize * 0.25, y + ghostSize * 0.75)
        this.ctx.lineTo(x, y + ghostSize)
        this.ctx.closePath()
        this.ctx.fill()
        
        // 绘制眼睛
        this.ctx.fillStyle = '#fff'
        this.ctx.beginPath()
        this.ctx.arc(x + ghostSize * 0.3125, y + ghostSize * 0.375, ghostSize * 0.1875, 0, Math.PI * 2)
        this.ctx.arc(x + ghostSize * 0.6875, y + ghostSize * 0.375, ghostSize * 0.1875, 0, Math.PI * 2)
        this.ctx.fill()
        
        this.ctx.fillStyle = '#000'
        this.ctx.beginPath()
        this.ctx.arc(x + ghostSize * 0.3125, y + ghostSize * 0.375, ghostSize * 0.0625, 0, Math.PI * 2)
        this.ctx.arc(x + ghostSize * 0.6875, y + ghostSize * 0.375, ghostSize * 0.0625, 0, Math.PI * 2)
        this.ctx.fill()
        
        this.ctx.restore()
      })
    },
    isValidMove(x, y) {
      if (x < 0 || x >= this.GRID_SIZE || y < 0 || y >= this.GRID_SIZE) {
        return false
      }
      const cell = this.gameGrid[y][x]
      return cell.type !== '#' && cell.type !== '='
    },
    getNextPosition(entity, direction) {
      const nextPos = { ...entity }
      switch (direction) {
        case 'up':
          nextPos.y--
          break
        case 'down':
          nextPos.y++
          break
        case 'left':
          nextPos.x--
          break
        case 'right':
          nextPos.x++
          break
      }
      return nextPos
    },
    movePacman() {
      // 尝试使用下一个方向
      if (this.isValidMove(this.getNextPosition(this.pacman, this.pacman.nextDirection).x, this.getNextPosition(this.pacman, this.pacman.nextDirection).y)) {
        this.pacman.direction = this.pacman.nextDirection
      }
      
      const nextPos = this.getNextPosition(this.pacman, this.pacman.direction)
      if (this.isValidMove(nextPos.x, nextPos.y)) {
        // 检查是否吃到食物
        const cell = this.gameGrid[nextPos.y][nextPos.x]
        if (cell.type === '.') {
          this.score += 1
          cell.type = ' '
        } else if (cell.type === 'o') {
          this.score += 5
          cell.type = ' '
          // 使幽灵害怕
          this.ghosts.forEach(ghost => {
            ghost.isAfraid = true
            ghost.afraidTimer = 100
          })
        }
        
        // 更新位置
        this.pacman.x = nextPos.x
        this.pacman.y = nextPos.y
        
        // 检查与幽灵的碰撞
        this.checkGhostCollision()
        
        // 检查游戏是否结束
        this.checkGameOver()
      }
    },
    moveGhosts() {
      for (let i = 0; i < this.ghosts.length; i++) {
        const ghost = this.ghosts[i]
        
        // 减少害怕时间
        if (ghost.isAfraid) {
          ghost.afraidTimer--
          if (ghost.afraidTimer <= 0) {
            ghost.isAfraid = false
          }
        }
        
        // 随机改变方向
        if (Math.random() < 0.2) {
          const directions = ['up', 'down', 'left', 'right']
          const validDirections = []
          for (let j = 0; j < directions.length; j++) {
            const dir = directions[j]
            const nextPos = this.getNextPosition(ghost, dir)
            if (this.isValidMove(nextPos.x, nextPos.y)) {
              validDirections.push(dir)
            }
          }
          if (validDirections.length > 0) {
            const randomIndex = Math.floor(Math.random() * validDirections.length)
            ghost.direction = validDirections[randomIndex]
          }
        }
        
        // 移动
        const nextPos = this.getNextPosition(ghost, ghost.direction)
        if (this.isValidMove(nextPos.x, nextPos.y)) {
          ghost.x = nextPos.x
          ghost.y = nextPos.y
        }
      }
      
      // 检查与幽灵的碰撞（当幽灵移动时也会检测）
      this.checkGhostCollision()
    },
    checkGhostCollision() {
      for (let i = 0; i < this.ghosts.length; i++) {
        const ghost = this.ghosts[i]
        if (ghost.x === this.pacman.x && ghost.y === this.pacman.y) {
          if (ghost.isAfraid) {
            this.score += 5
            // 重置幽灵位置
            ghost.x = 10
            ghost.y = 6
            ghost.isAfraid = false
          } else {
            this.gameOver()
          }
        }
      }
    },
    checkGameOver() {
      // 检查是否还有食物
      let hasFood = false
      for (let y = 0; y < this.GRID_SIZE; y++) {
        for (let x = 0; x < this.GRID_SIZE; x++) {
          if (this.gameGrid[y][x].type === '.' || this.gameGrid[y][x].type === 'o') {
            hasFood = true
            break
          }
        }
        if (hasFood) break
      }
      
      if (!hasFood) {
        this.gameOver(true)
      }
    },
    gameOver(isVictory = false) {
      this.clearIntervals()
      this.isGameRunning = false
      this.isGameOver = true
      this.submitScore()
    },
    startGame() {
      if (!this.isGameRunning) {
        this.isGameRunning = true
        this.isPaused = false
        this.isGameOver = false
        this.gameStartTime = Date.now()
        this.timeUsed = 0
        
        // 启动计时器
        this.timeInterval = setInterval(() => {
          this.timeUsed = Math.floor((Date.now() - this.gameStartTime) / 1000)
        }, 1000)
        
        this.gameInterval = setInterval(() => {
          if (!this.isPaused) {
            // 只移动幽灵，不自动移动吃豆豆（电脑端）
            if (Date.now() - this.gameStartTime > this.SAFE_TIME) {
              this.moveGhosts()
            }
            
            // 移动端自动前进逻辑
            if (this.isMobileDevice()) {
              this.movePacman()
            }
            
            this.drawGame()
          }
        }, this.GAME_SPEED)
      }
    },
    pauseGame() {
      this.isPaused = !this.isPaused
    },
    toggleGame() {
      if (!this.isGameRunning) {
        this.startGame()
      } else {
        this.pauseGame()
      }
    },
    resetGame() {
      this.clearIntervals()
      this.score = 0
      this.timeUsed = 0
      this.pacman = { x: 1, y: 18, direction: 'up', nextDirection: 'up', width: 16, height: 16, angle: 0 }
      this.isGameRunning = false
      this.isPaused = false
      this.isGameOver = false
      
      // 重新初始化游戏
      this.initGame()
    },
    clearIntervals() {
      clearInterval(this.gameInterval)
      clearInterval(this.timeInterval)
      clearTimeout(this.longPressTimer)
      clearInterval(this.longPressInterval)
      clearTimeout(this.mobileLongPressTimer)
      clearInterval(this.mobileLongPressInterval)
    },
    isMobileDevice() {
      return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini|Tablet|Mobile/i.test(navigator.userAgent) || 
             (navigator.maxTouchPoints > 0 && window.innerWidth < 1024)
    },
    handleDirectionKey(key) {
      // 确定方向
      let direction
      switch (key) {
        case 'ArrowUp':
        case 'w':
        case 'W':
          direction = 'up'
          break
        case 'ArrowDown':
        case 's':
        case 'S':
          direction = 'down'
          break
        case 'ArrowLeft':
        case 'a':
        case 'A':
          direction = 'left'
          break
        case 'ArrowRight':
        case 'd':
        case 'D':
          direction = 'right'
          break
        default:
          return
      }
      
      // 更新方向
      this.pacman.nextDirection = direction
      
      // 立即移动一次
      if (this.isGameRunning && !this.isPaused) {
        this.movePacman()
        this.drawGame()
      }
      
      // 清除之前的定时器
      clearTimeout(this.longPressTimer)
      clearInterval(this.longPressInterval)
      
      // 立即开始持续移动，无延迟
      if (this.isGameRunning && !this.isPaused) {
        this.longPressInterval = setInterval(() => {
          this.movePacman()
          this.drawGame()
        }, this.GAME_SPEED)
      }
    },
    handleDirectionKeyUp() {
      // 清除定时器，停止持续移动
      clearTimeout(this.longPressTimer)
      clearInterval(this.longPressInterval)
    },
    handleMobileDirection(dir) {
      // 更新方向
      this.pacman.nextDirection = dir
      
      // 立即移动一次
      if (this.isGameRunning && !this.isPaused) {
        this.movePacman()
        this.drawGame()
      }
      
      // 清除之前的定时器
      clearTimeout(this.mobileLongPressTimer)
      clearInterval(this.mobileLongPressInterval)
      
      // 启动长按定时器
      this.mobileLongPressTimer = setTimeout(() => {
        // 长按触发，开始持续移动
        if (this.isGameRunning && !this.isPaused) {
          this.mobileLongPressInterval = setInterval(() => {
            this.movePacman()
            this.drawGame()
          }, this.GAME_SPEED)
        }
      }, this.LONG_PRESS_DELAY)
    },
    handleMobileDirectionUp() {
      // 清除定时器，停止持续移动
      clearTimeout(this.mobileLongPressTimer)
      clearInterval(this.mobileLongPressInterval)
    },
    handleKeyDown(e) {
      // 检查是否已经按下该键，避免重复触发
      if (this.keyState[e.key]) return
      this.keyState[e.key] = true
      
      switch (e.key) {
        case 'ArrowUp':
        case 'w':
        case 'W':
        case 'ArrowDown':
        case 's':
        case 'S':
        case 'ArrowLeft':
        case 'a':
        case 'A':
        case 'ArrowRight':
        case 'd':
        case 'D':
          this.handleDirectionKey(e.key)
          break
        case ' ': // 空格键暂停/继续
          if (this.isGameRunning) {
            this.pauseGame()
          } else {
            this.startGame()
          }
          break
      }
    },
    handleKeyUp(e) {
      this.keyState[e.key] = false
      
      // 检查是否还有方向键被按下
      const hasDirectionKeyPressed = Object.keys(this.keyState).some(key => {
        return ['ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight', 'w', 'W', 's', 'S', 'a', 'A', 'd', 'D'].includes(key) && this.keyState[key]
      })
      
      if (!hasDirectionKeyPressed) {
        this.handleDirectionKeyUp()
      }
    },
    addEventListeners() {
      window.addEventListener('keydown', this.handleKeyDown.bind(this))
      window.addEventListener('keyup', this.handleKeyUp.bind(this))
      window.addEventListener('resize', this.setCanvasSize.bind(this))
    },
    removeEventListeners() {
      window.removeEventListener('keydown', this.handleKeyDown.bind(this))
      window.removeEventListener('keyup', this.handleKeyUp.bind(this))
      window.removeEventListener('resize', this.setCanvasSize.bind(this))
    },
    checkLoginStatus() {
      const user = localStorage.getItem('pacman_user')
      if (user) {
        this.currentUser = JSON.parse(user)
      }
    },
    async submitScore() {
      if (this.currentUser) {
        try {
          const deviceType = this.isMobileDevice() ? 'mobile' : 'desktop'
          const response = await fetch('/api/scores', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json'
            },
            body: JSON.stringify({
              user_id: this.currentUser.id,
              score: this.score,
              time_used: this.timeUsed,
              device_type: deviceType
            })
          })
          const data = await response.json()
          if (data.rank) {
            console.log('分数提交成功，排名:', data.rank)
          }
        } catch (error) {
          console.error('提交分数失败:', error)
        }
      }
    }
  }
}
</script>

<style scoped>
.pacman-game {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 20px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  max-width: 600px;
  margin-bottom: 10px;
  height: 50px;
}

h1 {
  color: #ffcc00;
  text-shadow: 0 0 20px rgba(255, 204, 0, 0.5);
  font-size: 28px;
  margin: 0;
  line-height: 50px;
}

.scoreboard {
  font-size: 18px;
  color: #ffcc00;
  background: rgba(0, 0, 0, 0.7);
  padding: 5px 10px;
  border-radius: 5px;
  border: 1px solid #ffcc00;
  display: inline-block;
  line-height: 30px;
  margin: 0 !important;
}

.game-container {
  position: relative;
  width: 80vmin;
  height: 80vmin;
  max-width: 600px;
  max-height: 600px;
  border: 2px solid #ffcc00;
  background: rgba(0, 0, 0, 0.8);
  border-radius: 10px;
  box-shadow: 0 0 30px rgba(255, 204, 0, 0.3);
  overflow: hidden;
}

canvas {
  display: block;
  background: #000;
  width: 100%;
  height: 100%;
}

.controls {
  margin-top: 10px;
  text-align: center;
}

.game-over {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  background: linear-gradient(145deg, rgba(0, 0, 0, 0.95), rgba(30, 30, 30, 0.95));
  padding: 30px;
  border-radius: 20px;
  text-align: center;
  z-index: 100;
  border: 3px solid #ffcc00;
  box-shadow: 0 0 50px rgba(255, 204, 0, 0.5), inset 0 0 20px rgba(255, 204, 0, 0.2);
  min-width: 280px;
}

.game-over h2 {
  color: #ffcc00;
  margin-bottom: 15px;
  font-size: 24px;
  text-shadow: 0 0 10px rgba(255, 204, 0, 0.8);
}

.game-over p {
  margin-bottom: 15px;
  font-size: 16px;
  color: #fff;
  text-shadow: 0 0 5px rgba(255, 255, 255, 0.5);
}

.game-over p span {
  color: #ffcc00;
  font-weight: bold;
  font-size: 18px;
  text-shadow: 0 0 10px rgba(255, 204, 0, 0.8);
}

.mobile-controls {
  position: fixed;
  bottom: 60px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 1000;
  display: none;
}

.control-pad {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 15px;
  padding: 0;
  background: transparent;
  border-radius: 0;
  box-shadow: none;
  border: none;
}

.control-row {
  display: flex;
  gap: 20px;
}

.control-btn {
  width: 15vmin;
  height: 15vmin;
  max-width: 100px;
  max-height: 100px;
  min-width: 60px;
  min-height: 60px;
  border: 3px solid #333;
  border-radius: 10px;
  background: linear-gradient(145deg, #ffcc00, #ffdd33);
  color: #333;
  font-size: 5vmin;
  font-weight: bold;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.1s ease;
  -webkit-tap-highlight-color: transparent;
  user-select: none;
  touch-action: manipulation;
  box-shadow: 4px 4px 0 #000;
  text-shadow: 1px 1px 0 rgba(255, 255, 255, 0.5);
}

.control-btn:active {
  background: linear-gradient(145deg, #ffdd33, #ffcc00);
  transform: translate(2px, 2px);
  box-shadow: 2px 2px 0 #000;
}

@media (max-width: 480px) {
  body {
    padding: 10px;
    min-height: 100vh;
    display: flex;
    flex-direction: column;
    align-items: center;
  }
  
  .header {
    max-width: 100%;
    margin-bottom: 10px;
  }
  
  h1 {
    font-size: 24px;
  }
  
  .scoreboard {
    font-size: 14px;
  }
  
  .game-container {
    width: calc(100vw - 4px);
    height: calc(100vw - 4px);
    max-width: 600px;
    max-height: 600px;
    margin-bottom: 15px;
  }
  
  canvas {
    width: 100%;
    height: 100%;
  }
  
  .controls {
    margin-top: 10px;
    margin-bottom: 20px;
    width: 100%;
    text-align: center;
  }
  
  .controls p {
    display: none;
  }
  
  .btn {
    padding: 8px 16px;
    font-size: 14px;
    margin: 3px;
  }
  
  .mobile-controls {
    display: block;
    bottom: 20px;
  }
  
  .control-btn {
    width: 15vmin;
    height: 15vmin;
    max-width: 80px;
    max-height: 80px;
    min-width: 50px;
    min-height: 50px;
    font-size: 5vmin;
  }
}
</style>