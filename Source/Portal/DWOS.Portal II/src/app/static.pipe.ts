import { Pipe, PipeTransform } from "@angular/core";
import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser";
import { UrlService } from "./url.service";

/**
 * Converts a partial static file path to a valid URL.
 */
@Pipe({
    name: "static",
})
export class StaticPipe implements PipeTransform {
    constructor(private sanitizer: DomSanitizer, private urlService: UrlService) { }

    public transform(partialUrl: string): SafeResourceUrl {
        return this.sanitizer.bypassSecurityTrustResourceUrl(this.urlService.getUrl(partialUrl));
    }
}
