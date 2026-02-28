<template>
  <div class="tank-game" @contextmenu.prevent @touchstart="handleScreenTouch">
    <div class="container">
      <div class="title-container">
        <button class="btn home-btn" @click="$router.push('/')" @touchstart.stop>返回首页</button>
        <h1>坦克大战</h1>
        <div class="user-info" v-if="currentUser">
          <div class="relative">
            <span class="username" @click="toggleUserMenu" :title="currentUser.username" @touchstart.stop>
              {{ truncateUsername(currentUser.username, 8) }}
            </span>
            <div class="user-menu" v-if="showUserMenu">
              <button @click="logout" @touchstart.stop>退出登录</button>
              <button @click="$router.push('/login?action=change')" @touchstart.stop>修改密码</button>
              <button @click="changeUsername" @touchstart.stop>修改用户名</button>
            </div>
          </div>
        </div>
      </div>
      
      <div class="game-container">
        <div class="game-info">
          <div class="lives">生命: {{ lives }}</div>
          <div class="score">分数: {{ score }}</div>
          <div class="level">等级: {{ level }}</div>
        </div>
        
        <canvas ref="gameCanvas" class="game-canvas" width="520" height="520"></canvas>
        
        <div class="game-controls">
          <button class="btn start-btn" @click="handleGameControl" @keydown.space.prevent @touchstart.stop>
            {{ gameStarted ? (gamePaused ? '继续游戏' : '暂停游戏') : '开始游戏' }}
          </button>
          <button class="btn reset-btn" @click="resetGameOnly" @touchstart.stop>
            重置游戏
          </button>
        </div>
        
        <div class="mobile-controls" v-if="isMobile">
          <div class="joystick-container">
            <div class="joystick-base" ref="joystickBase">
              <div class="joystick-thumb" ref="joystickThumb" 
                   @touchstart="startJoystick" 
                   @touchmove="moveJoystick" 
                   @touchend="stopJoystick" 
                   @touchcancel="stopJoystick"></div>
            </div>
          </div>
        </div>
      </div>
      
      <div class="game-over-panel" v-if="gameOver">
        <h2>游戏结束</h2>
        <p>最终分数: {{ score }}</p>
        <p>最终等级: {{ level }}</p>
        <button class="btn" @click="restartGame" @touchstart.stop>重新开始</button>
        <button class="btn btn-secondary" @click="$router.push('/')" @touchstart.stop>返回首页</button>
      </div>
      
      <div class="footer">
        <button class="btn btn-secondary" v-if="!currentUser" @click="$router.push('/login')" @touchstart.stop>登录</button>
        <button class="btn btn-secondary" v-if="!currentUser" @click="$router.push('/register')" @touchstart.stop>注册</button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'TankGame',
  data() {
      return {
        currentUser: null,
        showUserMenu: false,
        canvas: null,
        ctx: null,
        player: null,
        enemies: [],
        bullets: [],
        terrain: [],
        walls: [],
        explosions: [],
        lives: 3,
        score: 0,
        level: 1,
        gameOver: false,
        gameStarted: false,
        gamePaused: false,
        gameLoop: null,
        keys: {
          up: false,
          left: false,
          right: false,
          down: false,
          space: false
        },
        isMobile: false,
        moveInterval: null,
        // 摇杆相关状态
        joystickActive: false,
        joystickCenter: { x: 0, y: 0 },
        joystickRadius: 60,
        joystickMaxDistance: 40,
        // 地图随机种子
        mapSeed: 0
      }
    },
  mounted() {
    this.checkLoginStatus()
    this.checkMobile()
    this.initGame()
    this.setupEventListeners()
  },
  beforeUnmount() {
    this.cleanup()
  },
  methods: {
    checkLoginStatus() {
      const user = localStorage.getItem('pacman_user')
      if (user) {
        this.currentUser = JSON.parse(user)
      }
    },
    checkMobile() {
      this.isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)
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
        return
      }
      const trimmedUsername = newUsername.trim()
      if (trimmedUsername === currentUsername) {
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
    initGame() {
      this.canvas = this.$refs.gameCanvas
      this.ctx = this.canvas.getContext('2d')
      this.resetGame()
      // 绘制初始画面（即使游戏没有开始也要显示地图）
      this.drawInitialScreen()
      // 游戏未开始时不启动游戏循环
      if (this.gameStarted) {
        this.gameLoop = setInterval(() => this.updateGame(), 16)
      }
    },
    drawInitialScreen() {
      // 清空画布
      this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height)
      // 绘制背景
      this.ctx.fillStyle = '#000000'
      this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height)
      // 绘制地形
      this.drawTerrain()
    },
    resetGame() {
      // 生成新的地图随机种子
      this.mapSeed = Math.floor(Math.random() * 10000)
      
      this.lives = 3
      this.score = 0
      this.level = 1
      this.gameOver = false
      
      // 初始化玩家坦克（基地左边外面）
      this.player = {
        x: 168,
        y: 456,
        width: 24,
        height: 24,
        direction: 0, // 0: up, 1: right, 2: down, 3: left
        speed: 2,
        fireRate: 200,
        lastFire: 0
      }
      
      // 初始化敌人坦克（顶部）
      this.enemies = []
      const enemyPositions = [
        { x: 72, y: 48 },
        { x: 240, y: 48 },
        { x: 408, y: 48 }
      ]
      const enemyTypes = ['normal', 'fast', 'smart']
      enemyPositions.forEach((pos, index) => {
        this.enemies.push({
          x: pos.x,
          y: pos.y,
          width: 24,
          height: 24,
          direction: 2, // 0: up, 1: right, 2: down, 3: left
          speed: 1,
          fireRate: 1000,
          lastFire: 0,
          type: enemyTypes[index % enemyTypes.length] // 循环分配敌人类型
        })
      })
      
      // 初始化地形
      this.terrain = []
      const gridSize = 21
      const cellSize = 24
      
      // 创建21x21网格
      for (let y = 0; y < gridSize; y++) {
        for (let x = 0; x < gridSize; x++) {
          const terrainType = this.generateTerrain(x, y)
          if (terrainType !== 'floor') {
            const terrain = {
              x: x * cellSize,
              y: y * cellSize,
              width: cellSize,
              height: cellSize,
              type: terrainType
            }
            // 为砖头添加耐久度
            if (terrainType === 'brick') {
              terrain.health = 3
            }
            this.terrain.push(terrain)
          }
        }
      }
      
      // 初始化子弹和爆炸
      this.bullets = []
      this.explosions = []
      
      // 打印地形元素数量
      console.log('Terrain elements generated:', this.terrain.length)
      console.log('Terrain elements:', this.terrain)
    },
    
    generateTerrain(x, y) {
      // 基地位置（底部中央）
      if (x === 10 && y === 20) {
        return 'base'
      }
      
      // 基地周围的保护区域（地面）
      if ((x === 10) && (y === 19)) {
        return 'floor'
      }
      
      // 基地周围的围墙保护（砖墙）
      // 顶部围墙
      if ((x >= 9 && x <= 11) && (y === 18)) {
        return 'brick'
      }
      // 左侧围墙
      if (x === 9 && (y >= 18 && y <= 20)) {
        return 'brick'
      }
      // 右侧围墙
      if (x === 11 && (y >= 18 && y <= 20)) {
        return 'brick'
      }
      // 底部围墙
      if ((x >= 9 && x <= 11) && (y === 21)) {
        return 'brick'
      }
      
      // 顶部敌人区域
      if (y < 3) {
        return 'floor'
      }
      
      // 玩家初始区域
      if (y > 17) {
        return 'floor'
      }
      
      // 经典FC坦克大战地图布局
      // 中心通道
      if ((x === 10 || x === 11) && (y >= 5 && y <= 13)) {
        return 'floor'
      }
      
      // 左右通道
      if ((x === 3 || x === 4) && (y >= 6 && y <= 11)) {
        return 'floor'
      }
      if ((x === 16 || x === 17) && (y >= 6 && y <= 11)) {
        return 'floor'
      }
      
      // 使用随机种子生成地图
      const seed = (x * 7 + y * 13 + this.mapSeed) % 100
      
      // 随机生成一些地形元素
      // 砖墙区域 - 70%概率生成
      if ((x >= 1 && x <= 5) && (y >= 5 && y <= 8)) {
        if (seed < 70) return 'brick'
      }
      if ((x >= 15 && x <= 19) && (y >= 5 && y <= 8)) {
        if (seed < 70) return 'brick'
      }
      if ((x >= 1 && x <= 5) && (y >= 10 && y <= 13)) {
        if (seed < 70) return 'brick'
      }
      if ((x >= 15 && x <= 19) && (y >= 10 && y <= 13)) {
        if (seed < 70) return 'brick'
      }
      
      // 钢板区域 - 30%概率生成
      if ((x >= 6 && x <= 14) && (y >= 6 && y <= 8)) {
        if (seed > 60 && seed < 90) return 'steel'
      }
      if ((x >= 6 && x <= 14) && (y >= 10 && y <= 12)) {
        if (seed > 60 && seed < 90) return 'steel'
      }
      
      // 水域区域 - 40%概率生成
      if ((x >= 1 && x <= 5) && (y >= 9 && y <= 10)) {
        if (seed < 40) return 'water'
      }
      if ((x >= 15 && x <= 19) && (y >= 9 && y <= 10)) {
        if (seed < 40) return 'water'
      }
      
      // 森林区域 - 50%概率生成
      if ((x >= 5 && x <= 15) && (y >= 7 && y <= 11)) {
        if (seed < 50) return 'forest'
      }
      
      // 默认地板
      return 'floor'
    },
    setupEventListeners() {
      // 键盘控制
      window.addEventListener('keydown', (e) => {
        switch (e.key) {
          case 'ArrowUp':
            this.keys.up = true
            break
          case 'ArrowLeft':
            this.keys.left = true
            break
          case 'ArrowRight':
            this.keys.right = true
            break
          case 'ArrowDown':
            this.keys.down = true
            break
          case ' ': // space
            this.keys.space = true
            this.fire()
            break
        }
      })
      
      window.addEventListener('keyup', (e) => {
        switch (e.key) {
          case 'ArrowUp':
            this.keys.up = false
            break
          case 'ArrowLeft':
            this.keys.left = false
            break
          case 'ArrowRight':
            this.keys.right = false
            break
          case 'ArrowDown':
            this.keys.down = false
            break
          case ' ': // space
            this.keys.space = false
            break
        }
      })
    },
    startMove(direction) {
      this.keys[direction] = true
      if (direction === 'up') {
        this.player.direction = 0
      } else if (direction === 'left') {
        this.player.direction = 3
      } else if (direction === 'right') {
        this.player.direction = 1
      } else if (direction === 'down') {
        this.player.direction = 2
      }
      
      // 持续移动
      if (!this.moveInterval) {
        this.moveInterval = setInterval(() => {
          this.movePlayer()
        }, 20)
      }
    },
    stopMove(direction) {
      this.keys[direction] = false
      if (!this.keys.up && !this.keys.left && !this.keys.right && !this.keys.down) {
        clearInterval(this.moveInterval)
        this.moveInterval = null
      }
    },
    movePlayer() {
      if (this.keys.up) {
        this.player.direction = 0
        this.player.y -= this.player.speed
      } else if (this.keys.left) {
        this.player.direction = 3
        this.player.x -= this.player.speed
      } else if (this.keys.right) {
        this.player.direction = 1
        this.player.x += this.player.speed
      } else if (this.keys.down) {
        this.player.direction = 2
        this.player.y += this.player.speed
      }
      
      // 边界检查
      this.player.x = Math.max(0, Math.min(this.canvas.width - this.player.width, this.player.x))
      this.player.y = Math.max(0, Math.min(this.canvas.height - this.player.height, this.player.y))
      
      // 碰撞检查
      this.checkCollisions()
    },
    fire() {
      const now = Date.now()
      if (now - this.player.lastFire < this.player.fireRate) {
        return
      }
      
      this.player.lastFire = now
      
      let bulletX, bulletY, bulletDirection
      
      switch (this.player.direction) {
        case 0: // up
          bulletX = this.player.x + this.player.width / 2 - 2
          bulletY = this.player.y - 10
          bulletDirection = 0
          break
        case 1: // right
          bulletX = this.player.x + this.player.width + 2
          bulletY = this.player.y + this.player.height / 2 - 2
          bulletDirection = 1
          break
        case 2: // down
          bulletX = this.player.x + this.player.width / 2 - 2
          bulletY = this.player.y + this.player.height + 2
          bulletDirection = 2
          break
        case 3: // left
          bulletX = this.player.x - 10
          bulletY = this.player.y + this.player.height / 2 - 2
          bulletDirection = 3
          break
      }
      
      this.bullets.push({
        x: bulletX,
        y: bulletY,
        width: 4,
        height: 4,
        direction: bulletDirection,
        speed: 5,
        type: 'player'
      })
    },
    updateGame() {
      if (this.gameOver || !this.gameStarted || this.gamePaused) return
      
      // 移动玩家
      this.movePlayer()
      
      // 移动敌人
      this.moveEnemies()
      
      // 移动子弹
      this.moveBullets()
      
      // 敌人开火
      this.enemiesFire()
      
      // 检查碰撞
      this.checkCollisions()
      
      // 检查游戏结束
      this.checkGameOver()
      
      // 清空画布
      this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height)
      
      // 绘制背景（FC经典黑色背景）
      this.ctx.fillStyle = '#000000'
      this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height)
      
      // 绘制地形
      this.drawTerrain()
      
      // 绘制玩家
      this.drawPlayer()
      
      // 绘制敌人
      this.drawEnemies()
      
      // 绘制子弹
      this.drawBullets()
      
      // 绘制爆炸效果
      this.drawExplosions()
    },
    drawPlayer() {
      // 检查坦克是否在森林中
      let isInForest = false
      for (let i = 0; i < this.terrain.length; i++) {
        const tile = this.terrain[i]
        if (tile.type === 'forest' && this.checkCollision(this.player, tile)) {
          isInForest = true
          break
        }
      }
      
      // 如果在森林中，不绘制坦克
      if (isInForest) return
      
      this.ctx.save()
      this.ctx.translate(this.player.x + this.player.width / 2, this.player.y + this.player.height / 2)
      this.ctx.rotate(this.player.direction * Math.PI / 2)
      this.ctx.translate(-(this.player.x + this.player.width / 2), -(this.player.y + this.player.height / 2))
      
      // 简化的FC风格坦克设计
      // 坦克车身（绿色）
      this.ctx.fillStyle = '#00FF00'
      this.ctx.fillRect(this.player.x + 2, this.player.y + 2, this.player.width - 4, this.player.height - 4)
      
      // 坦克履带（黑色）
      this.ctx.fillStyle = '#000000'
      this.ctx.fillRect(this.player.x, this.player.y + 2, 2, this.player.height - 4)
      this.ctx.fillRect(this.player.x + this.player.width - 2, this.player.y + 2, 2, this.player.height - 4)
      
      // 坦克炮塔（深绿色）
      this.ctx.fillStyle = '#008000'
      this.ctx.fillRect(this.player.x + 4, this.player.y, this.player.width - 8, this.player.height)
      
      // 坦克炮管（绿色）
      this.ctx.fillStyle = '#00FF00'
      this.ctx.fillRect(this.player.x + this.player.width / 2 - 1, this.player.y - 8, 2, 10)
      
      // 炮管前端（深绿色）
      this.ctx.fillStyle = '#008000'
      this.ctx.fillRect(this.player.x + this.player.width / 2 - 2, this.player.y - 10, 4, 2)
      
      this.ctx.restore()
    },
    drawEnemies() {
      for (let i = 0; i < this.enemies.length; i++) {
        const enemy = this.enemies[i]
        
        // 检查敌人是否在森林中
        let isInForest = false
        for (let j = 0; j < this.terrain.length; j++) {
          const tile = this.terrain[j]
          if (tile.type === 'forest' && this.checkCollision(enemy, tile)) {
            isInForest = true
            break
          }
        }
        
        // 如果在森林中，不绘制敌人
        if (isInForest) continue
        
        this.ctx.save()
        this.ctx.translate(enemy.x + enemy.width / 2, enemy.y + enemy.height / 2)
        this.ctx.rotate(enemy.direction * Math.PI / 2)
        this.ctx.translate(-(enemy.x + enemy.width / 2), -(enemy.y + enemy.height / 2))
        
        // 根据敌人类型设置颜色
        let bodyColor, turretColor, cannonColor, cannonTipColor
        switch (enemy.type) {
          case 'fast':
            // 快速敌人：粉色
            bodyColor = '#FF69B4'
            turretColor = '#FF1493'
            cannonColor = '#FF69B4'
            cannonTipColor = '#FF1493'
            break
          case 'heavy':
            // 重型敌人：灰色
            bodyColor = '#808080'
            turretColor = '#404040'
            cannonColor = '#808080'
            cannonTipColor = '#404040'
            break
          case 'smart':
            // 智能敌人：白色
            bodyColor = '#FFFFFF'
            turretColor = '#C0C0C0'
            cannonColor = '#FFFFFF'
            cannonTipColor = '#C0C0C0'
            break
          default:
            // 普通敌人：红色
            bodyColor = '#FF0000'
            turretColor = '#800000'
            cannonColor = '#FF0000'
            cannonTipColor = '#800000'
        }
        
        // 简化的FC风格敌人坦克设计
        // 坦克车身
        this.ctx.fillStyle = bodyColor
        this.ctx.fillRect(enemy.x + 2, enemy.y + 2, enemy.width - 4, enemy.height - 4)
        
        // 坦克履带（黑色）
        this.ctx.fillStyle = '#000000'
        this.ctx.fillRect(enemy.x, enemy.y + 2, 2, enemy.height - 4)
        this.ctx.fillRect(enemy.x + enemy.width - 2, enemy.y + 2, 2, enemy.height - 4)
        
        // 坦克炮塔
        this.ctx.fillStyle = turretColor
        this.ctx.fillRect(enemy.x + 4, enemy.y, enemy.width - 8, enemy.height)
        
        // 坦克炮管
        this.ctx.fillStyle = cannonColor
        this.ctx.fillRect(enemy.x + enemy.width / 2 - 1, enemy.y - 8, 2, 10)
        
        // 炮管前端
        this.ctx.fillStyle = cannonTipColor
        this.ctx.fillRect(enemy.x + enemy.width / 2 - 2, enemy.y - 10, 4, 2)
        
        this.ctx.restore()
      }
    },
    drawTerrain() {
      for (let i = 0; i < this.terrain.length; i++) {
        const tile = this.terrain[i]
        switch (tile.type) {
          case 'brick':
            // 简化的砖墙设计
            this.ctx.fillStyle = '#FF8C00' // 橙色
            this.ctx.fillRect(tile.x, tile.y, tile.width, tile.height)
            // 砖墙纹理
            this.ctx.fillStyle = '#FF4500' // 橙红色
            this.ctx.fillRect(tile.x + 4, tile.y + 4, tile.width - 8, tile.height - 8)
            break
          case 'steel':
            // 简化的钢板设计
            this.ctx.fillStyle = '#C0C0C0' // 银色
            this.ctx.fillRect(tile.x, tile.y, tile.width, tile.height)
            // 钢板纹理
            this.ctx.strokeStyle = '#808080' // 灰色
            this.ctx.lineWidth = 1
            this.ctx.beginPath()
            this.ctx.moveTo(tile.x + 4, tile.y)
            this.ctx.lineTo(tile.x + 4, tile.y + tile.height)
            this.ctx.stroke()
            this.ctx.beginPath()
            this.ctx.moveTo(tile.x + 8, tile.y)
            this.ctx.lineTo(tile.x + 8, tile.y + tile.height)
            this.ctx.stroke()
            this.ctx.beginPath()
            this.ctx.moveTo(tile.x + 12, tile.y)
            this.ctx.lineTo(tile.x + 12, tile.y + tile.height)
            this.ctx.stroke()
            this.ctx.beginPath()
            this.ctx.moveTo(tile.x, tile.y + 4)
            this.ctx.lineTo(tile.x + tile.width, tile.y + 4)
            this.ctx.stroke()
            this.ctx.beginPath()
            this.ctx.moveTo(tile.x, tile.y + 8)
            this.ctx.lineTo(tile.x + tile.width, tile.y + 8)
            this.ctx.stroke()
            this.ctx.beginPath()
            this.ctx.moveTo(tile.x, tile.y + 12)
            this.ctx.lineTo(tile.x + tile.width, tile.y + 12)
            this.ctx.stroke()
            break
          case 'water':
            // 简化的水域设计
            this.ctx.fillStyle = '#0000FF' // 蓝色
            this.ctx.fillRect(tile.x, tile.y, tile.width, tile.height)
            break
          case 'forest':
            // 简化的森林设计
            this.ctx.fillStyle = '#008000' // 绿色
            this.ctx.fillRect(tile.x, tile.y, tile.width, tile.height)
            break
          case 'base':
            // 简化的基地设计
            this.ctx.fillStyle = '#FFFF00' // 黄色
            this.ctx.fillRect(tile.x, tile.y, tile.width, tile.height)
            // 基地标志（十字形）
            this.ctx.fillStyle = '#FFA500' // 橙色
            this.ctx.fillRect(tile.x + 6, tile.y + 3, 8, 14)
            this.ctx.fillRect(tile.x + 3, tile.y + 6, 14, 8)
            break
        }
      }
    },
    drawBullets() {
      for (let i = 0; i < this.bullets.length; i++) {
        const bullet = this.bullets[i]
        // FC经典风格的子弹
        const bulletColor = bullet.type === 'player' ? '#00FF00' : '#FF0000'
        this.ctx.fillStyle = bulletColor
        this.ctx.fillRect(bullet.x, bullet.y, bullet.width, bullet.height)
      }
    },
    moveEnemies() {
      for (let i = 0; i < this.enemies.length; i++) {
        const enemy = this.enemies[i]
        // FC原版敌人AI行为模式
        // 1. 随机改变方向（模拟FC原版的随机移动）
        if (Math.random() < 0.02) {
          enemy.direction = Math.floor(Math.random() * 4)
        }
        
        // 2. 尝试向玩家方向移动（智能敌人行为）
        if (Math.random() < 0.1) {
          // 优先水平方向移动
          if (Math.abs(this.player.x - enemy.x) > Math.abs(this.player.y - enemy.y)) {
            if (this.player.x > enemy.x) {
              enemy.direction = 1 // 向右
            } else if (this.player.x < enemy.x) {
              enemy.direction = 3 // 向左
            }
          } else {
            // 垂直方向移动
            if (this.player.y > enemy.y) {
              enemy.direction = 2 // 向下
            } else if (this.player.y < enemy.y) {
              enemy.direction = 0 // 向上
            }
          }
        }
        
        // 3. 碰撞检测和方向调整
        const oldX = enemy.x
        const oldY = enemy.y
        
        // 计算新位置
        let newX = enemy.x
        let newY = enemy.y
        
        switch (enemy.direction) {
          case 0: // up
            newY -= enemy.speed
            break
          case 1: // right
            newX += enemy.speed
            break
          case 2: // down
            newY += enemy.speed
            break
          case 3: // left
            newX -= enemy.speed
            break
        }
        
        // 检查新位置是否会碰撞到障碍物
        const tempEnemy = { ...enemy, x: newX, y: newY }
        let willCollide = false
        
        for (let j = 0; j < this.terrain.length; j++) {
          const tile = this.terrain[j]
          if (tile.type !== 'forest' && this.checkCollision(tempEnemy, tile)) {
            willCollide = true
            break
          }
        }
        
        // 如果不会碰撞且在边界内，才移动
        if (!willCollide) {
          // 边界检查
          newX = Math.max(0, Math.min(this.canvas.width - enemy.width, newX))
          newY = Math.max(0, Math.min(this.canvas.height - enemy.height, newY))
          
          enemy.x = newX
          enemy.y = newY
        } else {
          // 如果会碰撞，改变方向
          enemy.direction = (enemy.direction + 1) % 4
        }
        
        // 4. 敌人类型特定行为
        if (enemy.type === 'fast') {
          // 快速敌人：移动速度更快
          enemy.speed = 1.5
        } else if (enemy.type === 'heavy') {
          // 重型敌人：移动速度较慢，但更耐打
          enemy.speed = 0.8
        } else if (enemy.type === 'smart') {
          // 智能敌人：更频繁地追踪玩家
          if (Math.random() < 0.2) {
            if (this.player.x > enemy.x) {
              enemy.direction = 1
            } else if (this.player.x < enemy.x) {
              enemy.direction = 3
            } else if (this.player.y > enemy.y) {
              enemy.direction = 2
            } else if (this.player.y < enemy.y) {
              enemy.direction = 0
            }
          }
        }
      }
    },
    moveBullets() {
      const bulletsToKeep = []
      for (let i = 0; i < this.bullets.length; i++) {
        const bullet = this.bullets[i]
        // 移动子弹
        switch (bullet.direction) {
          case 0: // up
            bullet.y -= bullet.speed
            break
          case 1: // right
            bullet.x += bullet.speed
            break
          case 2: // down
            bullet.y += bullet.speed
            break
          case 3: // left
            bullet.x -= bullet.speed
            break
        }
        
        // 检查子弹是否超出边界
        if (bullet.x >= 0 && bullet.x <= this.canvas.width && bullet.y >= 0 && bullet.y <= this.canvas.height) {
          bulletsToKeep.push(bullet)
        }
      }
      this.bullets = bulletsToKeep
    },
    enemiesFire() {
      const now = Date.now()
      for (let i = 0; i < this.enemies.length; i++) {
        const enemy = this.enemies[i]
        if (now - enemy.lastFire < enemy.fireRate) {
          continue
        }
        
        if (Math.random() < 0.01) {
          enemy.lastFire = now
          
          let bulletX, bulletY, bulletDirection
          
          switch (enemy.direction) {
            case 0: // up
              bulletX = enemy.x + enemy.width / 2 - 2
              bulletY = enemy.y - 10
              bulletDirection = 0
              break
            case 1: // right
              bulletX = enemy.x + enemy.width + 2
              bulletY = enemy.y + enemy.height / 2 - 2
              bulletDirection = 1
              break
            case 2: // down
              bulletX = enemy.x + enemy.width / 2 - 2
              bulletY = enemy.y + enemy.height + 2
              bulletDirection = 2
              break
            case 3: // left
              bulletX = enemy.x - 10
              bulletY = enemy.y + enemy.height / 2 - 2
              bulletDirection = 3
              break
          }
          
          this.bullets.push({
            x: bulletX,
            y: bulletY,
            width: 4,
            height: 4,
            direction: bulletDirection,
            speed: 5,
            type: 'enemy'
          })
        }
      }
    },
    checkCollisions() {
      // 优化版碰撞检测
      
      // 玩家与地形碰撞
      for (let i = 0; i < this.terrain.length; i++) {
        const tile = this.terrain[i]
        if (tile.type !== 'forest' && this.checkCollision(this.player, tile)) {
          // 简单的碰撞响应：将坦克推回碰撞前的位置
          const oldX = this.player.x
          const oldY = this.player.y
          
          // 尝试不同方向的移动，找到最合适的位置
          const directions = [
            { dx: 0, dy: -this.player.speed }, // 上
            { dx: 0, dy: this.player.speed },  // 下
            { dx: -this.player.speed, dy: 0 },  // 左
            { dx: this.player.speed, dy: 0 }    // 右
          ]
          
          let bestDistance = Infinity
          let bestPosition = { x: oldX, y: oldY }
          
          for (const dir of directions) {
            this.player.x = oldX + dir.dx
            this.player.y = oldY + dir.dy
            
            // 检查新位置是否碰撞
            if (!this.checkCollision(this.player, tile)) {
              const distance = Math.abs(dir.dx) + Math.abs(dir.dy)
              if (distance < bestDistance) {
                bestDistance = distance
                bestPosition = { x: this.player.x, y: this.player.y }
              }
            }
          }
          
          // 恢复最佳位置
          this.player.x = bestPosition.x
          this.player.y = bestPosition.y
          break // 只处理第一个碰撞的地形
        }
      }
      
      // 敌人与地形碰撞
      for (let i = 0; i < this.enemies.length; i++) {
        const enemy = this.enemies[i]
        for (let j = 0; j < this.terrain.length; j++) {
          const tile = this.terrain[j]
          if (tile.type !== 'forest' && this.checkCollision(enemy, tile)) {
            // 敌人碰撞后改变方向
            enemy.direction = (enemy.direction + 1) % 4
            
            // 尝试向后移动一小段距离
            switch (enemy.direction) {
              case 0: enemy.y += enemy.speed; break
              case 1: enemy.x -= enemy.speed; break
              case 2: enemy.y -= enemy.speed; break
              case 3: enemy.x += enemy.speed; break
            }
            break // 只处理第一个碰撞的地形
          }
        }
      }
      
      // 子弹与地形碰撞
      // 首先复制所有地形元素
      let terrainToKeep = [...this.terrain]
      const bulletsToKeep = []
      
      for (let i = 0; i < this.bullets.length; i++) {
        const bullet = this.bullets[i]
        let hit = false
        
        for (let j = 0; j < terrainToKeep.length; j++) {
          const tile = terrainToKeep[j]
          if (this.checkCollision(bullet, tile)) {
            // 子弹可以穿过森林和水域
            if (tile.type === 'forest' || tile.type === 'water') {
              continue
            }
            hit = true
            // 只有砖墙可以被破坏
            if (tile.type === 'brick') {
              // 减少砖头的耐久度
              tile.health--
              // 创建爆炸效果
              this.createExplosion(tile.x + tile.width / 2, tile.y + tile.height / 2)
              // 只有当耐久度为0时才删除砖头
              if (tile.health <= 0) {
                terrainToKeep.splice(j, 1)
                j--
              }
            }
            // 基地被击中，游戏结束
            if (tile.type === 'base') {
              this.createExplosion(tile.x + tile.width / 2, tile.y + tile.height / 2)
              this.gameOver = true
            }
            break
          }
        }
        
        if (!hit) {
          bulletsToKeep.push(bullet)
        }
      }
      
      this.terrain = terrainToKeep
      this.bullets = bulletsToKeep
      
      // 玩家子弹与敌人碰撞
      const bulletsAfterEnemyCollision = []
      const enemiesToKeep = []
      
      // 首先标记所有敌人为未被击中
      const enemyHit = new Array(this.enemies.length).fill(false)
      
      for (let i = 0; i < this.bullets.length; i++) {
        const bullet = this.bullets[i]
        if (bullet.type === 'player') {
          let hit = false
          for (let j = 0; j < this.enemies.length; j++) {
            const enemy = this.enemies[j]
            if (!enemyHit[j] && this.checkCollision(bullet, enemy)) {
              // 创建爆炸效果
              this.createExplosion(enemy.x + enemy.width / 2, enemy.y + enemy.height / 2)
              hit = true
              enemyHit[j] = true
              
              // 根据敌人类型增加不同的分数
              switch (enemy.type) {
                case 'fast': this.score += 300; break
                case 'heavy': this.score += 200; break
                case 'smart': this.score += 400; break
                default: this.score += 100; break
              }
            }
          }
          if (!hit) {
            bulletsAfterEnemyCollision.push(bullet)
          }
        } else {
          bulletsAfterEnemyCollision.push(bullet)
        }
      }
      
      // 收集未被击中的敌人
      for (let i = 0; i < this.enemies.length; i++) {
        if (!enemyHit[i]) {
          enemiesToKeep.push(this.enemies[i])
        }
      }
      
      this.bullets = bulletsAfterEnemyCollision
      this.enemies = enemiesToKeep
      
      // 敌人子弹与玩家碰撞
      const bulletsAfterPlayerCollision = []
      for (let i = 0; i < this.bullets.length; i++) {
        const bullet = this.bullets[i]
        if (bullet.type === 'enemy') {
          if (this.checkCollision(bullet, this.player)) {
            // 创建爆炸效果
            this.createExplosion(this.player.x + this.player.width / 2, this.player.y + this.player.height / 2)
            this.lives--
            if (this.lives <= 0) {
              this.gameOver = true
            }
          } else {
            bulletsAfterPlayerCollision.push(bullet)
          }
        } else {
          bulletsAfterPlayerCollision.push(bullet)
        }
      }
      
      this.bullets = bulletsAfterPlayerCollision
      
      // 玩家与敌人碰撞
      for (let i = 0; i < this.enemies.length; i++) {
        const enemy = this.enemies[i]
        if (this.checkCollision(this.player, enemy)) {
          // 创建爆炸效果
          this.createExplosion(this.player.x + this.player.width / 2, this.player.y + this.player.height / 2)
          this.createExplosion(enemy.x + enemy.width / 2, enemy.y + enemy.height / 2)
          this.lives--
          if (this.lives <= 0) {
            this.gameOver = true
          }
        }
        
        // 敌人与基地碰撞，游戏结束
        for (let j = 0; j < this.terrain.length; j++) {
          const tile = this.terrain[j]
          if (tile.type === 'base' && this.checkCollision(enemy, tile)) {
            this.createExplosion(tile.x + tile.width / 2, tile.y + tile.height / 2)
            this.gameOver = true
          }
        }
      }
    },
    checkCollision(a, b) {
      return a.x < b.x + b.width &&
             a.x + a.width > b.x &&
             a.y < b.y + b.height &&
             a.y + a.height > b.y
    },
    createExplosion(x, y) {
      this.explosions.push({
        x: x,
        y: y,
        size: 0,
        maxSize: 40,
        alpha: 1,
        startTime: Date.now()
      })
    },
    
    drawExplosions() {
      const explosionsToKeep = []
      for (let i = 0; i < this.explosions.length; i++) {
        const explosion = this.explosions[i]
        const elapsed = Date.now() - explosion.startTime
        const duration = 500
        
        if (elapsed > duration) {
          continue
        }
        
        const progress = elapsed / duration
        explosion.size = explosion.maxSize * progress
        explosion.alpha = 1 - progress
        
        // 简化的FC风格爆炸效果
        this.ctx.globalAlpha = explosion.alpha
        
        // 爆炸外圈（橙色）
        this.ctx.fillStyle = '#FF8C00'
        this.ctx.fillRect(explosion.x - explosion.size / 2, explosion.y - explosion.size / 2, explosion.size, explosion.size)
        
        // 爆炸中心（黄色）
        this.ctx.fillStyle = '#FFFF00'
        this.ctx.fillRect(explosion.x - explosion.size / 4, explosion.y - explosion.size / 4, explosion.size / 2, explosion.size / 2)
        
        // 爆炸核心（白色）
        this.ctx.fillStyle = '#FFFFFF'
        this.ctx.fillRect(explosion.x - explosion.size / 6, explosion.y - explosion.size / 6, explosion.size / 3, explosion.size / 3)
        
        this.ctx.globalAlpha = 1
        explosionsToKeep.push(explosion)
      }
      this.explosions = explosionsToKeep
    },
    
    checkGameOver() {
      if (this.lives <= 0) {
        this.gameOver = true
        clearInterval(this.gameLoop)
      }
      
      // 检查是否所有敌人都被消灭
      if (this.enemies.length === 0) {
        this.level++
        // 生成新的敌人
        const enemyTypes = ['normal', 'fast', 'smart']
        for (let i = 0; i < 5 + this.level; i++) {
          this.enemies.push({
            x: 100 + i * 100,
            y: 50,
            width: 30,
            height: 30,
            direction: 2,
            speed: 1 + this.level * 0.1,
            fireRate: 1000 - this.level * 50,
            lastFire: 0,
            type: enemyTypes[i % enemyTypes.length] // 循环分配敌人类型
          })
        }
        
        // 玩家坦克升级
        // 暂时注释掉，因为upgradePlayerTank函数不存在
        // this.upgradePlayerTank()
      }
    },
    restartGame() {
      clearInterval(this.gameLoop)
      this.initGame()
    },
    resetGameOnly() {
      this.gameStarted = false
      this.gamePaused = false
      clearInterval(this.gameLoop)
      this.initGame()
    },
    handleGameControl() {
      if (!this.gameStarted) {
        this.startGame()
      } else if (this.gamePaused) {
        this.startGame()
      } else {
        this.pauseGame()
      }
    },
    startGame() {
      if (!this.gameStarted) {
        this.gameStarted = true
        this.gamePaused = false
        this.initGame()
      } else if (this.gamePaused) {
        this.gamePaused = false
        this.gameLoop = setInterval(() => this.updateGame(), 20)
      }
    },
    pauseGame() {
      if (this.gameStarted && !this.gameOver && !this.gamePaused) {
        this.gamePaused = true
        clearInterval(this.gameLoop)
      }
    },
    cleanup() {
      clearInterval(this.gameLoop)
      if (this.moveInterval) {
        clearInterval(this.moveInterval)
      }
      window.removeEventListener('keydown', this.handleKeyDown)
      window.removeEventListener('keyup', this.handleKeyUp)
    },
    handleScreenTouch(e) {
      // 移动端点击屏幕任意位置（非按钮区域）开火
      if (this.isMobile && this.gameStarted && !this.gamePaused) {
        // 检查点击的是否是按钮元素
        const target = e.target
        if (!target.closest('button') && !target.closest('.joystick-container')) {
          this.fire()
        }
      }
    },
    startJoystick(e) {
      // 开始触摸摇杆
      e.stopPropagation()
      console.log('startJoystick called')
      this.joystickActive = true
      if (this.$refs.joystickBase) {
        const rect = this.$refs.joystickBase.getBoundingClientRect()
        this.joystickCenter = {
          x: rect.left + rect.width / 2,
          y: rect.top + rect.height / 2
        }
        console.log('Joystick center:', this.joystickCenter)
      }
      this.moveJoystick(e)
    },
    moveJoystick(e) {
      // 移动摇杆
      if (!this.joystickActive) return
      
      const touch = e.touches[0]
      if (!touch || !this.joystickCenter) return
      
      const deltaX = touch.clientX - this.joystickCenter.x
      const deltaY = touch.clientY - this.joystickCenter.y
      const distance = Math.sqrt(deltaX * deltaX + deltaY * deltaY)
      const maxDistance = this.joystickMaxDistance
      
      // 限制摇杆移动范围
      let moveX = deltaX
      let moveY = deltaY
      
      if (distance > maxDistance) {
        moveX = (deltaX / distance) * maxDistance
        moveY = (deltaY / distance) * maxDistance
      }
      
      // 更新摇杆位置
      if (this.$refs.joystickThumb) {
        this.$refs.joystickThumb.style.transform = `translate(${moveX}px, ${moveY}px)`
        console.log('Joystick position updated:', moveX, moveY)
      }
      
      // 根据摇杆位置更新玩家移动
      const normalizedX = moveX / maxDistance
      const normalizedY = moveY / maxDistance
      this.updatePlayerMovement(normalizedX, normalizedY)
    },
    stopJoystick() {
      // 停止触摸摇杆
      console.log('stopJoystick called')
      this.joystickActive = false
      
      // 确保摇杆回到中心位置
      if (this.$refs.joystickThumb) {
        console.log('Resetting joystick position')
        // 直接设置为初始位置
        this.$refs.joystickThumb.style.transform = 'translate(0, 0)'
        console.log('Joystick position reset to center')
      } else {
        console.log('joystickThumb ref not found')
      }
      
      // 停止玩家移动
      this.keys.up = false
      this.keys.down = false
      this.keys.left = false
      this.keys.right = false
      
      // 重置摇杆中心位置
      this.joystickCenter = null
    },
    updatePlayerMovement(x, y) {
      // 根据摇杆输入更新玩家移动
      if (!this.gameStarted || this.gamePaused) return
      
      // 重置所有方向键状态
      this.keys.up = false
      this.keys.down = false
      this.keys.left = false
      this.keys.right = false
      
      // 根据输入设置方向
      if (Math.abs(y) > Math.abs(x)) {
        // 垂直方向优先
        if (y < -0.3) {
          this.keys.up = true
          this.player.direction = 0
        } else if (y > 0.3) {
          this.keys.down = true
          this.player.direction = 2
        }
      } else {
        // 水平方向优先
        if (x < -0.3) {
          this.keys.left = true
          this.player.direction = 3
        } else if (x > 0.3) {
          this.keys.right = true
          this.player.direction = 1
        }
      }
    }
  }
}
</script>

<style scoped>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
  -webkit-touch-callout: none;
  -webkit-user-select: none;
  -khtml-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
  user-select: none;
  -webkit-user-drag: none;
  user-drag: none;
}

.tank-game {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 10px;
  background: #000000;
  color: white;
  overflow: hidden;
  position: relative;
  z-index: 1;
  /* FC风格背景网格 */
  background-image: 
    linear-gradient(rgba(255, 255, 255, 0.1) 1px, transparent 1px),
    linear-gradient(90deg, rgba(255, 255, 255, 0.1) 1px, transparent 1px);
  background-size: 20px 20px;
  touch-action: none;
  -webkit-touch-action: none;
  /* 锁定缩放 */
  -webkit-text-size-adjust: 100%;
  text-size-adjust: 100%;
  -webkit-overflow-scrolling: none;
  overscroll-behavior: none;
  -webkit-tap-highlight-color: transparent;
  /* 防止双击放大 */
  -webkit-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
  user-select: none;
  -webkit-user-drag: none;
  user-drag: none;
  -webkit-touch-callout: none;
}

.container {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 100%;
  max-width: 800px;
  overflow: hidden;
}

.title-container {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
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
  position: relative;
}

.game-info {
  display: flex;
  justify-content: space-between;
  width: 100%;
  max-width: 800px;
  margin-bottom: 10px;
  padding: 10px;
  background: rgba(0, 0, 0, 0.5);
  border: 1px solid #00ffaa;
  border-radius: 5px;
}

.lives, .score, .level {
  font-size: 18px;
  color: #00ffaa;
  font-weight: bold;
}

.game-canvas {
  border: 2px solid #00ffaa;
  border-radius: 8px;
  box-shadow: 0 0 30px rgba(0, 255, 170, 0.3);
  background: #000;
  touch-action: none;
  display: block;
  width: 100%;
  max-width: 520px;
  height: auto;
  aspect-ratio: 1 / 1;
}

.game-controls {
  margin: 20px 0;
  text-align: center;
}

.start-btn {
  padding: 12px 30px;
  font-size: 18px;
  font-weight: bold;
  background: linear-gradient(135deg, #ff6b6b, #ff8e53);
  color: white;
  border: none;
  border-radius: 25px;
  cursor: pointer;
  box-shadow: 0 5px 15px rgba(255, 107, 107, 0.3);
  transition: all 0.3s ease;
  touch-action: auto;
  -webkit-tap-highlight-color: transparent;
}

.start-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(255, 107, 107, 0.5);
}

.reset-btn {
  padding: 12px 30px;
  font-size: 18px;
  font-weight: bold;
  background: linear-gradient(135deg, #6b73ff, #00ccff);
  color: white;
  border: none;
  border-radius: 25px;
  cursor: pointer;
  box-shadow: 0 5px 15px rgba(107, 115, 255, 0.3);
  transition: all 0.3s ease;
  touch-action: auto;
  -webkit-tap-highlight-color: transparent;
  margin-left: 10px;
}

.reset-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(107, 115, 255, 0.5);
}

.mobile-controls {
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;
  max-width: 800px;
  margin-top: 20px;
  padding: 0 20px;
}

.joystick-container {
  position: relative;
  width: 200px;
  height: 200px;
  display: flex;
  justify-content: center;
  align-items: center;
}

.joystick-base {
  width: 120px;
  height: 120px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.2);
  border: 2px solid #00ffaa;
  position: relative;
  touch-action: none;
}

.joystick-thumb {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  background: rgba(0, 255, 170, 0.5);
  border: 2px solid #00ffaa;
  position: absolute;
  top: 30px;
  left: 30px;
  touch-action: none;
  transition: transform 0.1s ease;
}

.control-group {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.control-row {
  display: flex;
  gap: 10px;
  justify-content: center;
}

.control-btn {
  width: 90px;
  height: 90px;
  border-radius: 50%;
  border: 2px solid #00ffaa;
  background: rgba(0, 0, 0, 0.7);
  color: #00ffaa;
  font-size: 36px;
  font-weight: bold;
  cursor: pointer;
  touch-action: auto;
  -webkit-tap-highlight-color: transparent;
  transition: all 0.3s ease;
}

.control-btn:hover {
  background: rgba(0, 255, 170, 0.2);
  transform: scale(1.1);
}

.control-btn.fire {
  background: linear-gradient(135deg, #ff6b6b, #ff8e53);
  border-color: #ff6b6b;
  color: white;
}

.game-over-panel {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  background: rgba(0, 0, 0, 0.9);
  border: 2px solid #00ffaa;
  border-radius: 10px;
  padding: 40px;
  text-align: center;
  z-index: 1000;
  box-shadow: 0 0 50px rgba(0, 255, 170, 0.5);
}

.game-over-panel h2 {
  font-size: 36px;
  color: #ff6b6b;
  margin-bottom: 20px;
  text-shadow: 0 0 20px rgba(255, 107, 107, 0.5);
}

.game-over-panel p {
  font-size: 18px;
  color: #00ffaa;
  margin-bottom: 10px;
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
  color: white;
}

@media (max-width: 768px) {
  h1 {
    font-size: 24px;
  }
  
  .game-canvas {
    width: 100%;
    height: auto;
    max-width: 100%;
  }
  
  .game-info {
    font-size: 14px;
  }
  
  .control-btn {
    width: 100px;
    height: 100px;
    font-size: 40px;
  }
  
  .game-over-panel {
    padding: 20px;
    width: 90%;
  }
  
  .game-over-panel h2 {
    font-size: 28px;
  }
  
  .footer {
    flex-direction: column;
    align-items: center;
  }
  
  .footer .btn {
    width: 100%;
    margin: 5px 0;
  }
  
  /* 锁定移动设备的缩放 */
  body {
    touch-action: none;
    -webkit-touch-action: none;
    -webkit-text-size-adjust: 100%;
    text-size-adjust: 100%;
    -webkit-overflow-scrolling: none;
    overscroll-behavior: none;
  }
}
</style>