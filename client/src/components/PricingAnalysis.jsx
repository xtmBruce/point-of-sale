import React from 'react';

const PricingAnalysis = ({ productId, products = [] }) => {
  const product = products.find((p) => p.id === productId);

  if (!product) {
    return (
      <div className="text-center py-12 bg-gray-50 rounded-lg">
        <p className="text-gray-500">Select a product to view pricing analysis</p>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="bg-white rounded-lg shadow p-6">
        <h3 className="text-lg font-semibold text-gray-900 mb-4">{product.name}</h3>
        
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="p-4 bg-gray-50 rounded-lg">
            <p className="text-sm text-gray-600">Current Price</p>
            <p className="text-2xl font-bold text-gray-900 mt-1">
              {Number(product.price).toLocaleString()} RWF
            </p>
          </div>

          <div className="p-4 bg-gray-50 rounded-lg">
            <p className="text-sm text-gray-600">Stock Level</p>
            <p className="text-2xl font-bold text-gray-900 mt-1">
              {product.current_stock || product.stock_quantity || 0}
            </p>
          </div>

          <div className="p-4 bg-gray-50 rounded-lg">
            <p className="text-sm text-gray-600">Category</p>
            <p className="text-2xl font-bold text-gray-900 mt-1">
              {product.category_name || 'N/A'}
            </p>
          </div>
        </div>
      </div>

      <div className="bg-blue-50 rounded-lg p-4 border border-blue-200">
        <p className="text-sm text-blue-800">
          💡 Pricing analysis features will be available soon. Optimize your pricing strategy with AI-powered recommendations.
        </p>
      </div>
    </div>
  );
};

export default PricingAnalysis;
