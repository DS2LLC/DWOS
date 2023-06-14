import { Location } from "@angular/common";
import { Component } from "@angular/core";

import { ApiService, ApiServiceError } from "./api.service";

/**
 * Component that prompts users to change their passwords.
 */
@Component({
    selector: "dwos-change-password",
    template: `
        <div class='container-fluid'>
          <h2>Change Password</h2>

          <div [hidden]="!submitError" class="alert alert-danger">
            {{ submitError }}
          </div>

          <form (ngSubmit)="submit()" #changePasswordForm="ngForm">
             <div [ngClass]="{'form-group': true, 'row': true,
                               'has-danger': currentPassword.errors &&
                                 (currentPassword.dirty || currentPassword.touched) }">
               <label class="col-2" for="currentPassword">Current Password</label>
               <div class="col-9">
                 <input type="password" id="currentPassword" required name="currentPassword"
                        [(ngModel)]="userCurrentPassword"
                        class="form-control"
                        #currentPassword="ngModel"
                        placeholder="Current Password">
               </div>
               <div class="col-1" *ngIf="currentPassword.errors && (currentPassword.dirty || currentPassword.touched)">
                 <span *ngIf="currentPassword.errors.required" class='error-indicator'
                       title="Current Password is required">
                   *
                 </span>
               </div>
             </div>

             <div [ngClass]="{'form-group': true, 'row': true,
                              'has-danger': newPassword.errors &&
                                (newPassword.dirty || newPassword.touched) }">
               <label class="col-2" for="newPassword">New Password</label>
               <div class="col-9">
                 <input type="password" id="newPassword" required triggerValidation="confirmPassword"
                        name="newPassword"
                        [(ngModel)]="userNewPassword"
                        class="form-control"
                        #newPassword="ngModel"
                        placeholder="New Password">
               </div>
               <div class="col-1" *ngIf="newPassword.errors && (newPassword.dirty || newPassword.touched)">
                 <span *ngIf="newPassword.errors.required" class='error-indicator' title="New Password is required">
                   *
                 </span>
               </div>
             </div>

             <div [ngClass]="{'form-group': true, 'row': true,
                             'has-danger': confirmPassword.errors
                                && (confirmPassword.dirty || confirmPassword.touched) }">
               <label class="col-2" for="confirmPassword">New Password</label>
               <div class="col-9">
                 <input type="password" id="confirmPassword" required validateEqual="newPassword"
                        name="confirmPassword"
                        [(ngModel)]="userConfirmPassword"
                        class="form-control"
                        #confirmPassword="ngModel"
                        placeholder="New Password">
               </div>
               <div class="col-1" *ngIf="confirmPassword.errors && (confirmPassword.dirty || confirmPassword.touched)">
                 <span *ngIf="confirmPassword.errors.required" class='error-indicator'
                       title="Confirm Password is required">
                   *
                 </span>
                 <span *ngIf="confirmPassword.errors.validateEqual" class='error-indicator'
                       title="Confirm Password must match new password.">
                   *
                 </span>
               </div>
             </div>

             <button type="submit" [disabled]="!changePasswordForm.form.valid" class="btn btn-primary">OK</button>
             <button type="button" (click)="cancel()" class="btn btn-secondary">Cancel</button>
          </form>
        </div>`,
})
export class ChangePasswordComponent {
    public submitError: string;
    public userCurrentPassword: string;
    public userNewPassword: string;
    public userConfirmPassword: string;

    constructor(private api: ApiService, private location: Location) { }

    public submit(): void {
        if (this.userNewPassword !== this.userConfirmPassword) {
            this.submitError = "New password and confirm password do not match.";
            return;
        }

        this.api.updatePassword(this.userCurrentPassword, this.userNewPassword).subscribe(
            () => {
                this.location.back();
            },
            (error: any) => {
                if (error instanceof ApiServiceError) {
                    if (error.isUnauthorized) {
                        this.submitError = "Please login with your current password before changing your password.";
                    } else {
                        this.submitError = "Current password is incorrect.";
                    }
                } else {
                    this.submitError = "Unable to update your password. Please try again.";
                    console.error(error);
                }
            });
    }

    public cancel(): void {
        this.location.back();
    }
}
