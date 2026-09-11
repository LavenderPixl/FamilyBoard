import {defineStore} from "pinia";
import type { User } from "../models/user.ts"
import api from "../api.ts"

export const user = defineStore("user", {
    state: () => ({
            user: null as null | {
                userId: number;
                username: string;
                email: string;
                points: number;
                isAdult: boolean;
                familyId: number;
            },
    }),
    actions: {
        setUser(user) {
            this.user = user;
        },
        async getUser() {
            const res = await api.getLoggedInUser()
            this.user = res.data;
        },
        clear() {
            this.user = null;
        },
    },
})