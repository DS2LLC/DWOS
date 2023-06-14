import { Component, ElementRef, EventEmitter, Output } from "@angular/core";
import * as $ from "jquery";

/**
 * Modal component that shows order search options.
 */
@Component({
    selector: "dwos-order-search-modal",
    template: `
        <div class='modal fade'>
          <div class="modal-dialog" role="document">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title">Order Search</h5>
                <button type="button" class="close" (click)="cancel()" aria-label="Close">
                  <span aria-hidden="true">&times;</span>
                </button>
              </div>
              <div class="modal-body">
                <p>Search for an existing order.</p>
                <form #f="ngForm">
                  <div class="form-check">
                    <label class="form-check-label">
                      <input class="form-check-input" type="radio" name="searchType" [(ngModel)]="searchType"
                             value="orderId">
                      Work order
                    </label>
                  </div>
                  <div class="form-check">
                    <label class="form-check-label">
                      <input class="form-check-input" type="radio" name="searchType" [(ngModel)]="searchType"
                             value="purchaseOrder">
                      Purchase Order
                    </label>
                  </div>
                  <div class="form-check">
                    <label class="form-check-label">
                      <input class="form-check-input" type="radio" name="searchType" [(ngModel)]="searchType"
                             value="partName">
                      Part Number
                    </label>
                  </div>
                  <div class="form-check">
                    <label class="form-check-label">
                      <input class="form-check-input" type="radio" name="searchType" [(ngModel)]="searchType"
                             value="customerWorkOrder">
                      Customer WO
                    </label>
                  </div>
                  <input type="text" class="form-control" name="searchText" [(ngModel)]="searchText">
                </form>
              </div>
              <div class="modal-footer">
                <button type="button" (click)="accept()" class="btn btn-primary">OK</button>
                <button type="button" (click)="cancel()" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
              </div>
            </div>
          </div>
        </div>`,
})
export class OrderSearchModalComponent {
    @Output()
    public search: EventEmitter<OrderSearchOptions> = new EventEmitter();

    public searchType = "orderId";
    public searchText: string = null;

    private previousSearchType = "orderId";
    private previousSearchText: string = null;

    constructor(private element: ElementRef) { }

    public accept(): void {
        // typings for Bootstrap v4 don't seem to be available
        const jqueryElement: any = $(this.element.nativeElement.querySelector(".modal"));
        jqueryElement.modal("hide");

        this.previousSearchType = this.searchType;
        this.previousSearchText = this.searchText;

        this.search.emit(new OrderSearchOptions(this.searchType, this.searchText));
    }

    public show(): void {
        // typings for Bootstrap v4 don't seem to be available
        const jqueryElement: any = $(this.element.nativeElement.querySelector(".modal"));
        jqueryElement.modal("show");
    }

    public cancel(): void {
        // typings for Bootstrap v4 don't seem to be available
        const jqueryElement: any = $(this.element.nativeElement.querySelector(".modal"));
        jqueryElement.modal("hide");

        this.searchType = this.previousSearchType;
        this.searchText = this.previousSearchText;
    }
}

export class OrderSearchOptions {
    constructor(public searchType: string, public searchText: string) { }
}
