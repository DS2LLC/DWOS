import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from "@angular/core";
import { Router } from "@angular/router";

import { Subscription } from "rxjs";

import { ApiService } from "./api.service";
import { AppSettings } from "./app-settings.model";
import { FileData } from "./file-data.model";
import { FileService } from "./file.service";
import { OrderSearchModalComponent, OrderSearchOptions } from "./order-search-modal.component";
import { OrderSummary } from "./order.model";
import { OrderService } from "./order.service";

import * as $ from "jquery";

/**
 * Component that shows orders and a toolbar with multiple options..
 */
@Component({
    // allow dynamically-added column inputs to use styles
    encapsulation: ViewEncapsulation.None,

    selector: "dwos-index",

    styles: [
        ".filter-form-control { font-size: 75%; height: 1.5rem; padding:0; margin:0; min-width: 50px; }",
    ],

    template: `
        <div class='container-fluid'>
          <div class="ribbon">
            <div class="ribbon-inner btn-toolbar" role="toolbar" aria-label="Toolbar with button groups">
              <div class="btn-group btn-group-sm mr-2" role="group" aria-label="Login">
                <button type="button" (click)="logout()" class='btn btn-light'>
                  <i class="fas fa-sign-out-alt" aria-hidden="true"></i><br>Logout
                </button>
              </div>
              <div class="btn-group btn-group-sm mr-2" role="group" aria-label="Orders">
                <button type="button" (click)="refresh()" [disabled]="isLoading" class='btn btn-light'>
                    <i class="fas fa-sync" aria-hidden="true"></i><br>Refresh
                </button>
                <button type="button" (click)="showOrderDetails()" [disabled]="isLoading" class='btn btn-light'>
                    <i class="fas fa-file" aria-hidden="true"></i><br>Order Details
                </button>
                <button type="button" (click)="downloadCoc()" [disabled]="isLoading || !canDownloadCoc"
                        class='btn btn-light'>
                    <i class="fas fa-certificate" aria-hidden="true"></i><br>Download COC
                </button>
              </div>
              <div class="btn-group btn-group-sm mr-2" role="group" aria-label="Filters">
                <button type="button" (click)="showAll()" [disabled]="isLoading" class='btn btn-light'>
                    <i class="fas fa-folder" aria-hidden="true"></i><br>Show All
                </button>
                <button type="button" (click)="showSearch()" [disabled]="isLoading" class='btn btn-light'>
                    <i class="fas fa-search" aria-hidden="true"></i><br>Search
                </button>
              </div>
              <div *ngIf="settings?.showOrderApprovals" class="btn-group btn-group-sm mr-2"
                    role="group" aria-label="Approvals">
                <button type="button" (click)="showApprovals()" class='btn btn-light'>
                  <i class="fas fa-check-circle" aria-hidden="true"></i><br>Approvals
                </button>
              </div>
              <div class="btn-group btn-group-sm " role="group" aria-label="Reports">
                <button type="button" (click)="downloadOpenOrders()" [disabled]="isLoading" class='btn btn-light'>
                    <i class="far fa-file-excel" aria-hidden="true"></i><br>Open Orders
                </button>
              </div>
            </div>
          </div>
        </div>
        <div *ngIf="isLoading" class="container-fluid text-center">
            <p>
                <i class="fas fa-spinner fa-spin fa-3x" aria-hidden="true"></i>
                <span class="sr-only">Loading...</span>
            </p>
        </div>
        <div *ngIf="summariesError" class='text-center'>
          <p>Unable to show your orders at this time. Please refresh:</p>
          <button type="button" (click)="refresh()" class='btn btn-primary'>Refresh</button>
        </div>
        <div *ngIf="!summariesError" class='container-fluid'>
          <table class='table table-striped table-sm table-responsive order-table'>
            <thead class='thead-dark'>
            </thead>
            <tbody>
            </tbody>
          </table>
        </div>

        <dwos-order-search-modal (search)="onSearch($event)"></dwos-order-search-modal>`,
})
export class IndexComponent implements OnInit, AfterViewInit, OnDestroy {
    private static resetSearch(dataTable: DataTables.Api): void {
        dataTable.search(""); // reset global search
        dataTable.columns().search(""); // reset column-specific searches
        $(".filter-form-control").val("");
    }

    public summaries: OrderSummary[] = [];
    public summariesError: boolean;
    public isLoading: boolean = true;

    @ViewChild(OrderSearchModalComponent, { static: false })
    private searchComponent: OrderSearchModalComponent;

    private authFailedSubscription: Subscription;
    private settings: AppSettings;
    private viewInitialized: boolean;

    constructor(
        private api: ApiService,
        private ordersApi: OrderService,
        private file: FileService,
        private router: Router,
        private element: ElementRef) {
    }

    public ngOnInit(): void {
        this.authFailedSubscription = this.api.authFailed.subscribe(() => {
            this.router.navigate(["/login"]);
        });

        this.api.getSettings().subscribe(
            (settings: AppSettings) => {
                this.settings = settings;
                this.refresh();
            },
            (error: any) => {
                alert("Error loading order settings. Please refresh the page and try again.");
                throw error;
            });
    }

    public ngAfterViewInit(): void {
        this.viewInitialized = true;
        if (this.summaries && !this.isTableInitialized()) {
            this.initializeTable();
        }
    }

    public ngOnDestroy(): void {
        if (this.authFailedSubscription) {
            this.authFailedSubscription.unsubscribe();
        }
    }

    public logout(): void {
        this.api.logout();
        this.router.navigate(["/login"]);
    }

    public refresh(): void {
        this.summariesError = false;
        this.isLoading = true;

        this.ordersApi.getOrderSummaries()
            .subscribe((orders: OrderSummary[]) => {
                this.summaries = orders;

                if (this.isTableInitialized()) {
                    const table = $(this.element.nativeElement.querySelector(".order-table"));
                    const dataTable: DataTables.Api = table.DataTable();
                    dataTable.clear();
                    dataTable.rows.add(this.summaries);
                    dataTable.draw();
                } else if (this.viewInitialized) {
                    this.initializeTable();
                }

                this.isLoading = false;
            }, (error: any) => {
                this.isLoading = false;
                this.summariesError = true;
                throw error;
            });
    }

    public showOrderDetails(): void {
        const table = $(this.element.nativeElement.querySelector(".order-table"));
        const dataTable: DataTables.Api = table.DataTable();

        const selectedOrders: DataTables.Api = dataTable.rows({ selected: true, page: "current" }).data();

        if (selectedOrders.length !== 1) {
            return;
        }

        const selectedOrder: OrderSummary = selectedOrders[0];

        this.router.navigate([`/order/${selectedOrder.orderId}`]);
    }

    public showApprovals(): void {
        this.router.navigate(["/approvals/"]);
    }

    public showSearch(): void {
        this.searchComponent.show();
    }

    public onSearch(options: OrderSearchOptions): void {
        const table = $(this.element.nativeElement.querySelector(".order-table"));
        const dataTable: DataTables.Api = table.DataTable();

        IndexComponent.resetSearch(dataTable);

        const column: DataTables.ColumnMethods = dataTable.column(`${options.searchType}:name`);
        column.search(options.searchText).draw();
    }

    public showAll(): void {
        const table = $(this.element.nativeElement.querySelector(".order-table"));
        const dataTable: DataTables.Api = table.DataTable();

        IndexComponent.resetSearch(dataTable);
        dataTable.draw();
    }

    public get canDownloadCoc(): boolean {
        if (!this.isTableInitialized()) {
            return false;
        }

        const table = $(this.element.nativeElement.querySelector(".order-table"));
        const dataTable: DataTables.Api = table.DataTable();

        const selectedOrders: DataTables.Api = dataTable.rows({ selected: true, page: "current" }).data();

        if (selectedOrders.length !== 1) {
            return false;
        }

        const selectedOrder: OrderSummary = selectedOrders[0];
        return selectedOrder.status === "Closed";
    }

    public downloadCoc(): void {
        const table = $(this.element.nativeElement.querySelector(".order-table"));
        const dataTable: DataTables.Api = table.DataTable();

        const selectedOrders: DataTables.Api = dataTable.rows({ selected: true, page: "current" }).data();

        if (selectedOrders.length !== 1) {
            return;
        }

        const selectedOrder: OrderSummary = selectedOrders[0];

        if (!selectedOrder.certificationUrl) {
            alert("COC not found for this order");
            return;
        }

        const fileName: string = `COCReport_${selectedOrder.certificationId}.pdf`;

        this.api.getFile(selectedOrder.certificationUrl)
            .subscribe(
                (data: FileData) => this.file.save(FileData.toBlob(data), fileName, data.type),
                (error: any) => {
                    alert("Could not retrieve COC for this order.");
                });
    }

    public downloadOpenOrders(): void {
        this.api.getFile("/api/reports/OpenOrders")
            .subscribe(
                (data: FileData) => this.file.save(FileData.toBlob(data), "CurrentOrderStatus.xlsx", data.type),
                (error: any) => {
                    alert("Could not download the Open Orders report.");
                });
    }

    private isTableInitialized(): boolean {
        const table = $(this.element.nativeElement.querySelector(".order-table"));
        const dtStatic: any = $.fn.DataTable;

        return dtStatic.isDataTable(table);
    }

    private initializeTable(): void {
        if (!this.settings) {
            // do not initialize until app data is ready
            return;
        }

        const table = $(this.element.nativeElement.querySelector(".order-table"));

        const tableOptions: DataTables.Settings = {
            autoWidth: false,
            columns: [
                {
                    data: "orderId",
                    name: "orderId",
                    title: "W.O",
                },
                {
                    data: "customerWorkOrder",
                    name: "customerWorkOrder",
                    title: "Cust W.O.",
                },
                {
                    data: "purchaseOrder",
                    name: "purchaseOrder",
                    title: "P.O.",
                },
                {
                    data: "manufacturerId",
                    name: "manufacturerId",
                    title: "Manufacturer",
                    visible: this.settings.showManufacturer,
                },
                {
                    data: "partName",
                    name: "partName",
                    title: "Part",
                },
                {
                    data: "partQuantity",
                    name: "partQuantity",
                    title: "Quantity",
                },
                {
                    data: "priority",
                    name: "priority",
                    title: "Priority",
                },
                {
                    data: "status",
                    name: "status",
                    title: "Status",
                },
                {
                    data: "currentLocation",
                    name: "currentLocation",
                    title: "Location",
                },
                {
                    data: "currentProcess",
                    name: "currentProcess",
                    title: "Current Process"
                },
                {
                    data: "currentProcessStartDate",
                    name: "currentProcessStartDate",
                    render: this.renderDate,
                    title: "Process Start Date"
                },
                {
                    data: "requiredDate",
                    name: "requiredDate",
                    render: this.renderDate,
                    title: "Required Date",
                    visible: this.settings.showRequiredDate,
                },
                {
                    data: "completedDate",
                    name: "completedDate",
                    render: this.renderDate,
                    title: "Completed",
                },
                {
                    data: "estShipDate",
                    name: "estShipDate",
                    render: this.renderDate,
                    title: "Est Ship Date",
                },
                {
                    data: "trackingNumber",
                    name: "trackingNumber",
                    render: this.renderTrackingNumber,
                    title: "Tracking Number",
                    visible: this.settings.showTrackingNumber,
                },
                {
                    data: "serialNumbers",
                    name: "serialNumbers",
                    render: this.renderSerialNumbers,
                    title: "Serial Number",
                    visible: this.settings.showSerialNumbers,
                },
            ],
            data: this.summaries,
            deferRender: true,
            // show processing display, table, information summary, pagination
            dom: "rtip",
            lengthChange: false,
            order: [
                [0, "desc"],
            ],
            pageLength: 15,
            select: {
                style: "single",
            },

        };

        const dataTable: DataTables.Api = table.DataTable(tableOptions);

        // add per-column filter boxes
        const headers = table.find("th");
        headers.each((index: number, element: Element) => {
            const title: string = $(element).text();
            $(element).html(`${title}<p><input type="text" class="form-control filter-form-control" /></p>`);
        });

        // implement per-column filter.
        dataTable.columns().every(function(): void {
            const column: DataTables.ColumnMethods = this;

            $("input", column.header()).on("keyup change", function(): void {
                const input: string = $(this).val().toString();
                if (column.search() !== input) {
                    column.search(input).draw();
                }
            });

            $("input", column.header()).click((evt) => {
                evt.stopPropagation();
            });
        });
    }

    /**
     * Renders a date for the data table.
     * @param data
     * @param type
     */
    private renderDate(data: string, type: string): string {
        if (type !== "display" || !data) {
            return data;
        }

        const date: Date = new Date(data);
        return (date.getMonth() + 1) + "/" + date.getDate() + "/" + date.getFullYear();
    }

    /**
     * Renders a tracking number for the data table.
     * @param data
     * @param type
     * @param row
     */
    private renderTrackingNumber(data: string, type: string, row: OrderSummary): string {
        if (type !== "display" || !row || !row.shippingCarrier || !data) {
            return data;
        }

        let url: string;

        switch (row.shippingCarrier) {
            case "UPS":
            case "UPS Ground":
                url = "http://wwwapps.ups.com/WebTracking/track?track=yes&trackNums=" + data;
                break;
            case "FedEx":
            case "FedEx Ground":
                url = "http://www.fedex.com/Tracking?action=track&tracknumbers=" + data;
                break;
            case "USPS":
                url = "https://tools.usps.com/go/TrackConfirmAction?qtc_tLabels1=" + data;
                break;
        }

        if (url) {
            return "<a target='_blank' href='" + url + "'>" + data + "</a>";
        }

        return data;
    }

    /**
     * Renders serial numbers for the data table.
     * @param data
     * @param type
     */
    private renderSerialNumbers(data: string[], type: string): string | string[] {
        if (type !== "display" || !data) {
            return data;
        }

        return data.join(", ");
    }
}
