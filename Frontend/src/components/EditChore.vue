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
      <div class="header">
        <h2> {{ editing ? 'Rediger pligt' : 'Opret ny pligt'}}</h2>
      </div>

      <form class="body" @submit.prevent="submit">
        <div class="field-row">
          <label for="name">Pligt</label>
          <input id="name" type="text" v-model="name" required class="field">
        </div>

        <div class="field-row">
          <label for="userId">Tildel</label>
          <select id="userId" v-model="userId" class="field">
            <option :value="null">Ingen</option>
            <option v-for="user in familyMembers" :key="user.id" :value="user.id">
              {{ user.username }}
            </option>
          </select>
        </div>

        <div class="field-row">
          <label for="reward">Point</label>
          <input id="reward" type="number" v-model.number="reward" class="field">
        </div>

        <div class="field-row">
          <label for="expireDate">Sidste dato</label>
          <input id="expireDate" type="date" v-model="expireDate" class="date-field">
        </div>

      <div class="actions">
        <button type="button" class="action-btn" @click="emit('close')">Luk</button>
        <button type="submit" class="action-btn">{{ editing ? 'Gem ændringer' : 'Opret ny pligt' }}</button>
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
  padding: 1.5em;
  width: 25em;
}

.body{
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 1.30rem;
}

.field-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.7rem;
}

.field-row label {
  color: var(--color-text);
  flex-shrink: 0;
}

.field {
  width: 13rem;
  padding: 0.5rem;
  border: none;
}

.actions {
  display: flex;
  gap: 0.8rem;
  margin-top: 0.3rem;
}

.action-btn {
  flex: 1;
  padding: 0.3rem;
  border: none;
}

</style>