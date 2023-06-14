import { Attribute, Directive, forwardRef } from "@angular/core";
import { AbstractControl, NG_VALIDATORS, Validator} from "@angular/forms";

/**
 * Validator that triggers validation for a second element on change.
 */
@Directive({
    providers: [
        { provide: NG_VALIDATORS, useExisting: forwardRef(() => TriggerValidationValidator), multi: true },
    ],
    selector: "[triggerValidation][formControlName],[triggerValidation][formControl],[triggerValidation][ngModel]",
})
export class TriggerValidationValidator implements Validator {
    constructor( @Attribute("triggerValidation") private triggerValidation: string) { }

    public validate(c: AbstractControl): { [index: string]: any; } {
        const otherElement: AbstractControl = c.root.get(this.triggerValidation);
        if (otherElement) {
            otherElement.updateValueAndValidity();
        }

        return null;
    }
}
