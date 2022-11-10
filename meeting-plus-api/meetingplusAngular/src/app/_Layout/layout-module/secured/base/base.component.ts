import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-base',
  templateUrl: './base.component.html',
  styleUrls: ['./base.component.scss']
})
export class BaseComponent implements OnInit {
  opened: boolean;
  constructor() { 
    this.opened = false;
  }
  sideMenuExpaned: boolean
  ngOnInit() {
  }

  handelExpantion()
  {
    this.opened = !this.opened;
  }

}
