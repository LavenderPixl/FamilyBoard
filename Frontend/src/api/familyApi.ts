import { HTTP } from './client.ts'
import type {IUser} from "@/models/user.ts";

export default {
  joinFamily(familyCode: string){
    return HTTP.patch('/family/join-family?inviteCode=' + familyCode)
  },

  leaveFamily(familyId: number) {
    return HTTP.patch('/family/leave-family?familyId='+ familyId)
  },

  kickFamilyMember(familyId: number, userId: number) {
    return HTTP.patch('/family/kick-family-member?familyId='+familyId+'&userId='+userId)
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
  },

  async createFamilyInvite(): Promise<string> {
    const response = await HTTP.post('/family/generate-invite')
    return response.data
  }
}
