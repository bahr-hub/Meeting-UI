import { Component, ViewEncapsulation, ViewChild } from '@angular/core';
import { TagService } from '../../services/tag.service';
import { TagModel } from '../../models/tag.model';
import { DataSource } from '../../../../../shared/models/data-source.model';
import { TagFormComponent } from '../../components/tag-form/tag-form.component';
import { AlertService } from 'src/app/shared/services/alert.service';
import { AbilityService } from 'src/app/shared/services/ability.service';

@Component({
  selector: 'app-tag-list',
  templateUrl: './tag-list.component.html',
  styleUrls: ['../../../lookups-style.scss']
})

export class TagListComponent {
  tagList = [];
  tag: TagModel = new TagModel();
  properties = ["name", "description"];//Displayed Columns 
  @ViewChild(TagFormComponent) tagFormComponent;//Add/Update Tag model
  dataSource: DataSource = new DataSource;
  total: number;

  constructor(private tagService: TagService, private alertService: AlertService, public ability: AbilityService) {

  }

  ngOnInit() {
    this.getAllTags();
  }

  getAllTags() {
    this.tagService.getAll(this.dataSource).subscribe(response => {
      this.tagList = response.data;
      this.total = response.count;
    }, err => {
    });
  }

  public getTagById(id: number) {
    this.tagService.get(id)
      .subscribe((response) => {
        this.tag = response.data;
        this.tagFormComponent.open(this.tag);
      })
      , error => {
      }
  }

  public save(entity) {
    this.tagService.save(entity)
      .subscribe((response) => {
        this.getAllTags();
        this.alertService.handleResponseMessage(response);
      })
      , error => {
      }
  }

  public delete(id: number) {
    this.tagService.delete(id)
      .subscribe((response) => {
        this.getAllTags();
        this.alertService.handleResponseMessage(response);
      })
      , error => {
      }
  }

  public openModal(id?: number) {
    if (id) {
      this.getTagById(id);
    }
    else {
      this.tag = new TagModel();
      this.tagFormComponent.open(this.tag);
    }
  }

  public onChangePagination(dataSource) {
    this.dataSource = dataSource;
    this.getAllTags();
  }
}
