import { mockProducts } from "./mockProducts";
import { mockCustomers } from "./mockCustomers";

export function getMockData(endpoint) {
  if (endpoint.includes("products")) return mockProducts;
  if (endpoint.includes("customers")) return mockCustomers;

  return [];
}
