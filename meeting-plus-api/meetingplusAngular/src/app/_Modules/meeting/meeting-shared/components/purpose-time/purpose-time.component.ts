import { Component, OnInit, Input } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MeetingOperationsService } from '../../../meeting-operation/services/meeting-opertaions.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-purpose-time',
  templateUrl: './purpose-time.component.html',
  styleUrls: ['./purpose-time.component.scss']
})
export class PurposeTimeComponent implements OnInit {
  @Input("meeting") meeting;
  preposeTimeForm: FormGroup;
  constructor(private fb: FormBuilder,private proposalService: MeetingOperationsService,private toster: ToastrService) {
    
   }

  ngOnInit(): void {
    this.prepareForm();
  }

  private prepareForm()
  {
    this.preposeTimeForm = this.fb.group({
      id:[this.meeting.id,Validators.required],
      PurposedTo:['',Validators.required]
    });
  }

  proposTime()
  {
    if(this.preposeTimeForm.invalid)
    {
      this.toster.error("please enter a valid date","Perposal failed")
    }
    else
    {
      let date = new Date(this.preposeTimeForm.get('PurposedTo').value).toISOString()
      this.proposalService.propose(this.preposeTimeForm.get('id').value,date)
      .subscribe(res =>{
      })
    }
  }


}
