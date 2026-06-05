import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GestionMultas } from './gestion-multas';

describe('GestionMultas', () => {
  let component: GestionMultas;
  let fixture: ComponentFixture<GestionMultas>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GestionMultas]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GestionMultas);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
