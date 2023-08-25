using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models.Enums
{
    public enum Perfil
    {
        [Description("Convidado")]
        Convidado = 1,
        [Description("Administrador")]
        Administrador = 2,
        [Description("Financeiro")]
        Financeiro = 3,
        [Description("Consulta Completa - Status-Pagamentos")]
        ConsultaCompletaStatusPagamentos = 4,
        [Description("Aprovação em lote")]
        AprovaçãoEmLote = 5,
        [Description("Acesso básico")]
        AcessoBásico = 6,
        [Description("EConsulta Completa - Assinatura")]
        ConsultaCompletaAssinatura = 7,
        [Description("Diretoria")]
        Diretoria = 8,
        [Description("Segurança")]
        Seguranca = 9,
        [Description(" Saída de Materia")]
        SaídaDeMaterial = 10,
        [Description("Secretária")]
        Secretaria = 11,
        [Description("Contabilidade")]
        Contabilidade = 12,

    }
}
