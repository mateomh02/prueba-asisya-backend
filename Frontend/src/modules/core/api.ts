import axios from 'axios'
import { API_URL } from './lib/consts'
import { useAuthStore } from '@/modules/app/auth/auth-store'
import { expireSession } from '@/modules/app/auth/session'

export const api = axios.create({
  baseURL: API_URL
})

api.interceptors.request.use((config) => {
  const token = useAuthStore.getState().token
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (axios.isAxiosError(error) && error.response?.status === 401) {
      expireSession()
    }
    return Promise.reject(error)
  }
)
