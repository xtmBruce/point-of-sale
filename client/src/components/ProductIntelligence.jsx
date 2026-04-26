import React from 'react';
import { Brain } from 'lucide-react';

const ProductIntelligence = ({ products = [] }) => {
  return (
    <div className="space-y-6">
      <div className="bg-gradient-to-br from-blue-50 to-indigo-50 rounded-lg shadow p-6 border border-blue-200">
        <div className="flex items-center gap-3 mb-4">
          <Brain className="w-6 h-6 text-indigo-600" />
          <h3 className="text-lg font-semibold text-gray-900">Product Intelligence</h3>
        </div>

        <p className="text-gray-700 mb-4">
          AI-powered insights for your products to optimize sales and inventory management.
        </p>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div className="bg-white rounded p-4">
            <h4 className="font-medium text-gray-900 mb-2">📊 Sales Forecasting</h4>
            <p className="text-sm text-gray-600">
              Predict future sales trends based on historical data and market patterns.
            </p>
          </div>

          <div className="bg-white rounded p-4">
            <h4 className="font-medium text-gray-900 mb-2">📈 Demand Analysis</h4>
            <p className="text-sm text-gray-600">
              Understand customer demand patterns and optimize stock levels.
            </p>
          </div>

          <div className="bg-white rounded p-4">
            <h4 className="font-medium text-gray-900 mb-2">💰 Price Optimization</h4>
            <p className="text-sm text-gray-600">
              Get AI-powered pricing recommendations to maximize profitability.
            </p>
          </div>

          <div className="bg-white rounded p-4">
            <h4 className="font-medium text-gray-900 mb-2">🎯 Recommendation Engine</h4>
            <p className="text-sm text-gray-600">
              Suggest products to customers based on purchase history.
            </p>
          </div>
        </div>
      </div>

      {products.length > 0 && (
        <div className="bg-white rounded-lg shadow p-6">
          <h4 className="font-semibold text-gray-900 mb-4">Top Insights</h4>
          <div className="space-y-3">
            {products.slice(0, 3).map((product) => (
              <div key={product.id} className="flex items-start gap-3 p-3 bg-gray-50 rounded">
                <div className="flex-shrink-0 w-8 h-8 bg-blue-100 rounded flex items-center justify-center">
                  <span className="text-sm font-bold text-blue-600">💡</span>
                </div>
                <div className="flex-1">
                  <p className="font-medium text-gray-900">{product.name}</p>
                  <p className="text-sm text-gray-600 mt-1">
                    Stock level optimal for current demand
                  </p>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
};

export default ProductIntelligence;
