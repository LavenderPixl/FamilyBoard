import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js'
import 'bootstrap-icons/font/bootstrap-icons.css'

import App from './App.vue'
import router from './router'
import { authStore } from "@/stores/authStore.ts";
const apiUrl = import.meta.env.VITE_API_URL

const app = createApp(App)

app.use(createPinia())
app.use(router)

// Starts refresh timer for JWT token on startup (or refresh).
const auth = authStore()
if (auth.isLoggedIn) {
    auth.startRefTimer()
}

app.mount('#app')
