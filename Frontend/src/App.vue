<script setup lang="ts">
import {RouterView } from 'vue-router'
import Header from './components/Header.vue'
// import HelloWorld from './components/HelloWorld.vue'
</script>

<template>
  <Header id="nav" v-if="!$route.meta.hideNavbar"/> <!-- RouterLinks -->
  <RouterView />
<!--    TODO: Create a navbar in header?-->
<!--    <img alt="Vue logo" class="logo" src="@/assets/logo.svg" width="125" height="125" />-->
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
