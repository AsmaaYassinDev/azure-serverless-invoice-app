export interface InvoiceItem {
  itemName: string;
  quantity: number;
  price: number;
}

export interface InvoiceModel {
  id?: string;
  customerName: string;
  description: string;
  items: InvoiceItem[];
}