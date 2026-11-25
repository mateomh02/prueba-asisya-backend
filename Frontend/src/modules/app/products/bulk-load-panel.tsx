import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { useSWRConfig } from 'swr'
import { toast } from 'react-toastify'
import { Loader2, Zap } from 'lucide-react'
import { Button, Form } from '@/modules/core'
import { getApiErrorMessage } from '@/modules/core/lib/api-error'
import { InputField } from '@/modules/core/components'
import { bulkLoadSchema, MAX_BULK_COUNT, type BulkLoadFormValues } from './bulk-load-schema'
import { bulkLoadProducts } from './bulk-load-service'
import { isProductsKey } from './use-product-form'

export const BulkLoadPanel = () => {
  const { mutate } = useSWRConfig()
  const [isLoading, setIsLoading] = useState(false)

  const form = useForm<BulkLoadFormValues>({
    resolver: zodResolver(bulkLoadSchema),
    defaultValues: { count: '100000' }
  })

  const onSubmit = async (values: BulkLoadFormValues) => {
    setIsLoading(true)
    try {
      const result = await bulkLoadProducts(Number(values.count))
      await mutate(isProductsKey)
      toast.success(`Se insertaron ${result.inserted.toLocaleString('es-CO')} productos en ${(result.elapsedMilliseconds / 1000).toFixed(2)} s`)
    } catch (error) {
      toast.error(getApiErrorMessage(error))
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className='grid gap-4 rounded-2xl border border-border bg-muted p-5 sm:grid-cols-[1fr_auto] sm:items-end'>
        <div className='grid gap-2'>
          <InputField
            control={form.control}
            name='count'
            label={`Cantidad de productos genéricos (máximo ${MAX_BULK_COUNT.toLocaleString('es-CO')})`}
            placeholder='Ej: 100000'
          />
          <p className='text-xs text-muted-foreground'>
            Se generan con datos aleatorios y se asignan a las categorías SERVIDORES y CLOUD.
          </p>
        </div>
        <Button type='submit' variant='secondary' disabled={isLoading}>
          {isLoading ? <Loader2 className='animate-spin' /> : <Zap />}
          {isLoading ? 'Cargando…' : 'Cargar productos'}
        </Button>
      </form>
    </Form>
  )
}
