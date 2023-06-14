import { Attribute, Directive, forwardRef } from "@angular/core";
import { AbstractControl, NG_VALIDATORS, Validator } from "@angular/forms";

/**
 * Validator that requires one control's value to be equal to another control's value.'
 */
@Directive({
    providers: [
        { provide: NG_VALIDATORS, useExisting: forwardRef(() => EqualFieldValidator), multi: true },
    ],
    selector: "[validateEqual][formControlName],[validateEqual][formControl],[validateEqual][ngModel]",
})
export class EqualFieldValidator implements Validator {
    constructor( @Attribute("validateEqual") private validateEqual: string) { }

    public validate(c: AbstractControl): { [index: string]: any; } {
        const actualValue: any = c.value;
        const expectedValue: any = c.root.get(this.validateEqual).value;

        if (expectedValue !== actualValue) {
            return { validateEqual: true };
        }

        return null;
    }
}
