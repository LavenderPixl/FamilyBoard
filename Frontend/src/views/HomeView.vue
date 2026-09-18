<script setup lang="ts">
import { authStore } from '@/stores/authStore.ts'
import { userStore } from '@/stores/userStore.ts'
import { ref } from 'vue'

import noFamily from './homeViews/NoFamily.vue'
import HasFamily from './homeViews/HasFamily.vue'

const auth = authStore()
const user = userStore()
const isInFamily = ref(isUserInFamily())

function isUserInFamily() {
  if (user.user !== null) {
    if (user.user.familyId !== 0) {
      return true
    }
  }
  return false
}
</script>

<template>
  <main>
    <no-family v-if="!isInFamily" />
    <has-family v-if="isInFamily"/>
  </main>
</template>
