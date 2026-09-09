<script setup lang="ts">
import {RouterView } from 'vue-router'
import Header from './components/Header.vue'

</script>

<template>
  <Header id="nav" v-if="!$route.meta.hideNavbar"/> <!-- RouterLinks -->
  <RouterView />
</template>


<style scoped>

#nav {
  position: fixed;
  left: 0;
  top: 0;
  width: 100%;
  z-index: 1000;
  height: 4rem;
  background-color: var(--color-background-soft);
}
:global(body) {
  background-color: var(--color-background);
}
</style>
