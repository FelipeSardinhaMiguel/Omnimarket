import { createFileRoute } from '@tanstack/react-router'
import { ProdutoPage } from '../../pages/Produto/ProdutoPage'

/*Essa root ($id.tsx) serve para pegar o id do produto que vem da ProdutoPage (que a mesma pega de ProductCard) e adapta a rota com base no produto clicado na home*/
export const Route = createFileRoute('/produto/$id')({
  component: ProdutoPage,
})

/*function RouteComponent() {
  return(
    <PageLayout>
      <div className='text-lime-600'>Hello "/produto/$id"!</div>
      <ProdutoPage/>
    </PageLayout>
  )
}*/