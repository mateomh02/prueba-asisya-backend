import { useEffect } from 'react'
import { useAuthStore } from './auth-store'
import { expireSession, getTokenExpiry } from './session'

export const useSessionExpiry = () => {
  const token = useAuthStore((s) => s.token)

  useEffect(() => {
    if (!token) return

    const expiry = getTokenExpiry(token)
    if (expiry === null) return

    const remaining = expiry - Date.now()
    if (remaining <= 0) {
      expireSession()
      return
    }

    const timer = setTimeout(expireSession, remaining)
    return () => clearTimeout(timer)
  }, [token])
}
