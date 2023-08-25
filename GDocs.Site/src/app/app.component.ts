import { Platform } from '@angular/cdk/platform';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AutenticacaoService } from './autenticacao';
import { LoaderContainerComponent } from './core/loader-container/loader-container.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  public componenteLoader = LoaderContainerComponent;
  title = 'ICE GDocs';

  constructor(
    private readonly autenticacaoService: AutenticacaoService,
    private readonly router: Router,
    public readonly platform: Platform

  ) {
  }

  ngOnInit(): void {
    this.autenticacaoService.obterLogoutEvent().subscribe(logoff => {
      if (logoff) {
        this.router.navigate(['/login']);
      }
    });
  }
}
