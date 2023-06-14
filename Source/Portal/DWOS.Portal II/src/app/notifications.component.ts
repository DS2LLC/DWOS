import { Component, OnDestroy, OnInit } from "@angular/core";
import { Subscription } from "rxjs";

import { ApiService } from "./api.service";
import { DwosNotification, DwosNotificationType } from "./notification.model";
import { NotificationService } from "./notification.service";

@Component({
    selector: "dwos-notifications",
    template: `
       <div class="container-fluid">
         <div *ngFor="let notification of notifications"
              class="alert alert-primary alert-dismissible fade show"
              role="alert">
           <ng-container [ngSwitch]="notification?.type">
             <ng-container *ngSwitchCase="types.PendingApproval">
               <p>You have a work order that is pending approval.</p>
               <p><a routerLink="/approvals" data-dismiss="alert">View Approvals</a></p>
             </ng-container>
             <ng-container *ngSwitchDefault>
               Notification text not found.
             </ng-container>
           </ng-container>
           <button type="button" class="close" data-dismiss="alert" aria-label="Close">
             <span aria-hidden="true">&times;</span>
           </button>
         </div>
       </div>`,
})
export class NotificationsComponent implements OnDestroy, OnInit {
    public notifications: DwosNotification[] = [];
    public types = DwosNotificationType;
    private notificationSubscription: Subscription;

    constructor(private notificationService: NotificationService,
                private apiService: ApiService) {
        this.apiService.userChange.subscribe(
            () => this.notifications = []);
    }

    public ngOnInit(): void {
        this.notificationSubscription = this.notificationService.newNotification.subscribe(
            (notification: DwosNotification) => this.addNotification(notification));
    }

    public ngOnDestroy(): void {
        this.notificationSubscription.unsubscribe();
    }

    private addNotification(addNotification: DwosNotification): void {
        if (!addNotification) {
            return;
        }

        this.notifications.push(addNotification);
    }
}
