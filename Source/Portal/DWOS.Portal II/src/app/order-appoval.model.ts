import { FileData } from "./file-data.model";

export class OrderApproval {
    public orderApprovalId: number;
    public orderId: number;
    public status: string;
    public primaryMedia: FileData;
    public mediaUrls: string[];
    public terms: string;
    public notes: string;
    public dateCreated: Date;
    public contactId: number;
    public contactNotes: string;
}
