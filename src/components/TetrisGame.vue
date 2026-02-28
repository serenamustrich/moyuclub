<template>
  <div class="tetris-game" @contextmenu.prevent>
    <div class="container">
      <audio ref="backgroundMusic" loop preload="auto">
        <source src="https://assets.codepen.io/21542/Tetris.mp3" type="audio/mpeg">
      </audio>
      <div class="title-container">
        <button class="btn home-btn" @click="$router.push('/')">返回首页</button>
        <h1>俄罗斯方块</h1>
        <div class="user-info" v-if="currentUser">
          <div class="relative">
            <span class="username" @click="toggleUserMenu" :title="currentUser.username">
              {{ truncateUsername(currentUser.username, 8) }}
            </span>
            <div class="user-menu" v-if="showUserMenu">
              <button @click="logout">退出登录</button>
              <button @click="$router.push('/login?action=change')">修改密码</button>
              <button @click="changeUsername">修改用户名</button>
            </div>
          </div>
        </div>
      </div>
      
      <div class="game-container">
        <div class="game-info">
          <div class="info-item">
            <label>分数</label>
            <div class="score">{{ score }}</div>
          </div>
          <div class="info-item">
            <button class="start-btn" @click="toggleGame">{{ gameButtonText }}</button>
          </div>
          <div class="info-item">
            <label>等级</label>
            <div class="level">{{ level }}</div>
          </div>
        </div>
        
        <div class="game-area">
          <canvas ref="gameCanvas" id="gameCanvas" @click="rotatePiece" @touchstart="handleCanvasTouchStart" @wheel.prevent></canvas>
          <div class="game-over" v-if="isGameOver">
            <h2>游戏结束</h2>
            <div class="game-over-stats">
              <div class="stat-item">
                <span class="stat-label">最终得分</span>
                <span class="stat-value">{{ score }}</span>
              </div>
              <div class="stat-item">
                <span class="stat-label">等级</span>
                <span class="stat-value">{{ level }}</span>
              </div>
              <div class="stat-item" v-if="rank">
                <span class="stat-label">本次排名</span>
                <span class="stat-value rank">{{ rank }}</span>
              </div>
            </div>
            <div class="game-over-buttons">
              <button class="restart-btn" @click="restartGame">重新开始</button>
              <button class="leaderboard-btn" @click="$router.push('/tetris-leaderboard')">查看排行榜</button>
            </div>
          </div>
          
          <!-- 方向键容器 -->
          <div class="controls-container">
            <button id="leftBtn" class="control-btn" @mousedown="handleLeftDown" @mouseup="handleLeftUp" @mouseleave="handleLeftUp" @touchstart="handleLeftDown" @touchend="handleLeftUp">
              ←
            </button>
            <button id="downBtn" class="control-btn" @click="hardDrop">
              ↓
            </button>
            <button id="rightBtn" class="control-btn" @mousedown="handleRightDown" @mouseup="handleRightUp" @mouseleave="handleRightUp" @touchstart="handleRightDown" @touchend="handleRightUp">
              →
            </button>
          </div>
        </div>
      </div>
      
      <div class="footer">
        <button class="btn btn-secondary" v-if="!currentUser" @click="$router.push('/login')">登录</button>
        <button class="btn btn-secondary" v-if="!currentUser" @click="$router.push('/register')">注册</button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'TetrisGame',
  data() {
    return {
      // 游戏状态
      currentUser: null,
      showUserMenu: false,
      isMobile: false,
      gameStarted: false,
      gamePaused: false,
      isGameOver: false,
      score: 0,
      level: 1,
      lines: 0,
      rank: null,
      
      // 音频相关
      backgroundMusic: null,
      
      // 游戏变量
      canvas: null,
      ctx: null,
      gameWidth: 0,
      gameHeight: 0,
      blockSize: 0,
      grid: [],
      currentPiece: null,
      dropInterval: 1000,
      lastDropTime: 0,
      animationId: null,
      
      // 长按定时器
      leftIntervalId: null,
      rightIntervalId: null,
      downIntervalId: null,
      LONG_PRESS_DELAY: 1000,
      LONG_PRESS_INTERVAL: 100,
      
      // 触摸标志
      isTouching: {
        left: false,
        right: false,
        down: false
      },
      
      // 点击时间戳
      lastClickTime: {
        left: 0,
        right: 0,
        down: 0
      },
      CLICK_DELAY: 300
    }
  },
  computed: {
    gameButtonText() {
      if (!this.gameStarted) {
        return '开始游戏'
      } else if (this.gamePaused) {
        return '继续游戏'
      } else {
        return '暂停游戏'
      }
    }
  },
  mounted() {
    this.checkLoginStatus()
    this.checkMobile()
    this.initializeGame()
    this.addEventListeners()
    // 初始化音频元素
    this.backgroundMusic = this.$refs.backgroundMusic
  },
  beforeUnmount() {
    this.removeEventListeners()
    this.clearIntervals()
  },
  methods: {
    checkLoginStatus() {
      const user = localStorage.getItem('pacman_user')
      if (user) {
        this.currentUser = JSON.parse(user)
      }
    },
    truncateUsername(username, maxLength) {
      if (username.length <= maxLength) {
        return username
      }
      return username.substring(0, maxLength - 2) + '..'
    },
    toggleUserMenu() {
      this.showUserMenu = !this.showUserMenu
    },
    logout() {
      localStorage.removeItem('pacman_user')
      this.currentUser = null
      this.showUserMenu = false
    },
    async changeUsername() {
      const currentUsername = this.currentUser.username
      const newUsername = prompt('请输入新的用户名:', currentUsername)
      if (newUsername === null) {
        // 用户取消输入
        return
      }
      const trimmedUsername = newUsername.trim()
      if (trimmedUsername === currentUsername) {
        // 用户没有修改用户名，直接关闭输入框
        return
      }
      if (trimmedUsername) {
        try {
          const response = await fetch('/api/auth/update_username', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json'
            },
            body: JSON.stringify({
              user_id: this.currentUser.id,
              new_username: trimmedUsername
            })
          })
          const data = await response.json()
          if (data.error) {
            alert(data.error)
          } else {
            // 更新本地存储
            this.currentUser.username = trimmedUsername
            localStorage.setItem('pacman_user', JSON.stringify(this.currentUser))
            alert('用户名修改成功')
          }
        } catch (error) {
          console.error('修改用户名失败:', error)
          alert('修改用户名失败，请稍后重试')
        }
      }
    },
    // 防止默认触摸事件
    preventDefaultTouch(e) {
      e.preventDefault()
    },
    // 防止方向键导致页面滚动
    preventScroll(e) {
      // 阻止方向键、空格键、PageUp/PageDown等可能导致滚动的按键
      const keys = ['ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight', ' ', 'PageUp', 'PageDown', 'Home', 'End']
      if (keys.includes(e.key)) {
        e.preventDefault()
      }
    },
    // 防止鼠标滚轮导致页面缩放
    preventZoom(e) {
      if (e.ctrlKey) {
        e.preventDefault()
      }
    },
    checkMobile() {
      this.isMobile = window.innerWidth <= 768
    },
    initializeGame() {
      this.canvas = this.$refs.gameCanvas
      this.ctx = this.canvas.getContext('2d')
      this.calculateGameSize()
      this.initGrid()
      this.drawGame()
    },
    calculateGameSize() {
      // 获取窗口尺寸
      const windowWidth = window.innerWidth
      const windowHeight = window.innerHeight
      
      // 计算游戏区域宽度（两边各留5px）
      const maxGameWidth = windowWidth - 10
      
      // 固定元素高度
      const titleHeight = 50 // 标题高度
      const startBtnHeight = 40 // 开始按钮高度
      const controlsHeight = 100 // 方向键高度
      const totalFixedHeight = titleHeight + startBtnHeight + controlsHeight + 40 // 加上40px的边距
      
      // 计算可用高度（总高度减去固定元素高度）
      let availableHeight = windowHeight - totalFixedHeight
      
      // 确保可用高度为正数，并有最小限制
      if (availableHeight < 180) {
        availableHeight = 180
      }
      
      // 计算合适的方块大小（游戏宽度从10个格子增加到12个格子）
      this.blockSize = Math.floor(Math.min(maxGameWidth / 12, availableHeight / 20))
      
      // 确保方块大小至少为18px
      if (this.blockSize < 18) {
        this.blockSize = 18
      }
      
      // 计算游戏画布尺寸（宽度增加到12个格子）
      this.gameWidth = this.blockSize * 12
      this.gameHeight = this.blockSize * 20
      
      this.canvas.width = this.gameWidth
      this.canvas.height = this.gameHeight
    },
    initGrid() {
      this.grid = []
      for (let y = 0; y < 20; y++) {
        this.grid[y] = []
        for (let x = 0; x < 12; x++) {
          this.grid[y][x] = 0
        }
      }
    },
    spawnPiece() {
      // 方块形状
      const tetrominoes = {
        I: [
          [0, 0, 0, 0],
          [1, 1, 1, 1],
          [0, 0, 0, 0],
          [0, 0, 0, 0]
        ],
        J: [
          [1, 0, 0],
          [1, 1, 1],
          [0, 0, 0]
        ],
        L: [
          [0, 0, 1],
          [1, 1, 1],
          [0, 0, 0]
        ],
        O: [
          [1, 1],
          [1, 1]
        ],
        S: [
          [0, 1, 1],
          [1, 1, 0],
          [0, 0, 0]
        ],
        T: [
          [0, 1, 0],
          [1, 1, 1],
          [0, 0, 0]
        ],
        Z: [
          [1, 1, 0],
          [0, 1, 1],
          [0, 0, 0]
        ]
      }
      
      // 方块颜色 - 使用更明亮、更鲜艳的颜色
      const colors = {
        I: '#00ffff', // 青色
        J: '#4169e1', // 皇家蓝
        L: '#ff6347', // 番茄红
        O: '#ffff66', // 亮黄色
        S: '#32cd32', // 酸橙绿
        T: '#9400d3', // 紫罗兰
        Z: '#ff4500'  // 橙红色
      }
      
      const keys = Object.keys(tetrominoes)
      const randKey = keys[Math.floor(Math.random() * keys.length)]
      
      this.currentPiece = {
        shape: tetrominoes[randKey],
        color: colors[randKey],
        x: 4,
        y: 0,
        type: randKey
      }
      
      if (!this.isValidMove(this.currentPiece.shape, this.currentPiece.x, this.currentPiece.y)) {
        this.gameOver()
      }
    },
    drawGame() {
      // 清空画布
      this.ctx.clearRect(0, 0, this.gameWidth, this.gameHeight)
      
      // 绘制背景
      this.ctx.fillStyle = '#000'
      this.ctx.fillRect(0, 0, this.gameWidth, this.gameHeight)
      
      // 绘制网格
      for (let y = 0; y < 20; y++) {
        for (let x = 0; x < 12; x++) {
          if (this.grid[y][x]) {
            this.drawBlock(x * this.blockSize, y * this.blockSize, this.grid[y][x])
          }
        }
      }
      
      // 绘制当前方块
      if (this.currentPiece) {
        for (let y = 0; y < this.currentPiece.shape.length; y++) {
          for (let x = 0; x < this.currentPiece.shape[y].length; x++) {
            if (this.currentPiece.shape[y][x]) {
              const drawX = this.currentPiece.x + x
              const drawY = this.currentPiece.y + y
              if (drawY >= 0) {
                this.drawBlock(drawX * this.blockSize, drawY * this.blockSize, this.currentPiece.color, true)
              }
            }
          }
        }
      }
      
      // 绘制边框
      this.ctx.strokeStyle = '#00ffaa'
      this.ctx.lineWidth = 2
      this.ctx.strokeRect(0, 0, this.gameWidth, this.gameHeight)
    },
    // 绘制方块，增强立体效果
    drawBlock(x, y, color, isCurrent = false) {
      const blockSize = this.blockSize - 2
      
      // 添加轻微的阴影效果，增强立体感
      if (isCurrent) {
        this.ctx.shadowColor = color
        this.ctx.shadowBlur = 5
        this.ctx.shadowOffsetX = 2
        this.ctx.shadowOffsetY = 2
      }
      
      // 创建更明显的渐变效果，增强立体感
      const gradient = this.ctx.createLinearGradient(x, y, x + blockSize, y + blockSize)
      gradient.addColorStop(0, this.lightenColor(color, 40)) // 更亮的高光
      gradient.addColorStop(0.5, color)
      gradient.addColorStop(1, this.darkenColor(color, 40)) // 更深的阴影
      
      // 绘制方块
      this.ctx.fillStyle = gradient
      this.ctx.fillRect(x, y, blockSize, blockSize)
      
      // 绘制高光边框，增强立体感
      this.ctx.strokeStyle = this.lightenColor(color, 50)
      this.ctx.lineWidth = 2
      this.ctx.strokeRect(x, y, blockSize, blockSize)
      
      // 添加内阴影效果，增强深度感
      this.ctx.strokeStyle = this.darkenColor(color, 50)
      this.ctx.lineWidth = 1
      this.ctx.strokeRect(x + 1, y + 1, blockSize - 2, blockSize - 2)
      
      // 重置阴影
      if (isCurrent) {
        this.ctx.shadowColor = 'transparent'
        this.ctx.shadowBlur = 0
        this.ctx.shadowOffsetX = 0
        this.ctx.shadowOffsetY = 0
      }
    },
    // 颜色处理工具函数
    lightenColor(color, percent) {
      const num = parseInt(color.replace('#', ''), 16)
      const amt = Math.round(2.55 * percent)
      const R = (num >> 16) + amt
      const G = (num >> 8 & 0x00FF) + amt
      const B = (num & 0x0000FF) + amt
      return '#' + (0x1000000 + (R < 255 ? R < 1 ? 0 : R : 255) * 0x10000 + (G < 255 ? G < 1 ? 0 : G : 255) * 0x100 + (B < 255 ? B < 1 ? 0 : B : 255)).toString(16).slice(1)
    },
    darkenColor(color, percent) {
      const num = parseInt(color.replace('#', ''), 16)
      const amt = Math.round(2.55 * percent)
      const R = (num >> 16) - amt
      const G = (num >> 8 & 0x00FF) - amt
      const B = (num & 0x0000FF) - amt
      return '#' + (0x1000000 + (R > 255 ? 255 : R < 0 ? 0 : R) * 0x10000 + (G > 255 ? 255 : G < 0 ? 0 : G) * 0x100 + (B > 255 ? 255 : B < 0 ? 0 : B)).toString(16).slice(1)
    },
    moveLeft() {
      if (!this.isGameOver && !this.gamePaused && this.currentPiece && this.isValidMove(this.currentPiece.shape, this.currentPiece.x - 1, this.currentPiece.y)) {
        this.currentPiece.x--
        this.drawGame()
      }
    },
    moveRight() {
      if (!this.isGameOver && !this.gamePaused && this.currentPiece && this.isValidMove(this.currentPiece.shape, this.currentPiece.x + 1, this.currentPiece.y)) {
        this.currentPiece.x++
        this.drawGame()
      }
    },
    moveDown() {
      if (!this.isGameOver && !this.gamePaused && this.currentPiece) {
        if (this.isValidMove(this.currentPiece.shape, this.currentPiece.x, this.currentPiece.y + 1)) {
          this.currentPiece.y++
          this.drawGame()
        } else {
          this.lockPiece()
          this.clearLines()
          this.spawnPiece()
          this.drawGame()
        }
      }
    },
    hardDrop() {
      if (!this.isGameOver && !this.gamePaused && this.currentPiece) {
        while (this.isValidMove(this.currentPiece.shape, this.currentPiece.x, this.currentPiece.y + 1)) {
          this.currentPiece.y++
        }
        this.lockPiece()
        this.clearLines()
        this.spawnPiece()
        this.drawGame()
      }
    },
    rotatePiece() {
      if (this.isGameOver || this.gamePaused || !this.currentPiece) return
      
      const rotatedShape = this.rotateMatrix(this.currentPiece.shape)
      if (this.isValidMove(rotatedShape, this.currentPiece.x, this.currentPiece.y)) {
        this.currentPiece.shape = rotatedShape
        this.drawGame()
      }
    },
    rotateMatrix(matrix) {
      const rows = matrix.length
      const cols = matrix[0].length
      const rotated = []
      
      for (let x = 0; x < cols; x++) {
        rotated[x] = []
        for (let y = rows - 1; y >= 0; y--) {
          rotated[x][rows - 1 - y] = matrix[y][x]
        }
      }
      
      return rotated
    },
    isValidMove(shape, x, y) {
      for (let row = 0; row < shape.length; row++) {
        for (let col = 0; col < shape[row].length; col++) {
          if (shape[row][col]) {
            const newX = x + col
            const newY = y + row
            
            if (
              newX < 0 ||
              newX >= 12 ||
              newY >= 20 ||
              (newY >= 0 && this.grid[newY] && this.grid[newY][newX])
            ) {
              return false
            }
          }
        }
      }
      return true
    },
    lockPiece() {
      for (let row = 0; row < this.currentPiece.shape.length; row++) {
        for (let col = 0; col < this.currentPiece.shape[row].length; col++) {
          if (this.currentPiece.shape[row][col]) {
            const y = this.currentPiece.y + row
            const x = this.currentPiece.x + col
            if (y >= 0) {
              this.grid[y][x] = this.currentPiece.color
            }
          }
        }
      }
    },
    clearLines() {
      let linesCleared = 0
      
      for (let y = 19; y >= 0; y--) {
        if (this.grid[y].every(cell => cell !== 0)) {
          for (let row = y; row > 0; row--) {
            this.grid[row] = [...this.grid[row - 1]]
          }
          this.grid[0] = Array(12).fill(0)
          linesCleared++
          y++
        }
      }
      
      if (linesCleared > 0) {
        // 计算得分：消除一行加(1*等级)分
        const points = linesCleared * this.level
        this.score += points
        this.lines += linesCleared
        
        // 更新等级：消除100行升一级
        const newLevel = Math.floor(this.lines / 100) + 1
        if (newLevel > this.level) {
          this.level = newLevel
          // 升一级方块下落速度快5%
          this.dropInterval = Math.max(100, this.dropInterval * 0.95)
        }
      }
    },
    gameOver() {
      this.isGameOver = true
      this.clearIntervals()
      // 停止背景音乐
      if (this.backgroundMusic) {
        this.backgroundMusic.pause()
        this.backgroundMusic.currentTime = 0
      }
      this.submitScore()
    },
    async submitScore() {
      if (this.currentUser) {
        try {
          const deviceType = this.isMobile ? 'mobile' : 'desktop'
          const response = await fetch('/api/tetris/scores', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json'
            },
            body: JSON.stringify({
              user_id: this.currentUser.id,
              score: this.score,
              device_type: deviceType
            })
          })
          const data = await response.json()
          if (data.rank) {
            console.log('分数提交成功，排名:', data.rank)
            this.rank = data.rank
          }
        } catch (error) {
          console.error('提交分数失败:', error)
        }
      }
    },
    gameLoop(time) {
      if (this.isGameOver || this.gamePaused) {
        this.animationId = requestAnimationFrame(this.gameLoop)
        return
      }
      
      if (time > this.lastDropTime + this.dropInterval) {
        this.moveDown()
        this.lastDropTime = time
      }
      
      this.animationId = requestAnimationFrame(this.gameLoop)
    },
    startGame() {
      if (!this.gameStarted) {
        this.gameStarted = true
        this.gamePaused = false
        this.isGameOver = false
        this.score = 0
        this.level = 1
        this.lines = 0
        this.dropInterval = 1000
        this.lastDropTime = 0
        
        this.initGrid()
        this.spawnPiece()
        this.drawGame()
        
        // 开始播放背景音乐
        if (this.backgroundMusic) {
          this.backgroundMusic.volume = 0.3 // 设置音量为30%
          this.backgroundMusic.play().catch(error => {
            console.log('自动播放被阻止:', error)
          })
        }
        
        this.lastDropTime = performance.now()
        this.animationId = requestAnimationFrame(this.gameLoop)
      } else {
        this.gamePaused = !this.gamePaused
        if (!this.gamePaused) {
          this.lastDropTime = performance.now()
          // 继续播放背景音乐
          if (this.backgroundMusic) {
            this.backgroundMusic.play().catch(error => {
              console.log('播放背景音乐失败:', error)
            })
          }
        } else {
          // 暂停背景音乐
          if (this.backgroundMusic) {
            this.backgroundMusic.pause()
          }
        }
      }
    },
    toggleGame() {
      // 先尝试播放音频，确保用户交互触发
      if (this.backgroundMusic) {
        this.backgroundMusic.volume = 0.3
        this.backgroundMusic.play().catch(error => {
          console.log('音频播放初始化:', error)
        })
      }
      this.startGame()
    },
    resetGame() {
      this.clearIntervals()
      this.gameStarted = false
      this.gamePaused = false
      this.isGameOver = false
      this.score = 0
      this.level = 1
      this.lines = 0
      this.currentPiece = null
      
      // 停止背景音乐
      if (this.backgroundMusic) {
        this.backgroundMusic.pause()
        this.backgroundMusic.currentTime = 0
      }
      
      this.initGrid()
      this.drawGame()
    },
    restartGame() {
      this.resetGame()
      this.startGame()
    },
    clearIntervals() {
      if (this.animationId) {
        cancelAnimationFrame(this.animationId)
        this.animationId = null
      }
      
      if (this.leftIntervalId) {
        clearTimeout(this.leftIntervalId)
        clearInterval(this.leftIntervalId)
        this.leftIntervalId = null
      }
      
      if (this.rightIntervalId) {
        clearTimeout(this.rightIntervalId)
        clearInterval(this.rightIntervalId)
        this.rightIntervalId = null
      }
      
      if (this.downIntervalId) {
        clearTimeout(this.downIntervalId)
        clearInterval(this.downIntervalId)
        this.downIntervalId = null
      }
    },
    handleLeftDown(e) {
      if (e.type === 'touchstart') {
        if (this.isTouching.left) return
        this.isTouching.left = true
      }
      
      const now = Date.now()
      if (now - this.lastClickTime.left < this.CLICK_DELAY) return
      this.lastClickTime.left = now
      
      this.moveLeft()
      this.leftIntervalId = setTimeout(() => {
        this.leftIntervalId = setInterval(this.moveLeft, this.LONG_PRESS_INTERVAL)
      }, this.LONG_PRESS_DELAY)
    },
    handleLeftUp(e) {
      if (e.type === 'touchend' || e.type === 'touchcancel') {
        this.isTouching.left = false
      }
      
      if (this.leftIntervalId) {
        clearTimeout(this.leftIntervalId)
        clearInterval(this.leftIntervalId)
        this.leftIntervalId = null
      }
    },
    handleRightDown(e) {
      if (e.type === 'touchstart') {
        if (this.isTouching.right) return
        this.isTouching.right = true
      }
      
      const now = Date.now()
      if (now - this.lastClickTime.right < this.CLICK_DELAY) return
      this.lastClickTime.right = now
      
      this.moveRight()
      this.rightIntervalId = setTimeout(() => {
        this.rightIntervalId = setInterval(this.moveRight, this.LONG_PRESS_INTERVAL)
      }, this.LONG_PRESS_DELAY)
    },
    handleRightUp(e) {
      if (e.type === 'touchend' || e.type === 'touchcancel') {
        this.isTouching.right = false
      }
      
      if (this.rightIntervalId) {
        clearTimeout(this.rightIntervalId)
        clearInterval(this.rightIntervalId)
        this.rightIntervalId = null
      }
    },
    handleDownDown(e) {
      if (e.type === 'touchstart') {
        if (this.isTouching.down) return
        this.isTouching.down = true
      }
      
      const now = Date.now()
      if (now - this.lastClickTime.down < this.CLICK_DELAY) return
      this.lastClickTime.down = now
      
      this.moveDown()
      this.downIntervalId = setTimeout(() => {
        this.downIntervalId = setInterval(this.moveDown, this.LONG_PRESS_INTERVAL)
      }, this.LONG_PRESS_DELAY)
    },
    handleDownUp(e) {
      if (e.type === 'touchend' || e.type === 'touchcancel') {
        this.isTouching.down = false
      }
      
      if (this.downIntervalId) {
        clearTimeout(this.downIntervalId)
        clearInterval(this.downIntervalId)
        this.downIntervalId = null
      }
    },
    addEventListeners() {
      window.addEventListener('keydown', this.handleKeyDown.bind(this))
      window.addEventListener('resize', this.handleResize.bind(this))
      // 添加防止页面滚动和缩放的事件监听器
      window.addEventListener('keydown', this.preventScroll.bind(this))
      window.addEventListener('wheel', this.preventZoom.bind(this))
      // 防止双击缩放
      document.addEventListener('dblclick', (e) => e.preventDefault())
    },
    removeEventListeners() {
      window.removeEventListener('keydown', this.handleKeyDown.bind(this))
      window.removeEventListener('resize', this.handleResize.bind(this))
      // 移除防止页面滚动和缩放的事件监听器
      window.removeEventListener('keydown', this.preventScroll.bind(this))
      window.removeEventListener('wheel', this.preventZoom.bind(this))
      // 移除双击事件监听器
      document.removeEventListener('dblclick', (e) => e.preventDefault())
    },
    handleKeyDown(e) {
      switch(e.key) {
        case 'ArrowLeft':
          this.moveLeft()
          break
        case 'ArrowRight':
          this.moveRight()
          break
        case 'ArrowDown':
          this.moveDown()
          break
        case 'ArrowUp':
          this.rotatePiece()
          break
        case ' ':
          e.preventDefault()
          this.hardDrop()
          break
      }
    },
    handleResize() {
      this.checkMobile()
      this.calculateGameSize()
      this.drawGame()
    },
    // 处理canvas触摸事件，防止默认行为导致的闪烁
    handleCanvasTouchStart(e) {
      e.preventDefault()
      this.rotatePiece()
    }
  }
}
</script>

<style scoped>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
  /* 禁用移动端触摸相关的默认行为 */
  -webkit-touch-callout: none; /* 禁用长按菜单 */
  -webkit-user-select: none; /* 禁用文本选择 */
  -khtml-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
  user-select: none;
  /* 防止文本选择 */
  -webkit-user-drag: none;
  user-drag: none;
}

/* 全局样式，防止页面滚动和缩放 */
html, body {
  width: 100%;
  height: 100%;
  overflow: hidden !important;
  position: fixed !important;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  -webkit-overflow-scrolling: none !important;
  overscroll-behavior: none !important;
  /* 防止用户缩放 */
  -webkit-text-size-adjust: 100% !important;
  text-size-adjust: 100% !important;
  /* 禁用双击缩放 */
  -webkit-tap-highlight-color: transparent !important;
}

/* Safari浏览器特定样式修复 */
@media not all and (min-resolution: .001dpcm) {
  /* Safari 10.1+ */
  .control-btn {
    cursor: pointer;
    -webkit-appearance: none; /* 移除默认按钮样式 */
  }
  
  /* 确保Safari中的触摸事件正常工作 */
  .controls {
    -webkit-transform: translateZ(0); /* 启用硬件加速 */
  }
}

.tetris-game {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 10px;
  background: linear-gradient(135deg, #1a1a2e 0%, #16213e 100%);
  color: white;
  /* 禁止页面滚动，提升游戏体验 */
  overflow: hidden;
  position: relative;
  z-index: 1;
}

.container {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 100%;
  height: 100vh;
  max-width: 100%;
  overflow: hidden;
}

.title-container {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  max-width: 500px;
  margin-bottom: 10px;
  flex-shrink: 0;
  position: relative;
}

.home-btn {
  padding: 4px 8px !important;
  font-size: 12px !important;
  min-width: 70px !important;
  border-radius: 0 !important;
  background: transparent !important;
  border: 1px solid #00ffaa !important;
  color: #00ffaa !important;
  box-shadow: none !important;
  font-weight: normal !important;
  transition: none !important;
  touch-action: auto !important;
  -webkit-tap-highlight-color: transparent !important;
}

.username {
  padding: 4px 8px;
  font-size: 12px;
  color: #00ffaa;
  cursor: pointer;
  text-shadow: 0 0 10px rgba(0, 255, 170, 0.5);
  white-space: nowrap;
}

h1 {
  font-size: 32px;
  margin: 10px 0;
  text-align: center;
  color: #00ffaa;
  text-shadow: 0 0 20px rgba(0, 255, 170, 0.5);
  flex-shrink: 0;
  flex: 1;
  margin: 0 20px;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 10px;
}

.relative {
  position: relative;
}

.user-menu {
  position: absolute;
  right: 0;
  top: 100%;
  margin-top: 10px;
  background: rgba(0, 0, 0, 0.9);
  border: 1px solid #00ffaa;
  border-radius: 5px;
  padding: 10px;
  z-index: 1000;
  min-width: 150px;
}

.user-menu button {
  display: block;
  width: 100%;
  text-align: left;
  padding: 8px 12px;
  background: none;
  border: none;
  color: #fff;
  cursor: pointer;
  border-radius: 3px;
  margin-bottom: 5px;
}

.user-menu button:hover {
  background-color: rgba(0, 255, 170, 0.2);
}

.game-container {
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  width: 100%;
  margin-top: 5px;
  margin-bottom: 5px;
}

.game-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  max-width: 500px;
  margin-bottom: 10px;
  font-size: 18px;
  flex-shrink: 0;
  z-index: 10;
}

.info-item {
  flex: 1;
  text-align: center;
  z-index: 11;
  color: #00ffaa;
  font-weight: bold;
}

.info-item:nth-child(2) {
  flex: 1.5;
  z-index: 12;
}

.game-area {
  position: relative;
  flex-shrink: 0;
  border: 2px solid #00ffaa;
  border-radius: 8px;
  box-shadow: 0 0 30px rgba(0, 255, 170, 0.3);
  background: rgba(0, 0, 0, 0.3);
  cursor: pointer;
  display: flex;
  flex-direction: column;
  align-items: center;
}

#gameCanvas {
  display: block;
  background: #000;
}

.game-over {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  background: rgba(0, 0, 0, 0.95);
  padding: 40px;
  border-radius: 20px;
  border: 2px solid #ff6b6b;
  text-align: center;
  box-shadow: 0 0 80px rgba(255, 107, 107, 0.6);
  z-index: 100;
  min-width: 300px;
  max-width: 90%;
  backdrop-filter: blur(10px);
}

.game-over h2 {
  font-size: 36px;
  margin-bottom: 30px;
  color: #ff6b6b;
  text-shadow: 0 0 20px rgba(255, 107, 107, 0.8);
  font-weight: bold;
}

.game-over-stats {
  margin-bottom: 30px;
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.stat-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 20px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 10px;
  border-left: 4px solid #00ffaa;
  transition: all 0.3s ease;
}

.stat-item:hover {
  background: rgba(255, 255, 255, 0.1);
  transform: translateX(5px);
}

.stat-label {
  font-size: 16px;
  color: #ccc;
  font-weight: 500;
}

.stat-value {
  font-size: 20px;
  font-weight: bold;
  color: #00ffaa;
  text-shadow: 0 0 10px rgba(0, 255, 170, 0.5);
}

.stat-value.rank {
  color: #ffd700;
  text-shadow: 0 0 15px rgba(255, 215, 0, 0.8);
  font-size: 24px;
}

.game-over-buttons {
  display: flex;
  gap: 15px;
  justify-content: center;
  flex-wrap: wrap;
}

.restart-btn {
  padding: 14px 28px;
  border: none;
  border-radius: 25px;
  background: linear-gradient(135deg, #00ffaa, #00ccff);
  color: black;
  font-size: 16px;
  font-weight: bold;
  cursor: pointer;
  transition: all 0.3s ease;
  touch-action: auto !important;
  -webkit-tap-highlight-color: transparent;
  min-width: 120px;
  box-shadow: 0 5px 15px rgba(0, 255, 170, 0.3);
}

.restart-btn:hover {
  transform: translateY(-3px);
  box-shadow: 0 8px 25px rgba(0, 255, 170, 0.6);
}

.leaderboard-btn {
  padding: 14px 28px;
  border: none;
  border-radius: 25px;
  background: linear-gradient(135deg, #6b73ff, #00ccff);
  color: white;
  font-size: 16px;
  font-weight: bold;
  cursor: pointer;
  transition: all 0.3s ease;
  touch-action: auto !important;
  -webkit-tap-highlight-color: transparent;
  min-width: 120px;
  box-shadow: 0 5px 15px rgba(107, 115, 255, 0.3);
}

.leaderboard-btn:hover {
  transform: translateY(-3px);
  box-shadow: 0 8px 25px rgba(107, 115, 255, 0.6);
}

.start-btn {
  padding: 8px 20px;
  border: none;
  border-radius: 20px;
  background: linear-gradient(135deg, #ff6b6b, #ee5a24);
  color: white;
  font-size: 14px;
  font-weight: bold;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 5px 15px rgba(255, 107, 107, 0.3);
  touch-action: auto !important;
  -webkit-tap-highlight-color: transparent;
}

.start-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(255, 107, 107, 0.5);
}

/* 方向键容器 */
.controls-container {
  display: flex;
  justify-content: space-between;
  width: 100%;
  max-width: 300px; /* 游戏区域宽度，确保方向键对齐游戏区域 */
  margin-top: 1px;
  margin-bottom: 0;
  flex-shrink: 0;
  min-height: 80px;
  padding: 0 10px;
  box-sizing: border-box;
  /* 确保在所有浏览器中可见 */
  position: relative;
  z-index: 1000;
  opacity: 1;
  visibility: visible;
}

/* 安卓Edge浏览器特定修复 */
@media all and (max-width: 1024px) {
  .controls-container {
    /* 确保在安卓Edge浏览器中可见 */
    display: flex !important;
    position: relative !important;
    z-index: 1000 !important;
    opacity: 1 !important;
    visibility: visible !important;
    min-height: 100px !important;
    margin-bottom: 0 !important;
  }
  
  .control-btn {
    /* 确保在安卓Edge浏览器中按钮可见 */
    opacity: 1 !important;
    visibility: visible !important;
    position: relative !important;
    z-index: 1001 !important;
  }
}

/* 确保左右方向键分别对齐游戏区域的左右 */
.controls-container .control-btn:first-child {
  align-self: center;
}

.controls-container .control-btn:last-child {
  align-self: center;
}

.controls-container .control-btn:nth-child(2) {
  align-self: center;
}

.control-btn {
  width: 70px; /* 改为原来的70% (100px * 0.7) */
  height: 56px; /* 改为原来的70% (80px * 0.7) */
  border-radius: 7px; /* 改为原来的70% (10px * 0.7) */
  border: none;
  background: linear-gradient(135deg, #00ffaa, #00ccff);
  color: black;
  font-size: 17px; /* 改为原来的70% (24px * 0.7) */
  font-weight: bold;
  cursor: pointer;
  transition: all 0.2s ease;
  box-shadow: 0 5px 15px rgba(0, 255, 170, 0.3);
  touch-action: auto !important;
  -webkit-tap-highlight-color: transparent;
}

.control-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(0, 255, 170, 0.5);
}

.control-btn:active {
  transform: translateY(0);
}

.footer {
  display: flex;
  justify-content: center;
  gap: 10px;
  margin-top: 10px;
  flex-wrap: wrap;
  flex-shrink: 0;
}

.btn {
  padding: 8px 16px;
  border: none;
  border-radius: 20px;
  background: linear-gradient(135deg, #00ffaa, #00ccff);
  color: black;
  font-size: 14px;
  font-weight: bold;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 5px 15px rgba(0, 255, 170, 0.3);
  touch-action: auto !important;
  -webkit-tap-highlight-color: transparent;
}

.btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(0, 255, 170, 0.5);
}

.btn-secondary {
  background: linear-gradient(135deg, #6b73ff, #00ccff);
}

/* Responsive Design */
@media (max-width: 480px) {
  h1 {
    font-size: 24px;
  }
  
  .game-info {
    font-size: 14px;
    flex-wrap: nowrap;
  }
  
  .info-item {
    flex: 1;
    min-width: 80px;
  }
  
  .info-item:nth-child(2) {
    flex: 1.5;
    min-width: 120px;
  }
  
  .start-btn {
    font-size: 12px;
    padding: 8px 20px;
    width: 100%;
    min-width: 100px;
    z-index: 100;
  }
  
  .control-btn {
    width: 56px; /* 改为原来的70% (80px * 0.7) */
    height: 56px; /* 改为原来的70% (80px * 0.7) */
    font-size: 14px; /* 改为原来的70% (20px * 0.7) */
  }
  
  .controls-container {
    gap: 20px;
  }
  
  .footer {
    flex-direction: column;
    align-items: center;
  }
  
  .footer .btn {
    width: 100%;
    margin: 5px 0;
  }
}
</style>