import { NgModule } from '@angular/core';
import { SharedModule } from '../../../shared/shared.module';
import { TagListComponent } from './components/tag-list/tag-list.component';
import { TagRoutingModule } from './tag-routing.module';
import { TagService } from './services/tag.service';
import { TagFormComponent } from './components/tag-form/tag-form.component';
import { ItzButtonsModule } from 'src/app/shared/itz-buttons/itz-buttons.module';

@NgModule({
  imports: [
    TagRoutingModule,
    ItzButtonsModule,
    SharedModule.forRoot()
  ],
  declarations: [
    TagListComponent,
    TagFormComponent
  ],
  providers: [
    TagService
  ],
  entryComponents: [
    TagFormComponent
  ]
})

export class TagModule {
}
