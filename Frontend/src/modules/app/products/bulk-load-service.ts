import { api } from '@/modules/core'
import type { BulkLoadResult } from './bulk-load-schema'

export const bulkLoadProducts = async (count: number): Promise<BulkLoadResult> => {
  const { data } = await api.post<BulkLoadResult>('/Products', { count })
  return data
}
