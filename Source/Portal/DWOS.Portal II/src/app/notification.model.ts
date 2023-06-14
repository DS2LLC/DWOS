export class DwosNotification {
    constructor(public type: DwosNotificationType) {

    }
}

export enum DwosNotificationType {
    PendingApproval,
}
