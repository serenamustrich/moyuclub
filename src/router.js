import { createRouter, createWebHistory } from 'vue-router'
import Index from './components/Index.vue'
import Leaderboard from './components/Leaderboard.vue'
import Login from './components/Login.vue'
import Register from './components/Register.vue'
import PacmanGame from './components/PacmanGame.vue'
import TetrisGame from './components/TetrisGame.vue'
import TetrisLeaderboard from './components/TetrisLeaderboard.vue'
import TankGame from './components/TankGame.vue'
import MBTITest from './components/MBTITest.vue'
import BombermanGame from './components/BombermanGame.vue'


const routes = [
  {
    path: '/',
    name: 'Index',
    component: Index
  },
  {
    path: '/leaderboard',
    name: 'Leaderboard',
    component: Leaderboard
  },
  {
    path: '/login',
    name: 'Login',
    component: Login
  },
  {
    path: '/register',
    name: 'Register',
    component: Register
  },
  {
    path: '/game',
    name: 'PacmanGame',
    component: PacmanGame
  },
  {
    path: '/tetris',
    name: 'TetrisGame',
    component: TetrisGame
  },
  {
    path: '/tetris-leaderboard',
    name: 'TetrisLeaderboard',
    component: TetrisLeaderboard
  },
  {
    path: '/mbti-test',
    name: 'MBTITest',
    component: MBTITest
  },
  {
    path: '/tank-game',
    name: 'TankGame',
    component: TankGame
  },
  {
    path: '/bomberman-game',
    name: 'BombermanGame',
    component: BombermanGame
  },

]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router