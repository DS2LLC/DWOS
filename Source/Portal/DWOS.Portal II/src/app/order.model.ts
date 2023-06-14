import { Address } from "./address.model";
import { FileData } from "./file-data.model";
import { OrderFee } from "./order-fee.model";

/**
 * Contains a summary of order information.
 */
export class OrderSummary {
    public partName: string;
    public  manufacturerId: string;
    public customerName: string;
    public orderId: number;
    public orderDate: Date;
    public requiredDate: Date;
    public status: string;
    public completedDate: Date;
    public priority: string;
    public partQuantity: number;
    public currentLocation: string;
    public workStatus: string;
    public currentProcess: string;
    public currentProcessStartDate: Date;
    public estShipDate: Date;
    public customerWorkOrder: string;
    public purchaseOrder: string;
    public customerId: number;
    public trackingNumber: string;
    public shippingCarrier: string;
    public certificationId: number;
    public certificationUrl: string;
    public serialNumbers: string[];
}

/**
 * Represents a work order.
 */
export class Order {
    public partName: string;
    public manufacturerId: string;
    public customerName: string;
    public orderId: number;
    public orderDate: Date;
    public requiredDate: Date;
    public status: string;
    public completedDate: Date;
    public priority: string;
    public partQuantity: number;
    public weight: number;
    public currentLocation: string;
    public estShipDate: Date;
    public customerWorkOrder: string;
    public purchaseOrder: string;
    public customerId: number;
    public trackingNumber: string;
    public shippingCarrier: string;
    public certificationId: number;
    public certificationUrl: string;
    public image: FileData;
    public processes: OrderProcess[];
    public orderType: number;
    public isOnHold: boolean;
    public holdReason: string;
    public fees: OrderFee[];
    public basePrice: number;
    public priceUnit: string;
    public shipToAddress: Address;
    public serialNumbers: string[];
}

/**
 * Represents a process of a work order.
 */
export class OrderProcess {
    public stepOrder: number;
    public department: string;
    public startDate: Date;
    public endDate: Date;
    public description: string;
}
