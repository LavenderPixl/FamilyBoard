<script setup lang="ts">
import {onMounted, ref} from "vue";
import taskApi from "@/api/taskApi.ts";
import type {ITask} from "@/models/task.ts";

const family = ref();
const tasks = ref<ITask[]>([]);

async function deleteTask(task: ITask) {
  await taskApi.deleteTask(task)
  tasks.value = await taskApi.getTasksForFamily();

}

onMounted(async () => {
  tasks.value = await taskApi.getTasksForFamily();
})
</script>

<template>
  <div class="container ">
    <h1 class="title text-center py-3">Administrer pligter</h1>
    <div>

    <h2></h2>
    <table class="table table-dark table-striped table-bordered">
      <thead>
      <tr>
        <th colspan="4" class="h2">Nuværende pligter</th>
        <th><button><i class="bi bi-plus-lg"></i></button></th>
      </tr>
      </thead>
      <thead>
        <tr class="thead">
          <th>Pligt</th>
          <th style="width: 15%">Point</th>
          <th style="width: 25%">Tildelt</th>
          <th style="width: 10%">Rediger</th>
          <th style="width: 10%">Slet</th>
        </tr>
      </thead>
      <tbody>
      <tr v-for="task in tasks" :key="task.id">
        <td>{{ task.name }}</td>
        <td>{{ task.reward}}</td>
        <td>{{ task.username }}</td>
        <td><button><i class="bi bi-pencil-square"></i></button></td>
        <td><button @click="deleteTask(task)"><i class="bi bi-x-lg"></i></button></td>
      </tr>
      </tbody>
    </table>
    </div>

  </div>
</template>

<style scoped>
.container {
  min-height: 87vh;
  min-width: 93vw;
  margin: 0;
}

</style>