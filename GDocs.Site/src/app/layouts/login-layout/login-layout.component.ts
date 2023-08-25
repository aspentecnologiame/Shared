import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-login-layout',
  template: `<router-outlet></router-outlet>`,
  styles: []
})
export class LoginLayoutComponent implements OnInit {

  constructor() {
    /* Only comments */
  }

  ngOnInit() {
    const body = document.getElementsByTagName('body')[0];
    body.classList.add('login-body-background');
  }
}
