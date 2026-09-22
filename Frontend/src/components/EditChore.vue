<script setup lang="ts">
  import type {ITask} from "@/models/task.ts"
  import type {IUser} from "@/models/user.ts";
  import {computed, ref} from "vue";

  const props = defineProps<{
    task?: ITask | null; // If task is null, we're creating a new one. Else, editing existing one.
    familyMembers: IUser[]; // Family members list.
  }>();
  const emit = defineEmits<{
    close: [];  // Call upon to close component, sets "editingTask" to null.
    save: [payload: { id?: number, name: string, reward: number; expireDate: string; userId: number | null; }]
  }>();

  const editing = computed(() => !!props.task)  // Checks if task exists

  const name = ref(props.task?.name ?? '');
  const reward = ref(props.task?.reward ?? 0);
  const userId = ref<number | null>(props.task?.userId ?? null) //
  const expireDate = ref(props.task?.expireDate ?? new Date().toISOString().slice(0, 10));

  function submit() {
    emit('save', {
      id: props.task?.id,
      name: name.value,
      reward: reward.value,
      expireDate: expireDate.value,
      userId: userId.value,
    })
  }
</script>

<template>
  <div class="backdrop" @click.self="emit('close')">
    <div class="content">
      <h2> {{ editing ? 'Rediger pligt' : 'Opret ny pligt'}}</h2>
      <form @submit.prevent="submit">

        <label> Pligt
          <input type="text" v-model="name" required>
        </label>

        <label> Tildel
          <select v-model="userId">
            <option :value="null">Ingen</option>
            <option v-for="user in familyMembers" :key="user.id" :value="user.id">
              {{ user.username }}
            </option>
          </select>
        </label>

        <label> Sidste dato
          <input type="date" v-model="expireDate">
        </label>

        <label> Belønning
          <input type="number" v-model.number="reward">
        </label>

      <div class="d-flex justify-content-end gap-2">
        <button type="button" @click="emit('close')">Luk</button>
        <button type="submit">{{ editing ? 'Gem ændringer' : 'Opret ny pligt' }}</button>
      </div>
      </form>
    </div>
  </div>
</template>

<style scoped>
.backdrop {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.content {
  background-color: var(--color-background-mute);
  padding: 2em;
  min-width: 1em;
}
</style>