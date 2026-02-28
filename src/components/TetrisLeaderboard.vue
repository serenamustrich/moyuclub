<template>
  <div class="tetris-leaderboard">
    <div class="container">
      <h1>俄罗斯方块 - 排行榜</h1>
      
      <div class="header">
        <div class="user-info" v-if="currentUser">
          <div class="relative">
            <button class="btn" @click="toggleUserMenu" :title="currentUser.username">
              {{ truncateUsername(currentUser.username, 8) }}
            </button>
            <div class="user-menu" v-if="showUserMenu">
              <button @click="logout">退出登录</button>
              <button @click="$router.push('/login?action=change')">修改密码</button>
              <button @click="changeUsername">修改用户名</button>
            </div>
          </div>
        </div>
        <div class="user-info" v-else>
          <p>未登录</p>
        </div>
      </div>
      
      <div class="message info" v-if="!currentUser">
        如需将成绩计入排行榜，请先登录
      </div>
      
      <div class="message" :class="messageType" v-if="message">{{ message }}</div>
      
      <table class="leaderboard-table" v-if="!loading">
        <thead>
          <tr>
            <th>排名</th>
            <th>用户名</th>
            <th>分数</th>
            <th>设备</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="userScore" class="user-row">
            <td>{{ userRank }}</td>
            <td :title="currentUser.username">{{ truncateUsername(currentUser.username, 12) }} (你)</td>
            <td>{{ userScore.score }}</td>
            <td>{{ userScore.device_type === 'mobile' ? '手机' : '电脑' }}</td>
          </tr>
          <tr v-for="item in leaderboard" :key="item.rank">
            <td>{{ item.rank }}</td>
            <td :title="item.username">{{ truncateUsername(item.username, 12) }}</td>
            <td>{{ item.score }}</td>
            <td>{{ item.device_type === 'mobile' ? '手机' : '电脑' }}</td>
          </tr>
          <tr v-if="leaderboard.length === 0">
            <td colspan="4" style="text-align: center; padding: 20px;">暂无排行榜数据</td>
          </tr>
        </tbody>
      </table>
      
      <div class="loading" v-if="loading">
        <p>加载中...</p>
      </div>
      
      <div class="footer">
        <button class="btn" @click="$router.push('/')">返回首页</button>
        <button class="btn" @click="$router.push('/tetris')">开始游戏</button>
        <button class="btn btn-secondary" v-if="!currentUser" @click="$router.push('/login')">登录</button>
        <button class="btn btn-secondary" v-if="!currentUser" @click="$router.push('/register')">注册</button>
        <button class="btn btn-secondary" @click="$router.push('/leaderboard')">吃豆豆排行榜</button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'TetrisLeaderboard',
  data() {
    return {
      currentUser: null,
      showUserMenu: false,
      leaderboard: [],
      userScore: null,
      userRank: null,
      loading: false,
      message: '',
      messageType: ''
    }
  },
  mounted() {
    this.checkLoginStatus()
    this.fetchLeaderboard()
  },
  methods: {
    checkLoginStatus() {
      const user = localStorage.getItem('pacman_user')
      if (user) {
        this.currentUser = JSON.parse(user)
        this.fetchUserScore()
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
      this.fetchLeaderboard()
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
            // 重新获取排行榜数据
            this.fetchLeaderboard()
            this.fetchUserScore()
            alert('用户名修改成功')
          }
        } catch (error) {
          console.error('修改用户名失败:', error)
          alert('修改用户名失败，请稍后重试')
        }
      }
    },
    async fetchLeaderboard() {
      this.loading = true
      try {
        const response = await fetch('/api/tetris/scores?limit=20')
        const data = await response.json()
        this.leaderboard = data.scores || []
      } catch (error) {
        console.error('获取排行榜失败:', error)
        this.message = '获取排行榜失败，请刷新页面'
        this.messageType = 'error'
      } finally {
        this.loading = false
      }
    },
    async fetchUserScore() {
      if (this.currentUser) {
        try {
          const response = await fetch(`/api/tetris/scores/user/${this.currentUser.id}`)
          const data = await response.json()
          if (data.rank) {
            this.userRank = data.rank
            this.userScore = {
              score: data.score,
              device_type: data.device_type || 'desktop'
            }
          }
        } catch (error) {
          console.error('获取用户分数失败:', error)
        }
      }
    }
  }
}
</script>

<style scoped>
.tetris-leaderboard {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
}

.container {
  width: 100%;
  max-width: 800px;
  background: rgba(0, 0, 0, 0.7);
  border-radius: 15px;
  border: 2px solid #00ffaa;
  box-shadow: 0 0 30px rgba(0, 255, 170, 0.3);
  padding: 30px;
}

h1 {
  color: #00ffaa;
  text-shadow: 0 0 20px rgba(0, 255, 170, 0.5);
  font-size: 32px;
  margin-bottom: 30px;
  text-align: center;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
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

.leaderboard-table {
  width: 100%;
  border-collapse: collapse;
  margin-bottom: 30px;
}

.leaderboard-table th,
.leaderboard-table td {
  padding: 12px;
  text-align: left;
  border-bottom: 1px solid #333;
}

.leaderboard-table th {
  background-color: rgba(0, 255, 170, 0.2);
  color: #00ffaa;
  font-weight: bold;
}

.leaderboard-table tr:hover {
  background-color: rgba(0, 255, 170, 0.1);
}

.user-row {
  background-color: rgba(0, 255, 170, 0.2) !important;
}

.footer {
  display: flex;
  justify-content: center;
  gap: 20px;
  margin-top: 30px;
  flex-wrap: wrap;
}

.loading {
  text-align: center;
  padding: 20px;
  font-size: 18px;
  color: #00ffaa;
}

.message {
  padding: 10px;
  margin-bottom: 20px;
  border-radius: 5px;
  text-align: center;
}

.message.error {
  background-color: rgba(255, 107, 107, 0.2);
  color: #ff6b6b;
  border: 1px solid #ff6b6b;
}

.message.info {
  background-color: rgba(0, 255, 170, 0.2);
  color: #00ffaa;
  border: 1px solid #00ffaa;
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
  .container {
    padding: 20px;
  }
  
  h1 {
    font-size: 24px;
  }
  
  .leaderboard-table th,
  .leaderboard-table td {
    padding: 8px;
    font-size: 14px;
  }
  
  .header {
    flex-direction: column;
    gap: 10px;
    align-items: stretch;
  }
  
  .user-info {
    justify-content: center;
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