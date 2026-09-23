import { HTTP } from './client.ts'
import type { IGoal } from '../models/goal.ts'

export default {
  async getGoal(goalId : number): Promise<IGoal> {
    const response = await HTTP.get('/goal?goalId=' + goalId)

    return response.data
  }
}