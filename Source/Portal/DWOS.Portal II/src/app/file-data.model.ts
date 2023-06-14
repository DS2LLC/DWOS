import { toByteArray } from "base64-js";

/**
 * Represents data for a single file.
 */
export class FileData {
    /**
     * Converts an instance to a Blob.
     * This is a static method because the server returns FileData instances
     * without instance methods.
     * @param file The file instance.
     */
    public static toBlob(file: FileData): Blob {
        return new Blob([toByteArray(file.content)], { type: file.type });
    }

    /**
     * The base64 content of the file.
     */
    public content: string;

    /**
     * The name of the file - may not be provided
     */
    public name: string;

    /**
     * The MIME type of the file.
     */
    public type: string;
}
