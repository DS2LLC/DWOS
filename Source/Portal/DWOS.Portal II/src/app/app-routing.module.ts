import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";

import { ApiGuard as AuthGuard } from "./api-guard.service";
import { ChangePasswordComponent } from "./change-password.component";
import { ContactInfoComponent } from "./contact-info.component";
import { ForgotPasswordComponent } from "./forgot-password.component";
import { IndexComponent } from "./index.component";
import { LoginComponent } from "./login.component";
import { OrderApprovalComponent } from "./order-approval.component";
import { OrderApprovalsComponent } from "./order-approvals.component";
import { OrderComponent } from "./order.component";
import { PageNotFoundComponent } from "./page-not-found.component";

const routes: Routes = [
    { path: "login", component: LoginComponent },
    { path: "login/forgot_password", component: ForgotPasswordComponent },
    { path: "change_password", component: ChangePasswordComponent, canActivate: [AuthGuard] },
    { path: "order/:id", component: OrderComponent, canActivate: [AuthGuard] },
    { path: "approvals", component: OrderApprovalsComponent, canActivate: [AuthGuard] },
    { path: "approvals/:id", component: OrderApprovalComponent, canActivate: [AuthGuard] },
    { path: "contactinfo", component: ContactInfoComponent , canActivate: [AuthGuard] },
    { path: "", component: IndexComponent, canActivate: [AuthGuard] },
    { path: "**", component: PageNotFoundComponent },
];

/**
 * The main routing module.
 */
@NgModule({
    exports: [RouterModule],
    imports: [RouterModule.forRoot(routes)],
})
export class AppRoutingModule { }
