import { ErrorHandler, Injectable } from "@angular/core";

import { ApiService } from "./api.service";

/**
 * Custom error handler that can send errors to the server.
 */
@Injectable()
export class ClientErrorHandler implements ErrorHandler {
    constructor(private api: ApiService) { }

    /**
     * Handles any client-side error.
     * @param error The error to log.
     */
    public handleError(error: any): void {
        console.error(error);

        if (error instanceof Error && error.stack) {
            const stack: string[] = error.stack.toString().split("\n", 3);

            this.api.sendError(stack).subscribe(
                null,
                (serverError: any) => console.error(serverError));
        }
    }
}
