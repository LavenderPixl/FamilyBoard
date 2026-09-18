<script setup lang="ts">
import { ref } from 'vue'

const familyName = ref('Jensen')
</script>

<template>
  <div class="container h-full">
    <h1>{{ familyName }}</h1>
    <div class="row">
      <div class="col">

      </div>
    </div>
  </div>
</template>

<style scoped>
.container {
  min-height: 90vh;
  background-color: red;
}
</style>
