<script setup lang="ts">
import {onMounted, ref} from "vue";
import type {IUser} from "@/models/user.ts";
import familyApi from "@/api/familyApi.ts";
import {userStore} from "@/stores/userStore.ts";
import userApi from "@/api/userApi.ts";

const familyMembers = ref<IUser[]>([]);
const familyName = ref('');
const currentUser = userStore()
const inviteCode = ref<string | null>(null)

function isSelf(member: IUser) {
  return member.id === currentUser.user?.id
}

function isUserChild(member: IUser){
  if (member.isAdult) {
    return "Skift til barn"}
  else {
    return "Skift til voksen" }
}

async function changeUserType(user: IUser) {
  const updatedUser = await userApi.changeUserType(user.id, !user.isAdult )
  const index = familyMembers.value.findIndex(m => m.id === user.id)
  if (index !== -1) {
    familyMembers.value[index] = updatedUser
  }
}

async function kickOut(member: IUser) {
  if(isSelf(member)) return // Can't kick self.
  await familyApi.kickFamilyMember(member.familyId, member.id)
  familyMembers.value = familyMembers.value.filter( m => m.id !== member.id)
}

async function createInvite() {
  inviteCode.value = await familyApi.createFamilyInvite()
}
function closeInvitePopup(){
  inviteCode.value = null
}

onMounted(async () => {
  familyName.value = await familyApi.getFamilyName()
  familyMembers.value = await familyApi.getFamilyMembers();
})
</script>

<template>
  <div class="container px-5">
    <h1 class="title text-center py-3">Familien {{ familyName }}</h1>
    <div>
      <table class="table table-dark table-striped table-bordered">
        <thead>
        <tr>
          <th colspan="2" class="h2">Familiemedlemmer</th>
          <th colspan="3" class="h4"> Opret ny invitations kode <button @click="createInvite"><i class="bi bi-plus-lg"></i></button></th>
        </tr>
        </thead>
        <thead>
        <tr class="thead">
          <th style="width: 20%">Navn</th>
          <th style="width: 10%">Points i alt</th>
          <th style="width: 10%">Skift til/fra barn</th>
          <th style="width: 5%">Se detaljer</th>
          <th style="width: 5%">Fjern medlem</th>
        </tr>
        </thead>
        <tbody>
        <tr v-for="member in familyMembers" :key="member.id">
          <td>{{ member.username }}</td>
          <td>{{ member.points}}</td>
          <td><button @click="changeUserType(member)" :disabled="isSelf(member)"> {{ isUserChild(member) }} </button></td>
          <td><button><i class="bi bi-pencil-square"></i></button></td>
          <td><button @click="kickOut(member)" :disabled="isSelf(member)"><i class="bi bi-x-lg"></i></button></td>
        </tr>
        </tbody>
      </table>
      <div v-if="inviteCode" class="backdrop" @click.self="closeInvitePopup()">
        <div class="backdrop-inner">
          <h3 class="title">Invitations kode</h3>
          <p class="code">{{ inviteCode }}</p>
          <button class="action-btn" @click="closeInvitePopup()">Luk</button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.container {
  min-height: 87vh;
  min-width: 93vw;
  margin: 0;
}

.backdrop {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.backdrop-inner{
  background: var(--color-background-mute);
  padding: 2rem;
  text-align: center;
}

.code{
  font-size: 2rem;
  letter-spacing: 0.2rem;
  margin: 1rem
}

.action-btn {
  flex: 1;
  padding: 0.3rem;
  border: none;
  width: 4rem;
}

label {
  color: var(--color-text)
}
</style>