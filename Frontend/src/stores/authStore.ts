import { defineStore } from "pinia";
import router from "@/router";
import api from "../api.ts";
import { userStore } from "./userStore.ts";
import { jwtDecode } from "jwt-decode";

// Gets token, decodes it, parses as json and converts to ms, so we can use it
function getTokenExpiry(token: string): number {
    return jwtDecode<{exp: number}>(token).exp * 1000;
}

export const authStore = defineStore('auth', {
    state: () => ({
        token: localStorage.getItem('token') as string | null,
        refreshToken: localStorage.getItem('refreshToken') as string | null,
        refTokenTimer: null as ReturnType<typeof setTimeout> | null,
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
            this.startRefTimer()
        },
        updateToken(token: string) {
            this.token = token
            localStorage.setItem('token', token)
            this.startRefTimer()
        },
        logout() {
            this.stopRefTimer()
            this.token = null
            this.refreshToken = null
            localStorage.removeItem('token')
            localStorage.removeItem('refreshToken')
            userStore().clearUser()
            router.push({name: 'login'})
        },
        // Ask api for new JWT token, update current token - If error is caught (f.ex. not valid user), force logout
        useRefreshToken() {
            if (typeof this.refreshToken === 'string') {
                api.refreshJwtToken(this.refreshToken).then(token => {
                        this.updateToken(token.data)
                    }).catch(() => this.logout())
            }
        },
        // Starts timer, and calculates in how many ms, a new token should be created.
        // Waits the calculated delay/time, then asks API for a new token.
        startRefTimer() {
            this.stopRefTimer()
            if (!this.token) return

            const bufferInMs = 60000
            const delay = getTokenExpiry(this.token) - Date.now() - bufferInMs;

            this.refTokenTimer = setTimeout(() => {
                this.useRefreshToken()
            }, Math.max(delay, 0))
        },
        // If timer is running, clear it. Else do nothing.
        stopRefTimer() {
            if (this.refTokenTimer) {
                clearTimeout(this.refTokenTimer)
                this.refTokenTimer = null
            }
        }
    },
})