import { Component, ElementRef, EventEmitter, Output } from "@angular/core";
import * as $ from "jquery";

/**
 * Modal component that shows an 'accept approval' dialog.
 */
@Component({
    selector: "dwos-accept-approval-modal",
    template: `
        <div class='modal fade'>
          <div class="modal-dialog" role="document">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title">Order Approval</h5>
                <button type="button" class="close" (click)="cancel()" aria-label="Close">
                  <span aria-hidden="true">&times;</span>
                </button>
              </div>
              <div class="modal-body">
                <p>Are you sure that you want to accept this approval?</p>
              </div>
              <div class="modal-footer">
                <button type="button" (click)="accept()" class="btn btn-primary">OK</button>
                <button type="button" (click)="cancel()" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
              </div>
            </div>
          </div>
        </div>`,
})
export class OrderApprovalAcceptModalComponent {
    @Output()
    public acceptApproval: EventEmitter<any> = new EventEmitter();

    constructor(private element: ElementRef) { }

    public accept(): void {
        // typings for Bootstrap v4 don't seem to be available
        const jqueryElement: any = $(this.element.nativeElement.querySelector(".modal"));
        jqueryElement.modal("hide");

        this.acceptApproval.emit();
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
    }
}
