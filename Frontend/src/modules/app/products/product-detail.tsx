import { Link, useParams } from 'react-router-dom'
import useSWR from 'swr'
import { ArrowLeft, Pencil } from 'lucide-react'
import { Button } from '@/modules/core'
import { getApiErrorMessage } from '@/modules/core/lib/api-error'
import { useRole } from '@/modules/app/auth'
import type { ProductDetail as ProductDetailData } from './product-types'

const money = new Intl.NumberFormat('es-CO', { style: 'currency', currency: 'COP', maximumFractionDigits: 0 })

const Field = ({ label, value }: { label: string; value: string | number | null | undefined }) => (
  <div className='grid gap-1'>
    <span className='text-xs font-semibold uppercase text-muted-foreground'>{label}</span>
    <span className='text-base text-foreground'>{value ?? '-'}</span>
  </div>
)

export const ProductDetailView = () => {
  const { id } = useParams()
  const productId = Number(id)
  const { isAdmin } = useRole()
  const { data: product, error, isLoading } = useSWR<ProductDetailData>(
    Number.isInteger(productId) ? `/Products/${productId}` : null
  )

  if (isLoading) return <p className='text-muted-foreground'>Cargando producto…</p>
  if (error || !product) return <p className='text-destructive'>{getApiErrorMessage(error, 'Producto no encontrado.')}</p>

  return (
    <section className='grid gap-6'>
      <div className='flex flex-wrap items-center justify-between gap-4'>
        <Button asChild variant='ghost'>
          <Link to='/products'>
            <ArrowLeft /> Volver a productos
          </Link>
        </Button>
        {isAdmin && (
          <Button asChild variant='outline'>
            <Link to={`/products/${product.id}/edit`}>
              <Pencil /> Editar
            </Link>
          </Button>
        )}
      </div>

      <div className='grid gap-6 lg:grid-cols-[1fr_1.5fr]'>
        <div className='grid content-start gap-4 rounded-2xl border border-border bg-background p-6 shadow-sm'>
          {product.categoryPicture ? (
            <img
              src={product.categoryPicture}
              alt={product.categoryName}
              loading='lazy'
              decoding='async'
              className='aspect-video w-full rounded-xl object-cover'
            />
          ) : (
            <div className='grid aspect-video place-items-center rounded-xl bg-muted text-muted-foreground'>Sin imagen</div>
          )}
          <span className='text-sm font-semibold text-brand-magenta-600'>{product.categoryName}</span>
        </div>

        <div className='grid gap-6 rounded-2xl border border-border bg-background p-6 shadow-sm'>
          <div className='grid gap-2'>
            <h1 className='text-3xl font-black text-foreground'>{product.productName}</h1>
            <span className={product.discontinued ? 'text-brand-coral-700' : 'text-brand-purple-600'}>
              {product.discontinued ? 'Descontinuado' : 'Activo'}
            </span>
          </div>

          <div className='grid gap-6 sm:grid-cols-2'>
            <Field label='Precio' value={money.format(product.unitPrice)} />
            <Field label='Existencias' value={product.unitsInStock} />
            <Field label='En pedido' value={product.unitsOnOrder} />
            <Field label='Nivel de reorden' value={product.reorderLevel} />
            <Field label='Presentación' value={product.quantityPerUnit} />
            <Field label='Proveedor' value={product.supplierId} />
            <Field label='Origen' value={product.source === 'BulkLoad' ? 'Carga masiva' : 'Manual'} />
          </div>
        </div>
      </div>
    </section>
  )
}
