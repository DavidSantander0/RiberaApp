import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListaPropiedades } from './lista-propiedades';

describe('ListaPropiedades', () => {
  let component: ListaPropiedades;
  let fixture: ComponentFixture<ListaPropiedades>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListaPropiedades]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ListaPropiedades);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
