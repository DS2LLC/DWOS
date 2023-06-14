import { Pipe, PipeTransform } from "@angular/core";
import { SafeResourceUrl } from "@angular/platform-browser";

import * as moment from "moment";

/**
 * Replacement for Angular's DatePipe that works on IE and Edge.
 */
@Pipe({
    name: "moment",
})
export class MomentDateFormatPipe implements PipeTransform {
    public transform(value: any, pattern: string): SafeResourceUrl {
        if (!value) {
            return null;
        }

        let date: moment.Moment = null;
        if ((typeof value) === "number" || value instanceof Date || (typeof value) === "string") {
            date = moment(value);
        }

        if (!date) {
            return null;
        }

        return date.format(pattern);
    }
}
