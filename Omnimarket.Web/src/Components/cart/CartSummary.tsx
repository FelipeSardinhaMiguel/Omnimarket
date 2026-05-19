type CartSummaryProps = {
  subtotal: number;
};

const currencyFormatter = new Intl.NumberFormat("pt-BR", {
  style: "currency",
  currency: "BRL",
});

export function CartSummary({ subtotal }: CartSummaryProps) {
  return (
    <section className="rounded-[28px] border border-white/10 bg-[linear-gradient(180deg,_rgba(255,255,255,0.05),_rgba(255,255,255,0.02))] p-5 shadow-[0_24px_60px_rgba(0,0,0,0.35)] sm:p-6">
      <div className="space-y-4">
        <div className="flex items-center justify-between gap-4 text-sm text-neutral-400">
          <span>Subtotal</span>
          <span className="font-medium text-neutral-200">
            {currencyFormatter.format(subtotal)}
          </span>
        </div>

        <div className="flex items-center justify-between gap-4 text-sm text-neutral-400">
          <span>Frete</span>
          <span className="font-medium text-emerald-300">Gratis</span>
        </div>

        <div className="h-px w-full bg-white/10" />

        <div className="flex items-end justify-between gap-4">
          <div className="space-y-1">
            <p className="text-sm text-neutral-400">Total</p>
            <p className="text-3xl font-bold tracking-tight text-yellow-400">
              {currencyFormatter.format(subtotal)}
            </p>
          </div>

          <p className="text-xs uppercase tracking-[0.22em] text-neutral-500">
            Pagamento seguro
          </p>
        </div>
      </div>
    </section>
  );
}
