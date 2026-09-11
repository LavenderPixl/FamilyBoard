<script setup lang="ts">
import router from "@/router";
import { authStore } from "../stores/authStore.ts"

const auth = authStore();
</script>

<template>
  <main>
    <h1>404 - Denne side findes ikke. </h1>
<!-- Check store, if user is logged in, create link to '/', else '/login' -->
    <button @click="auth.isLoggedIn ? router.push({name: 'home'}) : router.push({name: 'login'})">
      {{ auth.isLoggedIn ? 'Gå til forsiden.' : 'Gå til login.'}}</button>
  </main>
</template>

<style scoped>

</style>