import { Component, OnInit } from "@angular/core";
import { timer } from "rxjs";
import { map } from "rxjs/operators";

import { ApiService } from "./api.service";
import { Header } from "./header.model";
import { User } from "./user.model";

/**
 * The application's top header.'
 */
@Component({
    selector: "dwos-header",
    template: `
        <header class='container-fluid'>
          <div *ngIf="header" class='row pt-4'>
            <div class='col-md-4 d-flex'>
              <h1 class='mx-auto mx-md-0'>
                <a routerLink='/'>
                  <img *ngIf="header.logo" [src]="header.logo | allowData"
                       [alt]="header.companyName" class='img-fluid' />
                </a>
              </h1>
            </div>
            <div class='col-md-4 d-flex'>
              <h2 class='text-center mx-auto mx-md-0'>{{header.tagline}}</h2>
            </div>
            <div class='col-md-4 text-center d-md-flex align-items-md-center justify-content-md-end'>
              <div *ngIf="user">
                <p class="m-0 p-0">Welcome, <a routerLink='/contactinfo'>{{user.name}}</a></p>
                <p class="m-0 p-0">of</p>
                <p class="m-0 p-0">{{user.companyName}}</p>
              </div>

              <div *ngIf="!user">
                <p>Please log in.</p>
              </div>
            </div>
          </div>
          <div class='row'>
            <div class='col-sm-6 text-center text-sm-left'>
              <p *ngIf="user">Last Login: {{user.lastLogin | moment:"M/D/YYYY h:mm:ss A"}}</p>
            </div>

            <div *ngIf="header" class='col-sm-6 text-center text-sm-right'>
              <p>{{currentTime | async | moment:"dddd, MMM D, YYYY h:mm:ss A"}} {{ header.timezone }}</p>
            </div>
          </div>
        </header>`,
})
export class HeaderComponent implements OnInit {
    public header: Header = null;
    public user: User = null;

    /**
     * The current time - uses the client system's time with the server's timezone.
     */
    public currentTime = timer(0, 250).pipe(map(() => {
        const minutesToMillseconds: number = 60 * 1000;

        if (!this.header) {
            return null;
        }

        const now: Date = new Date();
        const localOffsetMinutes: number = now.getTimezoneOffset();
        const offsetDifferenceMillseconds: number = (localOffsetMinutes - this.header.timezoneOffsetMinutes)
            * minutesToMillseconds;
        return now.valueOf() + offsetDifferenceMillseconds;
    }));

    constructor(public api: ApiService) { }

    public ngOnInit(): void {
        this.api.getHeaderData()
            .subscribe((header: Header) => this.updateHeader(header), (error: any) => this.handleError(error));

        this.api.userChange.subscribe(() => this.refreshUser());
        this.refreshUser();
    }

    public updateHeader(header: Header): void {
        this.header = header;
    }

    public refreshUser(): void {
        if (this.api.loggedIn) {
            this.api.getUserData()
                .subscribe((user: User) => this.user = user, (error: any) => this.handleError(error));
        } else {
            this.user = null;
        }

    }

    private handleError(error: any): void {
        alert("Unable to retrieve data. Please refresh the page and try again.");
        throw error;
    }
}
