<script setup lang="ts">
import {onMounted, ref} from "vue";
import taskApi from "@/api/taskApi.ts";
import familyApi from "@/api/familyApi.ts";
import type {ITask} from "@/models/task.ts";
import type {IUser} from "@/models/user.ts";
import EditChore from "@/components/EditChore.vue";

const tasks = ref<ITask[]>([]);
const familyMembers = ref<IUser[]>([]);

const formOpen = ref(false);
const editingTask = ref<ITask | null>(null);

function openCreate() {
  editingTask.value = null;
  formOpen.value = true;
}
function openEdit(task: ITask) {
  editingTask.value = task;
  formOpen.value = true;
}
function closeForm() {
  formOpen.value = false;
}

async function deleteTask(task: ITask) {
  await taskApi.deleteTask(task)
  tasks.value = await taskApi.getTasksForFamily();
}

async function saveTask(payload: {id?: number; name: string; reward: number; expireDate: string; userId: number | null}) {
  if (payload.id) { // If editing (task exists).
    await taskApi.updateTask(payload.id, payload.name, payload.reward, payload.expireDate, payload.userId);
  } else {
    await taskApi.createTask(payload.name, payload.reward, payload.expireDate, payload.userId);
  }
  tasks.value = await taskApi.getTasksForFamily()
  closeForm()
}

onMounted(async () => {
  tasks.value = await taskApi.getTasksForFamily();
  familyMembers.value = await familyApi.getFamilyMembers();
})
</script>

<template>
  <div class="container px-5">
    <h1 class="title text-center py-3">Administrer pligter</h1>
    <div>
    <table class="table table-dark table-striped table-bordered">
      <thead>
      <tr>
        <th colspan="4" class="h2">Nuværende pligter</th>
        <th colspan="2" class="h4"> Opret ny pligt <button @click="openCreate"><i class="bi bi-plus-lg"></i></button></th>
      </tr>
      </thead>
      <thead>
        <tr class="thead">
          <th>Pligt</th>
          <th style="width: 15%">Point</th>
          <th style="width: 20%">Tildelt</th>
          <th style="width: 15%">Sidste dato</th>
          <th style="width: 10%">Rediger</th>
          <th style="width: 10%">Slet</th>
        </tr>
      </thead>
      <tbody>
      <tr v-for="task in tasks" :key="task.id">
        <td>{{ task.name }}</td>
        <td>{{ task.reward}}</td>
        <td>{{ task.username }}</td>
        <td>{{ task.expireDate }}</td>
        <td><button @click="openEdit(task)"><i class="bi bi-pencil-square"></i></button></td>
        <td><button @click="deleteTask(task)"><i class="bi bi-x-lg"></i></button></td>
      </tr>
      </tbody>
    </table>
    </div>
    <EditChore
        v-if="formOpen"
        :key="editingTask?.id ?? 'new'"
        :task="editingTask"
        :family-members="familyMembers"
        @close="closeForm"
        @save="saveTask"
    ></EditChore>


  </div>
</template>

<style scoped>
.container {
  min-height: 87vh;
  min-width: 93vw;
  margin: 0;
}

label {
  color: var(--color-text)
}

</style>