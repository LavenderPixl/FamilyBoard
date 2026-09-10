<script setup lang="ts">
import router from "@/router";
import { useAuth } from "@/stores/auth.ts";

const auth = useAuth();

function logout() {
  auth.logout()
  router.push("/login")
}
</script>

<template>
  <nav>
    <p class="title">FamilyBoard</p>
    <div>
      <RouterLink to="/">Forside</RouterLink>
      <RouterLink to="/administrer-pligter">Administrer pligter</RouterLink>
      <RouterLink to="/familieindstillinger">Familieindstillinger</RouterLink>
      <RouterLink to="/kontoindstillinger">Kontoindstillinger</RouterLink>
      <a href="#" @click.prevent="logout">Log ud</a>
    </div>
  </nav>
</template>

<style scoped>
.title {
  margin: 0;
  padding: 1rem;
}
nav {
  display: flex;
  align-items: center;
  justify-content: space-between;
  line-height: 1.5;
}

/* If you're already on the page */
nav a.router-link-exact-active {
  color: var(--color-text-hover);
}
nav a.router-link-exact-active:hover {
  background-color: transparent;
}

nav a {
  color: var(--color-text);
  display: inline-block;
  padding: 0 1rem;
  border-left: 1px solid var(--color-border);
}

/* Removes line in front of the first link ( | Forside | ) -> ( Forside | ) */
nav a:first-of-type {
  border: 0;
}

@media (min-width: 1024px) {
  header {
    display: flex;
    place-items: center;
    padding-right: calc(var(--section-gap) / 2);
  }
}
</style>