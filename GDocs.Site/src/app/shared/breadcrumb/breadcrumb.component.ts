import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BreadCrumb } from './breadcrumb';
import { Title } from '@angular/platform-browser';
import { AppConfig } from './../../app.config';

@Component({
    selector: 'app-breadcrumb',
    templateUrl: './breadcrumb.component.html',
    styleUrls: ['./breadcrumb.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class BreadcrumbComponent implements OnInit {
    breadcrumbs$: Array<BreadCrumb>;
    // breadcrumbs$ = this.router.events
    //     .pipe(filter(event => event instanceof NavigationEnd))
    //     .pipe(distinctUntilChanged())
    //     .pipe(map(event => this.buildBreadCrumb(this.activatedRoute.root)));
    // Build your breadcrumb starting with the root route of your current activated route
    constructor(private readonly activatedRoute: ActivatedRoute, private readonly router: Router, private readonly titleService: Title) {
        this.breadcrumbs$ = this.buildBreadCrumb(this.activatedRoute);
    }

    ngOnInit() {
      /* Only comments */
    }

    buildBreadCrumb(
        route: ActivatedRoute,
        url = '',
        breadcrumbs: Array<BreadCrumb> = []
    ): Array<BreadCrumb> {
        if (route.routeConfig !== null && route.routeConfig.data !== undefined) {
            // If no routeConfig is avalailable we are on the root path
            const label = route.routeConfig ? route.routeConfig.data['breadcrumb'] : 'Home';
            const path = route.routeConfig ? route.routeConfig.path : '';
            // In the routeConfig the complete path is not available,
            // so we rebuild it each time
            const nextUrl = `${url}${path}/`;
            const breadcrumb = {
                label,
                url: nextUrl
            };
            const newBreadcrumbs = [...breadcrumbs, breadcrumb];

            if (route.parent) {
                // If we are not on our current path yet,
                // there will be more children to look after, to build our breadcumb
                return this.buildBreadCrumb(route.parent, nextUrl, newBreadcrumbs);
            }

            return newBreadcrumbs;
        }

        if (route.parent) {
            return this.buildBreadCrumb(route.parent, url, breadcrumbs);
        }

        breadcrumbs.reverse();
        this.montarTituloDaPagina(breadcrumbs);
        return breadcrumbs;
    }

    atualizarTituloDaPagina( newTitle: string): void {
        this.titleService.setTitle( newTitle );
      }

    montarTituloDaPagina(breads: BreadCrumb[]): void {
        let caminho = '';

        breads.forEach(item => {
            if (caminho !== '') {
                caminho += ' / ';
            }
            caminho += item.label;
        });

        this.atualizarTituloDaPagina(`${AppConfig.settings.nomeApp} - ${caminho}`);
      }
}
