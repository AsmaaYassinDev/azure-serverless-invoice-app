import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { InvoiceModel } from '../models/invoice.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class InvoiceApiService {
  // This is the default local URL for C# Azure Functions
  
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  /**
   * Sends the invoice data to the C# backend to generate a PDF.
   * We expect a Blob (binary data) back because it is a PDF file.
   */
  generatePdf(invoiceData: InvoiceModel): Observable<Blob> {
    return this.http.post(this.apiUrl, invoiceData, {
      responseType: 'blob' // CRITICAL: Tells Angular we are downloading a file, not JSON
    });
  }
}