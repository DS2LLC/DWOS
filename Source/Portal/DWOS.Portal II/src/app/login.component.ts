import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { ApiService, ApiServiceError } from "./api.service";
import { User } from "./user.model";

/**
 * Component that prompts users to login.
 */
@Component({
    selector: "dwos-login",
    styles: [
        "form { max-width: 250px; }",
    ],
    template: `
        <div class='container-fluid'>
          <h2>Customer Login</h2>
          <p>Please Enter Login Information</p>

          <div [hidden]="!loginError" class='alert alert-danger'>
            {{loginError}}
          </div>

          <form (ngSubmit)="login()" #loginForm="ngForm">
             <div class='form-group'>
               <label for="email">E-mail</label>
               <input type="email" id="email" required name="email" [(ngModel)]="email" class='form-control'>
             </div>
             <div class='form-group'>
               <label for="password">Password</label>
               <input type="password" id="password" required name="password"
                      [(ngModel)]="password" class='form-control'>
             </div>

             <div class='form-check'>
               <label class='form-check-label'>
                 <input type="checkbox" id=checkbox name="rememberMe"
                        [(ngModel)]="rememberMe" class="form-check-input">
                 Remember Me
               </label>
             </div>

             <button type="submit" [disabled]="!loginForm.form.valid" class='btn btn-primary'>Login</button>
           </form>

           <p><a routerLink="/login/forgot_password">Forgot Password?</a></p>
           <p>
             The information provided is intended only for the person(s) to whom it is
             addressed and may contain confidential and/or privileged material. Any review,
             retransmission, dissemination or other use of, or taking of any action in
             reliance upon, this information by persons or entities other than the intended
             recipient is strictly prohibited.
           </p>
         </div>`,
})
export class LoginComponent implements OnInit {
    public email: string;
    public password: string;
    public rememberMe: boolean;
    public loginError: string;

    constructor(public api: ApiService, public router: Router) { }

    public ngOnInit(): void {
        if (this.api.loggedIn) {
            this.router.navigate(["/"]);
        }
    }

    public login(): void {
        this.api.login(this.email, this.password, this.rememberMe)
            .subscribe((user: User) => this.onLoginSuccess(user), (error: any) => this.onLoginFailure(error));
    }

    private onLoginSuccess(user: User): void {
        if (this.api.loggedIn) {
            this.router.navigate(["/"]);
        }
    }

    private onLoginFailure(error: any): void {
        if (error instanceof ApiServiceError && error.isUnauthorized) {
            // login failed
            this.loginError = "Invalid Credentials: Please try again.";
        } else {
            // general error
            this.loginError = "Could not login. Please try again.";
        }
    }
}
