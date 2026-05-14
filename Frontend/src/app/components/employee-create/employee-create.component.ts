import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { EmployeeService } from '../../services/employee.service';

declare module '@angular/forms';

function notInFutureValidator(control: AbstractControl): ValidationErrors | null {
  if (!control.value) return null;
  const selected = new Date(control.value);
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  return selected > today ? { futureDate: true } : null;
}

@Component({
  selector: 'app-employee-create',
  templateUrl: './employee-create.component.html',
  styleUrls: ['./employee-create.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule]
})
export class EmployeeCreateComponent implements OnInit {
  form!: FormGroup;
  loading = false;
  successMessage: string | null = null;
  error: string | null = null;
  today: string = new Date().toISOString().split('T')[0];

  constructor(
    private fb: FormBuilder,
    private employeeService: EmployeeService
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
      dateOfBirth: ['', [Validators.required, notInFutureValidator]],
      status: [true, Validators.required]
    });
  }

  isInvalid(field: string): boolean {
    const control = this.form.get(field);
    return !!control && control.invalid && control.touched;
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.error = null;
    this.successMessage = null;

    this.employeeService.createEmployee(this.form.getRawValue()).subscribe({
      next: () => {
        this.successMessage = 'Employee created successfully.';
        this.form.reset({ status: true });
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to create employee. Please try again later.';
        this.loading = false;
      }
    });
  }
}
