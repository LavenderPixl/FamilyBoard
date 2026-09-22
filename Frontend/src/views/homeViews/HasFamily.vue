<script setup lang="ts">
import { onMounted, ref } from 'vue'
import type { ITask } from '@/models/task.ts'
import taskApi from '@/api/taskApi.ts'
import { userStore } from '@/stores/userStore.ts'
import familyApi from '@/api/familyApi.ts'

const familyName = ref('')
const tasks = ref<ITask[]>([])
const openTasks = ref<ITask[]>([])
const isUserAdult = ref(false)

async function markTaskAsCompleted(task: ITask) {
  const updatedTask = await taskApi.markTaskAsCompleted(task)

  const index = tasks.value.findIndex((t) => t.id === updatedTask.id)
  if (index !== -1) {
    tasks.value[index] = updatedTask
    sortTasks()
  }
}

async function unMarkTaskAsCompleted(task: ITask) {
  const updatedTask = await taskApi.unMarkTaskAsCompleted(task)

  const index = tasks.value.findIndex((t) => t.id === updatedTask.id)
  if (index !== -1) {
    tasks.value[index] = updatedTask
    sortTasks()
  }
}

async function claimTask(task: ITask) {
  await taskApi.claimTask(task)
  await refreshTasks()
}

async function refreshTasks() {
  const userFromUserStore = userStore().user
  if (userFromUserStore !== null) {
    tasks.value = await taskApi.getTasksForUser(userFromUserStore.id)
    sortTasks()
  }
  await getOpenTasks();
}

function sortTasks() {
  tasks.value = tasks.value.sort((a, b) => Number(a.completed) - Number(b.completed))
}

async function getOpenTasks() {
  const familyTasks = await taskApi.getTasksForFamily()
  openTasks.value = familyTasks.filter((task) => task.userId === null)
}

onMounted(async () => {
  const userFromUserStore = userStore().user
  if (userFromUserStore !== null) {
    isUserAdult.value = userFromUserStore.isAdult
    tasks.value = await taskApi.getTasksForUser(userFromUserStore.id)
  }
  sortTasks()

  familyName.value = await familyApi.getFamilyName()

  await getOpenTasks()
})
</script>

<template>
  <div class="container h-full" id="main-container">
    <h1 class="title text-center py-3">{{ familyName }}</h1>
    <div class="row justify-content-center items-center" style="min-width: 93vw">
      <div class="col-2" id="scoreboard">
        <h2>Rank liste</h2>
        <table class="table table-dark table-striped table-bordered">
          <thead>
            <tr>
              <th scope="col">#</th>
              <th scope="col">Navn</th>
              <th scope="col">Points</th>
            </tr>
          </thead>

          <tbody>
            <tr>
              <td>1</td>
              <td>Søren</td>
              <td>1230</td>
            </tr>
            <tr>
              <td>2</td>
              <td>sofie</td>
              <td>1050</td>
            </tr>
            <tr>
              <td>3</td>
              <td>sigurd</td>
              <td>525</td>
            </tr>
          </tbody>
        </table>
      </div>
      <div class="col-6" id="user-tasks">
        <h2>Dine pligter</h2>
        <table class="table table-dark table-striped table-bordered">
          <thead>
            <tr>
              <th>Opgave</th>
              <th>belønning</th>
              <th>Udført</th>
              <th>Opdater</th>
            </tr>
          </thead>

          <tbody>
            <tr v-for="task in tasks" :key="task.id">
              <td>{{ task.name }}</td>
              <td>{{ task.reward }}</td>
              <td>{{ task.completed }}</td>
              <td>
                <button type="button" @click="markTaskAsCompleted(task)" :hidden="task.completed">
                  Gennemfør
                </button>
                <button
                  type="button"
                  @click="unMarkTaskAsCompleted(task)"
                  :hidden="!task.completed"
                  :disabled="!isUserAdult"
                >
                  Fjern gennemført
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <div class="col-3" id="open-tasks">
        <h2>åbne pligter</h2>
        <table class="table table-dark table-striped table-bordered">
          <thead>
            <tr>
              <th>Opgave</th>
              <th>belønning</th>
              <th>Udført</th>
              <th>Tag pligten</th>
            </tr>
          </thead>

          <tbody>
            <tr v-for="task in openTasks" :key="task.id">
              <td>{{ task.name }}</td>
              <td>{{ task.reward }}</td>
              <td>{{ task.completed }}</td>
              <td>
                <button type="button" @click="claimTask(task)" :hidden="task.completed">
                  Tag
                </button>
              </td>
            </tr>
          </tbody>
        </table>
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

#main-container {
}
#scoreboard {
  border-right-style: solid;
}

#open-tasks {
  border-left-style: solid;
}
</style>