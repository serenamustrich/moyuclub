<template>
  <div class="app">
    <router-view />
  </div>
</template>

<script>
export default {
  name: 'App'
}
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: Arial, sans-serif;
  background: linear-gradient(135deg, #1a1a2e 0%, #16213e 100%);
  color: #fff;
  min-height: 100vh;
}

.app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.btn {
  display: inline-block;
  padding: 10px 20px;
  background: linear-gradient(135deg, #ffcc00, #ffdd33);
  color: #000;
  border: none;
  border-radius: 20px;
  font-weight: bold;
  cursor: pointer;
  margin: 5px;
  transition: all 0.3s ease;
  box-shadow: 0 5px 15px rgba(255, 204, 0, 0.3);
}

.btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(255, 204, 0, 0.5);
}

.btn-secondary {
  background: linear-gradient(135deg, #666, #888);
  color: #fff;
}

.container {
  width: 100%;
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
}

.message {
  text-align: center;
  margin: 20px 0;
  padding: 10px;
  border-radius: 5px;
}

.message.success {
  background-color: rgba(40, 167, 69, 0.2);
  border: 1px solid #28a745;
}

.message.error {
  background-color: rgba(220, 53, 69, 0.2);
  border: 1px solid #dc3545;
}
</style>