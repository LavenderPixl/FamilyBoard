import {defineStore} from "pinia";

export const useAuth = defineStore('auth', {
    state: () => ({
        token: localStorage.getItem('token') as string | null,
    }),
    getters: {
        //
        isLoggedIn: (state) => !!state.token,
    },
    actions: {
        setToken: (token: string) => {
            // @ts-ignore
            this.token = token
            localStorage.setItem('token', token)
        },
        logout() {
            this.token = null
            localStorage.removeItem('token')
        },
    },
})