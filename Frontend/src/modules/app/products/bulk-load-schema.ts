import { z } from 'zod'

export const MAX_BULK_COUNT = 1_000_000

export const bulkLoadSchema = z.object({
  count: z
    .string()
    .trim()
    .min(1, 'Indique cuántos productos desea cargar')
    .refine((v) => /^\d+$/.test(v) && Number(v) >= 1 && Number(v) <= MAX_BULK_COUNT, `Debe ser un número entre 1 y ${MAX_BULK_COUNT.toLocaleString('es-CO')}`)
})

export type BulkLoadFormValues = z.infer<typeof bulkLoadSchema>

export interface BulkLoadResult {
  inserted: number
  elapsedMilliseconds: number
}
