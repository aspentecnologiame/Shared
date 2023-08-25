import { DragDropModule } from '@angular/cdk/drag-drop';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { CdkTableModule } from '@angular/cdk/table';
import { CdkTreeModule } from '@angular/cdk/tree';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  MatAutocompleteModule,
  MatBadgeModule,
  MatBottomSheetModule,
  MatButtonModule,
  MatButtonToggleModule,
  MatCardModule,
  MatCheckboxModule,
  MatChipsModule,
  MatDatepickerModule,
  MatDialogModule,
  MatDividerModule,
  MatExpansionModule,
  MatGridListModule,
  MatIconModule,
  MatInputModule,
  MatListModule,
  MatMenuModule,
  MatNativeDateModule,
  MatPaginatorIntl,
  MatPaginatorModule,
  MatProgressBarModule,
  MatProgressSpinnerModule,
  MatRadioModule,
  MatRippleModule,
  MatSelectModule,
  MatSidenavModule,
  MatSliderModule,
  MatSlideToggleModule,
  MatSnackBarModule,
  MatSortModule,
  MatStepperModule,
  MatTableModule,
  MatTabsModule,
  MatToolbarModule,
  MatTooltipModule,
  MAT_CHECKBOX_CLICK_ACTION,
  MAT_DATE_LOCALE
} from '@angular/material';
import { MdePopoverModule } from '@material-extended/mde';
import { NgSelectModule } from '@ng-select/ng-select';
import { PdfViewerModule } from 'ng2-pdf-viewer';
import { PdfJsViewerModule } from 'ng2-pdfjs-viewer';
import { NgxMaskModule } from 'ngx-mask';
import { PerfectScrollbarConfigInterface, PerfectScrollbarModule, PERFECT_SCROLLBAR_CONFIG } from 'ngx-perfect-scrollbar';
import { TooltipModule } from 'primeng/tooltip';
import { AutenticacaoSharedModule } from './../autenticacao/autenticacao-shared.module';
import { getPtBrPaginatorIntl } from './../helpers';
import { BreadcrumbComponent } from './breadcrumb/breadcrumb.component';
import { DetalheSaidaMaterialComponent } from './detalhe-saida-material/detalhe-saida-material.component';
import { FormControlNumericOnlyDirective } from './directives/form-builder-only-numbers.directive';
import { FormControlTextsOnlyDirective } from './directives/form-builder-only-texts.directive';
import { HistoricoProrrogacaoTabelaComponent } from './historico-prorrogacao-tabela/historico-prorrogacao-tabela.component';
import { JustificativaModalComponent } from './justificativa-modal/justificativa-modal.component';
import { MaterialSaidaItemTabelaComponent } from './material-saida-item-tabela/material-saida-item-tabela.component';
import { MaterialSaidaModalComponent } from './material-saida-modal/material-saida-modal.component';
import { NoRecordsFoundMessageComponent } from './no-records-found-message/no-records-found-message.component';
import { PdfPreviewForDialogComponent } from './pdf-preview-for-dialog/pdf-preview-for-dialog.component';
import { PdfPreviewComponent } from './pdf-preview/pdf-preview.component';
import { DateNullablePipe } from './pipes/date-nullable.pipe';
import { ModeloTerminalPipe } from './pipes/modelo-terminal.pipe';
import { NumeroDecimalPipe } from './pipes/numero-decimal.pipe';
import { SafePipe } from './pipes/safe.pipe';
import { SurroundPipe } from './pipes/surround.pipe';
import { UploadComponent } from './upload/upload.component';

import { CopyClipboardDirective } from './directives/copyClipboard';
import { JustificativaCienciaRejeitadaComponent } from './justificativa-ciencia-rejeitada/justificativa-ciencia-rejeitada.component';
import { CepPipe } from './pipes/cep/cep.pipe';
import { CpfCnpjPipe } from './pipes/cpf-cnpj/cpf-cnpj.pipe';
import { MotivoCancelamentoComponent } from './motivo-cancelamento/motivo-cancelamento.component';


const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    NgSelectModule,
    MatAutocompleteModule,
    MatBadgeModule,
    MatBottomSheetModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatCardModule,
    MatCheckboxModule,
    MatChipsModule,
    MatDatepickerModule,
    MatDialogModule,
    MatDividerModule,
    MatExpansionModule,
    MatGridListModule,
    MatIconModule,
    MatInputModule,
    MatListModule,
    MatMenuModule,
    MatNativeDateModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatRadioModule,
    MatRippleModule,
    MatSelectModule,
    MatSidenavModule,
    MatSliderModule,
    MatSlideToggleModule,
    MatSnackBarModule,
    MatSortModule,
    MatStepperModule,
    MatTableModule,
    MatTabsModule,
    MatToolbarModule,
    MatTooltipModule,
    TooltipModule,
    DragDropModule,
    CdkTableModule,
    CdkTreeModule,
    ScrollingModule,
    NgxMaskModule.forRoot(),
    AutenticacaoSharedModule,
    MdePopoverModule,
    PdfViewerModule,
    PdfJsViewerModule,
    PerfectScrollbarModule
    
  ],
  declarations: [
    BreadcrumbComponent,
    NoRecordsFoundMessageComponent,
    ModeloTerminalPipe,
    CpfCnpjPipe,
    NumeroDecimalPipe,
    SurroundPipe,
    SafePipe,
    DateNullablePipe,
    FormControlNumericOnlyDirective,
    FormControlTextsOnlyDirective,
    CopyClipboardDirective,
    UploadComponent,
    PdfPreviewComponent,
    JustificativaModalComponent,
    MaterialSaidaModalComponent,
    MaterialSaidaItemTabelaComponent,
    PdfPreviewForDialogComponent,
    HistoricoProrrogacaoTabelaComponent,
    JustificativaCienciaRejeitadaComponent,
    MotivoCancelamentoComponent,
    DetalheSaidaMaterialComponent,
    CepPipe
  ],
  exports: [
    BreadcrumbComponent,
    NoRecordsFoundMessageComponent,
    ReactiveFormsModule,
    FormsModule,
    NgSelectModule,
    MatAutocompleteModule,
    MatBadgeModule,
    MatBottomSheetModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatCardModule,
    MatCheckboxModule,
    MatChipsModule,
    MatDatepickerModule,
    MatDialogModule,
    MatDividerModule,
    MatExpansionModule,
    MatGridListModule,
    MatIconModule,
    MatInputModule,
    MatListModule,
    MatMenuModule,
    MatNativeDateModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatRadioModule,
    MatRippleModule,
    MatSelectModule,
    MatSidenavModule,
    MatSliderModule,
    MatSlideToggleModule,
    MatSnackBarModule,
    MatSortModule,
    MatStepperModule,
    MatTableModule,
    MatTabsModule,
    MatToolbarModule,
    MatTooltipModule,
    TooltipModule,
    DragDropModule,
    CdkTableModule,
    CdkTreeModule,
    ScrollingModule,
    NgxMaskModule,
    ModeloTerminalPipe,
    CpfCnpjPipe,
    CepPipe,
    NumeroDecimalPipe,
    AutenticacaoSharedModule,
    DateNullablePipe,
    MdePopoverModule,
    PdfViewerModule,
    PdfJsViewerModule,
    PerfectScrollbarModule,
    FormControlNumericOnlyDirective,
    FormControlTextsOnlyDirective,
    CopyClipboardDirective,
    UploadComponent,
    PdfPreviewComponent,
    JustificativaModalComponent,
    MaterialSaidaModalComponent,
    MaterialSaidaItemTabelaComponent,
    DetalheSaidaMaterialComponent,
    HistoricoProrrogacaoTabelaComponent,
    JustificativaCienciaRejeitadaComponent,
    MotivoCancelamentoComponent,
    PdfPreviewForDialogComponent,
    SafePipe
  ],
  providers: [
    { provide: MatPaginatorIntl, useValue: getPtBrPaginatorIntl() },
    { provide: MAT_CHECKBOX_CLICK_ACTION, useValue: 'check' },
    { provide: MAT_DATE_LOCALE, useValue: 'pt-Br' },
    { provide: PERFECT_SCROLLBAR_CONFIG, useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG }
  ],
  entryComponents: [
    JustificativaModalComponent,
    MaterialSaidaModalComponent,
    MaterialSaidaItemTabelaComponent,
    HistoricoProrrogacaoTabelaComponent,
    JustificativaCienciaRejeitadaComponent,
    MotivoCancelamentoComponent,
    DetalheSaidaMaterialComponent,
    PdfPreviewForDialogComponent
  ]
})
export class SharedModule { }
