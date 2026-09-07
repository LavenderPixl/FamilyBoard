<script setup lang="ts">

</script>

<template>
  <main>
    <div class="container">
      <h1 class="text-center"> FamilyBoard </h1>
      <form class="signup-form">
        <input name="email" placeholder="Indtast din email" required type="email"/>
        <input name="username" placeholder="Indtast dit brugernavn" required type="email"/>
        <input name="password" placeholder="Indtast dit kodeord" required type="password"/>
        <input name="password-again" placeholder="Indtast dit kodeord igen" required type="password"/>
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
}
.signup-form > * {
  margin-top: 1rem;
}
</style>