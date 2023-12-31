import { Directive, HostListener, Input } from '@angular/core';

@Directive({
  selector: '[copyClipboard]'
})
export class CopyClipboardDirective {
  @Input("copyClipboard")
  public payload: string = "";

  @HostListener("click", ["$event"])
  public onClick(event: MouseEvent): void {

    event.preventDefault();
    if (!this.payload)
      return;

    window.navigator['clipboard'].writeText(this.payload.toString());
  }

}
