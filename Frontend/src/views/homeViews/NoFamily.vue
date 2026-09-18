<script setup lang="ts">
import { ref } from 'vue'
import familyApi from '@/api/familyApi.ts'

const newFamilyName = ref('')
const incorrectFamilyName = ref(false)
const joinFamilyCode = ref('')
const incorrectFamilyCode = ref(false)

function JoinFamily() {
  if (joinFamilyCode.value === '') {
    incorrectFamilyCode.value = true
    return
  }
  familyApi.joinFamily(joinFamilyCode.value)
    .then(() => {
      window.location.reload()
    })
    .catch((err) => {
      if (err.response?.status === 404) {
        incorrectFamilyCode.value = true
      } else {
        console.error(err)
      }
    })
}

function CreateFamily() {
  familyApi.createFamily(newFamilyName.value)
      .then(() => {
        window.location.reload()
      })
      .catch((err) => {
        if (err.response?.status === 400) {
          incorrectFamilyName.value = true
        } else {
          console.error(err)
        }
      })
}

// async function CreateFamily() {
//   await familyApi.createFamily(newFamilyName.value)
//   window.location.reload()
// }
</script>

<template>
  <div class="text-center" id="no-family-container">
    <h1 class="title py-5">Du er ikke medlem af en familie endnu.</h1>
    <div class="row">
      <div class="col">
        <button class="btn btn-secondary" data-toggle="modal" data-target="#create-family-popup">
          Opret familie
        </button>
      </div>
      <div class="col">
        <button class="btn btn-secondary" data-toggle="modal" data-target="#join-family-popup">
          Deltag i familie
        </button>
      </div>
    </div>
  </div>

  <!--Create family popup-->
  <div class="modal fade" id="create-family-popup" tabindex="-1" role="dialog">
    <div class="modal-dialog" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h2 class="modal-title h4">Opret familie</h2>
          <button type="button" class="close" data-dismiss="modal" aria-label="close">
            <span aria-hidden="true">&times;</span>
          </button>
        </div>
        <div class="modal-body">
          <h3 class="h5">Familie navn</h3>
          <p v-if="incorrectFamilyName" class="alert-danger mt-3">
            Dette navn kan ikke bruges til en familie
          </p>
          <input
            v-model="newFamilyName"
            type="text"
            class="form-control"
            placeholder="Familie navn"
          />
        </div>
        <div class="modal-footer">
          <button v-on:click="CreateFamily()" type="button" class="btn, btn-primary">
            Opret familie
          </button>
        </div>
      </div>
    </div>
  </div>

  <!--Join family popup-->
  <div class="modal fade" id="join-family-popup" tabindex="-1" role="dialog">
    <div class="modal-dialog" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h2 class="modal-title h4">Deltag i familie</h2>
          <button type="button" class="close" data-dismiss="modal" aria-label="close">
            <span aria-hidden="true">&times;</span>
          </button>
        </div>
        <div class="modal-body">
          <h3 class="h5">Indtast den kode du har fået for at deltage familien</h3>
          <p v-if="incorrectFamilyCode" class="alert-danger mt-3">
            Denne Kode passer ikke med nogen familier
          </p>
          <input
            v-model="joinFamilyCode"
            type="text"
            class="form-control"
            placeholder="familie kode"
          />
        </div>
        <div class="modal-footer">
          <button v-on:click="JoinFamily()" type="button" class="btn, btn-primary">
            Deltag i familie
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
#no-family-container {
  background-color: var(--color-background-soft);
  padding: 10%;
  min-width: 50em;
}

.modal-content {
  background-color: var(--color-background-mute);
}
</style>
