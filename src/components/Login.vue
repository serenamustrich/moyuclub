<template>
  <div class="login">
    <div class="container">
      <h1>吃豆豆</h1>
      
      <div v-if="activeForm === 'login'">
        <h2>登录</h2>
        <div class="message" :class="messageType" v-if="message">{{ message }}</div>
        
        <div class="form-group">
          <label for="account">账号:</label>
          <input type="text" id="account" v-model="loginForm.account" placeholder="请输入账号">
        </div>
        
        <div class="form-group">
          <label for="password">密码:</label>
          <input type="password" id="password" v-model="loginForm.password" placeholder="请输入密码">
        </div>
        
        <button class="btn" @click="doLogin">登录</button>
        
        <div class="links">
          <a href="#" @click.prevent="activeForm = 'reset'">忘记密码?</a>
          <a href="#" @click.prevent="$router.push('/register')">注册账号</a>
        </div>
      </div>
      
      <div v-if="activeForm === 'reset'">
        <h2>找回密码</h2>
        <div class="message" :class="messageType" v-if="message">{{ message }}</div>
        
        <div class="form-group">
          <label for="resetAccount">账号:</label>
          <input type="text" id="resetAccount" v-model="resetForm.account" placeholder="请输入账号">
        </div>
        
        <div class="form-group">
          <label for="resetSecurityAnswer">密保答案:</label>
          <input type="text" id="resetSecurityAnswer" v-model="resetForm.securityAnswer" placeholder="请输入密保答案">
        </div>
        
        <div class="form-group">
          <label for="resetNewPassword">新密码:</label>
          <input type="password" id="resetNewPassword" v-model="resetForm.newPassword" placeholder="请输入新密码">
        </div>
        
        <button class="btn" @click="doResetPassword">重置密码</button>
        <button class="btn btn-secondary" @click="activeForm = 'login'">返回登录</button>
      </div>
      
      <div v-if="activeForm === 'change'">
        <h2>修改密码</h2>
        <div class="message" :class="messageType" v-if="message">{{ message }}</div>
        
        <div class="form-group">
          <label for="changeAccount">账号:</label>
          <input type="text" id="changeAccount" v-model="changeForm.account" disabled>
        </div>
        
        <div class="form-group">
          <label for="changeOldPassword">旧密码:</label>
          <input type="password" id="changeOldPassword" v-model="changeForm.oldPassword" placeholder="请输入旧密码">
        </div>
        
        <div class="form-group">
          <label for="changeNewPassword">新密码:</label>
          <input type="password" id="changeNewPassword" v-model="changeForm.newPassword" placeholder="请输入新密码">
        </div>
        
        <button class="btn" @click="doChangePassword">修改密码</button>
        <button class="btn btn-secondary" @click="$router.push('/leaderboard')">返回排行榜</button>
      </div>
      
      <div class="footer">
        <button class="btn btn-secondary" @click="$router.push('/game')">返回游戏</button>
        <button class="btn btn-secondary" @click="$router.push('/leaderboard')">查看排行榜</button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'Login',
  data() {
    return {
      activeForm: 'login',
      message: '',
      messageType: '',
      loginForm: {
        account: '',
        password: ''
      },
      resetForm: {
        account: '',
        securityAnswer: '',
        newPassword: ''
      },
      changeForm: {
        account: '',
        oldPassword: '',
        newPassword: ''
      },
      currentUser: null
    }
  },
  mounted() {
    this.checkLoginStatus()
    this.checkQueryParams()
  },
  methods: {
    checkLoginStatus() {
      const user = localStorage.getItem('pacman_user')
      if (user) {
        this.currentUser = JSON.parse(user)
        this.changeForm.account = this.currentUser.account
      }
    },
    checkQueryParams() {
      const query = this.$route.query
      if (query.action === 'change') {
        this.activeForm = 'change'
      }
    },
    async doLogin() {
      if (!this.loginForm.account || !this.loginForm.password) {
        this.message = '请输入账号和密码'
        this.messageType = 'error'
        return
      }
      
      try {
        const response = await fetch('/api/auth/login', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify(this.loginForm)
        })
        const data = await response.json()
        if (data.user) {
          localStorage.setItem('pacman_user', JSON.stringify(data.user))
          this.message = '登录成功'
          this.messageType = 'success'
          setTimeout(() => {
            this.$router.push('/leaderboard')
          }, 1000)
        } else {
          this.message = '登录失败: ' + data.error
          this.messageType = 'error'
        }
      } catch (error) {
        console.error('登录失败:', error)
        this.message = '登录失败，请检查网络连接'
        this.messageType = 'error'
      }
    },
    async doResetPassword() {
      if (!this.resetForm.account || !this.resetForm.securityAnswer || !this.resetForm.newPassword) {
        this.message = '请填写所有必填项'
        this.messageType = 'error'
        return
      }
      
      try {
        const response = await fetch('/api/auth/reset_password', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify({
            account: this.resetForm.account,
            security_answer: this.resetForm.securityAnswer,
            new_password: this.resetForm.newPassword
          })
        })
        const data = await response.json()
        if (data.message === '密码重置成功') {
          this.message = '密码重置成功，请登录'
          this.messageType = 'success'
          setTimeout(() => {
            this.activeForm = 'login'
          }, 1000)
        } else {
          this.message = '重置密码失败: ' + data.error
          this.messageType = 'error'
        }
      } catch (error) {
        console.error('重置密码失败:', error)
        this.message = '重置密码失败，请检查网络连接'
        this.messageType = 'error'
      }
    },
    async doChangePassword() {
      if (!this.changeForm.account || !this.changeForm.oldPassword || !this.changeForm.newPassword) {
        this.message = '请填写所有必填项'
        this.messageType = 'error'
        return
      }
      
      // 这里可以添加修改密码的API调用
      this.message = '修改密码功能需要后端支持，当前仅为演示'
      this.messageType = 'error'
    }
  }
}
</script>

<style scoped>
.login {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
}

.container {
  width: 100%;
  max-width: 400px;
  background: rgba(0, 0, 0, 0.7);
  border-radius: 15px;
  border: 2px solid #ffcc00;
  box-shadow: 0 0 30px rgba(255, 204, 0, 0.3);
  padding: 30px;
}

h1 {
  color: #ffcc00;
  text-shadow: 0 0 20px rgba(255, 204, 0, 0.5);
  font-size: 28px;
  margin-bottom: 30px;
  text-align: center;
}

h2 {
  color: #ffcc00;
  font-size: 20px;
  margin-bottom: 20px;
  text-align: center;
}

.form-group {
  margin-bottom: 20px;
}

label {
  display: block;
  margin-bottom: 8px;
  color: #fff;
  font-weight: bold;
}

input[type="text"],
input[type="password"] {
  width: 100%;
  padding: 12px;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid #333;
  border-radius: 5px;
  color: #fff;
  font-size: 16px;
}

input[type="text"]:focus,
input[type="password"]:focus {
  outline: none;
  border-color: #ffcc00;
  box-shadow: 0 0 10px rgba(255, 204, 0, 0.3);
}

.links {
  text-align: center;
  margin-top: 20px;
}

.links a {
  color: #ffcc00;
  text-decoration: none;
  margin: 0 10px;
}

.links a:hover {
  text-decoration: underline;
}

.footer {
  margin-top: 30px;
  display: flex;
  justify-content: center;
  gap: 20px;
  flex-wrap: wrap;
}

@media (max-width: 480px) {
  .container {
    padding: 20px;
  }
  
  h1 {
    font-size: 24px;
  }
  
  h2 {
    font-size: 18px;
  }
  
  input[type="text"],
  input[type="password"] {
    padding: 10px;
    font-size: 14px;
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