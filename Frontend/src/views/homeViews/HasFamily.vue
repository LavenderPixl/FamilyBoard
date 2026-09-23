<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import type { ITask } from '@/models/task.ts'
import taskApi from '@/api/taskApi.ts'
import { userStore } from '@/stores/userStore.ts'
import familyApi from '@/api/familyApi.ts'
import type { IGoal } from '@/models/goal.ts'
import goalApi from '@/api/goalApi.ts'

const familyName = ref('')
const goal = ref<IGoal>()
const tasks = ref<ITask[]>([])
const openTasks = ref<ITask[]>([])
const isUserAdult = ref(false)

const goalPercentage = computed(() => {
  if (goal.value) {
    const percentage = (goal.value.progress / goal.value.cost) * 100
    return Math.min(100, Math.max(0, Math.round(percentage)))
  }
  return 0
})

async function markTaskAsCompleted(task: ITask) {
  const updatedTask = await taskApi.markTaskAsCompleted(task)

  const index = tasks.value.findIndex((t) => t.id === updatedTask.id)
  if (index !== -1) {
    tasks.value[index] = updatedTask
    sortTasks()
    await refreshGoal()
  }
}

async function unMarkTaskAsCompleted(task: ITask) {
  const updatedTask = await taskApi.unMarkTaskAsCompleted(task)

  const index = tasks.value.findIndex((t) => t.id === updatedTask.id)
  if (index !== -1) {
    tasks.value[index] = updatedTask
    sortTasks()
    await refreshGoal()
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
  await getOpenTasks()
}

async function refreshGoal() {
  const userFromUserStore = userStore().user
  if (userFromUserStore !== null) {
    if (userFromUserStore.currentGoalId !== 0) {
      goal.value = await goalApi.getGoal(userFromUserStore.currentGoalId)
    }
  }
  console.log(goal.value)
}

function sortTasks() {
  tasks.value = tasks.value.sort((a, b) => Number(a.completed) - Number(b.completed))
}

async function getOpenTasks() {
  const familyTasks = await taskApi.getTasksForFamily()
  openTasks.value = familyTasks.filter((task) => task.userId === null)
}

function daysUntilExpire(date: string): number {
  const today = new Date()
  today.setHours(0, 0, 0, 0)

  const expireDate = new Date(date + 'T00:00:00')
  const difference = expireDate.getTime() - today.getTime()

  return Math.ceil(difference / (1000 * 60 * 60 * 24))
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
  await refreshGoal()
})
</script>

<template>
  <div class="container h-full" id="main-container">
    <h1 class="title text-center">Familien {{ familyName }}</h1>
    <div class="row justify-content-center items-center" id="goal-progress">
      <div v-if="!goal">
        <h2>Du har ikke et aktivt mål</h2>
      </div>
      <div v-if="goal">
        <h2 class="text-center">Mål: {{ goal.name }}</h2>
        <div class="progress">
          <div
            class="progress-bar"
            role="progressbar"
            :style="{ width: goalPercentage + '%' }"
            :aria-valuenow="goal.progress"
            aria-valuemin="0"
            :aria-valuemax="goal.cost"
          >
            {{ goal.progress + '/' + goal.cost }}
          </div>
        </div>
      </div>
    </div>
    <div class="row justify-content-center items-center" style="min-width: 93vw">
      <div class="col-2" id="scoreboard">
        <h2>Rang liste // WIP</h2>
        <div class="table-scroll">
          <table class="table table-dark table-striped">
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
      </div>
      <div class="col-6" id="user-tasks">
        <h2>Dine pligter</h2>
        <div class="table-scroll">
          <table class="table table-dark table-striped">
            <thead>
              <tr>
                <th>Opgave</th>
                <th>Points</th>
                <th>Udført</th>
                <th>Frist</th>
                <th>Opdater</th>
              </tr>
            </thead>

            <tbody>
              <tr
                v-for="task in tasks"
                :key="task.id"
                :hidden="daysUntilExpire(task.expireDate) < 0"
              >
                <td>{{ task.name }}</td>
                <td>{{ task.reward }}</td>
                <td>
                  <i v-if="task.completed" class="bi bi-check" style="font-size: x-large"></i>
                  <i v-if="!task.completed" class="bi bi-x" style="font-size: x-large"></i>
                </td>
                <td>{{ daysUntilExpire(task.expireDate) }} dage</td>
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
      </div>
      <div class="col-3" id="open-tasks">
        <h2>Åbne pligter</h2>
        <div class="table-scroll">
          <table class="table table-dark table-striped">
            <thead>
              <tr>
                <th>Opgave</th>
                <th>Points</th>
                <th>Tag pligten</th>
              </tr>
            </thead>

            <tbody>
              <tr v-for="task in openTasks" :key="task.id" :hidden="task.completed">
                <td>{{ task.name }}</td>
                <td>{{ task.reward }}</td>
                <td>
                  <button type="button" @click="claimTask(task)">Tag</button>
                </td>
              </tr>
            </tbody>
          </table>
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

.table {
}

.table-scroll {
  overflow-x: auto;
  max-height: 55vh;
}

.progress {
  min-width: 80vw;
  height: 2em;
}

#main-container {
}
#scoreboard {
  border-right-style: solid;
}

#open-tasks {
  border-left-style: solid;
}

#goal-progress {
  min-width: 93vw;
  margin-top: 1em;
  margin-bottom: 2em;
}
</style>