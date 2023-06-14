import { HTTP_INTERCEPTORS } from "@angular/common/http";
import { CachingInterceptor } from "./caching-interceptor";

/** HTTP interceptor providers. */
export const httpInterceptorProviders = [
    { provide: HTTP_INTERCEPTORS, useClass: CachingInterceptor, multi: true },
];
