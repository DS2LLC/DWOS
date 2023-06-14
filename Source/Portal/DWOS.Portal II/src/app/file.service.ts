import { DOCUMENT } from "@angular/common";
import { Inject, Injectable } from "@angular/core";

/**
 * Service that saves a blob to the user's system.
 */
@Injectable()
export class FileService {
    constructor(@Inject(DOCUMENT) private document: any) { }

    /**
     * Saves the blob to the user's system.
     * @param target The blob to save.
     * @param fileName The file name to use.
     * @param type The MIME type of the file.
     */
    public save(target: Blob, fileName: string, type: string): void {
        if (!target) {
            return;
        }

        if (window.navigator.msSaveBlob) {
            window.navigator.msSaveBlob(target, fileName);
        } else {
            let downloadsElement: HTMLDivElement = this.document.getElementById("downloads");
            if (!downloadsElement) {
                downloadsElement = this.document.createElement("div");
                this.document.body.appendChild(downloadsElement);
                downloadsElement.id = "downloads";
                downloadsElement.style.cssText = "display: none;";
            }

            downloadsElement.innerHTML = "";

            const link: HTMLAnchorElement & Node = this.document.createElement("a");
            downloadsElement.appendChild(link);

            link.href = URL.createObjectURL(target);
            link.download = fileName;
            link.type = type;
            link.click();
        }
    }

    /**
     * Opens the blob to a new tab
     * @param target The blob to open.
     * @param fileName The file name to use.
     * @param type The MIME type of the file.
     */
    public open(target: Blob, fileName: string, type: string): void {
        if (!target) {
            return;
        }

        if (window.navigator.msSaveOrOpenBlob) {
            window.navigator.msSaveOrOpenBlob(target, fileName);
        } else {
            let downloadsElement: HTMLDivElement = this.document.getElementById("downloads");
            if (!downloadsElement) {
                downloadsElement = this.document.createElement("div");
                this.document.body.appendChild(downloadsElement);
                downloadsElement.id = "downloads";
                downloadsElement.style.cssText = "display: none;";
            }

            downloadsElement.innerHTML = "";

            //const link: HTMLAnchorElement & Node = this.document.createElement("a");
            //downloadsElement.appendChild(link);

            const url = URL.createObjectURL(target);
            //link.href = url;
            //link.target = "_blank";
            //link.rel = "noopener noreferrer";
            //link.type = type;
            //link.click();
            window.open(url, "_blank");
        }
    }
}
