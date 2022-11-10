import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ItzCheckboxComponent } from './itz-checkbox.component';

describe('ItzCheckboxComponent', () => {
  let component: ItzCheckboxComponent;
  let fixture: ComponentFixture<ItzCheckboxComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ItzCheckboxComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ItzCheckboxComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
