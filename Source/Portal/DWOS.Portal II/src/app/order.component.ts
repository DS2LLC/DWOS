import { AfterViewInit, Component, ElementRef, OnInit } from "@angular/core";
import { ActivatedRoute, Params, Router } from "@angular/router";
import * as $ from "jquery";
import { finalize, tap } from "rxjs/operators";

import { Order } from "./order.model";
import { OrderService, SurroundingOrders } from "./order.service";

/**
 * Component that shows order details.
 */
@Component({
    template: `
        <div class="container-fluid">
          <h2>Order Details</h2>

          <p>
            <button class="btn btn-sm" [disabled]="!previousOrderId" (click)="goTo(previousOrderId)"
                    title="Click to view details on the previous order">&lt; Prev</button>
            <button class="btn btn-sm" [disabled]="!nextOrderId" (click)="goTo(nextOrderId)"
                    title="Click to view details on the next order">Next &gt;</button>
          </p>

          <div class="row" *ngIf="error">
            <div class="col-12">
              <div class="alert alert-danger" role="alert">
                Could not find order {{ orderId }}.
              </div>
            </div>
          </div>

          <div class="row">
            <div class="col-md-4 order-md-2" *ngIf="order">
              <p *ngIf="order.image"><img [src]="order.image | dataUri | allowData" class="img-fluid" /></p>
              <p *ngIf="!order.image"><img [src]="'/Content/Images/NoImage.jpg' | static" class='img-fluid'/></p>
            </div>

            <div class="col-md-8 order-md-1">
              <form>
                <div class="row">
                  <div class="form-group col-sm-6">
                    <label for="orderId">Work Order</label>
                    <input type="text" class="form-control form-control-sm" readonly="readonly" id="orderId"
                           name="orderId" [ngModel]="order?.orderId">
                  </div>
                  <div class="form-group col-sm-6">
                    <label for="priority">Priority</label>
                    <input type="text" class="form-control form-control-sm" readonly="readonly" id="priority"
                           name="priority" [ngModel]="order?.priority">
                  </div>
                </div>

                <div class="row">
                  <div class="form-group col-sm-6">
                    <label for="customerWorkOrder">Cust. W.O.</label>
                    <input type="text" class="form-control form-control-sm" readonly="readonly" id="customerWorkOrder"
                           name="customerWorkOrder" [ngModel]="order?.customerWorkOrder">
                  </div>
                  <div class="form-group col-sm-6">
                    <label for="status">Status</label>
                    <input type="text" class="form-control form-control-sm" readonly="readonly" id="status"
                           name="status" [ngModel]="order?.status">
                  </div>
                </div>

                <div class="row">
                  <div class="form-group col-sm-6">
                    <label for="purchaseOrder">P.O.</label>
                    <input type="text" class="form-control form-control-sm" readonly="readonly" id="purchaseOrder"
                           name="purchaseOrder" [ngModel]="order?.purchaseOrder">
                  </div>
                  <div class="form-group col-sm-6">
                    <label for="currentLocation">Location</label>
                    <input type="text" class="form-control form-control-sm" readonly="readonly" id="currentLocation"
                           name="currentLocation" [ngModel]="order?.currentLocation">
                  </div>
                </div>
                <div class="row">
                  <div class="form-group col-sm-6">
                    <label for="manufacturerId">Manufacturer</label>
                    <input type="text" class="form-control form-control-sm" readonly="readonly" id="manufacturerId"
                           name="manufacturerId" [ngModel]="order?.manufacturerId">
                  </div>
                  <div class="form-group col-sm-6">
                    <label for="orderDate">Created</label>
                    <input type="text" class="form-control form-control-sm" readonly="readonly" id="orderDate"
                           name="orderDate" [ngModel]="order?.orderDate | moment: 'M/D/YYYY h:mm A'">
                  </div>
                </div>

                <div class="row">
                  <div class="form-group col-sm-6">
                    <label for="partName">Part Name</label>
                    <input type="text" class="form-control form-control-sm" readonly="readonly" id="partName"
                           name="partName" [ngModel]="order?.partName">
                  </div>
                  <div class="form-group col-sm-6">
                    <label for="requiredDate">Required</label>
                    <input type="text" class="form-control form-control-sm" readonly="readonly" id="requiredDate"
                           name="requiredDate" [ngModel]="order?.requiredDate | moment: 'M/D/YYYY h:mm A'">
                  </div>
                </div>

                <div class="row">
                  <div class="form-group col-sm-6">
                    <label for="partQuantity">Part Quantity</label>
                    <input type="text" class="form-control form-control-sm" readonly="readonly" id="partQuantity"
                           name="partQuantity" [ngModel]="order?.partQuantity">
                  </div>
                  <div class="form-group col-sm-6">
                    <label for="completedDate">Completed</label>
                    <input type="text" class="form-control form-control-sm" readonly="readonly" id="completedDate"
                           name="completedDate" [ngModel]="order?.completedDate | moment: 'M/D/YYYY h:mm A'">
                  </div>
                </div>
              </form>
            </div>


          </div>

          <div class='row'>
            <div class='col-12'>
              <table class='table table-striped table-sm table-responsive process-table'>
                <thead class='thead-dark'>
                </thead>
                <tbody>
                </tbody>
              </table>
            </div>
          </div>

          <p><a routerLink="">Back to Main</a></p>
        </div>`,
})
export class OrderComponent implements OnInit, AfterViewInit {
    public order: Order = null;
    public orderId: any = null;
    public error = false;
    public previousOrderId: number = null;
    public nextOrderId: number = null;
    public viewInitialized: boolean;

    constructor(
        private orderApi: OrderService,
        private router: Router,
        private route: ActivatedRoute,
        private element: ElementRef) {
    }

    public ngOnInit(): void {
        this.route.params
            .pipe(tap((params: Params) => this.orderId = params.id))
            .subscribe((params: Params) => this.getOrder(+params.id));
    }

    public ngAfterViewInit(): void {
        this.viewInitialized = true;
        if (this.order && !this.isTableInitialized()) {
            this.initializeTable();
        }
    }

    public goTo(orderId: number): void {
        this.router.navigate([`/order/${orderId}`]);
    }

    private getOrder(orderId: number): void {
        this.orderApi.getOrder(orderId)
            .pipe(finalize(() => {
                // set previous/next orders
                const surroundingOrders: SurroundingOrders  = this.orderApi.getSurroundingOrders(orderId);

                if (surroundingOrders && surroundingOrders.previousOrder) {
                    this.previousOrderId = surroundingOrders.previousOrder.orderId;
                } else {
                    this.previousOrderId = null;
                }

                if (surroundingOrders && surroundingOrders.nextOrder) {
                    this.nextOrderId = surroundingOrders.nextOrder.orderId;
                } else {
                    this.nextOrderId = null;
                }

                // initialize/populate table
                if (this.isTableInitialized()) {
                    const dataTable: DataTables.Api = this.getTableElement().DataTable();
                    dataTable.clear();

                    if (this.order) {
                        dataTable.rows.add(this.order.processes);
                    }

                    dataTable.draw();
                } else if (this.viewInitialized) {
                    this.initializeTable();
                }
            }))
            .subscribe(
                (order: Order) => {
                    this.order = order;
                    this.error = false;

                },
                (error: any) => {
                    this.order = null;
                    this.error = true;
                    throw error;
                });
    }

    private getTableElement(): JQuery<HTMLElement> {
        return $(this.element.nativeElement.querySelector(".process-table"));
    }

    private isTableInitialized(): boolean {
        const table = this.getTableElement();
        const dtStatic: any = $.fn.DataTable;

        return dtStatic.isDataTable(table);
    }

    private initializeTable(): void {
        const table = this.getTableElement();

        const tableOptions: DataTables.Settings = {
            autoWidth: false,
            columns: [
                { title: "Step", data: "stepOrder", name: "stepOrder" },
                { title: "Department", data: "department", name: "department" },
                { title: "Start Date", data: "startDate", name: "startDate", render: this.renderDate },
                { title: "Finish Date", data: "endDate", name: "endDate", render: this.renderDate },
                { title: "Description", data: "description", name: "description" },
            ],
            data: this.order === null ? [] : this.order.processes,
            // show processing display, table
            dom: "rt",
            lengthChange: false,
            ordering: false,
            paging: false,
        };

        table.DataTable(tableOptions);
    }

    private renderDate(data: string, type: string): string {
        if (type !== "display" || !data) {
            return data;
        }

        const date: Date = new Date(data);
        return (date.getMonth() + 1) + "/" + date.getDate() + "/" + date.getFullYear();
    }
}
