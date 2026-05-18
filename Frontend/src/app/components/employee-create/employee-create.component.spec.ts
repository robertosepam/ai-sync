import { TestBed, ComponentFixture } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { EmployeeCreateComponent } from './employee-create.component';
import { EmployeeService } from '../../services/employee.service';

const mockEmployeeService = {
  createEmployee: vi.fn(),
};

describe('EmployeeCreateComponent', () => {
  let component: EmployeeCreateComponent;
  let fixture: ComponentFixture<EmployeeCreateComponent>;

  beforeEach(async () => {
    mockEmployeeService.createEmployee.mockReset();

    await TestBed.configureTestingModule({
      imports: [EmployeeCreateComponent, ReactiveFormsModule],
      providers: [{ provide: EmployeeService, useValue: mockEmployeeService }],
    }).compileComponents();

    fixture = TestBed.createComponent(EmployeeCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges(); // triggers ngOnInit
  });

  // ---------------------------------------------------------------------------
  // Initial state
  // ---------------------------------------------------------------------------
  describe('initial state', () => {
    it('should create the component', () => {
      expect(component).toBeTruthy();
    });

    it('should build the form with name, dateOfBirth and status controls', () => {
      expect(component.form.contains('name')).toBe(true);
      expect(component.form.contains('dateOfBirth')).toBe(true);
      expect(component.form.contains('status')).toBe(true);
    });

    it('should default status to true', () => {
      expect(component.form.get('status')?.value).toBe(true);
    });

    it('should default name and dateOfBirth to empty string', () => {
      expect(component.form.get('name')?.value).toBe('');
      expect(component.form.get('dateOfBirth')?.value).toBe('');
    });

    it('should have an invalid form on initialisation', () => {
      expect(component.form.invalid).toBe(true);
    });

    it('should expose today as a yyyy-mm-dd string', () => {
      const isoPattern = /^\d{4}-\d{2}-\d{2}$/;
      expect(component.today).toMatch(isoPattern);
    });
  });

  // ---------------------------------------------------------------------------
  // name field validation
  // ---------------------------------------------------------------------------
  describe('name field validation', () => {
    let setName: (value: string) => void;

    beforeEach(() => {
      setName = (value: string) => {
        component.form.get('name')?.setValue(value);
        component.form.get('name')?.markAsTouched();
      };
    });

    it('should be invalid when name is empty', () => {
      setName('');
      expect(component.form.get('name')?.hasError('required')).toBe(true);
    });

    it('should be invalid when name is a single character (minLength = 2)', () => {
      setName('A');
      expect(component.form.get('name')?.hasError('minlength')).toBe(true);
    });

    it('should be invalid when name exceeds 100 characters', () => {
      setName('A'.repeat(101));
      expect(component.form.get('name')?.hasError('maxlength')).toBe(true);
    });

    it('should be valid with exactly 2 characters', () => {
      setName('Jo');
      expect(component.form.get('name')?.valid).toBe(true);
    });

    it('should be valid with exactly 100 characters', () => {
      setName('A'.repeat(100));
      expect(component.form.get('name')?.valid).toBe(true);
    });

    it('should be valid with a typical full name', () => {
      setName('Alice Smith');
      expect(component.form.get('name')?.valid).toBe(true);
    });
  });

  // ---------------------------------------------------------------------------
  // dateOfBirth field validation
  // ---------------------------------------------------------------------------
  describe('dateOfBirth field validation', () => {
    let setDob: (value: string) => void;

    beforeEach(() => {
      setDob = (value: string) => {
        component.form.get('dateOfBirth')?.setValue(value);
        component.form.get('dateOfBirth')?.markAsTouched();
      };
    });

    it('should be invalid when dateOfBirth is empty', () => {
      setDob('');
      expect(component.form.get('dateOfBirth')?.hasError('required')).toBe(true);
    });

    it('should be invalid when dateOfBirth is a future date', () => {
      const future = new Date();
      future.setDate(future.getDate() + 1);
      setDob(future.toISOString().split('T')[0]);
      expect(component.form.get('dateOfBirth')?.hasError('futureDate')).toBe(true);
    });

    it('should be valid when dateOfBirth is today', () => {
      setDob(new Date().toISOString().split('T')[0]);
      expect(component.form.get('dateOfBirth')?.valid).toBe(true);
    });

    it('should be valid when dateOfBirth is a past date', () => {
      setDob('1990-06-15');
      expect(component.form.get('dateOfBirth')?.valid).toBe(true);
    });

    it('should be valid on the boundary of yesterday', () => {
      const yesterday = new Date();
      yesterday.setDate(yesterday.getDate() - 1);
      setDob(yesterday.toISOString().split('T')[0]);
      expect(component.form.get('dateOfBirth')?.valid).toBe(true);
    });
  });

  // ---------------------------------------------------------------------------
  // status field validation
  // ---------------------------------------------------------------------------
  describe('status field validation', () => {
    it('should be valid when status is true', () => {
      component.form.get('status')?.setValue(true);
      expect(component.form.get('status')?.valid).toBe(true);
    });

    it('should be valid when status is false', () => {
      component.form.get('status')?.setValue(false);
      expect(component.form.get('status')?.valid).toBe(true);
    });
  });

  // ---------------------------------------------------------------------------
  // isInvalid()
  // ---------------------------------------------------------------------------
  describe('isInvalid()', () => {
    it('should return false for an invalid but untouched field', () => {
      expect(component.isInvalid('name')).toBe(false);
    });

    it('should return true for an invalid and touched field', () => {
      component.form.get('name')?.markAsTouched();
      expect(component.isInvalid('name')).toBe(true);
    });

    it('should return false for a valid and touched field', () => {
      component.form.get('name')?.setValue('Alice');
      component.form.get('name')?.markAsTouched();
      expect(component.isInvalid('name')).toBe(false);
    });

    it('should return false for an unknown field name', () => {
      expect(component.isInvalid('nonExistentField')).toBe(false);
    });
  });

  // ---------------------------------------------------------------------------
  // onSubmit() — invalid form
  // ---------------------------------------------------------------------------
  describe('onSubmit() with invalid form', () => {
    it('should not call createEmployee when the form is invalid', () => {
      component.onSubmit();
      expect(mockEmployeeService.createEmployee).not.toHaveBeenCalled();
    });

    it('should mark all controls as touched when the form is invalid', () => {
      const spy = vi.spyOn(component.form, 'markAllAsTouched');
      component.onSubmit();
      expect(spy).toHaveBeenCalledTimes(1);
    });
  });

  // ---------------------------------------------------------------------------
  // onSubmit() — valid form, success scenario
  // ---------------------------------------------------------------------------
  describe('onSubmit() success scenario', () => {
    const validPayload = {
      name: 'Alice Smith',
      dateOfBirth: '1990-06-15',
      status: true,
    };

    beforeEach(() => {
      mockEmployeeService.createEmployee.mockReturnValue(of({ id: 1, ...validPayload }));
      component.form.setValue(validPayload);
    });

    it('should call createEmployee with the form values', () => {
      component.onSubmit();
      expect(mockEmployeeService.createEmployee).toHaveBeenCalledWith(validPayload);
    });

    it('should set successMessage on success', () => {
      component.onSubmit();
      expect(component.successMessage).toBe('Employee created successfully.');
    });

    it('should clear error on success', () => {
      component.error = 'previous error';
      component.onSubmit();
      expect(component.error).toBeNull();
    });

    it('should set loading to false after success', () => {
      component.onSubmit();
      expect(component.loading).toBe(false);
    });

    it('should reset the form with status defaulting to true after success', () => {
      component.onSubmit();
      expect(component.form.get('name')?.value).toBe('');
      expect(component.form.get('dateOfBirth')?.value).toBe('');
      expect(component.form.get('status')?.value).toBe(true);
    });

    it('should set loading to true while the request is in flight', () => {
      let loadingDuringRequest = false;
      mockEmployeeService.createEmployee.mockImplementation(() => {
        loadingDuringRequest = component.loading;
        return of({ id: 1, ...validPayload });
      });

      component.onSubmit();

      expect(loadingDuringRequest).toBe(true);
    });
  });

  // ---------------------------------------------------------------------------
  // onSubmit() — valid form, failure scenario
  // ---------------------------------------------------------------------------
  describe('onSubmit() failure scenario', () => {
    const validPayload = {
      name: 'Bob Jones',
      dateOfBirth: '1985-11-22',
      status: false,
    };

    beforeEach(() => {
      mockEmployeeService.createEmployee.mockReturnValue(
        throwError(() => new Error('Server error'))
      );
      component.form.setValue(validPayload);
    });

    it('should set error message on failure', () => {
      component.onSubmit();
      expect(component.error).toBe('Failed to create employee. Please try again later.');
    });

    it('should clear successMessage on failure', () => {
      component.successMessage = 'previous success';
      component.onSubmit();
      expect(component.successMessage).toBeNull();
    });

    it('should set loading to false after failure', () => {
      component.onSubmit();
      expect(component.loading).toBe(false);
    });

    it('should not reset the form on failure', () => {
      component.onSubmit();
      expect(component.form.get('name')?.value).toBe(validPayload.name);
    });
  });
});
