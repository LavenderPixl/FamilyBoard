import {defineStore} from "pinia";
import type { IUser } from "../models/user.ts"
import api from "../api.ts"

export const userStore = defineStore("user", {
    state: () => ({
            user: null as IUser | null }),
    actions: {
        setUser(user: IUser) {
            this.user = user;
        },
        async getUser() {
            const res = await api.getLoggedInUser()
            this.user = res.data;
        },
        clearUser() {
            this.user = null;
        },
    },
})