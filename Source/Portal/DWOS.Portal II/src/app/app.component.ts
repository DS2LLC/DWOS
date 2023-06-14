import { AfterViewInit, Component, OnInit } from "@angular/core";
import { Title } from "@angular/platform-browser";
import { NavigationEnd, Router } from "@angular/router";

import { ApiService } from "./api.service";
import { DwosNotification, DwosNotificationType } from "./notification.model";
import { NotificationService } from "./notification.service";
import { OrderApprovalSummary } from "./order-approval-summary.model";
import { OrderService } from "./order.service";

/**
 * The main component for the application.
 */
@Component({
    selector: "dwos",
    template: `
        <dwos-header></dwos-header>
        <dwos-notifications></dwos-notifications>
        <router-outlet></router-outlet>
        <footer class='container-fluid pt-4'>
          <div class='row'>
            <div class='col-auto'>
                <p>
                  <a href="http://www.ds2.com">
                    <img [src]="'/Content/Images/ds2_logo_zoho.png' | static" alt="Dynamic Software Solutions"/>
                  </a>
                </p>
            </div>
            <div class='col d-flex align-self-center'>
              <p>
                A Product of <a href="http://www.ds2.com">Dynamic Software Solutions</a> -
                Powered By <a href="http://www.getdwos.com"><b>DWOS</b></a> Work Order Management System
              </p>
            </div>
          </div>
        </footer>`,
})
export class AppComponent implements OnInit, AfterViewInit {
    constructor(private router: Router,
                private titleService: Title,
                private apiService: ApiService,
                private orderService: OrderService,
                private notificationService: NotificationService) { }

    public ngOnInit(): void {

        // Set the site's title based on the URL.
        this.router.events.subscribe((s: any) => {
            if (s instanceof NavigationEnd) {
                let title: string = "DWOS Portal";
                if (s.url.startsWith("/order/")) {
                    const results: RegExpExecArray = /^\/order\/(.*)$/.exec(s.url);

                    if (results && results.length > 1) {
                        title += `- WO ${results[1]}`;
                    }
                } else if (s.url.startsWith("/contactinfo")) {
                    title += " - Contact Information";
                }

                this.titleService.setTitle(title);
            }
        });
    }

    public ngAfterViewInit(): void {
        this.checkApprovals();

        this.apiService.userChange.subscribe(
            () => this.checkApprovals());
    }

    private checkApprovals(): void {
        if (!this.apiService.loggedIn) {
            return;
        }

        this.orderService.getApprovalSummaries().subscribe(
            (approvals: OrderApprovalSummary[]) => {
                for (const approval of approvals) {
                    if (approval.status === "Pending") {
                        this.notificationService.notify(new DwosNotification(DwosNotificationType.PendingApproval));
                        break;
                    }
                }
            });
    }
}
