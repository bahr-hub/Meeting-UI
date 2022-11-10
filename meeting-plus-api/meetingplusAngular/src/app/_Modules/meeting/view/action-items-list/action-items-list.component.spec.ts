import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ActionItemsListComponent } from './action-items-list.component';

describe('ActionItemsListComponent', () => {
  let component: ActionItemsListComponent;
  let fixture: ComponentFixture<ActionItemsListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ActionItemsListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ActionItemsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
