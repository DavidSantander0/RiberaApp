import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PagosPendientes } from './pagos-pendientes';

describe('PagosPendientes', () => {
  let component: PagosPendientes;
  let fixture: ComponentFixture<PagosPendientes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PagosPendientes]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PagosPendientes);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
