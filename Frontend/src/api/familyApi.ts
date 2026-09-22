import { HTTP } from './client.ts'
import type {IUser} from "@/models/user.ts";

export default {
  joinFamily(familyCode: string){
    return HTTP.patch('/family/join-family?inviteCode=' + familyCode)
  },

  createFamily(familyName: string) {
    return HTTP.post('/family/create-family?familyName=' + familyName)
  },

  async getFamilyName(): Promise<string> {
    const response = await HTTP.get('/family/get-family-name')

    return response.data
  },

  async getFamilyMembers(): Promise<IUser[]> {
    const response = await HTTP.get<IUser[]>('/family/get-family-members')
    return response.data
  }
}
