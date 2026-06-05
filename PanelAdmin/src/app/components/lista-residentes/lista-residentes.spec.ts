import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListaResidentesComponent } from './lista-residentes';

describe('ListaResidentes', () => {
  let component: ListaResidentesComponent;
  let fixture: ComponentFixture<ListaResidentesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListaResidentesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ListaResidentesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
