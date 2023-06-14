import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, of } from "rxjs";
import { tap } from "rxjs/operators";

import { AppSettings } from "../app-settings.model";

/**
 * Caches some HTTP responses.
 */
@Injectable()
export class CachingInterceptor implements HttpInterceptor {
    private settings: AppSettings;

    public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (!req.url.endsWith("/api/settings")) {
            return next.handle(req);
        }

        if (this.settings) {
            return of(new HttpResponse({ body: this.settings }));
        }

        return next.handle(req).pipe(
            tap((event: any) => {
                if (event instanceof HttpResponse) {
                    this.settings = event.body;
                }
            }));
    }
}
