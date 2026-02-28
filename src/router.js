import { createRouter, createWebHistory } from 'vue-router'
import Index from './components/Index.vue'
import Login from './components/Login.vue'
import Register from './components/Register.vue'
import Leaderboard from './components/Leaderboard.vue'
import TetrisLeaderboard from './components/TetrisLeaderboard.vue'
import MBTITest from './components/MBTITest.vue'
import TankGame from './components/TankGame.vue'
import PacmanGame from './components/PacmanGame.vue'
import TetrisGame from './components/TetrisGame.vue'
import BombermanGame from './components/BombermanGame.vue'

const routes = [
  { path: '/', component: Index },
  { path: '/login', component: Login },
  { path: '/register', component: Register },
  { path: '/leaderboard', component: Leaderboard },
  { path: '/tetris-leaderboard', component: TetrisLeaderboard },
  { path: '/mbti-test', component: MBTITest },
  { path: '/tank-game', component: TankGame },
  { path: '/pacman-game', component: PacmanGame },
  { path: '/tetris-game', component: TetrisGame },
  { path: '/bomberman-game', component: BombermanGame }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
