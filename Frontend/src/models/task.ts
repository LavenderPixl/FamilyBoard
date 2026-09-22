export interface ITask {
  id: number;
  name: string;
  reward: number;
  completed: boolean;
  familyId: number;
  userId: number | null;
  username: string | null;
  goalId: number | null;
}