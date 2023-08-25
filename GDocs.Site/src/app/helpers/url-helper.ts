export class UrlHelper {

    static buildUrl(urlTemplate: string, params: any): string {
        const names = Object.keys(params);

        names.forEach(key => {
            const reg = new RegExp(`\\{${key}\\}`, 'gm');
            urlTemplate = urlTemplate.replace(reg, params[key]);
        });

        return urlTemplate;
    }
}
