import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GestionPropiedades } from './gestion-propiedades';

describe('GestionPropiedades', () => {
  let component: GestionPropiedades;
  let fixture: ComponentFixture<GestionPropiedades>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GestionPropiedades]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GestionPropiedades);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
