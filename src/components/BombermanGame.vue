<template>
  <div class="bomberman-game-container">
    <h1 class="game-title">炸弹人游戏</h1>
    <div class="game-wrapper">
      <div id="gameContainer" class="game-container">
        <!-- 开始游戏按钮 -->
        <div v-if="!gameLoaded && !isLoading" class="start-screen">
          <h2>欢迎来到炸弹人游戏</h2>
          <p>准备好开始游戏了吗？</p>
          <button class="start-button" @click="startGame">开始游戏</button>
        </div>
        
        <!-- 加载进度条 -->
        <div v-if="isLoading" class="loading-screen">
          <h2>游戏加载中...</h2>
          <div class="progress-container">
            <div class="progress-bar" :style="{ width: loadingProgress + '%' }"></div>
          </div>
          <p class="progress-text">{{ Math.round(loadingProgress) }}%</p>
        </div>
      </div>
    </div>
    <div class="game-instructions">
      <h3>游戏操作说明：</h3>
      <ul>
        <li>WASD键：移动玩家</li>
        <li>空格键：放置炸弹</li>
        <li>ESC键：暂停游戏</li>
      </ul>
    </div>
  </div>
</template>

<script>
export default {
  name: 'BombermanGame',
  data() {
    return {
      isLoading: false,
      gameLoaded: false,
      loadingProgress: 0
    };
  },
  mounted() {
    // 不自动加载游戏，等待用户点击开始按钮
  },
  methods: {
    startGame() {
      this.isLoading = true;
      this.loadingProgress = 0;
      this.loadUnityGame();
    },
    loadUnityGame() {
      // 动态加载UnityLoader.js
      const script = document.createElement('script');
      script.src = '/bomberman/UnityLoader.js';
      script.onload = () => {
        this.initializeUnityGame();
      };
      document.head.appendChild(script);
      
      // 加载样式
      const link = document.createElement('link');
      link.rel = 'stylesheet';
      link.href = '/bomberman/style.css';
      document.head.appendChild(link);
    },
    initializeUnityGame() {
      // 检查UnityLoader是否加载完成
      if (window.UnityLoader) {
        // 实例化Unity游戏，使用箭头函数确保this绑定
        window.gameInstance = UnityLoader.instantiate('gameContainer', '/bomberman/Build/Bomberman.json', {
          onProgress: (gameInstance, progress) => this.unityProgress(gameInstance, progress)
        });
      } else {
        // 重试加载
        setTimeout(() => this.initializeUnityGame(), 100);
      }
    },
    unityProgress(gameInstance, progress) {
      console.log('Unity game loading progress:', progress);
      this.loadingProgress = progress * 100;
      
      // 当加载完成时
      if (progress >= 1) {
        this.isLoading = false;
        this.gameLoaded = true;
      }
    }
  }
}
</script>

<style scoped>
.bomberman-game-container {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
  font-family: Arial, sans-serif;
}

.game-title {
  text-align: center;
  color: #333;
  margin-bottom: 20px;
}

.game-wrapper {
  display: flex;
  justify-content: center;
  margin-bottom: 20px;
}

.game-container {
  width: 800px;
  height: 600px;
  background-color: #000;
  border: 1px solid #ccc;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
  position: relative;
  overflow: hidden;
}

.start-screen {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.8);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: white;
  z-index: 10;
}

.start-screen h2 {
  font-size: 36px;
  margin-bottom: 20px;
  color: #ff9800;
  text-shadow: 0 0 10px rgba(255, 152, 0, 0.5);
}

.start-screen p {
  font-size: 18px;
  margin-bottom: 40px;
  color: #ccc;
}

.start-button {
  padding: 15px 40px;
  font-size: 20px;
  font-weight: bold;
  background-color: #ff9800;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 0 15px rgba(255, 152, 0, 0.5);
}

.start-button:hover {
  background-color: #ffb74d;
  transform: scale(1.05);
  box-shadow: 0 0 20px rgba(255, 152, 0, 0.7);
}

.loading-screen {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.8);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: white;
  z-index: 10;
}

.loading-screen h2 {
  font-size: 28px;
  margin-bottom: 40px;
  color: #ff9800;
}

.progress-container {
  width: 60%;
  height: 20px;
  background-color: #333;
  border-radius: 10px;
  overflow: hidden;
  margin-bottom: 20px;
  box-shadow: 0 0 10px rgba(255, 152, 0, 0.3);
}

.progress-bar {
  height: 100%;
  background-color: #ff9800;
  border-radius: 10px;
  transition: width 0.3s ease;
  box-shadow: 0 0 10px rgba(255, 152, 0, 0.5);
}

.progress-text {
  font-size: 18px;
  font-weight: bold;
  color: #ff9800;
}

.game-instructions {
  background-color: #f5f5f5;
  padding: 15px;
  border-radius: 5px;
  margin-top: 20px;
}

.game-instructions h3 {
  margin-top: 0;
  color: #333;
}

.game-instructions ul {
  margin: 10px 0 0 0;
  padding-left: 20px;
}

.game-instructions li {
  margin-bottom: 5px;
}
</style>