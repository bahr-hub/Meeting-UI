import { Component } from '@angular/core';
import { EventEmitter } from '@angular/core';
import { Output } from '@angular/core';
import { Input } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Compiler } from '@angular/core';

@Component({
  selector: 'file-uploader',
  templateUrl: 'file-uploader.component.html',
  styleUrls: ['file-uploader.component.css'],
  inputs: ['activeColor', 'baseColor', 'overlayColor']
})

export class FileUploaderComponent {

  activeColor: string = 'green';
  baseColor: string = '#ccc';
  overlayColor: string = 'rgba(255,255,255,0.5)';

  dragging: boolean = false;
  loaded: boolean = false;
  imageLoaded: boolean = false;
  iconColor: any;
  borderColor: any;
  @Input() imageSrc: string = "null";
  @Output() imageBase64Emitter = new EventEmitter<string>();
 
  handleDragEnter() {
    this.dragging = true;
  }


  handleDragLeave() {
    this.dragging = false;
  }

  handleDrop(e) {
    e.preventDefault();
    this.dragging = false;
    this.handleInputChange(e);
  }

  handleImageLoad() {
    this.imageLoaded = true;
    this.iconColor = this.overlayColor;
  }

  handleInputChange(e) {
    var file = e.dataTransfer ? e.dataTransfer.files[0] : e.target.files[0];

    var pattern = /image-*/;
    var reader = new FileReader();

    if (!file.type.match(pattern)) {
      alert('invalid format');
      return;
    }

    this.loaded = false;

    reader.onload = this._handleReaderLoaded.bind(this);
    reader.readAsDataURL(file);
  }

  _handleReaderLoaded(e) {
    
    var reader = e.target;
    this.imageSrc = reader.result;
    this.loaded = true;
    this.imageBase64Emitter.emit(this.imageSrc);
  }

  _setActive() {
    this.borderColor = this.activeColor;
    if (this.imageSrc.length === 0) {
      this.iconColor = this.activeColor;
    }
  }

  _setInactive() {
    this.borderColor = this.baseColor;
    if (this.imageSrc.length === 0) {
      this.iconColor = this.baseColor;
    }
  }

  cancel() {
    this.imageSrc = null;
  }

  OnDestroy() {
    
    this.imageSrc = null;
    // this._compiler.clearCache();
  }
}