//Pagina destina ao ponto de luz amarelo que aparece nos principais containers do nosso site
//Para utilizar este tem que obrigatoriamente ter {relative overflow-hidden} no classname, caso seja um section é apenas adicionar
//caso seja outro componente, por ex: div, banner e etc, crie uma div envoltando tudo e coloque apenas estes componentes e este envoltado pelo spotlight

import { useRef } from "react";

type Props = {
  children: React.ReactNode;
};

export function Spotlight({ children }: Props) {
  const ref = useRef<HTMLDivElement>(null);

  const handleMouseMove = (e: React.MouseEvent) => {
    const rect = ref.current?.getBoundingClientRect();
    if (!rect) return;

    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;

    ref.current?.style.setProperty("--x", `${x}px`);
    ref.current?.style.setProperty("--y", `${y}px`);
  };

  return (
    <div
      ref={ref}
      onMouseMove={handleMouseMove}
      className="group relative overflow-hidden rounded-[32px]"
    >
      {/* Luz */}
      <div
        className="pointer-events-none absolute inset-0 z-10 opacity-0 blur-3x1 transition-opacity duration-300 group-hover:opacity-100"
        style={{
          background: `radial-gradient(circle at var(--x) var(--y), rgba(250,204,21,0.20), transparent 40%)`,
        }}
      />

      {children}
    </div>
  );
}