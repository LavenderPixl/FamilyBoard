export interface ITask {
  id: number
  name: string
  reward: number
  completed: boolean
  expireDate: string
  familyId: number
  userId: number | null
  username: string | null
  goalId: number | null
}