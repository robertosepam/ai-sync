import { TestBed, ComponentFixture } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { EmployeeGridComponent } from './employee-grid.component';
import { EmployeeService } from '../../services/employee.service';
import { Employee } from '../../models/employee.model';

const mockEmployees: Employee[] = [
  { id: 1, name: 'Alice Smith', dateOfBirth: '1990-05-10', status: true },
  { id: 2, name: 'Bob Jones', dateOfBirth: '1985-11-22', status: false },
];

const mockEmployeeService = {
  getEmployees: vi.fn(),
};

describe('EmployeeGridComponent', () => {
  let component: EmployeeGridComponent;
  let fixture: ComponentFixture<EmployeeGridComponent>;

  beforeEach(async () => {
    mockEmployeeService.getEmployees.mockReset();

    await TestBed.configureTestingModule({
      imports: [EmployeeGridComponent],
      providers: [{ provide: EmployeeService, useValue: mockEmployeeService }],
    }).compileComponents();

    fixture = TestBed.createComponent(EmployeeGridComponent);
    component = fixture.componentInstance;
  });

  describe('initial state', () => {
    it('should create the component', () => {
      expect(component).toBeTruthy();
    });

    it('should have empty employees, loading false and no error by default', () => {
      expect(component.employees).toEqual([]);
      expect(component.loading).toBe(false);
      expect(component.error).toBeNull();
    });
  });

  describe('ngOnInit', () => {
    it('should call loadEmployees on init', () => {
      mockEmployeeService.getEmployees.mockReturnValue(of(mockEmployees));
      const spy = vi.spyOn(component, 'loadEmployees');

      component.ngOnInit();

      expect(spy).toHaveBeenCalledTimes(1);
    });
  });

  describe('loadEmployees', () => {
    describe('success scenario', () => {
      beforeEach(() => {
        mockEmployeeService.getEmployees.mockReturnValue(of(mockEmployees));
      });

      it('should populate employees with data returned by the service', () => {
        component.loadEmployees();

        expect(component.employees).toEqual(mockEmployees);
      });

      it('should set loading to false after a successful load', () => {
        component.loadEmployees();

        expect(component.loading).toBe(false);
      });

      it('should keep error as null on success', () => {
        component.loadEmployees();

        expect(component.error).toBeNull();
      });

      it('should set loading to true while the request is in flight', () => {
        let loadingDuringRequest = false;
        mockEmployeeService.getEmployees.mockImplementation(() => {
          loadingDuringRequest = component.loading;
          return of(mockEmployees);
        });

        component.loadEmployees();

        expect(loadingDuringRequest).toBe(true);
      });
    });

    describe('failure scenario', () => {
      beforeEach(() => {
        mockEmployeeService.getEmployees.mockReturnValue(
          throwError(() => new Error('Network error'))
        );
      });

      it('should set error message when the service call fails', () => {
        component.loadEmployees();

        expect(component.error).toBe('Failed to load employees. Please try again later.');
      });

      it('should set loading to false after a failed load', () => {
        component.loadEmployees();

        expect(component.loading).toBe(false);
      });

      it('should keep employees array empty on failure', () => {
        component.loadEmployees();

        expect(component.employees).toEqual([]);
      });
    });
  });
});
