import { MenuModel } from './../../../autenticacao/models';
import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss']
})
export class MenuComponent implements OnInit {

  @Input() dataSource: Array<MenuModel> = [];

  constructor() { /* Only comments */ }

  ngOnInit() {
    /* Only comments */
  }

  onClickItem(item: MenuModel): void {

    this.dataSource.forEach(element => {
      element.ativo = false;
    });
    item.ativo = true;
  }
}
