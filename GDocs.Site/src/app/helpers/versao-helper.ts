export class VersaoHelper {
    static normalizacaoParaOrdenacao(versao: string): string {
        return [...versao.split('.'), '0', '0', '0', '0', '0', '0', '0', '0', '0', '0'].slice(0, 10).join('');
    }

    static sorting(a: string, b: string): number {
        return parseInt(VersaoHelper.normalizacaoParaOrdenacao(a || ''), 0) > parseInt(VersaoHelper.normalizacaoParaOrdenacao(b || ''), 0) ? 1 : -1;
    }
}
