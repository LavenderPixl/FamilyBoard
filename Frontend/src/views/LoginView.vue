<script lang="ts" setup>
</script>

<template>
  <main>
    <div class="container">
      <h1 class="text-center"> FamilyBoard </h1>
        <form class="login-form">
        <input name="email" placeholder="Indtast din email" required type="email"/>
        <input name="password" placeholder="Indtast dit kodeord" required type="password"/>
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
}
.login-form > * {
  margin-top: 1rem;
}
</style>