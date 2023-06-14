import { Pipe, PipeTransform } from "@angular/core";
import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser";

/**
 * Allows use of base64 strings in binding.
 */
@Pipe({
    name: "allowData",
})
export class AllowDataPipe implements PipeTransform {
    constructor(private sanitizer: DomSanitizer) { }

    public transform(src: string): SafeResourceUrl {
        if (src.startsWith("data")) {
            return this.sanitizer.bypassSecurityTrustResourceUrl(src);
        }

        return null;
    }
}
