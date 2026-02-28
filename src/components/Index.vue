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
            <p>经典坦克大战游戏，挑战你的策略能力</p>
          </div>
          
          <div class="game-card bomberman" @click="$router.push('/bomberman-game')" style="cursor: pointer;">
            <h2>炸弹人</h2>
            <div class="game-icon">
              <div class="bomberman-icon"></div>
            </div>
            <p>经典炸弹人游戏，挑战你的策略能力</p>
          </div>

        </div>
      </div>
      
      <section id="about">
        <div class="about-content">
          <div class="about-text" style="text-align: center;">
            <h2>关于我</h2>
            <p>一个很能折腾的产品经理。</p>
          </div>
        </div>
      </section>
      
      <section id="contact">
        <h2>联系方式</h2>
        <div class="contact-content">
          <p>想吃屁并没有任何联系方式，就一个美国号码确实是我的想存就存</p>
          <div class="contact-info">
            <div class="contact-item">
              <h3>邮箱</h3>
              <p>懂得都懂</p>
            </div>
            <div class="contact-item">
              <h3>电话</h3>
              <p>+1 4245974563</p>
            </div>
            <div class="contact-item">
              <h3>社交媒体</h3>
              <p>不该问的少问</p>
            </div>
          </div>
        </div>
      </section>
      
      <div class="footer">
        <button class="btn btn-secondary" v-if="!currentUser" @click="$router.push('/login')">登录</button>
        <button class="btn btn-secondary" v-if="!currentUser" @click="$router.push('/register')">注册</button>
        <p>&copy; 2026 AAA小财神来咯来咯左手抓金元宝右手抱聚宝盆脚踩祥云头顶福数钱数到手抽筋咯什么手抽筋了要不要我给你按。保留所有权利</p>
        <a style="color: white;" href="https://beian.miit.gov.cn/" target="_blank">粤ICP备2026017648</a>
        <a style="color: white;" href="https://beian.mps.gov.cn/#/query/webSearch?code=44030002010783" rel="noreferrer" target="_blank"><img :src="beianImageUrl" width="15" height="15">粤公网安备44030002010783号</a>
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
      showUserMenu: false,
      beianImageUrl: '/beian.png'
    }
  },
  mounted() {
    this.checkLoginStatus()
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
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
}

.container {
  width: 100%;
  max-width: 100%;
  background: rgba(0, 0, 0, 0.7);
  border-radius: 15px;
  border: 2px solid #ff6b6b;
  box-shadow: 0 0 30px rgba(255, 107, 107, 0.3);
  padding: 50px;
}

h1 {
  color: #ff6b6b;
  text-shadow: 0 0 20px rgba(255, 107, 107, 0.5);
  font-size: 48px;
  margin-bottom: 60px;
  text-align: center;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 60px;
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
  border: 1px solid #ff6b6b;
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
  background-color: rgba(255, 107, 107, 0.2);
}

.hero {
  margin-bottom: 80px;
}

.game-entries {
  display: flex;
  gap: 50px;
  margin-bottom: 60px;
  justify-content: center;
  flex-wrap: wrap;
}

.game-card {
  flex: 1;
  min-width: 300px;
  max-width: 350px;
  background: rgba(0, 0, 0, 0.5);
  border: 2px solid #333;
  border-radius: 20px;
  padding: 40px;
  text-align: center;
  transition: all 0.3s ease;
  cursor: pointer;
}

.game-card:hover {
  transform: translateY(-15px);
  box-shadow: 0 15px 30px rgba(0, 0, 0, 0.3);
}

.game-card.pacman {
  border-color: #ffcc00;
}

.game-card.pacman:hover {
  border-color: #ffdd33;
  box-shadow: 0 10px 20px rgba(255, 204, 0, 0.3);
}

.game-card.tetris {
  border-color: #00ccff;
}

.game-card.tetris:hover {
  border-color: #33ddff;
  box-shadow: 0 10px 20px rgba(0, 204, 255, 0.3);
}

.game-card.mbti {
  border-color: #9c27b0;
}

.game-card.mbti:hover {
  border-color: #e040fb;
  box-shadow: 0 10px 20px rgba(156, 39, 176, 0.3);
}

.game-card.tank {
  border-color: #4caf50;
}

.game-card.tank:hover {
  border-color: #81c784;
  box-shadow: 0 10px 20px rgba(76, 175, 80, 0.3);
}

.game-card.bomberman {
  border-color: #ff9800;
}

.game-card.bomberman:hover {
  border-color: #ffb74d;
  box-shadow: 0 10px 20px rgba(255, 152, 0, 0.3);
}



.game-card h2 {
  font-size: 28px;
  margin-bottom: 30px;
}

.game-card.pacman h2 {
  color: #ffcc00;
}

.game-card.tetris h2 {
  color: #00ccff;
}

.game-card.mbti h2 {
  color: #9c27b0;
}

.game-card.tank h2 {
  color: #4caf50;
}

.game-card.bomberman h2 {
  color: #ff9800;
}



.game-icon {
  width: 120px;
  height: 120px;
  margin: 0 auto 30px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.pacman-icon {
  width: 100px;
  height: 100px;
  border-radius: 50%;
  background: #ffcc00;
  position: relative;
  clip-path: polygon(0% 0%, 100% 0%, 100% 70%, 50% 50%, 100% 30%, 100% 0%, 0% 0%);
  animation: pacman-move 1s infinite alternate;
}

.tetris-icon {
  width: 100px;
  height: 100px;
  position: relative;
}

.tetris-icon::before {
  content: '';
  position: absolute;
  width: 25px;
  height: 25px;
  background: #00ccff;
  top: 25px;
  left: 25px;
  box-shadow: 
    25px 0 #00ccff,
    0 25px #00ccff,
    25px 25px #00ccff,
    50px 25px #00ccff;
  animation: tetris-move 1s infinite alternate;
}

.mbti-icon {
  width: 100px;
  height: 100px;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
}

.mbti-icon::before {
  content: 'MBTI';
  font-size: 30px;
  font-weight: bold;
  color: #9c27b0;
  text-shadow: 0 0 10px rgba(156, 39, 176, 0.5);
  animation: mbti-pulse 1s infinite alternate;
}

.tank-icon {
  width: 100px;
  height: 100px;
  position: relative;
}

.tank-icon::before {
  content: '';
  position: absolute;
  width: 40px;
  height: 20px;
  background: #4caf50;
  top: 40px;
  left: 30px;
  border-radius: 5px;
  box-shadow: 0 0 10px rgba(76, 175, 80, 0.5);
}

.tank-icon::after {
  content: '';
  position: absolute;
  width: 10px;
  height: 10px;
  background: #388e3c;
  top: 45px;
  left: 65px;
  border-radius: 50%;
  box-shadow: 0 0 10px rgba(56, 142, 60, 0.5);
  animation: tank-fire 1s infinite alternate;
}

.bomberman-icon {
  width: 100px;
  height: 100px;
  position: relative;
}

.bomberman-icon::before {
  content: '';
  position: absolute;
  width: 60px;
  height: 60px;
  background: #ff9800;
  top: 20px;
  left: 20px;
  border-radius: 50%;
  box-shadow: 0 0 10px rgba(255, 152, 0, 0.5);
}

.bomberman-icon::after {
  content: '';
  position: absolute;
  width: 20px;
  height: 20px;
  background: #ff5722;
  top: 30px;
  left: 40px;
  border-radius: 50%;
  box-shadow: 0 0 10px rgba(255, 87, 34, 0.5);
  animation: bomberman-blink 1s infinite alternate;
}



@keyframes mbti-pulse {
  0% {
    transform: scale(1);
  }
  100% {
    transform: scale(1.1);
  }
}

@keyframes tank-fire {
  0% {
    opacity: 0.5;
    transform: scale(1);
  }
  100% {
    opacity: 1;
    transform: scale(1.2);
  }
}

@keyframes bomberman-blink {
  0% {
    opacity: 0.5;
    transform: scale(1);
  }
  100% {
    opacity: 1;
    transform: scale(1.1);
  }
}

.game-card p {
  margin-bottom: 30px;
  color: #ccc;
  font-size: 16px;
  line-height: 1.6;
}

#about {
  margin-bottom: 80px;
  padding: 40px;
  background: rgba(0, 0, 0, 0.5);
  border-radius: 15px;
  border: 1px solid #ff6b6b;
}

#about h2 {
  color: #ff6b6b;
  font-size: 32px;
  margin-bottom: 30px;
  text-align: center;
}

.about-content {
  display: flex;
  gap: 40px;
  align-items: center;
}

.about-text {
  flex: 1;
}

.about-text p {
  margin-bottom: 15px;
  line-height: 1.6;
  color: #ccc;
}

.about-stats {
  display: flex;
  gap: 20px;
}

.stat {
  text-align: center;
  padding: 20px;
  background: rgba(0, 0, 0, 0.5);
  border-radius: 8px;
  min-width: 100px;
}

.stat h3 {
  color: #ff6b6b;
  font-size: 24px;
  margin-bottom: 5px;
}

.stat p {
  color: #ccc;
  font-size: 14px;
  margin: 0;
}

#contact {
  margin-bottom: 80px;
  padding: 40px;
  background: rgba(0, 0, 0, 0.5);
  border-radius: 15px;
  border: 1px solid #ff6b6b;
}

#contact h2 {
  color: #ff6b6b;
  font-size: 32px;
  margin-bottom: 30px;
  text-align: center;
}

.contact-content {
  text-align: center;
}

.contact-content p {
  margin-bottom: 30px;
  line-height: 1.6;
  color: #ccc;
}

.contact-info {
  display: flex;
  justify-content: center;
  gap: 60px;
  flex-wrap: wrap;
}

.contact-item {
  text-align: center;
  min-width: 150px;
}

.contact-item h3 {
  color: #ff6b6b;
  font-size: 20px;
  margin-bottom: 5px;
}

.contact-item p {
  color: #ccc;
  font-size: 16px;
  margin: 0;
}

.footer {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 20px;
  margin-top: 60px;
  padding-top: 40px;
  border-top: 1px solid #333;
}

.footer p {
  color: #ccc;
  font-size: 16px;
  text-align: center;
  margin: 0;
}

.footer a {
  color: #fff;
  text-decoration: none;
  font-size: 14px;
  margin: 5px 0;
}

.footer a:hover {
  text-decoration: underline;
}

.footer .btn {
  margin: 10px 0;
}

@keyframes pacman-move {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(30deg);
  }
}

@keyframes tetris-move {
  0% {
    transform: translateY(0);
  }
  100% {
    transform: translateY(-5px);
  }
}



@media (max-width: 900px) {
  .container {
    max-width: 900px;
    padding: 30px;
  }
  
  h1 {
    font-size: 36px;
    margin-bottom: 40px;
  }
  
  .header {
    margin-bottom: 40px;
  }
  
  .hero {
    margin-bottom: 60px;
  }
  
  .game-entries {
    flex-direction: column;
    align-items: center;
    gap: 30px;
  }
  
  .game-card {
    width: 100%;
    max-width: 300px;
    padding: 30px;
  }
  
  .game-card h2 {
    font-size: 24px;
    margin-bottom: 20px;
  }
  
  .game-icon {
    width: 100px;
    height: 100px;
    margin: 0 auto 20px;
  }
  
  .pacman-icon {
    width: 80px;
    height: 80px;
  }
  
  .tetris-icon {
    width: 80px;
    height: 80px;
  }
  
  .tetris-icon::before {
    width: 20px;
    height: 20px;
    top: 20px;
    left: 20px;
    box-shadow: 
      20px 0 #00ccff,
      0 20px #00ccff,
      20px 20px #00ccff,
      40px 20px #00ccff;
  }
  
  .mbti-icon {
    width: 80px;
    height: 80px;
  }
  
  .mbti-icon::before {
    font-size: 24px;
  }
  
  .game-card p {
    margin-bottom: 20px;
    font-size: 14px;
    line-height: 1.4;
  }
  
  #about {
    margin-bottom: 60px;
    padding: 30px;
  }
  
  #about h2 {
    font-size: 28px;
    margin-bottom: 20px;
  }
  
  #contact {
    margin-bottom: 60px;
    padding: 30px;
  }
  
  #contact h2 {
    font-size: 28px;
    margin-bottom: 20px;
  }
  
  .contact-info {
    gap: 40px;
  }
  
  .contact-item h3 {
    font-size: 18px;
  }
  
  .contact-item p {
    font-size: 14px;
  }
  
  .footer {
    gap: 15px;
    margin-top: 40px;
    padding-top: 30px;
  }
  
  .footer p {
    font-size: 14px;
  }
}

@media (max-width: 768px) {
  .container {
    padding: 20px;
  }
  
  h1 {
    font-size: 28px;
  }
  
  .header {
    flex-direction: column;
    gap: 10px;
    align-items: stretch;
  }
  
  .user-info {
    justify-content: center;
  }
  
  .about-content {
    flex-direction: column;
    text-align: center;
  }
  
  .about-stats {
    justify-content: center;
    margin-top: 20px;
  }
  
  .contact-info {
    flex-direction: column;
    align-items: center;
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