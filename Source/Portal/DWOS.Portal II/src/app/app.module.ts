import { HashLocationStrategy, Location, LocationStrategy } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { ErrorHandler, NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { BrowserModule, Title } from "@angular/platform-browser";

import { AllowDataPipe } from "./allow-data.pipe";
import { ApiGuard } from "./api-guard.service";
import { ApiService } from "./api.service";
import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { ChangePasswordComponent } from "./change-password.component";
import { ClientErrorHandler } from "./client-error-handler";
import { ContactInfoComponent } from "./contact-info.component";
import { DataUriPipe } from "./data-uri.pipe";
import { EqualFieldValidator } from "./equal-field-validator.directive";
import { FileService } from "./file.service";
import { ForgotPasswordComponent } from "./forgot-password.component";
import { HeaderComponent } from "./header.component";
import { httpInterceptorProviders } from "./http-interceptors";
import { IndexComponent } from "./index.component";
import { LoadingComponent } from "./loading.component";
import { LoginComponent } from "./login.component";
import { MomentDateFormatPipe } from "./moment-date-format.pipe";
import { NotificationService } from "./notification.service";
import { NotificationsComponent } from "./notifications.component";
import { OrderApprovalAcceptModalComponent } from "./order-approval-accept-modal.component";
import { OrderApprovalRejectModalComponent } from "./order-approval-reject-modal.component";
import { OrderApprovalComponent } from "./order-approval.component";
import { OrderApprovalsComponent } from "./order-approvals.component";
import { OrderSearchModalComponent } from "./order-search-modal.component";
import { OrderComponent } from "./order.component";
import { OrderService } from "./order.service";
import { PageNotFoundComponent } from "./page-not-found.component";
import { StaticPipe } from "./static.pipe";
import { TriggerValidationValidator } from "./trigger-validation.directive";
import { UrlService } from "./url.service";

// the order of these datatables.net imports matters
// for their type definitions to work properly
import "datatables.net";
import "datatables.net-bs4";
import "datatables.net-select";

import "bootstrap";
import "bootstrap/dist/css/bootstrap.css";
import "datatables.net-bs4/css/dataTables.bootstrap4.css";
import "datatables.net-select-bs4/css/select.bootstrap4.css";

import "@fortawesome/fontawesome-free/css/all.css";
import "../../stylesheets/main.css";

/**
 * The main application module.
 */
@NgModule({
    declarations: [
        AppComponent,
        LoadingComponent,
        IndexComponent,
        HeaderComponent,
        NotificationsComponent,
        LoginComponent,
        ForgotPasswordComponent,
        ChangePasswordComponent,
        OrderComponent,
        ContactInfoComponent,
        PageNotFoundComponent,
        OrderSearchModalComponent,
        OrderApprovalsComponent,
        OrderApprovalComponent,
        OrderApprovalAcceptModalComponent,
        OrderApprovalRejectModalComponent,
        AllowDataPipe,
        DataUriPipe,
        MomentDateFormatPipe,
        StaticPipe,
        EqualFieldValidator,
        TriggerValidationValidator,
    ],

    imports: [BrowserModule, HttpClientModule, FormsModule, AppRoutingModule],

    providers: [
        Location,
        { provide: LocationStrategy, useClass: HashLocationStrategy },
            // HashLocationStrategy makes it possible to serve with IIS w/o extensions
        { provide: ErrorHandler, useClass: ClientErrorHandler },
        Title,
        UrlService,
        ApiService,
        OrderService,
        ApiGuard,
        FileService,
        NotificationService,
        httpInterceptorProviders,
    ],

    bootstrap: [AppComponent],
})
export class AppModule { }
