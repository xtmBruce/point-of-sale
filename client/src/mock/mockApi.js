import db from "./mockDB";

export const mockApi = {
  get(endpoint) {
    if (endpoint.includes("products")) return { products: db.products, pagination: { total: db.products.length, totalPages: 1 } };
    if (endpoint.includes("categories")) return { flat: db.categories };
    if (endpoint.includes("customers")) return { customers: db.customers, pagination: { total: db.customers.length, totalPages: 1 } };
    if (endpoint.includes("brands")) return { brands: db.brands };
    if (endpoint.includes("shops")) return { shops: db.shops };
    if (endpoint.includes("loyalty/stats")) return { 
      totalPoints: 500, 
      activeCustomers: db.customers.length 
    };
    if (endpoint.includes("dashboard/overview")) return {
      totalSales: 15000,
      totalCustomers: db.customers.length,
      totalProducts: db.products.length
    };
    return [];
  },

  post(endpoint, data) {
    if (endpoint.includes("customers")) {
      const newItem = { 
        id: Date.now().toString(), 
        name: data.name || data.fullName || data.first_name + " " + data.last_name || "Unnamed Customer",
        phone: data.phone || "",
        email: data.email || "",
        loyalty_points: 0
      };
      db.customers.push(newItem);
      return newItem;
    }

    if (endpoint.includes("products")) {
      const newItem = { id: Date.now().toString(), ...data };
      db.products.push(newItem);
      return newItem;
    }

    if (endpoint.includes("categories")) {
      const newItem = { id: Date.now().toString(), ...data };
      db.categories.push(newItem);
      return newItem;
    }

    if (endpoint.includes("brands")) {
      const newItem = { id: Date.now().toString(), ...data };
      db.brands.push(newItem);
      return newItem;
    }

    return data;
  },
  
  put(endpoint, data) {
    if (endpoint.includes("customers")) {
      const idStr = endpoint.split("/").pop();
      const index = db.customers.findIndex(c => c.id === idStr || c.id === parseInt(idStr));

      if (index !== -1) {
        db.customers[index] = { ...db.customers[index], ...data, name: data.name || data.fullName || data.first_name + " " + data.last_name || db.customers[index].name };
        return db.customers[index];
      }
    }
    return data;
  },

  patch(endpoint, data) {
    return data;
  },

  delete(endpoint) {
    return { success: true };
  }
};
