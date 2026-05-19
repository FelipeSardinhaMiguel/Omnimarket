import { PageLayout } from "../../Components/PageLayout";
import { useState } from "react";
import { useNavigate } from "@tanstack/react-router";
import { Input } from "../../Components/Input";
import { useCart } from "../../context/CartContext";

export function PagamentPage() {
    const [metodo, setMetodo] = useState("pix");
    const [freteSelecionado, setFreteSelecionado] = useState("padrao");

    const { carrinhoItens } = useCart();

    const navigate = useNavigate(); 

    const total = carrinhoItens.reduce((acc, item) => acc + item.preco * item.quantidade, 0);
    const opcoesEntrega = [
        {
            id: "padrao",
            titulo: "Entrega padrão",
            prazo: "Receba em até 5 dias úteis",
            valor: "Grátis",
        },
        {
            id: "expresso",
            titulo: "Entrega expressa",
            prazo: "Receba em até 2 dias úteis",
            valor: "R$ 19,90",
        },
        {
            id: "retirada",
            titulo: "Retirada na loja",
            prazo: "Disponível em 1 dia útil",
            valor: "Sem custo",
        },
    ];

    const metodosPagamento = [
        {
            id: "pix",
            titulo: "PIX",
            descricao: "Confirmação rápida após a finalização da compra.",
        },
        {
            id: "credito",
            titulo: "Cartão de crédito",
            descricao: "Ideal para parcelamento e pagamentos online.",
        },
        {
            id: "debito",
            titulo: "Cartão de débito",
            descricao: "Débito imediato com validação simples.",
        },
        {
            id: "boleto",
            titulo: "Boleto bancário",
            descricao: "Pagamento com vencimento após a emissão.",
        },
    ];

    return (
        <PageLayout>
            <div className="min-h-screen bg-black text-white px-4 py-8">
                <div className="mx-auto max-w-7xl grid gap-8 lg:grid-cols-3">

                    {/* ESQUERDA */}
                    <div className="lg:col-span-2 space-y-6">

                        {/* Bloco principal do checkout: organiza o fluxo em cards para deixar a leitura mais clara e profissional. */}
                        <section className="rounded-2xl border border-white/10 bg-zinc-900/80 p-5 shadow-[0_0_0_1px_rgba(255,255,255,0.02)] sm:p-6">
                            <div className="mb-6 flex flex-col gap-2 border-b border-white/10 pb-5">
                                <span className="text-xs font-semibold uppercase tracking-[0.24em] text-yellow-400/80">
                                    Etapa 1
                                </span>
                                <h2 className="text-2xl font-semibold text-white">Endereco de entrega</h2>
                                <p className="max-w-2xl text-sm leading-6 text-zinc-400">
                                    Preencha os dados abaixo para garantir uma entrega mais rapida e sem divergencias no pedido.
                                </p>
                            </div>

                            {/* Grid responsivo: no mobile os campos ficam empilhados; em telas maiores o formulario ganha melhor aproveitamento horizontal. */}
                            <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                                <Input
                                    id="cep"
                                    name="cep"
                                    label="CEP"
                                    placeholder="00000-000"
                                    autoComplete="postal-code"
                                    className="h-12 rounded-2xl border-white/10 bg-black/40 px-4 focus:border-yellow-400 focus:ring-2 focus:ring-yellow-400/20"
                                />
                                <Input
                                    id="cidade"
                                    name="cidade"
                                    label="Cidade"
                                    placeholder="Sua cidade"
                                    autoComplete="address-level2"
                                    className="h-12 rounded-2xl border-white/10 bg-black/40 px-4 focus:border-yellow-400 focus:ring-2 focus:ring-yellow-400/20"
                                />
                                <div className="sm:col-span-2">
                                    <Input
                                        id="rua"
                                        name="rua"
                                        label="Rua"
                                        placeholder="Nome da rua, avenida ou alameda"
                                        autoComplete="address-line1"
                                        className="h-12 rounded-2xl border-white/10 bg-black/40 px-4 focus:border-yellow-400 focus:ring-2 focus:ring-yellow-400/20"
                                    />
                                </div>
                                <Input
                                    id="numero"
                                    name="numero"
                                    label="Numero"
                                    placeholder="Ex.: 123"
                                    inputMode="numeric"
                                    autoComplete="address-line2"
                                    className="h-12 rounded-2xl border-white/10 bg-black/40 px-4 focus:border-yellow-400 focus:ring-2 focus:ring-yellow-400/20"
                                />
                                <Input
                                    id="complemento"
                                    name="complemento"
                                    label="Complemento"
                                    placeholder="Apartamento, bloco, referencia"
                                    autoComplete="additional-name"
                                    className="h-12 rounded-2xl border-white/10 bg-black/40 px-4 focus:border-yellow-400 focus:ring-2 focus:ring-yellow-400/20"
                                />
                            </div>
                        </section>

                        {/* Fieldset melhora a semantica e ajuda na acessibilidade ao agrupar as opcoes de entrega. */}
                        <section className="rounded-2xl border border-white/10 bg-zinc-900/80 p-5 shadow-[0_0_0_1px_rgba(255,255,255,0.02)] sm:p-6">
                            <fieldset className="space-y-4">
                                <div className="mb-2 flex flex-col gap-2 border-b border-white/10 pb-5">
                                    <span className="text-xs font-semibold uppercase tracking-[0.24em] text-yellow-400/80">
                                        Etapa 2
                                    </span>
                                    <legend className="text-2xl font-semibold text-white">Opcoes de entrega</legend>
                                    <p className="text-sm leading-6 text-zinc-400">
                                        Escolha a modalidade que melhor se encaixa no seu prazo e preferencia de recebimento.
                                    </p>
                                </div>

                                {/* Cada label funciona como um card clicavel para ampliar a area de interacao e melhorar o toque no mobile. */}
                                <div className="grid gap-3">
                                    {opcoesEntrega.map((opcao) => {
                                        const selecionado = freteSelecionado === opcao.id;

                                        return (
                                            <label
                                                key={opcao.id}
                                                htmlFor={opcao.id}
                                                className={`flex cursor-pointer items-start gap-4 rounded-2xl border p-4 transition duration-200 hover:border-yellow-400/60 hover:bg-black/30 ${
                                                    selecionado
                                                        ? "border-yellow-400 bg-yellow-400/10 shadow-[0_0_0_1px_rgba(250,204,21,0.20)]"
                                                        : "border-white/10 bg-black/20"
                                                }`}
                                            >
                                                {/* Radio dourado reforca visualmente a selecao sem depender apenas da borda do card. */}
                                                <input
                                                    id={opcao.id}
                                                    type="radio"
                                                    name="frete"
                                                    checked={selecionado}
                                                    onChange={() => setFreteSelecionado(opcao.id)}
                                                    className="mt-1 h-4 w-4 border-white/20 bg-transparent text-yellow-400 focus:ring-2 focus:ring-yellow-400/30"
                                                />

                                                <div className="flex flex-1 flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
                                                    <div>
                                                        <p className="font-semibold text-white">{opcao.titulo}</p>
                                                        <p className="text-sm text-zinc-400">{opcao.prazo}</p>
                                                    </div>
                                                    <span className="text-sm font-semibold text-yellow-400">{opcao.valor}</span>
                                                </div>
                                            </label>
                                        );
                                    })}
                                </div>
                            </fieldset>
                        </section>

                        {/* Secao de pagamento com destaque mais claro entre estados normal, hover, foco e selecionado. */}
                        <section className="rounded-2xl border border-white/10 bg-zinc-900/80 p-5 shadow-[0_0_0_1px_rgba(255,255,255,0.02)] sm:p-6">
                            <div className="mb-6 flex flex-col gap-2 border-b border-white/10 pb-5">
                                <span className="text-xs font-semibold uppercase tracking-[0.24em] text-yellow-400/80">
                                    Etapa 3
                                </span>
                                <h2 className="text-2xl font-semibold text-white">Forma de pagamento</h2>
                                <p className="text-sm leading-6 text-zinc-400">
                                    Selecione a forma de pagamento desejada para concluir seu checkout com seguranca.
                                </p>
                            </div>

                            {/* O grid quebra em duas colunas no desktop para evitar uma lista longa demais e manter boa escaneabilidade. */}
                            <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
                                {metodosPagamento.map((tipo) => {
                                    const selecionado = metodo === tipo.id;

                                    return (
                                        <button
                                            key={tipo.id}
                                            type="button"
                                            onClick={() => setMetodo(tipo.id)}
                                            aria-pressed={selecionado}
                                            className={`rounded-2xl border p-4 text-left transition duration-200 hover:border-yellow-400/60 hover:bg-black/30 focus:outline-none focus:ring-2 focus:ring-yellow-400/30 ${
                                                selecionado
                                                    ? "border-yellow-400 bg-yellow-400/10 shadow-[0_0_0_1px_rgba(250,204,21,0.20)]"
                                                    : "border-white/10 bg-black/20"
                                            }`}
                                        >
                                            <div className="flex items-start justify-between gap-3">
                                                <div>
                                                    <p className="font-semibold text-white">{tipo.titulo}</p>
                                                    <p className="mt-1 text-sm leading-6 text-zinc-400">{tipo.descricao}</p>
                                                </div>
                                                <span
                                                    className={`mt-1 h-3 w-3 rounded-full transition ${
                                                        selecionado ? "bg-yellow-400 shadow-[0_0_12px_rgba(250,204,21,0.65)]" : "bg-white/20"
                                                    }`}
                                                />
                                            </div>
                                        </button>
                                    );
                                })}
                            </div>

                            {/*Caso o metodo de pagamento for no credito, aparecera dois inputs para a coleta de dados do cartao */}
                            {metodo === "credito" && (
                                <div className="mt-5 grid grid-cols-1 gap-4 rounded-2xl border border-yellow-400/20 bg-black/30 p-4 sm:grid-cols-2">
                                    {/* Area condicional do cartao: aparece apenas quando faz sentido, reduzindo ruido visual no restante do checkout. */}
                                    <div className="sm:col-span-2">
                                        <Input
                                            required
                                            id="numero-cartao"
                                            name="numero-cartao"
                                            label="Numero do cartao"
                                            placeholder="0000 0000 0000 0000"
                                            inputMode="numeric"
                                            autoComplete="cc-number"
                                            className="h-12 rounded-2xl border-white/10 bg-black/40 px-4 focus:border-yellow-400 focus:ring-2 focus:ring-yellow-400/20"
                                        />
                                    </div>
                                    <Input
                                        required
                                        id="validade-cartao"
                                        name="validade-cartao"
                                        label="Validade"
                                        placeholder="MM/AA"
                                        inputMode="numeric"
                                        autoComplete="cc-exp"
                                        className="h-12 rounded-2xl border-white/10 bg-black/40 px-4 focus:border-yellow-400 focus:ring-2 focus:ring-yellow-400/20"
                                    />
                                    <Input
                                        required
                                        id="nome-cartao"
                                        name="nome-cartao"
                                        label="Nome impresso no cartao"
                                        placeholder="Como aparece no cartao"
                                        autoComplete="cc-name"
                                        className="h-12 rounded-2xl border-white/10 bg-black/40 px-4 focus:border-yellow-400 focus:ring-2 focus:ring-yellow-400/20"
                                    />
                                </div>
                            )}

                            {/*Caso o metodo de pagamento for no pix, aparecera um QRCode paga escaneamento.*/}
                            {metodo === "pix" && (
                                <div className="mt-5 rounded-2xl border border-yellow-400/20 bg-black/30 p-4">
                                    <p className="text-sm leading-6 text-neutral-400">
                                        O QR Code sera gerado apos finalizar a compra.
                                    </p>
                                </div>
                            )}
                        </section>



                    </div>

                    {/* DIREITA - resumo do pedido realizado*/}
                    <div className="lg:col-span-1">
                        <aside className="border border-white/10 rounded-2xl p-5 space-y-4">

                            <h2 className="text-xl font-semibold">Resumo do pedido</h2>

                            {carrinhoItens.map((item) => (
                                <div key={item.id} className="flex gap-3">
                                    <img src={item.imagem} className="w-16 h-16 rounded-2xl" />

                                    <div className="flex-1 py-2">
                                        <p className="text-[16px] pb-2">{item.nome}</p>
                                        <p className="font-semibold">Valor unitário: {item.preco}</p>
                                        <p className="">Quantidade: {item.quantidade}</p>
                                        <p className="text-yellow-400">Valor total: {item.quantidade * item.preco}</p>

                                    </div>
                                </div>
                            ))}

                            <div className="border-t border-white/10 pt-4">
                                <div className="flex font-bold text-[30px]">
                                    <p className="px-3">Total: </p>
                                    <p className="text-emerald-300">R$ {total.toFixed(2)}</p>
                                </div>
                            </div>

                            <button
                            className="w-full bg-yellow-400 text-black py-3 rounded-xl"
                            onClick={() => navigate({
                                to:"/paginaSucesso"
                            })}>
                                Finalizar compra
                            </button>

                        </aside>
                    </div>

                </div>
            </div>
        </PageLayout>
    )
}

