import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";

import { Observable, Subject, throwError } from "rxjs";
import { catchError, tap } from "rxjs/operators";

import { AppSettings } from "./app-settings.model";
import { ContactInfo } from "./contact-info.model";
import { ClientError } from "./error.model";
import { FileData } from "./file-data.model";
import { Header } from "./header.model";
import { UrlService } from "./url.service";
import { User } from "./user.model";

/**
 * Represents an API service error.
 */
export class ApiServiceError {
    constructor(public status: number) {}

    get isUnauthorized(): boolean {
        return this.status === 401;
    }
}

/**
 * The primary service for communicating with the server.
 * Handles authentication and server access.
 */
@Injectable()
export class ApiService {
    /**
     * Emits whenever the current user changes.
     */
    public userChange = new Subject();

    /**
     * Occurs whenever authentication fails.
     */
    public authFailed  = new Subject<ApiServiceError>();

    private rememberMe: boolean;

    constructor(private http: HttpClient, private urlService: UrlService) { }

    /**
     * The current user's name.
     */
    get userName(): string {
        const userName: string = localStorage.getItem("username") || sessionStorage.getItem("username");
        return userName;
    }

    /**
     * The current user's password.
     */
    public get password(): string {
        return localStorage.getItem("password") || sessionStorage.getItem("password");
    }

    /**
     * Logs a user in.
     * @param userName The user's email address
     * @param password The user's password
     * @param rememberMe If true, persist user login info over multiple sessions.
     */
    public login(userName: string, password: string, rememberMe: boolean): Observable<User> {
        this.rememberMe = rememberMe;

        if (rememberMe) {
            localStorage.setItem("username", userName);
            localStorage.setItem("password", password);
        } else {
            sessionStorage.setItem("username", userName);
            sessionStorage.setItem("password", password);
        }

        return this.postApi<User>("/api/login/login", {}).pipe(tap(() => this.userChange.next()));
    }

    /**
     * Resets a user's password.
     * @param userName The user's email address.
     */
    public resetPassword(userName: string): Observable<any> {
        const body: any = {
            emailAddress: userName,
        };

        const headers: HttpHeaders = new HttpHeaders({
            "Content-Type": "application/json",
        });

        return this.http.post(this.urlService.getUrl("/api/login/reset_password"), body, { headers })
            .pipe(catchError((error: HttpErrorResponse) => this.handleError(error)));
    }

    /**
     * Retrieves data for the current user.
     */
    public getUserData(): Observable<User> {
        return this.getApi("/api/user");
    }

    /**
     * Retrieves contact information for the current user.
     */
    public getContactInfo(): Observable<ContactInfo> {
        return this.getApi("/api/contactinfo");
    }

    /**
     * Updates contact information for the current user.
     * @param info
     */
    public updateContactInfo(info: ContactInfo): Observable<any> {
        if (!info) {
            return throwError("Did not provide contact info.");
        }

        return this.putApi("/api/contactinfo", info);
    }

    /**
     * Updates the current user's password.
     * @param currentPassword
     * @param newPassword
     */
    public updatePassword(currentPassword: string, newPassword: string): Observable<any> {
        if (!currentPassword) {
            return throwError("Did not provide current password.");
        }

        if (!newPassword) {
            return throwError("Did not provide new password.");
        }

        const data: any = {
            currentPassword,
            newPassword,
        };

        return this.postApi("/api/login/updatepassword", data)
            .pipe(tap(() => {
                // update API credentials if successful.
                if (this.rememberMe) {
                    localStorage.setItem("password", newPassword);
                } else {
                    sessionStorage.setItem("password", newPassword);
                }
            }));
    }

    /**
     * Retrieves company-specific data to show in the site's header.
     */
    public getHeaderData(): Observable<Header> {
        const headers: HttpHeaders = new HttpHeaders({
            "Content-Type": "application/json",
        });

        return this.http.get<Header>(this.urlService.getUrl("/api/header"), { headers })
            .pipe(catchError((error: HttpErrorResponse) => this.handleError(error)));
    }

    /**
     * Retrieves a file (as FileData) from the server.
     * @param url
     */
    public getFile(url: string): Observable<FileData> {
        return this.getApi(url);
    }

    /**
     * Retrieves application settings from the server.
     */
    public getSettings(): Observable<AppSettings> {
        return this.getApi("/api/settings");
    }

    /**
     * Gets a value indicating if the current user is logged-in.
     */
    get loggedIn(): boolean {
        return (this.userName || "").length > 0;
    }

    /**
     * Logs a the current user out.
     */
    public logout(): void {
        localStorage.removeItem("username");
        localStorage.removeItem("password");
        sessionStorage.removeItem("username");
        sessionStorage.removeItem("password");
        this.userChange.next();
    }

    /**
     * Makes a GET request to the API.
     * @param url partial API url to request
     */
    public getApi<T>(url: string): Observable<T> {
        return this.http.get<T>(this.urlService.getUrl(url), { headers: this.authHeaders })
            .pipe(catchError((error: HttpErrorResponse) => this.handleError(error)));
    }

    /**
     * Makes a POST request to the API.
     * @param url partial API URL to request
     * @param body the request body
     */
    public postApi<T>(url: string, body: any): Observable<T> {
        return this.http.post<T>(this.urlService.getUrl(url), body, { headers: this.authHeaders })
            .pipe(catchError((error: HttpErrorResponse) => this.handleError(error)));
    }

    /**
     * Makes a PUT request to the API.
     * @param url partial API URL to request
     * @param body the request body
     */
    public putApi<T>(url: string, body: any): Observable<T> {
        return this.http.put<T>(this.urlService.getUrl(url), body, { headers: this.authHeaders })
            .pipe(catchError((error: HttpErrorResponse) => this.handleError(error)));
    }

    /**
     * Sends an error (with stackTrace) to the server.
     * @param stackTrace The stack trace for the error.
     */
    public sendError(stackTrace: string[]): Observable<any> {
        const clientError: ClientError = {
            stackTrace,
            userAgent: navigator.userAgent,
        };

        const headers: HttpHeaders = new HttpHeaders({
            "Content-Type": "application/json",
        });

        return this.http.post(this.urlService.getUrl("/api/error/logerror"), clientError, { headers })
            .pipe(catchError((error: HttpErrorResponse) => this.handleError(error)));
    }

    private get authHeaders(): HttpHeaders {
        return new HttpHeaders({
            "Authorization": "Basic " + btoa(this.userName + ":" + this.password),
            "Content-Type": "application/json",
        });
    }

    private extractData(res: Response): any {
        if (res.text()) {
            return res.json() || {};
        } else {
            return {};
        }
    }

    private handleError(error: HttpErrorResponse) {
        if (error.error instanceof ErrorEvent) {
            console.error(error.error.message);
            return throwError("An error has occurred.");
        } else {
            const apiError = new ApiServiceError(error.status);

            if (apiError.isUnauthorized) {
                // clear login info - cannot use this login
                console.error("Authentication failed");
                this.logout();
                this.authFailed.next(apiError);
            }

            return throwError(apiError);
        }
    }
}
