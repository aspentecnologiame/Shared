import { HostListener, OnInit } from '@angular/core';

export class ComponentFormBase implements OnInit {

  minHeight: string;
  private panelFooterHeight: number;

  constructor() {
    this.panelFooterHeight = 72;
  }

  ngOnInit() {
    const panelFooter = document.getElementsByClassName('panel-footer');
    if (panelFooter.length > 0) {
      this.panelFooterHeight = panelFooter[0].clientHeight;
    }

    this.calcularMinHeight();
  }

  calcularMinHeight(): void {
    const panelMinHeight = window.innerHeight - 224 - this.panelFooterHeight;
    this.minHeight = panelMinHeight.toString() + 'px';
  }

  @HostListener('window:resize', ['$event'])
  sizeChange(event) {
    this.calcularMinHeight();
  }
}
