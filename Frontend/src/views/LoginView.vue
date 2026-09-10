<script lang="ts" setup>
import api from "../api"
import { useAuth } from "../stores/auth.ts"
import { ref } from "vue";
import router from "@/router";

const email = ref('');
const password = ref('');
const incorrectLogin = ref(false);

const auth = useAuth();

function login() {
  incorrectLogin.value = false;

  api.login(email.value, password.value)
      .then(res => {
        auth.setToken(res.data);
        router.push('/')
        })
      .catch (err => {
        if (err.response?.status === 401) {
          incorrectLogin.value = true;
        } else {
          console.error(err)
        }
      })
}
</script>

<template>
  <main>
    <div class="container text-center">
      <h1 class="title"> FamilyBoard </h1>
      <p v-if="incorrectLogin" class="alert-danger mt-3">Vi kunne ikke finde en bruger <br>med denne email og kodeord.</p>
      <form class="login-form" @submit.prevent="login">
        <input v-model="email" name="email" placeholder="Indtast din email" required type="email"/>
        <input v-model="password" name="password" placeholder="Indtast dit kodeord" required type="password"/>
        <button type="submit">Log ind</button>
        <button v-on:click="$router.push('/signup')" type="button">Opret ny bruger</button>
      </form>
    </div>
  </main>
</template>

<style scoped>
.login-form {
  display:flex;
  flex-direction: column;
  width: 20rem;
}
.login-form > * {
  margin-top: 1rem;
}
</style>