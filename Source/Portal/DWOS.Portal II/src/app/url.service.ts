import { DOCUMENT } from "@angular/common";
import { Inject, Injectable } from "@angular/core";
import urljoin = require("url-join");

/**
 * Prepends the application's base URL to a partial URL.
 */
@Injectable()
export class UrlService {
    constructor(@Inject(DOCUMENT) private document: DocumentFragment) { }

    /**
     * Gets a full URL from a partial URL.
     * @param partialUrl
     */
    public getUrl(partialUrl: string): string {
        let baseUrl: string = "";
        const baseUrlElement: HTMLElement = this.document.getElementById("baseUrl");
        if (baseUrlElement instanceof HTMLInputElement) {
            baseUrl = baseUrlElement.value;
        }

        if (!baseUrl || baseUrl === "/") {
            return partialUrl;
        }

        return urljoin(baseUrl, partialUrl);
    }
}
