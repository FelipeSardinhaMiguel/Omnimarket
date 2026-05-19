import { createFileRoute } from '@tanstack/react-router'
import { CadastroPage } from '../pages/Auth/CadastrePage'

export const Route = createFileRoute('/cadastro')({
  component: RouteComponent,
})

function RouteComponent() {
  return <CadastroPage />
}
