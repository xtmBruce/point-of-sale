import React, { useState, useEffect } from 'react';
import { useQuery } from '@tanstack/react-query';
import {
  FileCheck,
  Plus,
  Search,
  Filter,
  CheckCircle,
  Truck,
  Trash2,
  Edit,
  MoreVertical
} from 'lucide-react';
import { api } from '../lib/api';
import toast from 'react-hot-toast';
import { procurementAPI } from '../lib/api';
import Swal from 'sweetalert2';

import PurchaseOrderForm from '../Components/procurement/PurchaseOrderForm';
import PurchaseOrderDetails from '../Components/procurement/PurchaseOrderDetails';
import { useMutation, useQueryClient } from '@tanstack/react-query'; // Import missing hooks

const Procurement = () => {
  const queryClient = useQueryClient(); // Initialize queryClient
  const [activeTab, setActiveTab] = useState('purchase-orders');
  const [searchTerm, setSearchTerm] = useState('');
  const [viewMode, setViewMode] = useState('grid');
  const [sortBy, setSortBy] = useState('created_at');
  const [sortOrder, setSortOrder] = useState('desc');
  const [showFilters, setShowFilters] = useState(false);
  const [selectedItems, setSelectedItems] = useState([]);
  const [showBulkActions, setShowBulkActions] = useState(false);
  const [showPOForm, setShowPOForm] = useState(false);
  const [selectedOrderId, setSelectedOrderId] = useState(null);
  const [editingOrder, setEditingOrder] = useState(null);

  // ... (analytics query) ...

  // Fetch procurement analytics - Removed
  // Fetch suppliers - Removed
  // Fetch requisitions - Removed

  // Fetch purchase orders
  const { data: purchaseOrdersData, isLoading: purchaseOrdersLoading } = useQuery({
    queryKey: ['purchase-orders'],
    queryFn: async () => {
      const response = await procurementAPI.getPurchaseOrders();
      return response.data;
    }
  });

  const purchaseOrders = purchaseOrdersData?.purchase_orders || [];

  const deleteOrderMutation = useMutation({
    mutationFn: (id) => procurementAPI.deletePurchaseOrder(id),
    onSuccess: () => {
      queryClient.invalidateQueries(['purchase-orders']);
      toast.success('Order deleted successfully');
    },
    onError: (error) => {
      toast.error('Failed to delete order');
      console.error(error);
    }
  });

  const handleDelete = async (id) => {
    const result = await Swal.fire({
      title: 'Are you sure?',
      text: "You won't be able to revert this!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Yes, delete it!'
    });

    if (result.isConfirmed) {
      deleteOrderMutation.mutate(id);
    }
  };

  const handleEdit = (order) => {
    setEditingOrder(order);
    setShowPOForm(true);
  };

  const formatCurrency = (amount, currency = 'USD') => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: currency
    }).format(amount || 0);
  };

  const formatRWF = (amount) => {
    return new Intl.NumberFormat('en-RW', {
      style: 'currency',
      currency: 'RWF'
    }).format(amount || 0);
  };

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  };

  const getStatusColor = (status) => {
    switch (status?.toLowerCase()) {
      case 'approved':
      case 'delivered':
      case 'completed':
        return 'bg-green-100 text-green-800';
      case 'pending':
      case 'in_transit':
        return 'bg-yellow-100 text-yellow-800';
      case 'rejected':
      case 'cancelled':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const getPriorityColor = (priority) => {
    switch (priority?.toLowerCase()) {
      case 'urgent':
        return 'bg-red-100 text-red-800';
      case 'high':
        return 'bg-orange-100 text-orange-800';
      case 'normal':
        return 'bg-blue-100 text-blue-800';
      case 'low':
        return 'bg-gray-100 text-gray-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const toggleSelection = (itemId) => {
    setSelectedItems(prev =>
      prev.includes(itemId)
        ? prev.filter(id => id !== itemId)
        : [...prev, itemId]
    );
  };

  const selectAll = () => {
    const currentItems = getCurrentItems();
    setSelectedItems(currentItems.map(item => item.id));
  };

  const clearSelection = () => {
    setSelectedItems([]);
  };

  const getCurrentItems = () => {
    return purchaseOrders;
  };

  const filteredItems = getCurrentItems().filter(item => {
    const searchLower = searchTerm.toLowerCase();
    return item.po_number?.toLowerCase().includes(searchLower) ||
      item.supplier_name?.toLowerCase().includes(searchLower);
  });

  const sortedItems = [...filteredItems].sort((a, b) => {
    const aValue = a[sortBy];
    const bValue = b[sortBy];

    if (sortBy === 'created_at') {
      return sortOrder === 'desc'
        ? new Date(bValue || 0).getTime() - new Date(aValue || 0).getTime()
        : new Date(aValue || 0).getTime() - new Date(bValue || 0).getTime();
    }

    if (typeof aValue === 'string' && typeof bValue === 'string') {
      return sortOrder === 'desc'
        ? bValue.localeCompare(aValue)
        : aValue.localeCompare(bValue);
    }

    return sortOrder === 'desc' ? bValue - aValue : aValue - bValue;
  });

  const tabs = [
    {
      id: 'purchase-orders',
      name: 'Purchase Orders',
      icon: FileCheck,
      description: 'Create and track purchase orders'
    }
  ];

  return (
    <div className="min-h-screen bg-gray-50 relative">
      {/* Modal Overlay for Purchase Order Form */}
      {showPOForm && (
        <div className="fixed inset-0 z-50 overflow-y-auto bg-black bg-opacity-50 flex items-center justify-center p-4">
          <PurchaseOrderForm
            onClose={() => {
              setShowPOForm(false);
              setEditingOrder(null);
            }}
            initialData={editingOrder}
          />
        </div>
      )}

      {/* Modal Overlay for Purchase Order Details */}
      {selectedOrderId && (
        <div className="fixed inset-0 z-50 overflow-y-auto bg-black bg-opacity-50 flex items-center justify-center p-4">
          <PurchaseOrderDetails
            orderId={selectedOrderId}
            onClose={() => setSelectedOrderId(null)}
            onEdit={(order) => {
              setEditingOrder(order);
              setSelectedOrderId(null);
              setShowPOForm(true);
            }}
          />
        </div>
      )}

      {/* Header */}
      <div className="bg-white shadow-sm border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center py-6">
            <div className="flex items-center space-x-3">
              <div className="flex-shrink-0">
                <Truck className="h-8 w-8 text-primary-600" />
              </div>
              <div>
                <h1 className="text-2xl font-bold text-gray-900">Procurement</h1>
                <p className="text-sm text-gray-500">Raw material acquisition & supplier management</p>
              </div>
            </div>
            <div className="flex items-center space-x-3">
              {activeTab === 'purchase-orders' && (
                <button
                  onClick={() => setShowPOForm(true)}
                  className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500"
                >
                  <Plus className="h-4 w-4 mr-2" />
                  New Purchase Order
                </button>
              )}
            </div>
          </div>
        </div>
      </div>

      {/* Tabs */}
      <div className="bg-white border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <nav className="flex space-x-8">
            {tabs.map((tab) => {
              const Icon = tab.icon;
              return (
                <button
                  key={tab.id}
                  onClick={() => setActiveTab(tab.id)}
                  className={`py-4 px-1 border-b-2 font-medium text-sm flex items-center space-x-2 ${activeTab === tab.id
                    ? 'border-primary-500 text-primary-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                    }`}
                >
                  <Icon className="h-4 w-4" />
                  <span>{tab.name}</span>
                </button>
              );
            })}
          </nav>
        </div>
      </div>

      {/* Search and Filters Bar */}
      <div className="bg-white border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <div className="flex flex-col sm:flex-row gap-4">
            <div className="flex-1">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
                <input
                  type="text"
                  placeholder={`Search ${activeTab}...`}
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md leading-5 bg-white placeholder-gray-500 focus:outline-none focus:placeholder-gray-400 focus:ring-1 focus:ring-primary-500 focus:border-primary-500 sm:text-sm"
                />
              </div>
            </div>
            <div className="flex items-center space-x-3">
              <button
                onClick={() => setShowFilters(!showFilters)}
                className={`inline-flex items-center px-3 py-2 border border-gray-300 shadow-sm text-sm leading-4 font-medium rounded-md ${showFilters
                  ? 'bg-primary-50 border-primary-300 text-primary-700'
                  : 'bg-white text-gray-700 hover:bg-gray-50'
                  } focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500`}
              >
                <Filter className="h-4 w-4 mr-2" />
                Filters
              </button>
              <div className="flex items-center space-x-2">
                <button
                  onClick={() => setViewMode('grid')}
                  className={`p-2 rounded-md ${viewMode === 'grid'
                    ? 'bg-primary-100 text-primary-600'
                    : 'text-gray-400 hover:text-gray-600'
                    }`}
                >
                  <div className="grid grid-cols-2 gap-1 w-4 h-4">
                    <div className="bg-current rounded-sm"></div>
                    <div className="bg-current rounded-sm"></div>
                    <div className="bg-current rounded-sm"></div>
                    <div className="bg-current rounded-sm"></div>
                  </div>
                </button>
                <button
                  onClick={() => setViewMode('list')}
                  className={`p-2 rounded-md ${viewMode === 'list'
                    ? 'bg-primary-100 text-primary-600'
                    : 'text-gray-400 hover:text-gray-600'
                    }`}
                >
                  <div className="space-y-1 w-4 h-4">
                    <div className="bg-current rounded-sm h-1"></div>
                    <div className="bg-current rounded-sm h-1"></div>
                    <div className="bg-current rounded-sm h-1"></div>
                  </div>
                </button>
              </div>
              <select
                value={`${sortBy}-${sortOrder}`}
                onChange={(e) => {
                  const [field, order] = e.target.value.split('-');
                  setSortBy(field);
                  setSortOrder(order);
                }}
                className="block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-primary-500 focus:border-primary-500 sm:text-sm rounded-md"
              >
                <option value="created_at-desc">Newest First</option>
                <option value="created_at-asc">Oldest First</option>
                <option value="name-asc">Name A-Z</option>
                <option value="name-desc">Name Z-A</option>
              </select>
            </div>
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {activeTab === 'purchase-orders' && (
          <PurchaseOrdersTab
            purchaseOrders={sortedItems}
            isLoading={purchaseOrdersLoading}
            searchTerm={searchTerm}
            viewMode={viewMode}
            selectedItems={selectedItems}
            sortBy={sortBy}
            sortOrder={sortOrder}
            onToggleSelection={toggleSelection}
            formatCurrency={formatCurrency}
            formatRWF={formatRWF}
            formatDate={formatDate}
            getStatusColor={getStatusColor}
            onOrderClick={setSelectedOrderId}
            onEdit={handleEdit}
            onDelete={handleDelete}
          />
        )}
      </div>
    </div>
  );
};



// Purchase Orders Tab Component
const PurchaseOrdersTab = ({ purchaseOrders, isLoading, searchTerm, viewMode, selectedItems, sortBy, sortOrder, onToggleSelection, formatCurrency, formatRWF, formatDate, getStatusColor, onOrderClick, onEdit, onDelete }) => {
  if (isLoading) {
    return <div className="text-center py-8">Loading purchase orders...</div>;
  }

  return (
    <div className="space-y-6">
      {viewMode === 'grid' ? (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {purchaseOrders.map((order) => {
            const isGermanOrder = order.supplier_name?.toLowerCase().includes('german') ||
              order.notes?.toLowerCase().includes('citrus burst energy cologne');

            return (
              <div
                key={order.id}
                onClick={() => onOrderClick && onOrderClick(order.id)}
                className={`bg-white rounded-lg shadow border-2 transition-all duration-200 cursor-pointer relative group ${isGermanOrder
                  ? 'border-green-500 bg-green-50'
                  : 'border-gray-200 hover:border-gray-300 hover:shadow-md'
                  }`}>
                <div className="p-6">
                  <div className="flex items-start justify-between">
                    <div className="flex-1">
                      <div className="flex items-center">
                        <h3 className={`text-lg font-medium ${isGermanOrder ? 'text-green-800' : 'text-gray-900'}`}>
                          {order.po_number}
                        </h3>
                        {isGermanOrder && (
                          <span className="ml-2 inline-flex items-center px-2 py-1 text-xs font-medium rounded-full bg-green-100 text-green-800">
                            🇩🇪 German Import
                          </span>
                        )}
                      </div>
                      <p className={`text-sm ${isGermanOrder ? 'text-green-600 font-medium' : 'text-gray-500'}`}>
                        {order.supplier_name}
                        {isGermanOrder && <span className="ml-1">🇩🇪</span>}
                      </p>
                    </div>
                    <div className="flex items-center space-x-2">
                      <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${getStatusColor(order.status)}`}>
                        {order.status}
                      </span>
                    </div>
                  </div>
                  <div className="mt-4 space-y-2">
                    <div className="flex justify-between text-sm">
                      <span className="text-gray-500">Order Date:</span>
                      <span className="text-gray-900">{formatDate(order.order_date)}</span>
                    </div>
                    <div className="flex justify-between text-sm">
                      <span className="text-gray-500">Total ({order.currency || 'USD'}):</span>
                      <span className="text-gray-900 font-medium">{formatCurrency(order.total_amount, order.currency)}</span>
                    </div>
                    {order.total_amount_rfw && (
                      <div className="flex justify-between text-sm">
                        <span className="text-gray-500">Total Amount (RWF):</span>
                        <span className="text-blue-600 font-bold">{formatRWF(order.total_amount_rfw)}</span>
                      </div>
                    )}
                    <div className="flex justify-between text-sm">
                      <span className="text-gray-500">Items:</span>
                      <span className="text-gray-900">{order.items_count || 0}</span>
                    </div>
                    <div className="flex justify-between text-sm">
                      <span className="text-gray-500">Quantity:</span>
                      <span className="text-gray-900">{Number(order.total_quantity || 0)}</span>
                    </div>
                    {isGermanOrder && order.notes && (
                      <div className="mt-2 pt-2 border-t border-green-100">
                        <div className="text-xs text-green-600 font-medium">Notes:</div>
                        <div className="text-xs text-green-700">{order.notes}</div>
                      </div>
                    )}

                    {/* Action Buttons for Pending Orders */}
                    {order.status === 'pending' && (
                      <div className="mt-4 pt-4 border-t border-gray-100 flex justify-end space-x-2">
                        <button
                          onClick={(e) => {
                            e.stopPropagation();
                            onEdit(order);
                          }}
                          className="p-2 text-blue-600 hover:bg-blue-50 rounded-full transition-colors"
                          title="Edit Order"
                        >
                          <Edit className="w-4 h-4" />
                        </button>
                        <button
                          onClick={(e) => {
                            e.stopPropagation();
                            onDelete(order.id);
                          }}
                          className="p-2 text-red-600 hover:bg-red-50 rounded-full transition-colors"
                          title="Delete Order"
                        >
                          <Trash2 className="w-4 h-4" />
                        </button>
                      </div>
                    )}
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      ) : (
        <div className="bg-white shadow overflow-hidden sm:rounded-md">
          <ul className="divide-y divide-gray-200">
            {purchaseOrders.map((order) => {
              const isGermanOrder = order.supplier_name?.toLowerCase().includes('german') ||
                order.notes?.toLowerCase().includes('citrus burst energy cologne');

              return (
                <li key={order.id} className={`px-6 py-4 ${isGermanOrder ? 'bg-green-50' : ''}`}>
                  <div className="flex items-center justify-between">
                    <div className="flex items-center">
                      <div className="flex-shrink-0">
                        <div className={`h-10 w-10 rounded-full flex items-center justify-center ${isGermanOrder ? 'bg-green-100' : 'bg-gray-100'
                          }`}>
                          <FileCheck className={`h-5 w-5 ${isGermanOrder ? 'text-green-600' : 'text-gray-400'}`} />
                        </div>
                      </div>
                      <div className="ml-4">
                        <div className="flex items-center">
                          <div className={`text-sm font-medium ${isGermanOrder ? 'text-green-800' : 'text-gray-900'}`}>
                            {order.po_number}
                          </div>
                          {isGermanOrder && (
                            <span className="ml-2 inline-flex items-center px-2 py-1 text-xs font-medium rounded-full bg-green-100 text-green-800">
                              🇩🇪 German
                            </span>
                          )}
                        </div>
                        <div className="text-sm text-gray-500">
                          {order.supplier_name} • {formatDate(order.order_date)}
                        </div>
                        {isGermanOrder && order.notes && (
                          <div className="text-xs text-green-600">
                            {order.notes}
                          </div>
                        )}
                      </div>
                    </div>
                    <div className="flex items-center space-x-4">
                      <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${getStatusColor(order.status)}`}>
                        {order.status}
                      </span>
                      <div className="text-right">
                        <div className="text-sm font-medium text-gray-900">{formatCurrency(order.total_amount)}</div>
                        {order.total_amount_rfw && (
                          <div className="text-xs font-bold text-blue-600">{formatRWF(order.total_amount_rfw)}</div>
                        )}
                      </div>

                      {/* List View Actions */}
                      {order.status === 'pending' && (
                        <div className="flex items-center space-x-1 ml-4">
                          <button
                            onClick={(e) => {
                              e.stopPropagation();
                              onEdit(order);
                            }}
                            className="p-1 text-blue-600 hover:bg-blue-50 rounded transition-colors"
                            title="Edit"
                          >
                            <Edit className="w-4 h-4" />
                          </button>
                          <button
                            onClick={(e) => {
                              e.stopPropagation();
                              onDelete(order.id);
                            }}
                            className="p-1 text-red-600 hover:bg-red-50 rounded transition-colors"
                            title="Delete"
                          >
                            <Trash2 className="w-4 h-4" />
                          </button>
                        </div>
                      )}
                    </div>
                  </div>
                </li>
              );
            })}
          </ul>
        </div>
      )}
    </div>
  );
};



export default Procurement; 