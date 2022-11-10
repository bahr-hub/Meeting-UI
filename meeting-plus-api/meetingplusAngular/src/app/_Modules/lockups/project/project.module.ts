import { NgModule } from '@angular/core';
import { SharedModule } from '../../../shared/shared.module';
import { ProjectListComponent } from './components/project-list/project-list.component';
import { ProjectRoutingModule } from './project-routing.module';
import { ProjectService } from './services/project.service';
import { ProjectFormComponent } from 'src/app/_Modules/lockups/project/components/project-form/project-form.component';
import { ItzButtonsModule } from 'src/app/shared/itz-buttons/itz-buttons.module';

@NgModule({
  imports: [
    ProjectRoutingModule,
    ItzButtonsModule,
    SharedModule.forRoot()
  ],
  declarations: [
    ProjectListComponent,
    ProjectFormComponent
  ],
  providers: [
    ProjectService
  ],
  entryComponents: [
    ProjectFormComponent
  ]
})

export class ProjectModule {
}
