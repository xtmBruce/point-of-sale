import React, { useState } from 'react';

const PricingOptimization = ({ selectedProducts = [], onOptimizationComplete }) => {
  const [isOptimizing, setIsOptimizing] = useState(false);

  const handleOptimize = async () => {
    if (selectedProducts.length === 0) {
      alert('Please select products to optimize');
      return;
    }

    setIsOptimizing(true);

    try {
      // Simulate optimization process
      await new Promise((resolve) => setTimeout(resolve, 1500));

      if (onOptimizationComplete) {
        onOptimizationComplete({
          summary: {
            successful_optimizations: selectedProducts.length,
            total_products: selectedProducts.length,
            avg_price_change: '5.2%'
          }
        });
      }
    } catch (error) {
      console.error('Optimization error:', error);
    } finally {
      setIsOptimizing(false);
    }
  };

  return (
    <div className="space-y-6">
      <div className="bg-white rounded-lg shadow p-6">
        <h3 className="text-lg font-semibold text-gray-900 mb-4">Pricing Optimization</h3>

        <div className="mb-4 p-4 bg-amber-50 border border-amber-200 rounded-lg">
          <p className="text-sm text-amber-800">
            {selectedProducts.length > 0
              ? `Ready to optimize pricing for ${selectedProducts.length} product(s)`
              : 'Select products from the dashboard to optimize pricing'}
          </p>
        </div>

        <button
          onClick={handleOptimize}
          disabled={isOptimizing || selectedProducts.length === 0}
          className={`w-full px-4 py-2 font-medium rounded-lg transition-all ${
            isOptimizing || selectedProducts.length === 0
              ? 'bg-gray-300 text-gray-500 cursor-not-allowed'
              : 'bg-green-600 text-white hover:bg-green-700'
          }`}
        >
          {isOptimizing ? 'Optimizing...' : 'Optimize Pricing'}
        </button>
      </div>

      <div className="bg-blue-50 rounded-lg p-4 border border-blue-200">
        <p className="text-sm text-blue-800">
          🚀 Pricing optimization uses AI to recommend optimal prices based on market demand, competition, and inventory levels.
        </p>
      </div>
    </div>
  );
};

export default PricingOptimization;
