import { Component, OnInit } from '@angular/core';
import { Params, Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_Modules/auth/services/auth.service';

@Component({
  selector: 'app-redirect',
  templateUrl: './redirect.component.html',
  styleUrls: ['./redirect.component.scss']
})
export class RedirectComponent implements OnInit {
  private url:string;
  dataObj:Params;

  constructor(private route: ActivatedRoute,private http: HttpClient,private router: Router,
    private toastr: ToastrService, private authService: AuthService) { 
      this.route.queryParams.subscribe(params=>{
        this.dataObj = params;
        this.prepareURL();
      });
    }

  ngOnInit(): void {
  }
  private prepareURL()
  {
    debugger;
    let currentUser = this.authService.currentUserValue;
      this.url = `${environment.apiUrl}api/` 
      this.url += `${this.dataObj['module']}/${this.dataObj['action']}?`;
      //then all the remaining are data to be sent
      
      Object.keys(this.dataObj).forEach(key => {
       
        var _key =  key.replace('amp;', '');
        if (_key != "module" && _key != "action" && _key !="meetingId" && _key != "Participant")
        {
          this.url += `${_key}=${this.dataObj[key]}&`
        }
      });
      this.url = this.url.slice(0, -1);
      
      if(this.dataObj['module'] == "")
      {
        this.viewMeeting(this.dataObj.ID);
      } 
      else if(this.dataObj['Participant'] != null && currentUser != null){
      
       if (currentUser.id == this.dataObj['Participant'])
       this.execute(encodeURI(this.url),this.dataObj.MeetingId);
       else {
         this.toastr.error("ErrorMsg.InvalidUser");
         this.router.navigate(['/Meetings']);
       } 
       
      }
      else
      {
        this.execute(encodeURI(this.url),this.dataObj.MeetingId);
      }
      
  }
  private execute(url :string , meetingId: any) {
debugger;
    // let _headers:HttpHeaders = new HttpHeaders();
    // _headers = _headers.set("Authorization",`Bearer ${this.dataObj.access_token}`);

    this.http.post(url,{}).subscribe((response:any)=>
    {
     debugger;
      if (response["success"] == true) {
        this.toastr.success(response["message"]);
      let _meetingId = meetingId === undefined ? this.dataObj.ID : meetingId
 
      this.router.navigateByUrl(`Meetings/view/${_meetingId}`);
      }
      else{
        this.toastr.error(response["message"]);
       this.router.navigate(['/Meetings']);
       
      }
    })
  }


  private viewMeeting(meetingId:string)
  {
    this.router.navigateByUrl(`Meetings/view/${meetingId}`);
  }
  
}
