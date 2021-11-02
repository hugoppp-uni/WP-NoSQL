<template>
  <img alt="Vue logo" src="./assets/logo.png">
  <RedisDBFrontend msg="Welcome to Redis"/>
</template>

<script>
import RedisDBFrontend from './components/RedisDBFrontend.vue'

export default {
  name: 'App',
  components: {
    RedisDBFrontend
  }
}
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
