import { createFileRoute } from '@tanstack/react-router'
import { PerfilUsuarioPage } from '../pages/User/ProfileUserPage'

export const Route = createFileRoute('/perfilUsuario')({
  component: RouteComponent,
})

function RouteComponent() {
  return <PerfilUsuarioPage/>
}
