import { AutenticacaoService } from '../services/autenticacao.service';
import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';

@Directive({
  selector: '[appHasPermition]'
})
export class HasPermitionDirective {

  @Input() set appHasPermition(key: any) {
    if (this.autenticacaoService.validarPermissao(key)) {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainerRef.clear();
    }
  }

  constructor(
    private readonly templateRef: TemplateRef<any>,
    private readonly viewContainerRef: ViewContainerRef,
    private readonly autenticacaoService: AutenticacaoService
  ) {
  }
}
