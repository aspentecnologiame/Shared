import { CommonModule } from '@angular/common';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule, Optional, SkipSelf } from '@angular/core';
import { MatTooltipModule } from '@angular/material';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { NgHttpLoaderModule } from 'ng-http-loader';
import { AutenticacaoCoreModule } from './../autenticacao';
import { DropdownDirective } from './directives/dropdown.directive';
import { HeaderBarComponent } from './header-bar/header-bar.component';
import { MenuComponent } from './header-bar/menu/menu.component';
import { CacheInterceptor } from './interceptors/cache.interceptor';
import { ErrorInterceptor } from './interceptors/error.interceptor';
import { LoaderContainerComponent } from './loader-container/loader-container.component';
import { ExibicaoDeAlertaMappingProfile, ExibicaoDeAlertaService } from './services/exibicao-de-alerta';
import { FeatureTogglesService } from './services/feature-toggles/feature-toggles.service';
import { GDocsApiEndpointService } from './services/gdocs-api/gdocs-api-endpoint.service';


@NgModule({
  imports: [
    RouterModule,
    CommonModule,
    NgHttpLoaderModule,
    BrowserModule,
    BrowserAnimationsModule,
    AutenticacaoCoreModule,
    MatTooltipModule
  ],
  declarations: [
    LoaderContainerComponent,
    HeaderBarComponent,
    MenuComponent,
    DropdownDirective
  ],
  exports: [
    RouterModule,
    NgHttpLoaderModule,
    BrowserModule,
    BrowserAnimationsModule,
    HeaderBarComponent,
    AutenticacaoCoreModule,
    DropdownDirective,
    MatTooltipModule
  ],
  providers: [
    ExibicaoDeAlertaService,
    GDocsApiEndpointService,
    FeatureTogglesService,
    { provide: HTTP_INTERCEPTORS, useClass: CacheInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
  ],
  entryComponents: [LoaderContainerComponent]
})
export class CoreModule {
  constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
    if (parentModule) {
      throw new Error(
        'CoreModule is already loaded. Import it in the AppModule only');
    }

    automapper.initialize((cfg: AutoMapperJs.IConfiguration) => {
      cfg.addProfile(new ExibicaoDeAlertaMappingProfile());
    });

    automapper.createMap('AlertaModel', 'SweetAlertOptions').withProfile('ExibicaoDeAlertaMappingProfile');
  }
}
