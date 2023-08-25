import { ItemSaidaMateialModel } from './../shared/models/item-saida-material.model';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { ExibicaoDeAlertaService } from 'src/app/core';
import { SaidaMaterialModel } from 'src/app/FI347/shared/models/saida-material.model';
import { UsuarioModel } from 'src/app/usuario/models';
import { UsuarioService } from '../../usuario/usuario.service';
import { Fi1347Service } from '../FI347.service';
import { TipoSaida } from '../shared/models/tipo-saida.model';
import { FormValidationHelper } from 'src/app/helpers';


@Component({
  selector: 'app-adicionar',
  templateUrl: './adicionar.component.html',
  styleUrls: ['./adicionar.component.scss']
})
export class AdicionarComponent implements OnInit {
  DocumentoSaidaDeMaterial: SaidaMaterialModel;
  formDadosMaterialSaida: FormGroup;
  listarTipoSaida: TipoSaida[];
  dataSource:ItemSaidaMateialModel[]=[];
  formValidationMaterialHelper: FormValidationHelper;

  @ViewChild('btnDataPicker') botaoDatePicker: ElementRef;

  constructor(
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
    private readonly formBuilder: FormBuilder,
    private readonly router: Router,
    private readonly usuarioService: UsuarioService,
    private readonly service: Fi1347Service,
    private readonly activedRoute: ActivatedRoute,
    public dialog: MatDialog,
    ) { }

  async ngOnInit() {

    await this.formInit();

  }

    async formInit()  {
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.dialog.closeAll();
      }
    });

    this.tipoSaidaInit();

    this.formValidationMaterialHelper = new FormValidationHelper({
      tiposaida:"Tipos de saída",
      origem: 'Origem',
      destino:"Destino",
      dataPrevista:"Data prevista de retorno",
      motivo:"Motivo do Serviço",
      observacoes:"Observações",
      setor:"Sigla do setor"

    });


    this.formDadosMaterialSaida = this.formBuilder.group({
      tiposaida: new FormControl(null, [
        Validators.required,
      ]),
      setor: new FormControl(null, [
        Validators.required,
        Validators.maxLength(3)
      ]),
      origem: new FormControl(null, [
        Validators.required,
        Validators.maxLength(10),
      ]),
      destino: new FormControl(null, [
        Validators.required,
        Validators.maxLength(70),
      ]),
      dataPrevista: new FormControl(null, [
        Validators.required,
      ]),
      motivo: new FormControl(null, [
        Validators.required,
        Validators.maxLength(255),
      ]),
      observacoes: new FormControl(null, [
        Validators.maxLength(200),
      ])
    })

    await this.EditarDocumento();
}


private CompletarFormularioEdicao()
{
  this.formDadosMaterialSaida.get('tiposaida').setValue(this.DocumentoSaidaDeMaterial.flgRetorno?1:0);
  this.formDadosMaterialSaida.get('setor').setValue(this.DocumentoSaidaDeMaterial.setorResponsavel);
  this.formDadosMaterialSaida.get('origem').setValue(this.DocumentoSaidaDeMaterial.origem);
  this.formDadosMaterialSaida.get('destino').setValue(this.DocumentoSaidaDeMaterial.destino);
  this.formDadosMaterialSaida.get('dataPrevista').setValue(this.DocumentoSaidaDeMaterial.retorno);
  this.formDadosMaterialSaida.get('motivo').setValue(this.DocumentoSaidaDeMaterial.motivo);
  this.formDadosMaterialSaida.get('observacoes').setValue(this.DocumentoSaidaDeMaterial.observacao);

  this.ClickTipoRetorno()
}


  EventoItemMaterial(valor :ItemSaidaMateialModel[]){
    this.dataSource = valor;
  }


  gravarFormDados(): void {
     if (!this.validarFormulario()) {
      return;
    }

      const SaidaDeMaterial = new SaidaMaterialModel();
      SaidaDeMaterial.flgRetorno = this.formDadosMaterialSaida.controls['tiposaida'].value;
      SaidaDeMaterial.setorResponsavel = this.formDadosMaterialSaida.controls['setor'].value;
      SaidaDeMaterial.origem = this.formDadosMaterialSaida.controls['origem'].value;
      SaidaDeMaterial.destino = this.formDadosMaterialSaida.controls['destino'].value;
      SaidaDeMaterial.retorno = this.formDadosMaterialSaida.controls['dataPrevista'].value as Date;
      SaidaDeMaterial.motivo = this.formDadosMaterialSaida.controls['motivo'].value;
      SaidaDeMaterial.itemMaterial = this.dataSource;
      SaidaDeMaterial.observacao = this.formDadosMaterialSaida.controls['observacoes'].value;

      if(this.DocumentoSaidaDeMaterial !== undefined && this.DocumentoSaidaDeMaterial != null)
      {
        SaidaDeMaterial.id = this.DocumentoSaidaDeMaterial.id
        SaidaDeMaterial.numero = this.DocumentoSaidaDeMaterial.numero;
        SaidaDeMaterial.autor = this.DocumentoSaidaDeMaterial.autor;
        SaidaDeMaterial.status = this.DocumentoSaidaDeMaterial.status;
        SaidaDeMaterial.statusId = this.DocumentoSaidaDeMaterial.statusId;
      }

      this.service.cadastrarMaterialSaida(SaidaDeMaterial)
       .subscribe(documento => {
          if(documento != null){
            this.service.enviarMaterialAssinatura(documento)
            .subscribe(pad => {
           this.router.navigate(['assinatura/edicao/',pad,documento.id]);
            })
          }
      });


  }

  ClickTipoRetorno()
  {
    if(this.formDadosMaterialSaida.value.tiposaida == 0){
      this.formDadosMaterialSaida.get('dataPrevista').disable();
      this.formDadosMaterialSaida.get('dataPrevista').setValue(null);
      this.botaoDatePicker.nativeElement.classList.add('desabilitado');
    }else{
      this.formDadosMaterialSaida.get('dataPrevista').enable();
      this.botaoDatePicker.nativeElement.classList.remove('desabilitado');
    }
  }

    private validarFormulario(): boolean {
      this.formDadosMaterialSaida.get('tiposaida').setValue(this.formDadosMaterialSaida.value.tiposaida === null ? null : this.formDadosMaterialSaida.value.tiposaida);
      this.formDadosMaterialSaida.get('setor').setValue(this.formDadosMaterialSaida.value.setor === null ? null : this.formDadosMaterialSaida.value.setor);
      this.formDadosMaterialSaida.get('origem').setValue(this.formDadosMaterialSaida.value.origem === null ? null : this.formDadosMaterialSaida.value.origem);
      this.formDadosMaterialSaida.get('destino').setValue(this.formDadosMaterialSaida.value.destino === null ? null : this.formDadosMaterialSaida.value.destino);
      this.formDadosMaterialSaida.get('dataPrevista').setValue(this.formDadosMaterialSaida.value.dataPrevista === null ? null : this.formDadosMaterialSaida.value.dataPrevista);
      this.formDadosMaterialSaida.get('motivo').setValue(this.formDadosMaterialSaida.value.motivo === null ? null : this.formDadosMaterialSaida.value.motivo);
      this.formDadosMaterialSaida.get('observacoes').setValue(this.formDadosMaterialSaida.value.observacoes === null ? null : this.formDadosMaterialSaida.value.observacoes);
      const dataFormulario = this.formDadosMaterialSaida.value.dataPrevista as Date;
      const dataAtual = new Date();

      dataAtual.setDate(dataAtual.getDate()-1);

      const errorList: Array<string> = this.formValidationMaterialHelper.getFormValidationErrorsMessages(this.formDadosMaterialSaida.controls);


      if (dataFormulario != null &&   dataFormulario < dataAtual ) {
        errorList.push('A data prevista retorno não pode ser menor que a data atual.');
      }

      if (errorList.length > 0) {
        this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', errorList.join('<br />'));
        return false;
      }

      if(this.dataSource.length == 0){
        this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', 'Obrigatório adicionar ao menos um material.');
        return false;
      }

      return true;
    }

    cancelar(): void {
      this.exibicaoDeAlertaService
        .exibirMensagemAviso('Atenção!', 'Operação cancelada.')
        .then(() => {
          this.router.navigate(['fi347/consultar']);
        });
    }

    private tipoSaidaInit () {
      this.service.listarTipoSaida().subscribe((saidaTipo) => {
        this.listarTipoSaida = saidaTipo;
      });
    }

    async EditarDocumento(){
      const ssmId = this.activedRoute.snapshot.paramMap.get('ssmid');

      if (ssmId !== null && ssmId !== undefined && ssmId !== '' && !isNaN(Number(ssmId.toString()))) {
          await this.buscaMaterial(ssmId);
      }

      if(this.DocumentoSaidaDeMaterial !== undefined ){
        this.CompletarFormularioEdicao()
      }

    }

    async buscaMaterial(ssmId){
      await this.service.obterSolicitacaoMaterialPorId(ssmId)
      .toPromise()
      .then((material) => {
        if(material !== undefined && material != null){
          this.DocumentoSaidaDeMaterial = material
          this.dataSource = this.DocumentoSaidaDeMaterial.itemMaterial;
        }
      })
    }

}


