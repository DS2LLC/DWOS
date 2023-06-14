import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { DwosNotification } from "./notification.model";

@Injectable()
export class NotificationService {
    public newNotification: Subject<DwosNotification> =
        new Subject<DwosNotification>();

    public notify(notification: DwosNotification): void {
        if (!notification) {
            return;
        }

        this.newNotification.next(notification);
    }
}
