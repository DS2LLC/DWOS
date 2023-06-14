import { Injectable } from "@angular/core";
import { CanActivate, Router } from "@angular/router";
import { ApiService } from "./api.service";

/**
 * Route guard for resources that that require login.
 */
@Injectable()
export class ApiGuard implements CanActivate {
    constructor(private api: ApiService, private router: Router) {}

    public canActivate(): boolean {
        if (this.api.loggedIn) {
            return true;
        }

        this.router.navigate(["/login"]);
        return false;
    }
}
