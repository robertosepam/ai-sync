import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EmployeeGridComponent } from './components/employee-grid/employee-grid.component';
import { EmployeeCreateComponent } from './components/employee-create/employee-create.component';

const routes: Routes = [
  { path: '', redirectTo: 'employees', pathMatch: 'full' },
  { path: 'employees', component: EmployeeGridComponent },
  { path: 'employees/create', component: EmployeeCreateComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
