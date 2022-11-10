import { Component, ViewEncapsulation, ViewChild } from '@angular/core';
import { ProjectService } from '../../services/project.service';
import { ProjectModel } from '../../models/project.model';
import { DataSource } from '../../../../../shared/models/data-source.model';
import { ProjectFormComponent } from '../../components/project-form/project-form.component';
import { AlertService } from 'src/app/shared/services/alert.service';
import { AbilityService } from 'src/app/shared/services/ability.service';

@Component({
  selector: 'app-project-list',
  templateUrl: './project-list.component.html',
  styleUrls: ['../../../lookups-style.scss']
})

export class ProjectListComponent {
  projectList = [];
  project: ProjectModel = new ProjectModel();
  properties = ["name", "description"];//Displayed Columns 
  @ViewChild(ProjectFormComponent) projectFormComponent;//Add/Update Project model
  dataSource: DataSource = new DataSource;
  total: number;
  constructor(private projectService: ProjectService, private alertService: AlertService, public ability: AbilityService) {

  }

  ngOnInit() {
    this.getAllProjects();
  }

  getAllProjects() {
    this.projectService.getAll(this.dataSource).subscribe(response => {
      this.projectList = response.data;
      this.total = response.count;
    }, err => {
    });
  }

  public getProjectById(id: number) {
    this.projectService.get(id)
      .subscribe((response) => {
        this.project = response.data;
        this.projectFormComponent.open(this.project);
      })
      , error => {
      }
  }

  public save(entity) {
    this.projectService.save(entity)
      .subscribe((response) => {
        this.getAllProjects();
        this.alertService.handleResponseMessage(response);
      })
      , error => {
      }
  }

  public delete(id: number) {
    this.projectService.delete(id)
      .subscribe((response) => {
        this.getAllProjects();
        this.alertService.handleResponseMessage(response);
      })
      , error => {
      }
  }

  public openModal(id?: number) {
    if (id) {
      this.getProjectById(id);
    }
    else {
      this.project = new ProjectModel();
      this.projectFormComponent.open(this.project);
    }
  }

  public onChangePagination(dataSource) {
    this.dataSource = dataSource;
    this.getAllProjects();
  }
}
