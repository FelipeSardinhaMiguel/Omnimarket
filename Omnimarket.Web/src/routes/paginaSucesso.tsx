import { createFileRoute } from '@tanstack/react-router'
import { SuccessPage } from '../pages/Pedido/SucessPage'

export const Route = createFileRoute('/paginaSucesso')({
  component: RouteComponent,
})

function RouteComponent() {
  return <SuccessPage/>
}
