import { formatCurrency } from "@angular/common";
import { AfterViewInit, Component, ElementRef, Inject, LOCALE_ID, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute, Params, Router } from "@angular/router";
import { of, Subscription } from "rxjs";
import { catchError, mergeMap } from "rxjs/operators";

import { ApiService } from "./api.service";
import { AppSettings } from "./app-settings.model";
import { OrderApproval } from "./order-appoval.model";
import { OrderFee } from "./order-fee.model";
import { Order } from "./order.model";
import { OrderService } from "./order.service";

import * as $ from "jquery";
import { OrderApprovalAcceptModalComponent } from "./order-approval-accept-modal.component";
import { OrderApprovalRejectModalComponent } from "./order-approval-reject-modal.component";
import { FileData } from "./file-data.model";
import { FileService } from "./file.service";

@Component({
    template: `
        <div class="container-fluid">
          <h2>Order Approval</h2>
          <dwos-loading *ngIf="isLoading"></dwos-loading>

          <div *ngIf="loadError" class="row">
            <div class="col-12">
              <div class="alert alert-danger" role="alert">
                Could not load order approval.
              </div>
            </div>
          </div>
          <div [hidden]="!approval || !order">
            <div class="row">
              <div class="col-md-auto">
                <div [ngSwitch]="approval?.status">
                  <p *ngSwitchCase="'Pending'" class='bg-primary text-white p-3 mb-2 rounded'>Pending Approval</p>
                  <p *ngSwitchCase="'Accepted'" class='bg-success text-white p-3 mb-2 rounded'>Accepted</p>
                  <p *ngSwitchCase="'Rejected'" class='bg-danger text-white p-3 mb-2 rounded'>Rejected</p>
                </div>
              </div>
            </div>
            <div class="row">
              <div class="col-md-4 order-md-2">
                <p *ngIf="approval?.primaryMedia">
                  <a [href]="approval?.primaryMedia | dataUri | allowData">
                    <img [src]="approval?.primaryMedia | dataUri | allowData" class="img-fluid" style="max-height: 300px" />
                  </a>
                </p>
                <p *ngIf="!approval?.primaryMedia"><img [src]="'/Content/Images/NoImage.jpg' | static" class='img-fluid'/></p>
                <div *ngFor="let mediaUrl of approval?.mediaUrls; index as i">
                    <button (click)="open(mediaUrl)" class="btn btn-light">{{i + 1}}</button> 
                </div>
                <p><a [routerLink]="'/order/' + order?.orderId">Go to order</a></p>
              </div> 
              <div class="col-md-8 order-md-1">
                <form>
                  <div class="form-group">
                    <label for="orderId">Work Order</label>
                    <input type="text" class="form-control form-control-sm" readonly="readonly" id="orderId"
                           name="orderId" [ngModel]="approval?.orderId">
                  </div>
                  <div class="form-group">
                    <label for="partName">Part Name</label>
                    <input type="text" class="form-control form-control-sm" readonly="readonly" id="partName"
                           name="partName" [ngModel]="order?.partName">
                  </div>
                  <div class="form-group">
                    <label for="purchaseOrder">Purchase Order</label>
                    <input type="text" class="form-control form-control-sm" readonly="readonly" id="purchaseOrder"
                           name="purchaseOrder" [ngModel]="order?.purchaseOrder">
                  </div>
                  <div class="form-group">
                    <label for="orderApprovalId">Approval</label>
                    <input type="text" class="form-control form-control-sm" readonly="readonly" id="orderApprovalId"
                           name="orderApprovalId" [ngModel]="approval?.orderApprovalId">
                  </div>
                  <div class="form-group">
                    <label for="dateCreated">Date</label>
                    <input type="text" class="form-control form-control-sm" readonly="readonly" id="dateCreated"
                           name="dateCreated" [ngModel]="approval?.dateCreated | moment: 'M/D/YYYY h:mm A'">
                  </div>
                </form>
              </div>
            </div>
            <div class="row">
              <div class="col-12">
                <h3 *ngIf="order?.shipToAddress">Shipping Address</h3>
                <div *ngIf="order?.shipToAddress">
                  <p>{{order.shipToAddress.name}}</p>
                  <p>{{order.shipToAddress.address1}}</p>
                  <p *ngIf="order?.shipToAddress.address2">{{order.shipToAddress.address2}}</p>
                  <p>{{order.shipToAddress.city}}, {{order.shipToAddress.state}} {{order.shipToAddress.zip}}</p>
                </div>

                <h3 *ngIf="order?.serialNumbers && order.serialNumbers.length > 0">
                  Serial Numbers
                </h3>
                <table class="table table-striped table-sm table-responsive"
                       *ngIf="order?.serialNumbers && order.serialNumbers.length > 0">
                  <thead class="thead-dark">
                    <tr>
                      <th scope="col">Serial #</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr *ngFor="let serialNumber of order.serialNumbers">
                      <td>{{serialNumber}}</td>
                    </tr>
                  </tbody>
                </table>

                <h3>Notes</h3>

                <p *ngIf="!approval?.terms && !approval?.notes">N/A</p>
                <p *ngIf="approval?.terms">{{approval.terms}}</p>
                <p *ngIf="approval?.notes">{{approval.notes}}</p>

                <h3>Price</h3>
                <p class='font-weight-bold'>
                  Subtotal: {{toCurrencyString(getSubtotal(order) + getDiscountTotal(order))}}
                </p>
                <h4>Fees</h4>
                <table class="table table-striped table-sm table-responsive fees-table">
                  <thead class="thead-dark">
                  </thead>
                  <tbody>
                  </tbody>
                </table>

                <p class='font-weight-bold'>Fees: {{toCurrencyString(getFeeTotal(order))}}</p>
                <p class='font-weight-bold'>Total: {{toCurrencyString(getTotal(order))}}</p>

                <h3>Actions</h3>
                <form *ngIf="approval">
                  <div class="form-group">
                    <label for="contactNotes">Your Notes:</label>
                    <textarea type="text" class="form-control form-control-sm" id="contactNotes"
                              name="contactNotes" [(ngModel)]="approval.contactNotes">
                    </textarea>
                  </div>
                  <button type="button" class="btn btn-success mr-3" (click)="startAccept()">
                    <i class="fas fa-check"></i> Accept
                  </button>
                  <button type="button" class="btn btn-danger" (click)="startReject()">
                    <i class="fas fa-times"></i> Reject
                  </button>
                </form>
              </div>
            </div>
          </div>

          <div [hidden]="!submitError" class="alert alert-danger mt-3">
            Unable to update order approval. Please try again.
          </div>

          <div [hidden]="!submitSuccess" class="alert alert-success mt-3">
            Successfully updated the order approval.
          </div>
        </div>
        <dwos-accept-approval-modal (acceptApproval)="completeAccept()"></dwos-accept-approval-modal>
        <dwos-reject-approval-modal (rejectApproval)="completeReject()"></dwos-reject-approval-modal>`,
})
export class OrderApprovalComponent implements AfterViewInit, OnInit, OnDestroy {
    public approval: OrderApproval;
    public order: Order;
    public settings: AppSettings;
    public isLoading: boolean;
    public loadError: boolean;
    public submitSuccess: boolean;
    public submitError: boolean;
    private authFailedSubscription: Subscription;
    private viewInitialized: boolean;

    @ViewChild(OrderApprovalAcceptModalComponent, { static: false })
    private acceptComponent: OrderApprovalAcceptModalComponent;

    @ViewChild(OrderApprovalRejectModalComponent, { static: false })
    private rejectComponent: OrderApprovalRejectModalComponent;

    constructor(
        private api: ApiService,
        private orderApi: OrderService,
        private file: FileService,
        private route: ActivatedRoute,
        private router: Router,
        private element: ElementRef,
        @Inject(LOCALE_ID) private locale: string) {
    }

    public ngAfterViewInit(): void {
        this.viewInitialized = true;
        if (this.approval && this.order && this.settings && !this.isTableInitialized()) {
            this.initializeTable();
        }
    }

    public ngOnInit(): void {
        this.authFailedSubscription = this.api.authFailed.subscribe(() => {
            this.router.navigate(["/login"]);
        });

        this.api.getSettings().subscribe(
            (settings: AppSettings) => {
                this.settings = settings;

                this.route.params
                    .subscribe((params: Params) => this.getApproval(+params.id));
            },
            (error: any) => {
                alert("Error loading order settings. Please refresh the page and try again.");
                throw error;
            });
    }

    public ngOnDestroy(): void {
        if (this.authFailedSubscription) {
            this.authFailedSubscription.unsubscribe();
        }
    }

    public getApproval(orderApprovalId: number): void {
        this.approval = null;
        this.isLoading = true;
        this.loadError = false;
        this.orderApi.getApproval(orderApprovalId)
            .pipe(
                catchError((error: any) => of(error)),
                mergeMap((orderApproval: OrderApproval) => {
                    this.approval = orderApproval;
                    return this.orderApi.getOrder(orderApproval.orderId);
                }))
            .subscribe(
                (order: Order) => {
                    this.order = order;
                    this.isLoading = false;
                    this.loadError = false;

                    if (this.isTableInitialized()) {
                        const table = $(this.element.nativeElement.querySelector(".fees-table"));
                        const dataTable: DataTables.Api = table.DataTable();
                        const fees = this.order.fees
                            ? this.order.fees.filter((fee: OrderFee) => fee.total > 0)
                            : new OrderFee[0]();

                        dataTable.clear();
                        dataTable.rows.add(fees);
                        dataTable.draw();
                    } else if (this.viewInitialized) {
                        this.initializeTable();
                    }
                },
                (error: any) => {
                    this.approval = null;
                    this.order = null;
                    this.isLoading = false;
                    this.loadError = error;
                    throw error;
                });
    }

    public getSubtotal(order: Order): number {
        if (!order) {
            return null;
        }

        switch (order.priceUnit) {
            case "Each":
                return order.basePrice * order.partQuantity;
            case "EachByWeight":
                return order.basePrice * order.weight;
            case "Lot":
            case "LotByWeight":
                return order.basePrice;
            default:
                return order.basePrice * order.partQuantity;
        }
    }

    public getDiscountTotal(order: Order): number {
        if (!order) {
            return null;
        }

        if (!order.fees) {
            return 0;
        }

        return order.fees
            .filter((fee: OrderFee) => fee.total <= 0)
            .map((fee) => fee.total)
            .reduce((prev, cur) => prev + cur, 0);
    }

    public getFeeTotal(order: Order): number {
        if (!order) {
            return null;
        }

        if (!order.fees) {
            return 0;
        }

        return order.fees
            .filter((fee: OrderFee) => fee.total > 0)
            .map((fee) => fee.total)
            .reduce((prev, cur) => prev + cur, 0);
    }

    public getTotal(order: Order): number {
        if (!order) {
            return null;
        }

        return this.getSubtotal(order) +
            this.getDiscountTotal(order) +
            this.getFeeTotal(order);
    }

    public startAccept(): void {
        this.acceptComponent.show();
    }

    public startReject(): void {
        this.rejectComponent.show();
    }

    public completeAccept(): void {
        this.submitSuccess = false;
        this.submitError = false;

        if (!this.approval) {
            return;
        }

        this.approval.status = "Accepted";

        this.orderApi.updateOrderApproval(this.approval).subscribe(
            () => this.submitSuccess = true,
            () => {
                this.submitError = true;
            });
    }

    public completeReject(): void {
        this.submitSuccess = false;
        this.submitError = false;

        if (!this.approval) {
            return;
        }

        this.approval.status = "Rejected";

        this.orderApi.updateOrderApproval(this.approval).subscribe(
            () => this.submitSuccess = true,
            () => {
                this.submitError = true;
            });
    }

    public toCurrencyString(data: any): string {
        const decimalPlaces: number = this.settings.priceDecimalPlaces;

        return formatCurrency(data, this.locale, "$", "USD",
            `1.${decimalPlaces}-${decimalPlaces}`);
    }

    public open(mediaUrl: string): boolean {
        if (!this.isTableInitialized()) {
            return false;
        }

        this.api.getFile(mediaUrl)
            .subscribe(
                (data: FileData) => this.file.open(FileData.toBlob(data), data.name || "test.png", data.type),
                (error: any) => {
                    alert("Could not retrieve media for the order approval.");
                });
    }

    private isTableInitialized(): boolean {
        const table = $(this.element.nativeElement.querySelector(".fees-table"));
        const dtStatic: any = $.fn.DataTable;

        return dtStatic.isDataTable(table);
    }

    private initializeTable(): void {
        const table = $(this.element.nativeElement.querySelector(".fees-table"));

        const tableOptions: DataTables.Settings = {
            autoWidth: false,
            columns: [
                {
                    data: "name",
                    name: "name",
                    title: "Fee",
                },
                {
                    data: "total",
                    name: "total",
                    render: (data: number, type: string) => this.renderCurrency(data, type),
                    title: "Amount",
                },
            ],
            data: this.order.fees || new OrderFee[0](),
            // show processing display, table
            dom: "rt",
            language: {
                emptyTable: "This order does not have any fees.",
            },
            lengthChange: false,
            order: [
                [0, "asc"],
            ],

        };

        table.DataTable(tableOptions);
    }

    private renderCurrency(data: number, type: string): number | string {
        if (type !== "display" || !data) {
            return data;
        }

        return this.toCurrencyString(data);
    }
}
