import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GestionResidentes } from './gestion-residentes';

describe('GestionResidentes', () => {
  let component: GestionResidentes;
  let fixture: ComponentFixture<GestionResidentes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GestionResidentes]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GestionResidentes);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
