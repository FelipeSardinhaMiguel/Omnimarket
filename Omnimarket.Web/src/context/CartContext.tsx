/*Esse arquivo serve para que o carrinho possa ter o acesso a todas as páginas dos produtos ao invés de ter em apenas uma (caso colocássemos ela no CarrinhoPage). 
Se não tiver isso, quando o user add um produto no carrinho, for sair da pagina de produto e tentar add outro produto, o 1º produto não salvará
(Você adiciona produto → vai para carrinho → volta → perde tudo.)*/

import { createContext, useContext, useState } from "react";
import type { HomeProduct } from "../types/home"

export type CartItemType = HomeProduct & {
    quantidade: number;
}

type CartContextType = {
    carrinhoItens: CartItemType[],
    adicionarAoCarrinho: (produto: HomeProduct) => void;
    increaseQuantity: (id: string) => void;
    decreaseQuantity: (id: string) => void;
    removeItem: (id: string) => void;
};

const CartContext = createContext({} as CartContextType);

export function CartProvider({
    children,
}: {
    children: React.ReactNode;
}) {
    const [carrinhoItens, setCarrinhoItens] = useState<CartItemType[]>([]);

    function adicionarAoCarrinho(produto: HomeProduct){
        setCarrinhoItens((prev) => {
            const produtoExistente = prev.find((item) =>item.id === produto.id);

            //para verificar se o produto já foi adicionado no carrinho
            if (produtoExistente) {
                return prev.map((item) => item.id === produto.id? {...item, quantidade: item.quantidade +1}
            : item);
            }

            //se for um novo produto adicionado no carrinho
            return[
                ...prev,
                {
                    ...produto,
                    quantidade: 1,
                },
            ];
        });
    }

    function increaseQuantity(id: string){
        setCarrinhoItens((prev) => prev.map((item) => item.id === id? {...item, quantidade: item.quantidade + 1}: item).filter((item) => item.quantidade > 0));
    }

    function decreaseQuantity(id: string){
        setCarrinhoItens((prev) => prev.map((item) => item.id === id? {...item, quantidade: item.quantidade - 1}: item).filter((item) => item.quantidade > 0));
    }

    function removeItem(id: string){
        setCarrinhoItens((prev) => prev.filter((item) => item.id !== id));
    }

    return(
        <CartContext.Provider
        value={{
            carrinhoItens,
            adicionarAoCarrinho,
            increaseQuantity,
            decreaseQuantity,
            removeItem
        }}>
            {children}
        </CartContext.Provider>
    );
}

export function useCart(){
    return useContext(CartContext);
}