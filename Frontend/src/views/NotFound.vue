<script setup lang="ts">
import {useAuth} from "../stores/auth.ts"
import {RouterLink} from "vue-router";
import router from "@/router";

const auth = useAuth();
</script>

<template>
  <main>
    <h1>404 - Denne side findes ikke. </h1>
<!-- Check store, if user is logged in, create link to '/', else '/login' -->
    <button @click="auth.isLoggedIn ? router.push('/') : router.push('/login')">
      {{ auth.isLoggedIn ? 'Gå til forsiden.' : 'Gå til login.'}}</button>
  </main>
</template>

<style scoped>

</style>