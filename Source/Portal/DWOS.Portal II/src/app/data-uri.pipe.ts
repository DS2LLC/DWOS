import { Pipe, PipeTransform } from "@angular/core";

import { FileData } from "./file-data.model";

/**
 * Generates a full base64 string for file data.
 * @see FileData
 */
@Pipe({
    name: "dataUri",
})
export class DataUriPipe implements PipeTransform {
    public transform(value: FileData, ...args: any[]): any {
        return `data:${value.type};base64,${value.content}`;
    }
}
