import { toast } from 'react-toastify'
import { useAuthStore } from './auth-store'

export const getTokenExpiry = (token: string): number | null => {
  try {
    const payload = token.split('.')[1]
    const base64 = payload.replace(/-/g, '+').replace(/_/g, '/')
    const json = JSON.parse(atob(base64)) as { exp?: number }
    return typeof json.exp === 'number' ? json.exp * 1000 : null
  } catch {
    return null
  }
}

export const expireSession = () => {
  const { token, logout } = useAuthStore.getState()
  if (!token) return
  logout()
  toast.info('Tu sesión venció. Inicia sesión nuevamente.')
  window.location.assign('/login')
}
