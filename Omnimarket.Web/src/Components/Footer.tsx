import LogoOmnimarket from "../assets/Logo_omnimarket.jpg";
import { FaFacebook, FaGithub, FaInstagram, FaTwitter, FaWhatsapp, FaYoutube } from "react-icons/fa";
import { FaLinkedin } from "react-icons/fa6";
export default function Footer() {

  return (
    <footer className="border-t border-white/10 bg-black text-white">
      <div className="mx-auto flex max-w-7xl flex-col gap-10 px-6 py-12 lg:flex-row lg:justify-between">

        {/* ESQUERDA */}
        <div className="max-w-sm space-y-4">
            <div className="flex items-center gap-4">

                <img
                    src={LogoOmnimarket}
                    alt="Logo do OmniMarket"
                    className="h-14 w-14 rounded-2xl object-cover hover:animate-spin"
                />
            
                <h2 className="text-2xl font-bold tracking-tight text-yellow-400">
                    Omnimarket
                </h2>
            </div>

          <p className="text-sm leading-6 text-neutral-400">
            Produtos de alta qualidade em um site com visual moderno, experiência elegante
            e com foco total na qualidade.
          </p>
        </div>

        {/* CENTRO */}
        <div className="grid grid-cols-2 gap-10 sm:grid-cols-3">

          <div className="space-y-4">
            <h3 className="text-sm font-semibold uppercase tracking-[0.2em] text-neutral-500">
              Omnimarket Inc.
            </h3>

            <ul className="grid grid-cols-3 gap-4 text-sm text-neutral-400 w-fit">
              <li>
                <a href="#" className="transition hover:text-yellow-300">
                  <FaInstagram className="h-5 w-5" />
                </a>
              </li>

              <li>
                <a href="#" className="transition hover:text-yellow-300">
                  <FaLinkedin className="h-5 w-5" />
                </a>
              </li>

              <li>
                <a href="#" className="transition hover:text-yellow-300">
                  <FaFacebook className="h-5 w-5" />
                </a>
              </li>

              <li>
                <a href="#" className="transition hover:text-yellow-300">
                  <FaWhatsapp className="h-5 w-5" />
                </a>
              </li>

              <li>
                <a href="#" className="transition hover:text-yellow-300">
                  <FaTwitter className="h-5 w-5" />
                </a>
              </li>

              <li>
                <a href="#" className="transition hover:text-yellow-300">
                  <FaGithub className="h-5 w-5" />
                </a>
              </li>

              <li>
                <a href="#" className="transition hover:text-yellow-300">
                  <FaYoutube className="h-5 w-5" />
                </a>
              </li>
            </ul>
          </div>

          <div className="space-y-4">
            <h3 className="text-sm font-semibold uppercase tracking-[0.2em] text-neutral-500">
              Suporte
            </h3>

            <ul className="space-y-3 text-sm text-neutral-400">
              <li>
                <a href="#" className="transition hover:text-yellow-300">
                  FAQ
                </a>
              </li>

              <li>
                <a href="#" className="transition hover:text-yellow-300">
                  Central de ajuda
                </a>
              </li>

              <li>
                <a href="#" className="transition hover:text-yellow-300">
                  Contato
                </a>
              </li>

              <li>
                <a href="#" className="transition hover:text-yellow-300">
                  Política de privacidade
                </a>
              </li>
            </ul>
          </div>

          <div className="space-y-4">
            <h3 className="text-sm font-semibold uppercase tracking-[0.2em] text-neutral-500">
              Participantes
            </h3>

            <ul className="space-y-3 text-sm text-neutral-400">
              <li>
                <p className="transition hover:text-yellow-300">
                  Felipe Sardinha Miguel
                </p>
              </li>

              <li>
                <p className="transition hover:text-yellow-300">
                  Gustavo Henrique da Silva
                </p>
              </li>

              <li>
                <p className="transition hover:text-yellow-300">
                  Icaro Dias Camargo
                </p>
              </li>

              <li>
                <p className="transition hover:text-yellow-300">
                  Lucas Chani Soiler
                </p>
              </li>

              <li>
                <p className="transition hover:text-yellow-300">
                  Mauro Alexandre da Silva Roque
                </p>
              </li>
            </ul>
          </div>

        </div>
      </div>

      {/* BOTTOM */}
      <div className="border-t border-white/10">
        <div className="mx-auto flex max-w-7xl flex-col items-center justify-between gap-4 px-6 py-5 text-sm text-neutral-500 sm:flex-row">
          
          <p>
            © 2026 Omnimarket. Todos os direitos reservados.
          </p>

          <p className="text-neutral-600">
            Desenvolvido com React + Tailwind
          </p>
        </div>
      </div>
    </footer>
  );
}