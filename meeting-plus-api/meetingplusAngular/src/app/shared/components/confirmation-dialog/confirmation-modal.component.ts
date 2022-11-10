import { Component, ViewChild, EventEmitter, Output, Input } from '@angular/core';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';
import { NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-confirmation-modal',
  templateUrl: './confirmation-modal.component.html'
})
export class ConfirmationModalComponent {
  closeResult: string;
  @ViewChild('content') content: any;
  message: string = "Do you want to delete this record ?"
  @Output() confirmDeleteEmitter = new EventEmitter<number>();
  @Output() cancel = new EventEmitter<boolean>();

  id: number;
  constructor(private modalService: NgbModal) {

  }

  open(id?: number, message?: string) {
    if (message) {
      this.message = message;//Getting from Create/Update Meeting when change Date From/To  
    }
    this.id = id;
    
    let ngbModalOptions: NgbModalOptions = {//To prevent Bootstrap modal from closing when clicking outside
      backdrop: 'static',
      keyboard: false
    };
    const modalRef = this.modalService.open(this.content, ngbModalOptions);
  }

  public confirmDelete() {
    this.confirmDeleteEmitter.emit(this.id);
    this.modalService.dismissAll();
  }

  public close() {
    this.cancel.emit(true);
    this.modalService.dismissAll();
  }
}
