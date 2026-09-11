import {defineStore} from "pinia";
import api from "../api.ts";

export const authStore = defineStore('auth', {
    state: () => ({
        token: localStorage.getItem('token') as string | null,
        refreshToken: localStorage.getItem('refreshToken') as string | null,
        refTokenTime: null
    }),
    getters: {
        isLoggedIn: (state) => !!state.token,
    },
    actions: {
        setToken (token: string, refreshToken: string) {
            this.token = token
            this.refreshToken = refreshToken
            localStorage.setItem('token', token)
            localStorage.setItem('refreshToken', refreshToken)
        },
        updateToken(token: string) {
            this.token = token
            localStorage.setItem('token', token)
        },
        logout() {
            this.token = null
            this.refreshToken = null
            localStorage.removeItem('token')
            localStorage.removeItem('refreshToken')
        },
        useRefreshToken() {
            if (typeof this.refreshToken === 'string') {
                api.refreshJwtToken(this.refreshToken).then(token => {
                        this.updateToken(token.data)
                    })
            }
        },
        startRefTimer() {

        }
        // startRefTimer
        // stopRefTimer
    },
})