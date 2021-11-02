<template>
  <div class="hello">
    <h1>{{ msg }}</h1>
    <p>
      If you give me a zip code, I'll provide you with the corresponding city and state. <br>
      You can even tell me a city and I'll know the Zip code. <br>
          </p>
    <h3>Queries:</h3>
    <input
        type="text"
        v-model="zipCode" placeholder="You can enter a zip Code here"
        class="testInput">
  </div>
  <div>
    <button @click="getCityAndState()">
      Get city and state
    </button>`
  </div>
</template>

<script>
export default {
  name: 'RedisDBFrontend',
  props: {
    msg: String
  },
  created() {

  },
  data() {
    return {
      zipCode: ''
    }
  },
  methods: {
    getCityAndState() {
      this.zipCode = "veraendert"
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}

.testInput {
  width: 180px;
  padding: 12px 16px;
  border-radius: 12px;
}
</style>
