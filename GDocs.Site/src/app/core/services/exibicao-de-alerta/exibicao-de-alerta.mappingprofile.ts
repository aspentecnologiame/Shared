import { AlertaModel } from './alerta.model';
import 'automapper-ts';

export class ExibicaoDeAlertaMappingProfile extends AutoMapperJs.Profile {
    public profileName = 'ExibicaoDeAlertaMappingProfile';

    public configure() {
        const sourceKey = 'AlertaModel';
        const destinationKey = 'SweetAlertOptions';

        this.createMap(sourceKey, destinationKey)
            .forMember('title', (opts: AutoMapperJs.IMemberConfigurationOptions) => opts.mapFrom('titulo'))
            .forMember('html', (opts: AutoMapperJs.IMemberConfigurationOptions) => opts.mapFrom('mensagem'))
            .forMember('type', (opts: AutoMapperJs.IMemberConfigurationOptions) => opts.mapFrom('tipo'))
            .forMember('confirmButtonText', (opts: AutoMapperJs.IMemberConfigurationOptions) => opts.mapFrom('textoBotaoOk'))
            .forMember('cancelButtonText', (opts: AutoMapperJs.IMemberConfigurationOptions) => opts.mapFrom('textoBotaoCancelar'))
            .forMember('showCancelButton', (opts: AutoMapperJs.IMemberConfigurationOptions) => opts.mapFrom('exibirBotaoCancelar'))
            .forMember('showCancelButton', (opts: AutoMapperJs.IMemberConfigurationOptions) => opts.mapFrom('exibirBotaoCancelar'))
            .forMember('confirmButtonClass', (opts: AutoMapperJs.IMemberConfigurationOptions) => opts.mapFrom('estiloBotaoOk'))
            .convertToType(AlertaModel);
    }
}

