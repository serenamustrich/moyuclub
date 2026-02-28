<template>
  <div class="tank-game">
    <canvas ref="gameCanvas" class="game-canvas" width="520" height="520"></canvas>
    
    <div class="game-info">
      <span>分数: {{ score }}</span>
      <span>生命: {{ lives }}</span>
      <span>等级: {{ level }}</span>
    </div>
    
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
          <div class="joystick-thumb" ref="joystickThumb"></div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'TankGame',
  data() {
    return {
      canvas: null,
      ctx: null,
      gameLoop: null,
      gameStarted: false,
      gamePaused: false,
      gameOver: false,
      score: 0,
      lives: 3,
      level: 1,
      player: null,
      enemies: [],
      bullets: [],
      explosions: [],
      terrain: [],
      keys: {
        up: false,
        down: false,
        left: false,
        right: false,
        space: false
      },
      isMobile: false,
      moveInterval: null,
      joystickActive: false,
      joystickCenter: { x: 0, y: 0 },
      joystickRadius: 60,
      joystickMaxDistance: 40,
      mapSeed: 0
    }
  },
  mounted() {
    this.checkMobile()
    this.initGame()
    this.setupEventListeners()
  },
  beforeUnmount() {
    this.cleanup()
  },
  methods: {
    initGame() {
      this.canvas = this.$refs.gameCanvas
      this.ctx = this.canvas.getContext('2d')
      this.resetGame()
      this.drawInitialScreen()
      if (this.gameStarted) {
        this.gameLoop = setInterval(() => this.updateGame(), 16)
      }
    },
    drawInitialScreen() {
      this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height)
      this.ctx.fillStyle = '#000000'
      this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height)
      this.drawTerrain()
    },
    resetGame() {
      this.mapSeed = Math.floor(Math.random() * 10000)
      this.lives = 3
      this.score = 0
      this.level = 1
      this.gameOver = false
      
      this.player = {
        x: 168,
        y: 456,
        width: 24,
        height: 24,
        direction: 0,
        speed: 2,
        fireRate: 200,
        lastFire: 0
      }
      
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
          direction: 2,
          speed: 1,
          fireRate: 1000,
          lastFire: 0,
          type: enemyTypes[index % enemyTypes.length]
        })
      })
      
      this.terrain = []
      const gridSize = 21
      const cellSize = 24
      
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
            if (terrainType === 'brick') {
              terrain.health = 3
            }
            this.terrain.push(terrain)
          }
        }
      }
      
      this.bullets = []
      this.explosions = []
    },
    generateTerrain(x, y) {
      if (x === 10 && y === 20) return 'base'
      if (x === 10 && y === 19) return 'floor'
      if ((x >= 9 && x <= 11) && (y === 18)) return 'brick'
      if (x === 9 && (y >= 18 && y <= 20)) return 'brick'
      if (x === 11 && (y >= 18 && y <= 20)) return 'brick'
      if ((x >= 9 && x <= 11) && (y === 21)) return 'brick'
      if (y < 3) return 'floor'
      if (y > 17) return 'floor'
      if ((x === 10 || x === 11) && (y >= 5 && y <= 13)) return 'floor'
      if ((x === 3 || x === 4) && (y >= 6 && y <= 11)) return 'floor'
      if ((x === 16 || x === 17) && (y >= 6 && y <= 11)) return 'floor'
      
      const seed = (x * 7 + y * 13 + this.mapSeed) % 100
      
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
      
      if ((x >= 6 && x <= 14) && (y >= 6 && y <= 8)) {
        if (seed > 60 && seed < 90) return 'steel'
      }
      if ((x >= 6 && x <= 14) && (y >= 10 && y <= 12)) {
        if (seed > 60 && seed < 90) return 'steel'
      }
      
      if ((x >= 1 && x <= 5) && (y >= 9 && y <= 10)) {
        if (seed < 40) return 'water'
      }
      if ((x >= 15 && x <= 19) && (y >= 9 && y <= 10)) {
        if (seed < 40) return 'water'
      }
      
      if ((x >= 5 && x <= 15) && (y >= 7 && y <= 11)) {
        if (seed < 50) return 'forest'
      }
      
      return 'floor'
    },
    setupEventListeners() {
      window.addEventListener('keydown', (e) => {
        switch (e.key) {
          case 'ArrowUp': this.keys.up = true; break
          case 'ArrowLeft': this.keys.left = true; break
          case 'ArrowDown': this.keys.down = true; break
          case 'ArrowRight': this.keys.right = true; break
          case ' ': this.keys.space = true; this.fire(); break
        }
      })
      window.addEventListener('keyup', (e) => {
        switch (e.key) {
          case 'ArrowUp': this.keys.up = false; break
          case 'ArrowLeft': this.keys.left = false; break
          case 'ArrowDown': this.keys.down = false; break
          case 'ArrowRight': this.keys.right = false; break
          case ' ': this.keys.space = false; break
        }
      })
      
      if (this.isMobile) {
        this.setupJoystick()
      }
    },
    setupJoystick() {
      const base = this.$refs.joystickBase
      if (!base) return
      
      base.addEventListener('touchstart', (e) => {
        e.preventDefault()
        this.joystickActive = true
        const touch = e.touches[0]
        const rect = base.getBoundingClientRect()
        this.joystickCenter = {
          x: rect.left + rect.width / 2,
          y: rect.top + rect.height / 2
        }
      })
      
      base.addEventListener('touchmove', (e) => {
        e.preventDefault()
        if (!this.joystickActive) return
        
        const touch = e.touches[0]
        let dx = touch.clientX - this.joystickCenter.x
        let dy = touch.clientY - this.joystickCenter.y
        const distance = Math.sqrt(dx * dx + dy * dy)
        
        if (distance > this.joystickMaxDistance) {
          dx = (dx / distance) * this.joystickMaxDistance
          dy = (dy / distance) * this.joystickMaxDistance
        }
        
        this.$refs.joystickThumb.style.transform = `translate(${dx}px, ${dy}px)`
        
        this.keys.up = dy < -10
        this.keys.down = dy > 10
        this.keys.left = dx < -10
        this.keys.right = dx > 10
      })
      
      base.addEventListener('touchend', () => {
        this.joystickActive = false
        this.$refs.joystickThumb.style.transform = 'translate(0, 0)'
        this.keys = { up: false, down: false, left: false, right: false, space: false }
      })
    },
    updateGame() {
      if (this.gameOver || !this.gameStarted || this.gamePaused) return
      
      this.movePlayer()
      this.moveEnemies()
      this.moveBullets()
      this.enemiesFire()
      this.checkCollisions()
      this.checkGameOver()
      
      this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height)
      this.ctx.fillStyle = '#000000'
      this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height)
      
      this.drawTerrain()
      this.drawPlayer()
      this.drawEnemies()
      this.drawBullets()
      this.drawExplosions()
      
      if (this.gameOver) {
        this.drawGameOver()
      }
    },
    movePlayer() {
      if (!this.player) return
      
      let newX = this.player.x
      let newY = this.player.y
      
      if (this.keys.up) { newY -= this.player.speed; this.player.direction = 0 }
      if (this.keys.down) { newY += this.player.speed; this.player.direction = 2 }
      if (this.keys.left) { newX -= this.player.speed; this.player.direction = 3 }
      if (this.keys.right) { newX += this.player.speed; this.player.direction = 1 }
      
      if (!this.checkTerrainCollision(newX, newY, this.player.width, this.player.height)) {
        this.player.x = newX
        this.player.y = newY
      }
      
      if (this.keys.space) {
        this.fire()
      }
    },
    fire() {
      if (!this.player) return
      const now = Date.now()
      if (now - this.player.lastFire < this.player.fireRate) return
      
      this.player.lastFire = now
      
      let bulletX, bulletY, bulletDirection
      switch (this.player.direction) {
        case 0: bulletX = this.player.x + this.player.width / 2 - 2; bulletY = this.player.y - 10; break
        case 1: bulletX = this.player.x + this.player.width + 2; bulletY = this.player.y + this.player.height / 2 - 2; break
        case 2: bulletX = this.player.x + this.player.width / 2 - 2; bulletY = this.player.y + this.player.height + 2; break
        case 3: bulletX = this.player.x - 10; bulletY = this.player.y + this.player.height / 2 - 2; break
      }
      
      this.bullets.push({
        x: bulletX,
        y: bulletY,
        width: 4,
        height: 4,
        direction: this.player.direction,
        speed: 6,
        isEnemy: false
      })
    },
    checkTerrainCollision(x, y, width, height) {
      const playerRect = { x, y, width, height }
      for (const tile of this.terrain) {
        if (tile.type === 'forest' || tile.type === 'water') continue
        if (this.checkCollision(playerRect, tile)) return true
      }
      return false
    },
    checkCollision(rect1, rect2) {
      return rect1.x < rect2.x + rect2.width &&
             rect1.x + rect1.width > rect2.x &&
             rect1.y < rect2.y + rect2.height &&
             rect1.y + rect1.height > rect2.y
    },
    drawTerrain() {
      for (const tile of this.terrain) {
        switch (tile.type) {
          case 'brick':
            this.ctx.fillStyle = '#FF8C00'
            this.ctx.fillRect(tile.x, tile.y, tile.width, tile.height)
            this.ctx.fillStyle = '#FF4500'
            this.ctx.fillRect(tile.x + 4, tile.y + 4, tile.width - 8, tile.height - 8)
            break
          case 'steel':
            this.ctx.fillStyle = '#C0C0C0'
            this.ctx.fillRect(tile.x, tile.y, tile.width, tile.height)
            this.ctx.strokeStyle = '#808080'
            this.ctx.lineWidth = 1
            for (let i = 4; i < 24; i += 4) {
              this.ctx.beginPath()
              this.ctx.moveTo(tile.x + i, tile.y)
              this.ctx.lineTo(tile.x + i, tile.y + tile.height)
              this.ctx.stroke()
            }
            break
          case 'water':
            this.ctx.fillStyle = '#0000FF'
            this.ctx.fillRect(tile.x, tile.y, tile.width, tile.height)
            break
          case 'forest':
            this.ctx.fillStyle = '#008000'
            this.ctx.fillRect(tile.x, tile.y, tile.width, tile.height)
            break
          case 'base':
            this.ctx.fillStyle = '#FFFF00'
            this.ctx.fillRect(tile.x, tile.y, tile.width, tile.height)
            this.ctx.fillStyle = '#FFA500'
            this.ctx.fillRect(tile.x + 6, tile.y + 3, 8, 14)
            this.ctx.fillRect(tile.x + 3, tile.y + 6, 14, 8)
            break
        }
      }
    },
    drawPlayer() {
      if (!this.player) return
      this.ctx.fillStyle = '#00FF00'
      this.ctx.fillRect(this.player.x, this.player.y, this.player.width, this.player.height)
      this.ctx.fillStyle = '#008800'
      const directions = [
        { x: 8, y: -6, w: 8, h: 6 },
        { x: 22, y: 8, w: 6, h: 8 },
        { x: 8, y: 22, w: 8, h: 6 },
        { x: -6, y: 8, w: 6, h: 8 }
      ]
      const d = directions[this.player.direction]
      this.ctx.fillRect(this.player.x + d.x, this.player.y + d.y, d.w, d.h)
    },
    drawEnemies() {
      for (const enemy of this.enemies) {
        const colors = { normal: '#FF0000', fast: '#FF00FF', smart: '#00FFFF' }
        this.ctx.fillStyle = colors[enemy.type] || '#FF0000'
        this.ctx.fillRect(enemy.x, enemy.y, enemy.width, enemy.height)
      }
    },
    drawBullets() {
      for (const bullet of this.bullets) {
        this.ctx.fillStyle = bullet.isEnemy ? '#FF0000' : '#FFFF00'
        this.ctx.fillRect(bullet.x, bullet.y, bullet.width, bullet.height)
      }
    },
    drawExplosions() {
      const explosionsToKeep = []
      for (const explosion of this.explosions) {
        const age = Date.now() - explosion.startTime
        if (age > 500) continue
        explosionsToKeep.push(explosion)
        
        this.ctx.fillStyle = `rgba(255, 100, 0, ${1 - age / 500})`
        this.ctx.beginPath()
        this.ctx.arc(explosion.x, explosion.y, explosion.radius * (1 + age / 500), 0, Math.PI * 2)
        this.ctx.fill()
      }
      this.explosions = explosionsToKeep
    },
    createExplosion(x, y) {
      this.explosions.push({ x, y, radius: 10, alpha: 1, startTime: Date.now() })
    },
    moveEnemies() {},
    moveBullets() {
      const bulletsToKeep = []
      for (const bullet of this.bullets) {
        switch (bullet.direction) {
          case 0: bullet.y -= bullet.speed; break
          case 1: bullet.x += bullet.speed; break
          case 2: bullet.y += bullet.speed; break
          case 3: bullet.x -= bullet.speed; break
        }
        
        if (bullet.x < 0 || bullet.x > this.canvas.width || bullet.y < 0 || bullet.y > this.canvas.height) {
          continue
        }
        bulletsToKeep.push(bullet)
      }
      this.bullets = bulletsToKeep
    },
    enemiesFire() {},
    checkCollisions() {
      let terrainToKeep = [...this.terrain]
      const bulletsToKeep = []
      
      for (const bullet of this.bullets) {
        let hit = false
        for (let j = 0; j < terrainToKeep.length; j++) {
          const tile = terrainToKeep[j]
          if (this.checkCollision(bullet, tile)) {
            if (tile.type === 'forest' || tile.type === 'water') continue
            hit = true
            if (tile.type === 'brick') {
              tile.health--
              this.createExplosion(tile.x + tile.width / 2, tile.y + tile.height / 2)
              if (tile.health <= 0) {
                terrainToKeep.splice(j, 1)
                j--
              }
            }
            if (tile.type === 'base') {
              this.createExplosion(tile.x + tile.width / 2, tile.y + tile.height / 2)
              this.gameOver = true
            }
            break
          }
        }
        if (!hit) bulletsToKeep.push(bullet)
      }
      
      this.terrain = terrainToKeep
      this.bullets = bulletsToKeep
    },
    checkGameOver() {
      if (this.lives <= 0) {
        this.gameOver = true
        clearInterval(this.gameLoop)
      }
      if (this.enemies.length === 0) {
        this.level++
      }
    },
    drawGameOver() {
      this.ctx.fillStyle = 'rgba(0, 0, 0, 0.7)'
      this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height)
      this.ctx.fillStyle = '#FF0000'
      this.ctx.font = '48px Arial'
      this.ctx.textAlign = 'center'
      this.ctx.fillText('游戏结束', this.canvas.width / 2, this.canvas.height / 2)
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
      if (this.moveInterval) clearInterval(this.moveInterval)
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
    checkMobile() {
      this.isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)
    }
  }
}
</script>

<style scoped>
.tank-game {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 20px;
}

.game-canvas {
  border: 3px solid #333;
  border-radius: 8px;
  background: #000;
}

.game-info {
  display: flex;
  gap: 20px;
  margin: 15px 0;
  font-size: 18px;
  font-weight: bold;
  color: #ffcc00;
}

.game-controls {
  display: flex;
  gap: 10px;
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
}

.mobile-controls {
  margin-top: 20px;
}

.joystick-container {
  display: flex;
  justify-content: center;
}

.joystick-base {
  width: 120px;
  height: 120px;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.joystick-thumb {
  width: 50px;
  height: 50px;
  background: rgba(255, 255, 255, 0.8);
  border-radius: 50%;
  transition: transform 0.1s;
}
</style>
