import { Location } from "@angular/common";
import { Component, ElementRef, OnInit, ViewChild } from "@angular/core";

import { ApiService } from "./api.service";
import { AppSettings } from "./app-settings.model";
import { ContactInfo } from "./contact-info.model";

import * as $ from "jquery";
import "jquery-mask-plugin";

/**
 * Component that prompts users to edit their contact information.
 */
@Component({
    selector: "dwos-contact-info",
    template: `
        <div class="container-fluid">
          <h2>Contact Information</h2>
          <p>You can change your account information here.</p>

          <div [hidden]="!submitError" class="alert alert-danger">
            Unable to update contact information. Please try again.
          </div>

          <div [hidden]="!getError" class="alert alert-danger">
            Unable to retrieve contact information. Please try again.
          </div>

          <form (ngSubmit)="submit()" #contactForm="ngForm" *ngIf="data" #contactFormElement>
             <div [ngClass]="{'form-group': true, 'row': true }">
               <label class="col-12 col-md-2 col-form-label" for="name">Name</label>
               <div class="col-11 col-md-9">
                 <input type="text" id="name" required pattern="^[a-zA-Z ]+$" maxlength="50" name="name"
                        [(ngModel)]="data.name"
                        [ngClass]="{'is-invalid': name?.errors}"
                        class="form-control"
                        #name="ngModel"
                        placeholder="Name">
               </div>
               <div class="col-1">
                 <span *ngIf="name?.errors?.required" class='error-indicator' title="Name is required">*</span>
                 <span *ngIf="name?.errors?.pattern" class='error-indicator'
                       title="Name field must contain only letters and spaces.">
                   *
                 </span>
                 <span *ngIf="name?.errors?.maxlength" class='error-indicator' title="Name is too long.">*</span>
               </div>
             </div>

             <div [ngClass]="{'form-group': true, 'row': true }">
               <label class="col-12 col-md-2 col-form-label" for="phoneNumber">Phone Number</label>
               <div class="col-11 col-md-6">
                 <input type="tel" id="phoneNumber" name="phoneNumber" [pattern]="phoneNumberRegex" maxlength="50"
                        [(ngModel)]="data.phoneNumber"
                        [ngClass]="{'is-invalid': phoneNumber?.errors}"
                        class="form-control"
                        #phoneNumber="ngModel"
                        placeholder="(###) ###-####">
               </div>
               <div class="col-1">
                 <span *ngIf="phoneNumber?.errors?.pattern" class='error-indicator'
                     title="Phone number must be entered as (###) ###-####.">
                   *
                 </span>
                 <span *ngIf="phoneNumber?.errors?.maxlength" class='error-indicator' title="Phone number is too long.">
                   *
                 </span>
               </div>
               <label class="col-12 col-md-1 col-form-label" for="phoneNumberExt">Ext.</label>
               <div class="col-11 col-md-1">
                 <input type="text" id="phoneNumberExt" pattern="^[0-9 ]+$" [maxlength]="phoneNumberExtMaxLength"
                        name="phoneNumberExt"
                        [(ngModel)]="data.phoneNumberExt"
                        [ngClass]="{'is-invalid': phoneNumberExt?.errors}"
                        class="form-control"
                        #phoneNumberExt="ngModel">
               </div>
               <div class="col-1">
                 <span *ngIf="phoneNumberExt?.errors?.pattern" class='error-indicator'
                       title="Phone Extension must contain numbers only.">
                   *
                 </span>
                 <span *ngIf="phoneNumberExt?.errors?.maxlength" class='error-indicator'
                       title="Phone Extension is too long.">
                   *
                 </span>
               </div>
             </div>

             <div [ngClass]="{'form-group': true, 'row': true }">
               <label class="col-12 col-md-2 col-form-label" for="faxNumber">Fax Number</label>
               <div class="col-11 col-md-9">
                 <input type="tel" id="faxNumber" name="faxNumber"
                        [pattern]="phoneNumberRegex"
                        maxlength="50"
                        [(ngModel)]="data.faxNumber"
                       [ngClass]="{'is-invalid': faxNumber?.errors}"
                        class="form-control"
                        #faxNumber="ngModel"
                        placeholder="(###) ###-####">
               </div>
               <div class="col-1">
                 <span *ngIf="faxNumber?.errors?.pattern" class='error-indicator'
                       title="Fax number must be entered as (###) ###-####.">
                   *
                 </span>
                 <span *ngIf="faxNumber?.errors?.maxlength" class='error-indicator' title="Fax number is too long.">
                   *
                 </span>
               </div>
             </div>

             <div [ngClass]="{'form-group': true, 'row': true }">
               <label class="col-12 col-md-2 col-form-label" for="email">Email</label>
               <div class="col-11 col-md-9">
                 <input type="email" id="email" name="email" [pattern]="emailRegex" maxlength="50"
                        [(ngModel)]="data.email"
                        [ngClass]="{'is-invalid': email?.errors}"
                        class="form-control"
                        readonly="readonly"
                        #email="ngModel"
                        placeholder="Email address">
               </div>
               <div class="col-1">
                 <span *ngIf="email?.errors?.pattern" class='error-indicator'
                       title="Email must be entered as name@domain.com.">
                   *
                 </span>
                 <span *ngIf="email?.errors?.maxlength" class='error-indicator' title="Email is too long.">*</span>
               </div>
             </div>

             <div [ngClass]="{'form-group': true, 'row': true }">
               <label class="col-12 col-md-2 col-form-label" for="manufacturer">Manufacturer</label>
               <div class="col-11 col-md-9">
                 <input type="text" id="manufacturer" pattern="^[0-9a-zA-Z .]+$" maxlength="50"
                        name="manufacturer"
                        [(ngModel)]="data.manufacturer"
                        [ngClass]="{'is-invalid': manufacturer?.errors}"
                        class="form-control"
                        #manufacturer="ngModel"
                        placeholder="Manufacturer's name">
               </div>
               <div class="col-1">
                 <span *ngIf="manufacturer?.errors?.pattern" class='error-indicator'
                       title="Manufacturer's name must be entered with letters, numbers, and spaces only.">
                   *
                 </span>
                 <span *ngIf="manufacturer?.errors?.maxlength" class='error-indicator'
                       title="Manufacturer's name is too long.">
                   *
                 </span>
               </div>
             </div>

             <div class="form-group row">
               <label class="col-12 col-md-2 col-form-label" for="invoicePreference">Invoice Preference</label>
               <div class="col-11 col-md-9">
                 <select id="invoicePreference" name="invoicePreference"
                         [(ngModel)]="data.invoicePreference"
                         class="form-control">
                   <option value="None">None</option>
                   <option value="Email">Email</option>
                   <option value="Fax">Fax</option>
                   <option value="Mail">Mail</option>
                 </select>
               </div>
             </div>

             <div class="form-group row">
               <label class="col-12 col-md-2 col-form-label">Shipping Notifications</label>
               <div class="col-11 col-md-9">
                 <div class="form-check">
                   <label class="form-check-label">
                     <input class="form-check-input" type="checkbox" name="receiveShippingNotifications"
                            [(ngModel)]="data.receiveShippingNotifications">
                   </label>
                 </div>
               </div>
             </div>

             <div class="form-group row">
               <label class="col-12 col-md-2 col-form-label">Hold Notifications</label>
               <div class="col-11 col-md-9">
                 <div class="form-check">
                   <label class="form-check-label">
                     <input class="form-check-input" type="checkbox" name="receiveHoldNotifications"
                            [(ngModel)]="data.receiveHoldNotifications">
                   </label>
                 </div>
               </div>
             </div>

             <div class="form-group row" *ngIf="settings && settings.showOrderApprovals">
               <label class="col-12 col-md-2 col-form-label">Approval Notifications</label>
               <div class="col-11 col-md-9">
                 <div class="form-check">
                   <label class="form-check-label">
                     <input class="form-check-input" type="checkbox" name="receiveApprovalNotifications"
                            [(ngModel)]="data.receiveApprovalNotifications">
                   </label>
                 </div>
               </div>
             </div>

             <div class="form-group row" *ngIf="settings && settings.showLateOrderNotificationOption">
               <label class="col-12 col-md-2 col-form-label">Late Order Notifications</label>
               <div class="col-11 col-md-9">
                 <div class="form-check">
                   <label class="form-check-label">
                     <input class="form-check-input" type="checkbox" name="receiveLateOrderNotifications"
                            [(ngModel)]="data.receiveLateOrderNotifications">
                   </label>
                 </div>
               </div>
             </div>

             <button type="submit" [disabled]="!contactForm.form.valid" class="btn btn-primary">OK</button>
             <button type="button" (click)="cancel()" class="btn btn-secondary">Cancel</button>
          </form>

          <p><a routerLink='/change_password'>Change Password...</a></p>
        </div>`,
})
export class ContactInfoComponent implements OnInit {
    /* tslint:disable:max-line-length */
    public phoneNumberRegex = /^[\(]{0,1}([0-9]){3}[\)]{0,1}[ ]?([^0-1]){1}([0-9]){2}[ ]?[-]?[ ]?([0-9]){4}[ ]*((x){0,1}([0-9]){1,5}){0,1}$/;
    public emailRegex = /^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    /* tslint:enable:max-line-length */

    public data: ContactInfo;
    public submitError: boolean;
    public getError: boolean;
    private settings: AppSettings;
    private initializedForm: boolean;

    constructor(private location: Location, private api: ApiService) { }

    public get phoneNumberExtMaxLength(): number {
        if (!this.data.phoneNumber) {
            return 50;
        }

        return 50 - this.data.phoneNumber.length;
    }

    public submit(): void {
        this.api.updateContactInfo(this.data).subscribe(
            () => {
                this.location.back();
            },
            (error: any) => {
                this.submitError = true;
            });
    }

    public cancel(): void {
        this.location.back();
    }

    public ngOnInit(): void {
        this.submitError = null;
        this.getError = null;
        this.api.getSettings().subscribe(
            (settings: AppSettings) => {
                this.settings = settings;

                this.api.getContactInfo().subscribe(
                    (contactInfo: ContactInfo) => {
                        this.setContactInfo(contactInfo);
                    },
                    (error: any) => {
                        this.getError = true;
                    });
            },
            (error: any) => {
                this.getError = true;
            });
    }

    /**
     * Applies jQuery masks to phone number fields.
     */
    @ViewChild("contactFormElement", { static: false })
    private set contactFormElement(contactForm: ElementRef) {
        if (!contactForm || this.initializedForm) {
            return;
        }

        const form: JQuery<HTMLElement> = $(contactForm.nativeElement);
        form.find("[name='phoneNumber']").mask("(000) 000-0000");
        form.find("[name='phoneNumberExt']").mask("0000");
        form.find("[name='faxNumber']").mask("(000) 000-0000");
        this.initializedForm = true;
    }

    private setContactInfo(contactInfo: ContactInfo): void {
        this.data = new ContactInfo();

        if (!contactInfo) {
            this.getError = true;
            return;
        }

        // copy contactInfo but set default for invoicePreference
        Object.assign(this.data, contactInfo);
        this.data.invoicePreference = contactInfo.invoicePreference || "None";
    }
}
