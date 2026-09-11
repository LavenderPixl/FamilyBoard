import axios from "axios";

const HTTP = axios.create({
    baseURL: import.meta.env.VITE_API_URL,
    timeout: 10000,
    headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${localStorage.getItem("token")}`,
    }
})

export default {
    login(email: string, password: string) {
        return HTTP.post('/auth/log-in', {
            email: email,
            password: password
        })
    },
    createUser(email: string, username: string, password: string) {
        return HTTP.post('/user/create-user', {
            email: email,
            username: username,
            password: password
        })
    }
}
