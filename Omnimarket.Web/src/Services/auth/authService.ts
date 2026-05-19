const API_URL = "https://localhost:7194/api/auth";

export async function loginUsuario(email, senha) {
    try {
        const response = await fetch(`${API_URL}/login`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                email,
                senha,
            }),
        });

        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.mensagem || "Erro ao realizar login");
        }

        return data;
    } catch (error) {
        console.error("Erro no login:", error.message);
        throw error;
    }

    
}