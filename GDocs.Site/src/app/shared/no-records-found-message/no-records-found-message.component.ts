import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-no-records-found-message',
  templateUrl: './no-records-found-message.component.html',
  styleUrls: ['./no-records-found-message.component.scss']
})
export class NoRecordsFoundMessageComponent implements OnInit {

  @Input() message = 'Nenhum registro localizado.';

  constructor() {
    /* Only comments */
  }

  ngOnInit() {
    /* Only comments */
  }
}
