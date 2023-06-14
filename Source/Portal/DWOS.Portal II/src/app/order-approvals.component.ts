import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { Subscription } from "rxjs";
import { ApiService } from "./api.service";
import { OrderApprovalSummary } from "./order-approval-summary.model";
import { OrderService } from "./order.service";

import * as $ from "jquery";

@Component({
    template: `
        <div class='container-fluid'>
          <div class="ribbon">
            <div class="ribbon-inner btn-toolbar" role="toolbar" aria-label="Toolbar for approvals">
              <div class="btn-group btn-group-sm mr-2" role="group" aria-label="View">
                <button type="button" (click)="viewApproval()" class='btn btn-light'>
                  <i class="fas fa-file" aria-hidden="true"></i><br>View
                </button>
              </div>
            </div>
          </div>
        </div>
        <dwos-loading *ngIf="isLoading"></dwos-loading>
        <div *ngIf="summariesError" class='text-center'>
          <p>Unable to show your orders at this time. Please refresh:</p>
          <button type="button" (click)="refresh()" class='btn btn-primary'>Refresh</button>
        </div>
        <div *ngIf="!summariesError" class='container-fluid'>
          <table class='table table-striped table-sm table-responsive approvals-table'>
            <thead class='thead-dark'>
            </thead>
            <tbody>
            </tbody>
          </table>
        </div>`,
})
export class OrderApprovalsComponent implements AfterViewInit, OnInit, OnDestroy {
    private static renderStatus(data: string, type: string): string | number {
        if (type !== "sort" || !data) {
            return data;
        }

        switch (data) {
            case "Pending":
                return 0;
            case "Rejected":
                return 1;
            default:
                return 2;
        }
    }

    private static renderPendingIndicator(data: string, type: string): string {
        return (data === "Pending") ? "*" : "";
    }

    public summaries: OrderApprovalSummary[];
    public loadError: boolean;
    public isLoading: boolean = true;
    private authFailedSubscription: Subscription;
    private viewInitialized: boolean;

    constructor(
        private api: ApiService,
        private ordersApi: OrderService,
        private router: Router,
        private element: ElementRef) {
    }

    public ngAfterViewInit(): void {
        this.viewInitialized = true;
        if (this.summaries && !this.isTableInitialized()) {
            this.initializeTable();
        }
    }

    public ngOnInit(): void {
        this.authFailedSubscription = this.api.authFailed.subscribe(() => {
            this.router.navigate(["/login"]);
        });

        this.refresh();
    }

    public ngOnDestroy(): void {
        if (this.authFailedSubscription) {
            this.authFailedSubscription.unsubscribe();
        }
    }

    public viewApproval(): void {
        const table = $(this.element.nativeElement.querySelector(".approvals-table"));
        const dataTable: DataTables.Api = table.DataTable();

        const selectedApprovals: DataTables.Api = dataTable.rows({ selected: true, page: "current" }).data();

        if (selectedApprovals.length !== 1) {
            return;
        }

        const selectedApproval: OrderApprovalSummary = selectedApprovals[0];

        this.router.navigate([`/approvals/${selectedApproval.orderApprovalId}`]);
    }

    private refresh(): void {
        this.loadError = false;
        this.isLoading = true;

        this.ordersApi.getApprovalSummaries()
            .subscribe((approvals: OrderApprovalSummary[]) => {
                this.summaries = approvals;

                if (this.isTableInitialized()) {
                    const table = $(this.element.nativeElement.querySelector(".approvals-table"));
                    const dataTable: DataTables.Api = table.DataTable();
                    dataTable.clear();
                    dataTable.rows.add(this.summaries);
                    dataTable.draw();
                } else if (this.viewInitialized) {
                    this.initializeTable();
                }

                this.isLoading = false;
            },
                (error: any) => {
                    this.isLoading = false;
                    this.loadError = true;
                    throw error;
                });
    }

    private isTableInitialized(): boolean {
        const table = $(this.element.nativeElement.querySelector(".approvals-table"));
        const dtStatic: any = $.fn.DataTable;

        return dtStatic.isDataTable(table);
    }

    private initializeTable(): void {
        const table = $(this.element.nativeElement.querySelector(".approvals-table"));

        const tableOptions: DataTables.Settings = {
            autoWidth: false,
            columns: [
                {
                    data: "status",
                    name: "pendingIndicator",
                    orderable: false,
                    render: OrderApprovalsComponent.renderPendingIndicator,
                },
                {
                    data: "orderId",
                    name: "orderId",
                    title: "W.O",
                },
                {
                    data: "orderApprovalId",
                    name: "orderApprovalId",
                    title: "Approval #",
                },
                {
                    data: "status",
                    name: "status",
                    render: OrderApprovalsComponent.renderStatus,
                    title: "Status",
                },
            ],
            data: this.summaries,
            // show processing display, table, information summary, pagination
            dom: "rtip",
            lengthChange: false,
            order: [
                [3, "asc"],
                [1, "desc"],
            ],
            pageLength: 15,
            select: {
                style: "single",
            },

        };

        table.DataTable(tableOptions);
    }
}
