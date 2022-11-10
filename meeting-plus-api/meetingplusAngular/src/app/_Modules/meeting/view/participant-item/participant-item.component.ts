import { Component, OnInit, Input } from "@angular/core";
import { environment } from 'src/environments/environment';
@Component({
  selector: "app-participant-item",
  templateUrl: "./participant-item.component.html",
  styleUrls: ["./participant-item.component.scss"]
})
export class ParticipantItemComponent implements OnInit {
  @Input("participant") _participant;
  @Input("creator") isCreator :boolean;
  image:string;
  constructor() {
  
  }

  ngOnInit() {
    debugger;
    console.log(this._participant);
    this.image = environment.imageUrl  + this._participant.participant.imageUrl +".png"
  }
  getimage(image)
  {
   
    return environment.imageUrl  + image;
  }

}
