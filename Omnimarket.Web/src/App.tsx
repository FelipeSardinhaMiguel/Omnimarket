//import utilizado no botãao de adicionar produtos ao carrinho na product page
import { Toaster } from "react-hot-toast";

function App() {
  return (
    <div className="flex min-h-screen items-center justify-center bg-black px-6 text-center text-white">
      <Toaster/>
      <p>O aplicativo usa o roteador em `src/main.tsx`.</p>
    </div>
  );
}

export default App;
