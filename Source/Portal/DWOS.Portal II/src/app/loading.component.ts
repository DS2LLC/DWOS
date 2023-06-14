import { Component } from "@angular/core";

@Component({
    selector: "dwos-loading",
    template: `
        <div class="container-fluid text-center">
          <p>
            <i class="fas fa-spinner fa-spin fa-3x" aria-hidden="true"></i>
              <span class="sr-only">Loading...</span>
          </p>
        </div>
    `,
})
export class LoadingComponent {

}
