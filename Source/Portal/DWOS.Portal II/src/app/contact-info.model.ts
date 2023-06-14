/**
 * Represents contact information.
 */
export class ContactInfo {
    public contactId: number;
    public name: string;
    public phoneNumber: string;
    public phoneNumberExt: string;
    public email: string;
    public faxNumber: string;
    public manufacturer: string;
    public invoicePreference: string;
    public receiveShippingNotifications: boolean;
    public receiveApprovalNotifications: boolean;
    public receiveHoldNotifications: boolean;
    public receiveLateOrderNotifications: boolean;
}
