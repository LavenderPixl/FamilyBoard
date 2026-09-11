import {defineStore} from "pinia";
import type { User } from '../models/user.ts'

export const userStore = defineStore("userStore", {
    state: () => ({
            user: localStorage.getItem('user') as User | null,
        }),
    actions: {

    }
})