let db = {
  products: [
    { id: "1", name: "Rice", price: 1500, stock: 20, categoryId: "1", stock_quantity: 20, categories: [{name: "General"}], min_stock_level: 5 },
    { id: "2", name: "Milk", price: 800, stock: 35, categoryId: "1", stock_quantity: 35, categories: [{name: "General"}], min_stock_level: 10 },
    { id: "3", name: "Bread", price: 500, stock: 15, categoryId: "1", stock_quantity: 15, categories: [{name: "Food"}], min_stock_level: 5 },
    { id: "4", name: "Eggs", price: 3000, stock: 10, categoryId: "1", stock_quantity: 10, categories: [{name: "Food"}], min_stock_level: 5 },
    { id: "5", name: "Water", price: 500, stock: 50, categoryId: "2", stock_quantity: 50, categories: [{name: "Drink"}], min_stock_level: 10 }
  ],
  categories: [
    { id: "1", name: "General", level: 0 },
    { id: "2", name: "Drink", level: 0 },
    { id: "3", name: "Food", level: 0 }
  ],
  customers: [
    { id: "1", name: "John Doe", first_name: "John", last_name: "Doe", phone: "0780000000", email: "john@example.com", loyalty_points: 100 },
    { id: "2", name: "Jane Smith", first_name: "Jane", last_name: "Smith", phone: "0780000001", email: "jane@example.com", loyalty_points: 50 },
    { id: "3", name: "Bob Johnson", first_name: "Bob", last_name: "Johnson", phone: "0780000002", email: "bob@example.com", loyalty_points: 0 }
  ],
  brands: [
    { id: "1", name: "Inyange" },
    { id: "2", name: "Skol" }
  ],
  shops: [
    { id: "1", name: "Main Shop" }
  ]
};

export default db;
