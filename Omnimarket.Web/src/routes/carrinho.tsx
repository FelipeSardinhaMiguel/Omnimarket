import { createFileRoute } from '@tanstack/react-router'
import { CarrinhoPage } from '../pages/Carrinho/CarrinhoPage'

export const Route = createFileRoute('/carrinho')({
  component: CarrinhoPage,
})