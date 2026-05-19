import { Outlet, createRootRoute, useLocation } from '@tanstack/react-router'
import AppHeader from '../Components/AppHeader'
import Footer from '../Components/Footer'

export const Route = createRootRoute({
  component: RootComponent,
})

function RootComponent() {
  //Isso guarda numa variaval a rota atual que o user está para a verificação a seguir
  const urlAtual = useLocation();

  //aqui ele confere se a rota que está na urlAtual (a variavel) coincide com a rota que ele está comparando
  const esconderFooter = urlAtual.pathname === '/login' || urlAtual.pathname === '/cadastro' || urlAtual.pathname === '/recuperarSenha';

  //caso a variavel esconderFooter esteja com uma das rotas acima, o footer não será exibido na tela
  return (
      <>
      <AppHeader />
      <Outlet />
      {!esconderFooter && <Footer />}
    </>
  )
}
