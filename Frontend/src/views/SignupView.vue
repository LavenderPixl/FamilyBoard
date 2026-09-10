<script setup lang="ts">
import {ref} from "vue";
import api from "@/api.ts";
import router from "@/router";

const email = ref('');
const username = ref('');
const password = ref('');
const passwordAgain = ref('');

const error = ref(false);
const errorMessage = ref('');

function checkMatch() {
  return password.value === passwordAgain.value;
}

function createUser() {
  if (!checkMatch()) { // Check if passwords match
    error.value = true;
    errorMessage.value = 'Kodeordene matcher ikke.';
    return;
  }
  // Check if password is within rules
  if (!/^(?=.*[a-z])(?=.*[A-Z]).{4,}$/.test(password.value) ) {
    error.value = true;
    errorMessage.value = 'Kodeordet skal være minimum 4 karakterer,\nog indeholde en stor og en lille karakter.';
    return;
  }
  // Check if username is within rules
  if (!/^[a-zA-Z0-9]{3,}$/.test(username.value)) {
    error.value = true;
    errorMessage.value = 'Brugernavn skal være minimum 3 karakterer\nog kan kun indeholde bogstaver eller numre.';
    return;
  }

  error.value = false;

  api.createUser(email.value, username.value, password.value)
      .then(res => {
        router.push({name: 'login', query: { created: "true"}});
      }).catch(err => {
        if (err.response?.status === 409) {
          error.value = true;
          errorMessage.value = "En bruger med denne email eksisterer allerede."
        }
      })
}

</script>

<template>
  <main>
    <div class="container text-center ">
      <h1 class="text-center"> FamilyBoard </h1>
      <p v-if="error" class="alert-danger p-2">{{ errorMessage }}</p>
      <form class="signup-form" @submit.prevent="createUser">
        <input v-model="email" name="email" placeholder="Indtast din email" required type="email"/>
        <input v-model="username" name="username" placeholder="Indtast dit brugernavn - Min. 3 karakterer" required type="text"/>
        <input v-model="password" name="password" placeholder="Indtast dit kodeord" required type="password"/>
        <input v-model="passwordAgain" name="passwordAgain" placeholder="Indtast dit kodeord igen" required type="password"/>
        <button type="submit">Opret bruger</button>
        <button v-on:click="$router.push('/login')" type="button"><i class="bi-arrow-left"></i> Tilbage til login</button>
      </form>
    </div>
  </main>
</template>

<style scoped>
.signup-form {
  display:flex;
  flex-direction: column;
  width: 20rem;
  margin: 0 auto;
}
.signup-form > * {
  margin-top: 1rem;
}

.alert-danger {
  white-space: pre-line;
}
</style>