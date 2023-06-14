import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { ApiService, ApiServiceError } from "./api.service";

/**
 * Component that prompts users to reset passwords.
 */
@Component({
    selector: "dwos-forgot-password",
    styles: [
        "form { max-width: 250px; }",
    ],
    template: `
        <div class='container-fluid'>
          <h2>Forgot your password?</h2>

          <div [hidden]="success">
            <p>We'll send you a new one, just provide us with the email address you use to login.</p>
          </div>

          <div [hidden]="!success" class='alert alert-success'>
            If you have a customer portal account at {{email}}, a new password will be sent to you.
          </div>

          <div [hidden]="!error" class='alert alert-danger'>
            {{error}}
          </div>

          <form (ngSubmit)="submit()" #forgotPasswordForm="ngForm" [hidden]="success">
            <div class='form-group'>
              <label for="email">E-mail:</label>
              <input type="email" id="email" required name="email" [(ngModel)]="email" class='form-control'>
            </div>

            <button type="submit" [disabled]="!forgotPasswordForm.form.valid" class='btn btn-primary'>Submit</button>
          </form>

          <p><a routerLink="/login">Back to Login page</a></p>
        </div>`,
})
export class ForgotPasswordComponent {
    public email: string;
    public error: string;
    public success: boolean;

    constructor(public api: ApiService, public router: Router) { }

    public submit(): void {
        this.api.resetPassword(this.email)
            .subscribe(() => this.handleSuccess(), (error: any) => this.handleError(error));
    }

    public handleSuccess(): void {
        this.success = true;
        this.error = null;
    }

    public handleError(error: any): void {
        if (error instanceof ApiServiceError && error.status === 400) {
            this.error = "Unable to create a new password, "
                + "please check that you have entered a valid email address.";
        } else {
            this.error = "Unable to create a new password. Please try again later.";
        }
    }
}
