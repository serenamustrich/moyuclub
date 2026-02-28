<template>
  <div class="register">
    <div class="container">
      <h1>吃豆豆</h1>
      
      <h2>注册账号</h2>
      <div class="message" :class="messageType" v-if="message">{{ message }}</div>
      
      <div class="form-group">
        <label for="username">用户名:</label>
        <input type="text" id="username" v-model="registerForm.username" placeholder="请输入用户名">
      </div>
      
      <div class="form-group">
        <label for="account">账号:</label>
        <input type="text" id="account" v-model="registerForm.account" placeholder="请输入账号">
      </div>
      
      <div class="form-group">
        <label for="password">密码:</label>
        <input type="password" id="password" v-model="registerForm.password" placeholder="请输入密码">
      </div>
      
      <div class="form-group">
        <label for="securityQuestion">密保问题:</label>
        <input type="text" id="securityQuestion" v-model="registerForm.securityQuestion" placeholder="请输入密保问题">
      </div>
      
      <div class="form-group">
        <label for="securityAnswer">密保答案:</label>
        <input type="text" id="securityAnswer" v-model="registerForm.securityAnswer" placeholder="请输入密保答案">
      </div>
      
      <button class="btn" @click="doRegister">注册</button>
      
      <div class="links">
        <a href="#" @click.prevent="$router.push('/login')">已有账号？立即登录</a>
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
  name: 'Register',
  data() {
    return {
      message: '',
      messageType: '',
      registerForm: {
        username: '',
        account: '',
        password: '',
        securityQuestion: '',
        securityAnswer: ''
      }
    }
  },
  methods: {
    async doRegister() {
      if (!this.registerForm.username || !this.registerForm.account || !this.registerForm.password || !this.registerForm.securityQuestion || !this.registerForm.securityAnswer) {
        this.message = '请填写所有必填项'
        this.messageType = 'error'
        return
      }
      
      try {
        const response = await fetch('/api/auth/register', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify({
            username: this.registerForm.username,
            account: this.registerForm.account,
            password: this.registerForm.password,
            security_question: this.registerForm.securityQuestion,
            security_answer: this.registerForm.securityAnswer
          })
        })
        const data = await response.json()
        if (data.message === '注册成功') {
          this.message = '注册成功，请登录'
          this.messageType = 'success'
          setTimeout(() => {
            this.$router.push('/login')
          }, 1000)
        } else {
          this.message = '注册失败: ' + data.error
          this.messageType = 'error'
        }
      } catch (error) {
        console.error('注册失败:', error)
        this.message = '注册失败，请检查网络连接'
        this.messageType = 'error'
      }
    }
  }
}
</script>

<style scoped>
.register {
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