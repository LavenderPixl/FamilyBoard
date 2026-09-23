import { HTTP } from "./client.ts"
import type {IUser} from "@/models/user.ts";

export default {
    login(email: string, password: string) {
        return HTTP.post('/auth/log-in', {
            email: email,
            password: password
        })
    },
    createUser(email: string, username: string, password: string) {
        return HTTP.post('/user', {
            email: email,
            username: username,
            password: password
        })
    },
    getLoggedInUser() {
      return HTTP.get(`/user/get-logged-in-user`);
    },
    async changeUserType(userId: number, isAdult: boolean): Promise <IUser> {
        const response = await HTTP.patch('/user/update-adult-status?userId='+ userId, {
            isAdult: isAdult
        })
        return response.data
    },
    refreshJwtToken(refreshToken: string) {
        return HTTP.post(`/auth/refresh-jwt`, refreshToken)
    },
}
