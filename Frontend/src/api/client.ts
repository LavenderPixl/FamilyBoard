import axios from 'axios'

const HTTP = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  timeout: 10000,
  headers: {    'Content-Type': 'application/json'},
})

HTTP.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) config.headers.Authorization = `Bearer ${token}`;
  else delete config.headers.Authorization
  return config
});

export { HTTP }