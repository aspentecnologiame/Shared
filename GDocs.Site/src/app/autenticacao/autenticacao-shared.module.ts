import { NgModule } from '@angular/core';
import { HasPermitionDirective } from './directives/has-permition.directive';

@NgModule({
    declarations: [
        HasPermitionDirective
    ],
    exports: [HasPermitionDirective]
})
export class AutenticacaoSharedModule { }
