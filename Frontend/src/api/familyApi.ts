import { HTTP } from './client.ts'

export default {
  joinFamily(familyCode: string){
    return HTTP.patch('/family/join-family?inviteCode=' + familyCode)
  },

  createFamily(familyName: string) {
    return HTTP.post('/family/create-family?familyName=' + familyName)
  }
}
