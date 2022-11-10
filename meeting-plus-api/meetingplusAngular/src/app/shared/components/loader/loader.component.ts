import { Component, OnInit } from '@angular/core';
import { LoaderService } from '../../services/loader.service';

@Component({
  selector: 'app-loader',
  templateUrl: './loader.component.html',
  styleUrls: ['./loader.component.scss']
})
export class LoaderComponent implements OnInit {
  block: boolean;
  constructor(private loader: LoaderService) {
    loader.load.subscribe(res=>{
      let dom = document.getElementById("main-content");
      dom.style.transform = "scale(0)";
      
      this.block = true;
    })


    loader.finshed.subscribe(res=>{
      
      let dom = document.getElementById("main-content");
      dom.style.transform = "scale(1)";
      this.block = false;
    })
   }

  ngOnInit(): void {
  }



}
