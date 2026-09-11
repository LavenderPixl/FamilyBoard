<script setup lang="ts">
import api from "../api"
import { useAuth } from "../stores/auth.ts"
import {onMounted, ref} from "vue";
import router from "@/router";
import { useRoute } from "vue-router"

const route = useRoute()

const email = ref('');
const password = ref('');
const incorrectLogin = ref(false);
const userCreated = ref(route.query.created === "true");

const auth = useAuth();

// Success message on new user created - Clears when user reloads, so message isn't stuck.
onMounted(() => {
  if (userCreated.value) {
    history.replaceState(history.state,'','/login');
  }
})

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
      <p v-if="incorrectLogin" class="alert-danger mt-3">Vi kunne ikke finde en bruger med denne email og kodeord.</p>
      <p v-if="userCreated" class="alert-success mt-3">Din bruger er nu oprettet.</p>
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
  margin: 0 auto;
}
.login-form > * {
  margin-top: 1rem;
}
</style>