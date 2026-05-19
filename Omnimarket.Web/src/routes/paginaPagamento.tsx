import { createFileRoute } from '@tanstack/react-router'
import { PagamentPage } from '../pages/Pedido/PagamentPage'

export const Route = createFileRoute('/paginaPagamento')({
  component: RouteComponent,
})

function RouteComponent() {
  return <PagamentPage/>
}
