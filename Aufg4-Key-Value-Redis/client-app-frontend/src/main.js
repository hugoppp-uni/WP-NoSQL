import { createApp } from 'vue'
// import Vue from './App.vue'

import App from "./App";

let redis = require('redis');
let PORT = 6379;
let HOST = '127.0.0.1';
const client = redis.createClient(PORT, HOST);

client.on('connect', function() {
    console.log('Connected!');
});
console.log(redis)
console.log(PORT)
console.log(HOST)

createApp(App).mount('#app')

// const app = createApp(
//     App)
//
// app.mount('#app')
// app.use(client)

// console.log(redis)
// console.log(PORT)
// console.log(HOST)