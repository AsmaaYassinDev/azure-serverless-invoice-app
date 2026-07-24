import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
// 1. IMPORT THE NEW API SERVICE HERE
import { InvoiceApiService } from '../../../../core/services/invoice-api'

@Component({
  selector: 'app-invoice-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule], 
  templateUrl: './invoice-form.html',
  styleUrl: './invoice-form.scss'
})
export class InvoiceForm {
  invoiceForm: FormGroup;

  // 2. INJECT THE API SERVICE INTO THE CONSTRUCTOR
  constructor(private fb: FormBuilder, private apiService: InvoiceApiService) {
    this.invoiceForm = this.fb.group({
      customerName: ['', Validators.required],
      description: ['', Validators.required],
      items: this.fb.array([this.createItem()]) 
    });
  }

  get items(): FormArray {
    return this.invoiceForm.get('items') as FormArray;
  }

  createItem(): FormGroup {
    return this.fb.group({
      itemName: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]],
      price: [0.00, [Validators.required, Validators.min(0)]]
    });
  }

  addItem(): void {
    this.items.push(this.createItem());
  }

  removeItem(index: number): void {
    if (this.items.length > 1) {
      this.items.removeAt(index);
    }
  }

  // Resets the entire form and array back to its default state
onClear(): void {
    // 1. Reset all the text values to empty
    this.invoiceForm.reset();
    
    // 2. Brutally delete every single row from the array
    while (this.items.length !== 0) {
      this.items.removeAt(0);
    }
    
    // 3. Inject exactly one fresh, default row back into the UI
    this.items.push(this.createItem());
  }

  // 3. UPDATE SUBMIT TO ACTUALLY CALL THE C# BACKEND
  onSubmit(): void {
    if (this.invoiceForm.valid) {
      
      this.apiService.generatePdf(this.invoiceForm.value).subscribe({
        next: (pdfBlob: Blob) => {
          const url = window.URL.createObjectURL(pdfBlob);
          const link = document.createElement('a');
          link.href = url;
          link.download = `Invoice_${this.invoiceForm.value.customerName}.pdf`;
          link.click();
          window.URL.revokeObjectURL(url);

          // --- ADD THESE 3 LINES RIGHT HERE ---
          // 1. Clear the customer name and description
          this.invoiceForm.reset(); 
          // 2. Delete all dynamically added rows
          this.items.clear(); 
          // 3. Add exactly one fresh, empty row back to the UI
          this.items.push(this.createItem()); 
        },
        error: (err) => {
          console.error('Failed to generate PDF', err);
          alert('Error connecting to the C# backend! Check the browser console.');
        }
      });

    } else {
      this.invoiceForm.markAllAsTouched(); 
    }
  }
}