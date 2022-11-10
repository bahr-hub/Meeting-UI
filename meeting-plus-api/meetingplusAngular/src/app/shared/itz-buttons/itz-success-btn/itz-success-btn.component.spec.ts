import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ItzSuccessBtnComponent } from './itz-success-btn.component';

describe('ItzSuccessBtnComponent', () => {
  let component: ItzSuccessBtnComponent;
  let fixture: ComponentFixture<ItzSuccessBtnComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ItzSuccessBtnComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ItzSuccessBtnComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
