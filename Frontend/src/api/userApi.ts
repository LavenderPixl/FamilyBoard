import { HTTP } from "./client.ts"

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
    refreshJwtToken(refreshToken: string) {
        return HTTP.post(`/auth/refresh-jwt`, refreshToken)
    },
}
