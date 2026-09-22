import { HTTP } from './client.ts'
import type { ITask } from '@/models/task.ts'

export default {
  async getTasksForUser(userId: number): Promise<ITask[]> {
    const response = await HTTP.get<ITask[]>('/task/get-tasks-for-user?userId=' + userId)

    return response.data
  },

  async markTaskAsCompleted(task: ITask): Promise<ITask> {
    console.log(task)
    const response = await HTTP.patch('/task/mark-task-as-completed?id=' + task.id)

    return response.data
  },

  async unMarkTaskAsCompleted(task: ITask): Promise<ITask> {
    console.log(task)
    const response = await HTTP.patch('/task/unmark-task-as-completed?id=' + task.id)

    return response.data
  },

  async getTasksForFamily(): Promise<ITask[]> {
    const response = await HTTP.get<ITask[]>('/task/get-tasks-for-your-family')

    return response.data
  },

  async claimTask(task: ITask): Promise<ITask> {
    const response = await HTTP.patch<ITask>('/task/claim-task?taskId=' + task.id)

    return response.data
  }
}
