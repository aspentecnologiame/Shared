import {
    Directive,
    HostListener,
} from "@angular/core";
import { NgControl } from "@angular/forms";

@Directive({
    selector: "[formControlNumericOnly]"
})
export class FormControlNumericOnlyDirective {
  constructor(private readonly control: NgControl) { }

  @HostListener("input", ["$event"])
  @HostListener("paste", ["$event"])
  onEventResolver(event: any) {
    this.control.control.setValue(this.control.value.replace(/[^0-9]/g, ''));
    if(this.control.value.match(/^(\d*)$/gm)){
      event.stopPropagation();
    }
  }
}
