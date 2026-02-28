<template>
  <div class="index">
    <div class="container">
      <h1>小财神摸鱼club</h1>
      
      <div class="header">
        <div class="user-info" v-if="currentUser">
          <div class="relative">
            <button class="btn" @click="toggleUserMenu">
              {{ currentUser.username }}
            </button>
            <div class="user-menu" v-if="showUserMenu">
              <button @click="logout">退出登录</button>
              <button @click="$router.push('/login?action=change')">修改密码</button>
            </div>
          </div>
        </div>
      </div>
      
      <div class="hero">
        <div class="game-entries">
          <div class="game-card pacman" @click="$router.push('/leaderboard')" style="cursor: pointer;">
            <h2>吃豆豆</h2>
            <div class="game-icon">
              <div class="pacman-icon"></div>
            </div>
            <p>经典吃豆豆游戏，挑战你的反应能力</p>
          </div>
          
          <div class="game-card tetris" @click="$router.push('/tetris-leaderboard')" style="cursor: pointer;">
            <h2>俄罗斯方块</h2>
            <div class="game-icon">
              <div class="tetris-icon"></div>
            </div>
            <p>经典俄罗斯方块游戏，挑战你的策略能力</p>
          </div>
          
          <div class="game-card mbti" @click="$router.push('/mbti-test')" style="cursor: pointer;">
            <h2>MBTI 测试</h2>
            <div class="game-icon">
              <div class="mbti-icon"></div>
            </div>
            <p>了解你的人格类型，探索自我</p>
          </div>
          
          <div class="game-card tank" @click="$router.push('/tank-game')" style="cursor: pointer;">
            <h2>坦克大战</h2>
            <div class="game-icon">
              <div class="tank-icon"></div>
            </div>
            <p>经典坦克大战，保护你的基地</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'Index',
  data() {
    return {
      currentUser: null,
      showUserMenu: false
    }
  },
  mounted() {
    this.checkLoginStatus()
    window.addEventListener('click', this.handleClickOutside)
  },
  beforeUnmount() {
    window.removeEventListener('click', this.handleClickOutside)
  },
  methods: {
    checkLoginStatus() {
      const user = localStorage.getItem('pacman_user')
      if (user) {
        this.currentUser = JSON.parse(user)
      }
    },
    toggleUserMenu() {
      this.showUserMenu = !this.showUserMenu
    },
    handleClickOutside(e) {
      if (!e.target.closest('.relative')) {
        this.showUserMenu = false
      }
    },
    logout() {
      localStorage.removeItem('pacman_user')
      this.currentUser = null
      this.showUserMenu = false
    }
  }
}
</script>

<style scoped>
.index {
  min-height: 100vh;
  padding: 20px;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
}

h1 {
  text-align: center;
  font-size: 48px;
  margin-bottom: 40px;
  background: linear-gradient(135deg, #ffcc00, #ff6b6b);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.header {
  display: flex;
  justify-content: flex-end;
  margin-bottom: 20px;
}

.relative {
  position: relative;
}

.user-menu {
  position: absolute;
  top: 100%;
  right: 0;
  background: #1a1a2e;
  border: 1px solid #333;
  border-radius: 8px;
  padding: 10px;
  z-index: 100;
}

.user-menu button {
  display: block;
  width: 100%;
  padding: 8px 16px;
  background: transparent;
  border: none;
  color: #fff;
  cursor: pointer;
  text-align: left;
}

.user-menu button:hover {
  background: #333;
}

.hero {
  display: flex;
  justify-content: center;
}

.game-entries {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 30px;
  width: 100%;
}

.game-card {
  background: linear-gradient(135deg, #1a1a2e 0%, #16213e 100%);
  border-radius: 20px;
  padding: 30px;
  text-align: center;
  transition: all 0.3s ease;
  border: 2px solid transparent;
}

.game-card:hover {
  transform: translateY(-10px);
  border-color: #ffcc00;
  box-shadow: 0 10px 30px rgba(255, 204, 0, 0.3);
}

.game-card h2 {
  font-size: 24px;
  margin-bottom: 15px;
  color: #ffcc00;
}

.game-icon {
  width: 80px;
  height: 80px;
  margin: 0 auto 15px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.pacman-icon {
  width: 60px;
  height: 60px;
  background: #ffcc00;
  border-radius: 50%;
  position: relative;
}

.pacman-icon::before {
  content: '';
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 0;
  height: 0;
  border-top: 15px solid transparent;
  border-bottom: 15px solid transparent;
  border-left: 25px solid #1a1a2e;
}

.tetris-icon {
  width: 50px;
  height: 50px;
  background: linear-gradient(135deg, #00ccff, #6b73ff);
  transform: rotate(45deg);
}

.mbti-icon {
  width: 60px;
  height: 60px;
  background: linear-gradient(135deg, #ff6b6b, #ffcc00);
  border-radius: 10px;
}

.tank-icon {
  width: 60px;
  height: 40px;
  background: #4CAF50;
  border-radius: 5px;
  position: relative;
}

.tank-icon::before {
  content: '';
  position: absolute;
  top: -10px;
  left: 50%;
  transform: translateX(-50%);
  width: 15px;
  height: 15px;
  background: #4CAF50;
  border-radius: 50%;
}

.game-card p {
  color: #aaa;
  font-size: 14px;
}
</style>
