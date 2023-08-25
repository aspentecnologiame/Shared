import { FormGroup, AbstractControl, ValidationErrors } from '@angular/forms';

export class FormValidationHelper {

  private readonly messagesTemplate = {
    required: 'O campo {0} é obrigatório.',
    pattern: 'O campo {0} está no formato inválido.',
    email: 'O campo {0} está no formato inválido.',
    minlength: 'O campo {0} deve ter no minimo {1} caractere(s).',
    maxlength: 'O campo {0} deve ter no máximo {1} caracteres.',
    maxRows: 'O campo {0} ultrapassou o limite da área destinada.'
  };

  constructor(private readonly fieldsConfig: { [key: string]: string }) { }

  private readonly stringFormat = (str: string, ...args: string[]) => str.replace(/{(\d+)}/g, (_, index) => args[index] || '');

  private getErrorValue(errorName: string, validationErrors: ValidationErrors): any {
    switch (errorName) {
      case 'minlength':
        return validationErrors.requiredLength;
      case 'maxlength':
        return validationErrors.requiredLength;
      default:
        return validationErrors;
    }
  }

  getFormValidationErrors(controls: FormGroupControls): AllValidationErrors[] {
    let errors: AllValidationErrors[] = [];
    Object.keys(controls).forEach(key => {
      const control = controls[key];
      if (control instanceof FormGroup) {
        errors = errors.concat(this.getFormValidationErrors(control.controls));
      }
      const controlErrors: ValidationErrors = controls[key].errors;
      if (controlErrors !== null) {
        Object.keys(controlErrors).forEach(keyError => {
          errors.push({
            control_name: key,
            control_display_name: this.fieldsConfig[key] || key,
            error_name: keyError,
            error_value: this.getErrorValue(keyError, controlErrors[keyError])
          });
        });
      }
    });
    return errors;
  }

  getFormValidationErrorsMessages(controls: FormGroupControls): string[] {

    for(const control in controls){
      controls[control].markAsDirty();
    }

    return this.getFormValidationErrors(controls).map((error) => {
      const template = this.messagesTemplate[error.error_name];
      if (template) {
        return this.stringFormat(template, error.control_display_name, error.error_value);
      }

      return `${error.control_display_name}: ${error.error_name}: ${error.error_value}`;
    });
  }
}

export interface AllValidationErrors {
  control_name: string;
  control_display_name: string;
  error_name: string;
  error_value: any;
}

export interface FormGroupControls {
  [key: string]: AbstractControl;
}
