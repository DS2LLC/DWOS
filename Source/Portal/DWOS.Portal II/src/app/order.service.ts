import { Injectable } from "@angular/core";
import { Observable, throwError } from "rxjs";
import { tap } from "rxjs/operators";

import { ApiService } from "./api.service";
import { OrderApproval } from "./order-appoval.model";
import { OrderApprovalSummary } from "./order-approval-summary.model";
import { Order, OrderSummary } from "./order.model";

/**
 * Service that retrieves and manages order information from the server.
 */
@Injectable()
export class OrderService {
    private orders: OrderSummary[];

    constructor(private api: ApiService) { }

    /**
     * Gets order summaries from the server.
     */
    public getOrderSummaries(): Observable<OrderSummary[]> {
        return this.api.getApi<OrderSummary[]>("/api/orders/")
            .pipe(tap((orders: OrderSummary[]) => this.orders = orders.slice().sort(this.sortCompare)));
    }

    /**
     * Gets an order from the server.
     * @param orderId
     */
    public getOrder(orderId: number): Observable<Order> {
        return this.api.getApi(`/api/orders/getorder/${orderId}`);
    }

    /**
     * Gets previous and next orders for a given order ID.
     * @param orderId
     */
    public getSurroundingOrders(orderId: number): SurroundingOrders {
        if (!this.orders) {
            return null;
        }

        let prevIndex: number = -1;
        for (let x: number = 0; x < this.orders.length; ++x) {
            const currentOrder: OrderSummary = this.orders[x];

            if (currentOrder.orderId === orderId) {
                // found order
                const nextIndex: number = x + 1;
                const prevOrder: OrderSummary = prevIndex < 0 ? null : this.orders[prevIndex];
                const nextOrder: OrderSummary = nextIndex >= this.orders.length ? null : this.orders[nextIndex];
                return new SurroundingOrders(prevOrder, nextOrder);
            }

            prevIndex = x;
        }

        return null;
    }

    /***
     * Gets approval summaries from the server.
     */
    public getApprovalSummaries(): Observable<OrderApprovalSummary[]> {
        return this.api.getApi<OrderApprovalSummary[]>("/api/orderapprovals/");
    }

    /**
     * Gets an order approvalfrom the server.
     * @param orderApprovalId
     */
    public getApproval(orderApprovalId: number): Observable<OrderApproval> {
        return this.api.getApi(`/api/orderapprovals/getapproval/${orderApprovalId}`);
    }

    /**
     * Updates an order approval.
     * @param approval
     */
    public updateOrderApproval(approval: OrderApproval): Observable<any> {
        if (!approval) {
            return throwError("Did not provide order approval.");
        }

        return this.api.putApi("/api/orderapprovals/", approval);
    }

    /**
     * Compares two order summaries by their order IDs from greatest to least.
     * @param a
     * @param b
     */
    private sortCompare(a: OrderSummary, b: OrderSummary): number {
        return b.orderId - a.orderId;
    }
}

/**
 * Represents the orders that came immediately before or immediately after an order.
 */
export class SurroundingOrders {
    constructor(public previousOrder: OrderSummary, public nextOrder: OrderSummary) { }
}
