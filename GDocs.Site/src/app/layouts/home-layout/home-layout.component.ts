import { Component, OnInit,  } from '@angular/core';

@Component({
  selector: 'app-home-layout',
  template: `
    <div class="container">
      <app-header-bar></app-header-bar>
      <div class="wrap-box">
        <router-outlet></router-outlet>
      </div>
    </div>
  `,
  styles: []
})
export class HomeLayoutComponent implements OnInit {

  constructor() {
    /* Only comments */
  }

  ngOnInit() {
    const body = document.getElementsByTagName('body')[0];
    body.classList.remove('login-body-background');
  }
}
