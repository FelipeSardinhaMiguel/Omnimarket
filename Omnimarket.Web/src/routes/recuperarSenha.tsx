import { createFileRoute } from "@tanstack/react-router";
import { ForgetPasswordPage } from "../pages/Auth/ForgetPasswordPage";

export const Route = createFileRoute('/recuperarSenha')({
    component: RouteComponent,
})

function RouteComponent(){
    return <ForgetPasswordPage/>
}