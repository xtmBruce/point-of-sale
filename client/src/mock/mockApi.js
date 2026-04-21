import db from "./mockDB";

export const mockApi = {
  get(endpoint) {
    if (endpoint.includes("products")) return { products: db.products, pagination: { total: db.products.length, totalPages: 1 } };
    if (endpoint.includes("categories")) return { flat: db.categories };
    if (endpoint.includes("brands")) return { brands: db.brands };
    if (endpoint.includes("shops")) return { shops: db.shops };

    // Customers
    if (endpoint.match(/\/customers\/([^/]+)\/orders/)) {
      return { orders: [], total: 0 };
    }
    if (endpoint.match(/\/customers\/([^/]+)$/)) {
      const id = endpoint.split("/").pop();
      const customer = db.customers.find(c => String(c.id) === String(id));
      return customer ? { customer } : null;
    }
    if (endpoint.includes("customers/stats")) {
      return {
        total_customers: db.customers.length,
        active_customers: db.customers.length,
        total_spent: db.customers.reduce((s, c) => s + (c.total_spent || 0), 0),
        average_spent: 0
      };
    }
    if (endpoint.includes("customers")) {
      return { customers: db.customers, total: db.customers.length, page: 1, limit: 100 };
    }

    // Loyalty
    if (endpoint.match(/\/loyalty\/customer\/([^/]+)/)) {
      const id = endpoint.split("/").pop();
      const customer = db.customers.find(c => String(c.id) === String(id));
      return {
        customer_id: id,
        loyalty_points: customer?.loyalty_points || 0,
        loyalty_tier: customer?.loyalty_tier || "bronze",
        total_spent: customer?.total_spent || 0,
        next_tier: "silver",
        points_to_next_tier: 500 - (customer?.loyalty_points || 0),
        transactions: []
      };
    }
    if (endpoint.includes("loyalty/stats")) {
      const totalPoints = db.customers.reduce((s, c) => s + (c.loyalty_points || 0), 0);
      return {
        total_customers: db.customers.length,
        total_points_issued: totalPoints,
        tier_distribution: [
          { tier: "bronze", count: db.customers.filter(c => c.loyalty_tier === "bronze").length },
          { tier: "silver", count: db.customers.filter(c => c.loyalty_tier === "silver").length },
          { tier: "gold", count: db.customers.filter(c => c.loyalty_tier === "gold").length },
        ],
        recent_transactions: []
      };
    }
    if (endpoint.includes("loyalty/tiers")) {
      return {
        tiers: [
          { name: "bronze", min_points: 0, benefits: "1% points on purchases" },
          { name: "silver", min_points: 500, benefits: "1.5% points + birthday bonus" },
          { name: "gold", min_points: 2000, benefits: "2% points + priority support" },
          { name: "platinum", min_points: 5000, benefits: "3% points + exclusive offers" },
        ]
      };
    }

    if (endpoint.includes("dashboard/overview")) return {
      totalSales: 15000,
      totalCustomers: db.customers.length,
      totalProducts: db.products.length
    };
    return [];
  },

  post(endpoint, data) {
    // Customers
    if (endpoint.includes("customers")) {
      const newCustomer = {
        id: Date.now().toString(),
        first_name: data.firstName || data.first_name || "",
        last_name: data.lastName || data.last_name || "",
        email: data.email || null,
        phone: data.phone || null,
        address: data.address || null,
        city: data.city || null,
        state: data.state || null,
        country: data.country || null,
        postal_code: data.postalCode || data.postal_code || null,
        loyalty_points: 0,
        loyalty_tier: "bronze",
        total_spent: 0,
        is_active: true,
        created_at: new Date().toISOString(),
        updated_at: new Date().toISOString()
      };
      db.customers.push(newCustomer);
      return { customer: newCustomer };
    }

    // Loyalty - add points
    if (endpoint.includes("loyalty/points/add")) {
      const customer = db.customers.find(c => String(c.id) === String(data.customerId || data.customer_id));
      if (customer) {
        const pts = data.points || Math.floor((data.orderAmount || data.order_amount || 0) * 0.01);
        customer.loyalty_points = (customer.loyalty_points || 0) + pts;
        if (data.orderAmount || data.order_amount) {
          customer.total_spent = (customer.total_spent || 0) + (data.orderAmount || data.order_amount);
        }
      }
      return { points_added: data.points, total_points: customer?.loyalty_points || 0 };
    }

    // Loyalty - redeem points
    if (endpoint.includes("loyalty/points/redeem")) {
      const customer = db.customers.find(c => String(c.id) === String(data.customerId || data.customer_id));
      if (customer && customer.loyalty_points >= data.points) {
        customer.loyalty_points -= data.points;
      }
      return { points_redeemed: data.points, total_points: customer?.loyalty_points || 0 };
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
      const index = db.customers.findIndex(c => String(c.id) === String(idStr));
      if (index !== -1) {
        db.customers[index] = {
          ...db.customers[index],
          first_name: data.firstName || data.first_name || db.customers[index].first_name,
          last_name: data.lastName || data.last_name || db.customers[index].last_name,
          email: data.email ?? db.customers[index].email,
          phone: data.phone ?? db.customers[index].phone,
          address: data.address ?? db.customers[index].address,
          city: data.city ?? db.customers[index].city,
          state: data.state ?? db.customers[index].state,
          country: data.country ?? db.customers[index].country,
          postal_code: data.postalCode || data.postal_code || db.customers[index].postal_code,
          updated_at: new Date().toISOString()
        };
        return { customer: db.customers[index] };
      }
    }
    return data;
  },

  patch(endpoint, data) {
    return data;
  },

  delete(endpoint) {
    if (endpoint.includes("customers")) {
      const idStr = endpoint.split("/").pop();
      const index = db.customers.findIndex(c => String(c.id) === String(idStr));
      if (index !== -1) db.customers.splice(index, 1);
    }
    return { success: true };
  }
};
