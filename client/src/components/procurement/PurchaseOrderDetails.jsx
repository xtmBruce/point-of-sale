import React from 'react';
import { X } from 'lucide-react';

const PurchaseOrderDetails = ({ order, onClose }) => {
  if (!order) {
    return null;
  }

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center">
      <div className="bg-white rounded-lg shadow-lg max-w-2xl w-full mx-4">
        <div className="flex items-center justify-between p-6 border-b">
          <h2 className="text-xl font-bold text-gray-900">Purchase Order Details</h2>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600"
          >
            <X className="w-6 h-6" />
          </button>
        </div>

        <div className="p-6 space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <p className="text-sm text-gray-500">PO Number</p>
              <p className="text-lg font-medium text-gray-900">{order.po_number || order.id}</p>
            </div>
            <div>
              <p className="text-sm text-gray-500">Status</p>
              <p className="text-lg font-medium">
                <span className={`px-2 py-1 rounded text-white text-sm font-medium ${
                  order.status === 'completed' ? 'bg-green-600' :
                  order.status === 'pending' ? 'bg-yellow-600' :
                  'bg-gray-600'
                }`}>
                  {order.status || 'Pending'}
                </span>
              </p>
            </div>
            <div>
              <p className="text-sm text-gray-500">Order Date</p>
              <p className="text-lg font-medium text-gray-900">
                {order.order_date ? new Date(order.order_date).toLocaleDateString() : 'N/A'}
              </p>
            </div>
            <div>
              <p className="text-sm text-gray-500">Expected Delivery</p>
              <p className="text-lg font-medium text-gray-900">
                {order.expected_delivery_date ? new Date(order.expected_delivery_date).toLocaleDateString() : 'N/A'}
              </p>
            </div>
          </div>
        </div>

        <div className="flex justify-end gap-3 p-6 border-t">
          <button
            onClick={onClose}
            className="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50"
          >
            Close
          </button>
        </div>
      </div>
    </div>
  );
};

export default PurchaseOrderDetails;
