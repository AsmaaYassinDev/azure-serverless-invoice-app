import { Routes } from '@angular/router';
import { InvoiceForm } from './features/create-invoice/components/invoice-form/invoice-form';

export const routes: Routes = [
  { path: '', component: InvoiceForm }, // Load the form on the home page
  { path: '**', redirectTo: '' }        // Catch-all for bad URLs
];