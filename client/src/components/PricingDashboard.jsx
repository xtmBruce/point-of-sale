import React from 'react';

const PricingDashboard = ({ products = [], onProductAnalysis }) => {
  return (
    <div className="space-y-6">
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {products.map((product) => (
          <div
            key={product.id}
            className="bg-white rounded-lg shadow p-4 hover:shadow-md transition-shadow"
          >
            <h3 className="font-medium text-gray-900">{product.name}</h3>
            <div className="mt-2 space-y-1">
              <p className="text-sm text-gray-600">
                Current Price: <span className="font-medium">{Number(product.price).toLocaleString()} RWF</span>
              </p>
              <p className="text-sm text-gray-600">
                Stock: <span className="font-medium">{product.current_stock || product.stock_quantity || 0}</span>
              </p>
            </div>
            {onProductAnalysis && (
              <button
                onClick={() => onProductAnalysis(product.id)}
                className="mt-4 w-full px-3 py-2 text-sm font-medium text-white bg-blue-600 rounded hover:bg-blue-700 transition-colors"
              >
                Analyze
              </button>
            )}
          </div>
        ))}
      </div>

      {products.length === 0 && (
        <div className="text-center py-12 bg-gray-50 rounded-lg">
          <p className="text-gray-500">No products to display</p>
        </div>
      )}
    </div>
  );
};

export default PricingDashboard;
