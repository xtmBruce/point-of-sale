import React, { useState, useEffect } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { X, Save, Plus, Trash2, Search, RefreshCw } from 'lucide-react';
import { api } from '../lib/api';
import toast from 'react-hot-toast';

const OrderEditModal = ({ orderId, onClose, onSave }) => {
  const queryClient = useQueryClient();
  const [items, setItems] = useState([]);
  const [paymentMethod, setPaymentMethod] = useState('');
  const [productSearch, setProductSearch] = useState('');
  const [showSearch, setShowSearch] = useState(false);
  const [replacingIndex, setReplacingIndex] = useState(null);

  const { data: order, isLoading } = useQuery({
    queryKey: ['order', orderId],
    queryFn: () => api.get(`/orders/${orderId}`).then(r => r.data)
  });

  const { data: orderItems } = useQuery({
    queryKey: ['order-items', orderId],
    queryFn: () => api.get(`/orders/${orderId}/items`).then(r => r.data),
    enabled: !!orderId
  });

  const { data: productsData } = useQuery({
    queryKey: ['products-search', productSearch],
    queryFn: () => api.get(`/products?search=${productSearch}`).then(r => r.data),
    enabled: showSearch && productSearch.length > 0
  });

  useEffect(() => {
    if (order) setPaymentMethod(order.payment_method || '');
  }, [order]);

  useEffect(() => {
    if (orderItems) {
      setItems(orderItems.map(i => ({
        id: i.id,
        product_id: i.product_id,
        product_name: i.name,
        sku: i.sku,
        quantity: i.quantity,
        unit_price: parseFloat(i.unit_price),
        total_price: parseFloat(i.total_price)
      })));
    }
  }, [orderItems]);

  const updateMutation = useMutation({
    mutationFn: (data) => api.put(`/orders/${orderId}`, data),
    onSuccess: () => {
      toast.success('Order updated successfully!');
      queryClient.invalidateQueries(['order', orderId]);
      queryClient.invalidateQueries(['orders']);
      queryClient.invalidateQueries(['order-items', orderId]);
      onSave?.();
      onClose();
    },
    onError: (err) => toast.error(err.response?.data?.error || 'Failed to update order')
  });

  const products = productsData?.products || [];

  const handleSelectProduct = (product) => {
    if (replacingIndex !== null) {
      const updated = [...items];
      const qty = updated[replacingIndex].quantity;
      updated[replacingIndex] = {
        id: null,
        product_id: product.id,
        product_name: product.name,
        sku: product.sku,
        quantity: qty,
        unit_price: parseFloat(product.price),
        total_price: qty * parseFloat(product.price)
      };
      setItems(updated);
      toast.success(`Replaced with ${product.name}`);
      setReplacingIndex(null);
    } else {
      const existing = items.findIndex(i => i.product_id === product.id);
      if (existing >= 0) {
        const updated = [...items];
        updated[existing].quantity += 1;
        updated[existing].total_price = updated[existing].quantity * updated[existing].unit_price;
        setItems(updated);
      } else {
        setItems(prev => [...prev, {
          id: null,
          product_id: product.id,
          product_name: product.name,
          sku: product.sku,
          quantity: 1,
          unit_price: parseFloat(product.price),
          total_price: parseFloat(product.price)
        }]);
      }
      toast.success(`${product.name} added`);
    }
    setShowSearch(false);
    setProductSearch('');
  };

  const handleQty = (index, qty) => {
    if (qty < 1) return;
    const updated = [...items];
    updated[index].quantity = qty;
    updated[index].total_price = qty * updated[index].unit_price;
    setItems(updated);
  };

  const handleRemove = (index) => {
    setItems(items.filter((_, i) => i !== index));
  };

  const total = items.reduce((s, i) => s + i.total_price, 0);

  const handleSave = () => {
    if (items.length === 0) return toast.error('Add at least one item');
    if (!paymentMethod) return toast.error('Select a payment method');
    updateMutation.mutate({
      items: items.map(i => ({
        product_id: i.product_id,
        quantity: i.quantity,
        unit_price: i.unit_price
      })),
      total_amount: paymentMethod === 'gift' ? 0 : total,
      payment_method: paymentMethod
    });
  };

  const fmt = (n) => new Intl.NumberFormat('en-US', { style: 'currency', currency: 'RWF' }).format(n || 0);

  if (isLoading) return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-8">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto mb-4" />
        <p className="text-gray-600">Loading order...</p>
      </div>
    </div>
  );

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg w-full max-w-2xl mx-4 max-h-[90vh] overflow-y-auto">
        <div className="flex items-center justify-between p-6 border-b">
          <div>
            <h2 className="text-xl font-bold text-gray-900">Edit Order</h2>
            <p className="text-sm text-gray-500">#{order?.order_number}</p>
          </div>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600"><X className="h-6 w-6" /></button>
        </div>

        <div className="p-6 space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Payment Method</label>
            <select
              value={paymentMethod}
              onChange={e => setPaymentMethod(e.target.value)}
              className="w-full px-3 py-2 border rounded-lg text-sm focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              <option value="">Select payment method</option>
              <option value="cash">Cash</option>
              <option value="card">Card</option>
              <option value="mobile_money">Mobile Money</option>
              <option value="split">Split</option>
              <option value="gift">Gift (Free)</option>
            </select>
          </div>

          <div className="flex items-center justify-between">
            <h3 className="font-medium text-gray-900">Order Items</h3>
            <button
              onClick={() => { setReplacingIndex(null); setShowSearch(true); setProductSearch(''); }}
              className="px-3 py-1.5 bg-blue-600 text-white text-sm rounded-lg hover:bg-blue-700 flex items-center gap-1"
            >
              <Plus className="w-4 h-4" /> Add Item
            </button>
          </div>

          {showSearch && (
            <div className="p-3 border rounded-lg bg-gray-50">
              {replacingIndex !== null && (
                <p className="text-xs text-blue-600 mb-2">Replacing: <strong>{items[replacingIndex]?.product_name}</strong></p>
              )}
              <div className="flex gap-2 mb-2">
                <Search className="w-4 h-4 text-gray-400 mt-2.5" />
                <input
                  autoFocus
                  type="text"
                  placeholder="Search products..."
                  value={productSearch}
                  onChange={e => setProductSearch(e.target.value)}
                  className="flex-1 px-3 py-2 border rounded-lg text-sm focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
                <button
                  onClick={() => { setShowSearch(false); setProductSearch(''); setReplacingIndex(null); }}
                  className="px-3 py-2 text-sm text-gray-600 hover:text-gray-900"
                >Cancel</button>
              </div>
              {products.length > 0 && (
                <div className="max-h-48 overflow-y-auto divide-y">
                  {products.map(p => (
                    <div key={p.id} onClick={() => handleSelectProduct(p)}
                      className="flex justify-between items-center p-2 hover:bg-white cursor-pointer rounded">
                      <div>
                        <p className="text-sm font-medium">{p.name}</p>
                        <p className="text-xs text-gray-500">{p.sku}</p>
                      </div>
                      <p className="text-sm font-medium">{fmt(p.price)}</p>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}

          <div className="space-y-2">
            {items.map((item, i) => (
              <div key={i} className="flex items-center gap-3 p-3 border rounded-lg">
                <div className="flex-1 min-w-0">
                  <p className="font-medium text-sm truncate">{item.product_name}</p>
                  <p className="text-xs text-gray-500">{item.sku}</p>
                </div>
                <div className="flex items-center gap-1">
                  <button onClick={() => handleQty(i, item.quantity - 1)}
                    className="w-7 h-7 flex items-center justify-center border rounded hover:bg-gray-50 text-sm">−</button>
                  <span className="w-8 text-center text-sm">{item.quantity}</span>
                  <button onClick={() => handleQty(i, item.quantity + 1)}
                    className="w-7 h-7 flex items-center justify-center border rounded hover:bg-gray-50 text-sm">+</button>
                </div>
                <div className="text-right min-w-[80px]">
                  <p className="text-sm font-medium">{fmt(item.total_price)}</p>
                  <p className="text-xs text-gray-500">{fmt(item.unit_price)} each</p>
                </div>
                <button onClick={() => { setReplacingIndex(i); setShowSearch(true); setProductSearch(''); }}
                  className="p-1.5 text-blue-600 hover:bg-blue-50 rounded" title="Replace item">
                  <RefreshCw className="w-4 h-4" />
                </button>
                <button onClick={() => handleRemove(i)} className="p-1.5 text-red-600 hover:bg-red-50 rounded">
                  <Trash2 className="w-4 h-4" />
                </button>
              </div>
            ))}
            {items.length === 0 && (
              <p className="text-center py-8 text-gray-400 text-sm">No items. Click "Add Item" to add products.</p>
            )}
          </div>

          {items.length > 0 && (
            <div className="flex justify-between items-center pt-3 border-t font-semibold">
              <span>Total</span>
              <span>{fmt(total)}</span>
            </div>
          )}
        </div>

        <div className="flex justify-end gap-3 px-6 pb-6">
          <button onClick={onClose} className="px-4 py-2 text-sm text-gray-700 border rounded-lg hover:bg-gray-50">Cancel</button>
          <button onClick={handleSave} disabled={updateMutation.isPending}
            className="px-4 py-2 text-sm bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 flex items-center gap-2">
            <Save className="w-4 h-4" />
            {updateMutation.isPending ? 'Saving...' : 'Save Changes'}
          </button>
        </div>
      </div>
    </div>
  );
};

export default OrderEditModal;
