import React, { useState, useEffect } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { procurementAPI, productsAPI } from '../../lib/api';
import { Plus, Trash2, Save, X, Calculator } from 'lucide-react';
import Swal from 'sweetalert2';
import toast from 'react-hot-toast';

const PurchaseOrderForm = ({ onClose, onSuccess, initialData }) => {
    const queryClient = useQueryClient();
    const [items, setItems] = useState([]);
    const [formData, setFormData] = useState({
        supplier_id: '',
        supplier_name: '',
        order_date: new Date().toISOString().split('T')[0],
        expected_delivery_date: '',
        payment_terms: '',
        shipping_address: '',
        currency: 'USD',
        currency_rate: 1,
        transport_supplier_cost: 0,
        bank_charges: 0,
        transport_kigali_cost: 0,
        laisse_suivre_cost: 0,
        import_taxes: 0,
        storage_cost: 0,
        declarant_fees: 0,
        transport_warehouse_cost: 0,
        notes: ''
    });

    // Fetch Suppliers (Optional now, but good for auto-complete if we wanted)
    const { data: suppliersData } = useQuery({
        queryKey: ['suppliers'],
        queryFn: async () => {
            const response = await procurementAPI.getSuppliers();
            return response.data.suppliers;
        }
    });

    const { data: materialsData } = useQuery({
        queryKey: ['raw-materials'],
        queryFn: async () => {
            const response = await procurementAPI.getMaterials();
            return response.data.materials;
        }
    });

    // Fetch Full Order Details if editing and items are missing
    const { data: fullOrderData } = useQuery({
        queryKey: ['purchase-order', initialData?.id],
        queryFn: () => procurementAPI.getPurchaseOrderById(initialData.id),
        enabled: !!initialData?.id && (!initialData.items || initialData.items.length === 0),
    });

    // Use fullOrderData if available, otherwise initialData
    const dataToUse = fullOrderData ? fullOrderData.data.purchase_order : initialData;

    const createPOMutation = useMutation({
        mutationFn: (data) => procurementAPI.createPurchaseOrder(data),
        onSuccess: () => {
            queryClient.invalidateQueries(['purchase-orders']);
            toast.success('Purchase order created successfully');
            if (onSuccess) onSuccess();
            if (onClose) onClose();
        },
        onError: (error) => {
            toast.error(error.response?.data?.error || 'Failed to create purchase order');
        }
    });

    const updatePOMutation = useMutation({
        mutationFn: (data) => procurementAPI.updatePurchaseOrder(dataToUse.id, data),
        onSuccess: () => {
            queryClient.invalidateQueries(['purchase-orders']);
            if (dataToUse?.id) {
                queryClient.invalidateQueries(['purchase-order', dataToUse.id]);
            }
            toast.success('Purchase order updated successfully');
            if (onSuccess) onSuccess();
            if (onClose) onClose();
        },
        onError: (error) => {
            toast.error(error.response?.data?.error || 'Failed to update purchase order');
        }
    });

    useEffect(() => {
        const sourceData = dataToUse;
        if (sourceData) {
            setFormData({
                supplier_id: sourceData.supplier_id || '',
                supplier_name: sourceData.supplier_name || '',
                order_date: sourceData.order_date ? sourceData.order_date.split('T')[0] : new Date().toISOString().split('T')[0],
                expected_delivery_date: sourceData.expected_delivery_date ? sourceData.expected_delivery_date.split('T')[0] : '',
                payment_terms: sourceData.payment_terms || '',
                shipping_address: sourceData.shipping_address || '',
                currency: sourceData.currency || 'USD',
                currency_rate: sourceData.currency_rate || 1,
                transport_supplier_cost: sourceData.transport_supplier_cost || 0,
                bank_charges: sourceData.bank_charges || 0,
                transport_kigali_cost: sourceData.transport_kigali_cost || 0,
                laisse_suivre_cost: sourceData.laisse_suivre_cost || 0,
                import_taxes: sourceData.import_taxes || 0,
                storage_cost: sourceData.storage_cost || 0,
                declarant_fees: sourceData.declarant_fees || 0,
                transport_warehouse_cost: sourceData.transport_warehouse_cost || 0,
                notes: sourceData.notes || ''
            });

            if (sourceData.items) {
                setItems(sourceData.items.map(item => ({
                    id: item.id || Date.now() + Math.random(),
                    material_id: item.material_id || '',
                    item_name: item.item_name || item.material_name || '',
                    quantity_ordered: item.quantity_ordered,
                    unit_cost: item.unit_cost,
                    total_cost: item.total_cost || (item.quantity_ordered * item.unit_cost)
                })));
            }
        }
    }, [dataToUse]);

    const addItem = () => {
        setItems([...items, {
            id: Date.now(),
            material_id: '',
            quantity_ordered: 1,
            unit_cost: 0,
            total_cost: 0
        }]);
    };

    const updateItem = (id, field, value) => {
        const newItems = items.map(item => {
            if (item.id === id) {
                const updatedItem = { ...item, [field]: value };
                if (field === 'quantity_ordered' || field === 'unit_cost') {
                    updatedItem.total_cost = updatedItem.quantity_ordered * updatedItem.unit_cost;
                }
                return updatedItem;
            }
            return item;
        });
        setItems(newItems);
    };

    const removeItem = (id) => {
        setItems(items.filter(item => item.id !== id));
    };

    // Calculations
    const subtotal = items.reduce((sum, item) => sum + Number(item.total_cost || 0), 0);

    const totalLandedCost =
        Number(formData.transport_supplier_cost) +
        Number(formData.bank_charges) +
        Number(formData.transport_kigali_cost) +
        Number(formData.laisse_suivre_cost) +
        Number(formData.import_taxes) +
        Number(formData.storage_cost) +
        Number(formData.declarant_fees) +
        Number(formData.transport_warehouse_cost);

    const totalInvoice = subtotal + totalLandedCost;
    const totalInvoiceRfw = totalInvoice * Number(formData.currency_rate);

    const handleSubmit = (e) => {
        e.preventDefault();
        if (items.length === 0) {
            toast.error('Please add at least one item');
            return;
        }

        // Clean up items for submission
        const formattedItems = items.map(item => ({
            material_id: item.material_id, // Might be null if manual entry
            item_name: item.item_name,
            quantity_ordered: Number(item.quantity_ordered),
            unit_cost: Number(item.unit_cost)
        }));

        if (formattedItems.some(i => !i.item_name)) {
            toast.error('Please enter a description for all items');
            return;
        }

        const mutation = dataToUse?.id ? updatePOMutation : createPOMutation;

        mutation.mutate({
            ...formData,
            items: formattedItems,
            total_amount: totalInvoice,
            total_amount_rfw: totalInvoiceRfw
        });
    };

    const handleCurrencySelect = (currency) => {
        setFormData({ ...formData, currency });
        // Optional: Can verify if we want to auto-set rate based on currency later
    };

    return (
        <div className="bg-white rounded-lg shadow-xl max-w-4xl w-full mx-2 sm:mx-4 max-h-[98vh] sm:max-h-[90vh] overflow-y-auto p-2 sm:p-4">
            <div className="flex items-center justify-between p-6 border-b border-gray-200">
                <h3 className="text-lg font-semibold text-gray-900">
                    {dataToUse?.id ? 'Edit Supplier Invoice' : 'New Supplier Invoice'}
                </h3>
                <button onClick={onClose} className="text-gray-400 hover:text-gray-600">
                    <X className="h-6 w-6" />
                </button>
            </div>

            <form id="po-form" onSubmit={handleSubmit} className="p-6 space-y-6">

                {/* General Info */}
                <div>
                    <h4 className="text-md font-semibold text-gray-900 mb-4">General Information</h4>
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Supplier *</label>
                            <input
                                type="text"
                                required
                                className="w-full rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 transition-colors py-3 px-3"
                                placeholder="Enter Supplier Name"
                                value={formData.supplier_name}
                                onChange={e => setFormData({ ...formData, supplier_name: e.target.value, supplier_id: '' })}
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Invoice Date *</label>
                            <input
                                type="date"
                                required
                                className="w-full rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 py-3 px-3"
                                value={formData.order_date}
                                onChange={e => setFormData({ ...formData, order_date: e.target.value })}
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Currency</label>
                            <select
                                className="w-full rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 py-3 px-3 transition-colors"
                                value={formData.currency}
                                onChange={e => handleCurrencySelect(e.target.value)}
                            >
                                {['USD', 'EUR', 'GBP', 'RWF'].map((curr) => (
                                    <option key={curr} value={curr}>{curr}</option>
                                ))}
                            </select>
                        </div>
                    </div>
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mt-4">
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Rate of the Day</label>
                            <div className="relative rounded-md shadow-sm">
                                <input
                                    type="number"
                                    min="0"
                                    step="0.0001"
                                    className="w-full rounded-lg border-gray-200 shadow-sm focus:border-blue-500 focus:ring-blue-500 pr-12 text-right font-mono py-3 px-3"
                                    value={formData.currency_rate}
                                    onChange={e => setFormData({ ...formData, currency_rate: e.target.value })}
                                />
                                <div className="absolute inset-y-0 right-0 pr-3 flex items-center pointer-events-none">
                                    <span className="text-gray-500 sm:text-sm">RWF</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                {/* Items */}
                <div>
                    <div className="flex justify-between items-center mb-4">
                        <h4 className="text-md font-semibold text-gray-900">Items</h4>
                        <button
                            type="button"
                            onClick={addItem}
                            className="inline-flex items-center px-3 py-1.5 text-sm font-medium text-blue-600 bg-blue-50 rounded-lg hover:bg-blue-100 transition-colors"
                        >
                            <Plus size={16} className="mr-1" /> Add Item
                        </button>
                    </div>

                    {items.length === 0 ? (
                        <div className="text-center py-8 bg-gray-50 rounded-lg border-2 border-dashed border-gray-200">
                            <p className="text-gray-500">No items added yet. Click "Add Item" to start.</p>
                        </div>
                    ) : (
                        <div className="space-y-4">
                            {items.map((item, index) => (
                                <div key={item.id} className="flex flex-col md:flex-row gap-4 items-start md:items-end bg-gray-50 p-4 rounded-lg border border-gray-200 animate-fadeIn">
                                    <div className="flex-grow w-full md:w-auto">
                                        <label className="block text-xs font-medium text-gray-500 mb-1">Item Description</label>
                                        <input
                                            type="text"
                                            className="w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-sm py-3 px-3"
                                            placeholder="Enter item description"
                                            value={item.item_name || ''}
                                            onChange={e => {
                                                const val = e.target.value;
                                                updateItem(item.id, 'material_id', null);
                                                updateItem(item.id, 'item_name', val);
                                            }}
                                        />
                                    </div>
                                    <div className="w-full md:w-32">
                                        <label className="block text-xs font-medium text-gray-500 mb-1">Quantity</label>
                                        <input
                                            type="number"
                                            min="0"
                                            step="0.01"
                                            className="w-full rounded-md border-gray-200 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-sm text-right font-mono py-3 px-3"
                                            value={item.quantity_ordered}
                                            onChange={e => updateItem(item.id, 'quantity_ordered', e.target.value)}
                                        />
                                    </div>
                                    <div className="w-full md:w-32">
                                        <label className="block text-xs font-medium text-gray-500 mb-1">Unit Cost</label>
                                        <input
                                            type="number"
                                            min="0"
                                            step="0.01"
                                            className="w-full rounded-md border-gray-200 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-sm text-right font-mono py-3 px-3"
                                            value={item.unit_cost}
                                            onChange={e => updateItem(item.id, 'unit_cost', e.target.value)}
                                        />
                                    </div>
                                    <div className="w-full md:w-32">
                                        <label className="block text-xs font-medium text-gray-500 mb-1">Total</label>
                                        <div className="py-3 px-3 bg-white rounded-md border border-gray-200 text-right text-sm font-mono">
                                            {Number(item.total_cost || 0).toFixed(2)}
                                        </div>
                                    </div>
                                    <div className="pb-1">
                                        <button
                                            type="button"
                                            onClick={() => removeItem(item.id)}
                                            className="p-3 text-red-400 hover:text-red-600 hover:bg-red-50 rounded-full transition-colors"
                                        >
                                            <Trash2 size={18} />
                                        </button>
                                    </div>
                                </div>
                            ))}
                            <div className="flex justify-end pt-2">
                                <div className="text-right">
                                    <span className="text-sm text-gray-600 mr-4">Items Subtotal:</span>
                                    <span className="text-lg font-bold text-gray-900">{subtotal.toFixed(2)}</span>
                                </div>
                            </div>
                        </div>
                    )}
                </div>

                {/* Landed Costs */}
                <div>
                    <h4 className="text-md font-semibold text-gray-900 mb-4">Landed Costs</h4>
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                        <div>
                            <label className="block text-sm font-medium text-gray-600 mb-1">Transport to Supplier</label>
                            <input
                                type="number"
                                min="0"
                                step="0.01"
                                className="w-full rounded-lg border-gray-200 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-right font-mono py-3 px-3"
                                value={formData.transport_supplier_cost}
                                onChange={e => setFormData({ ...formData, transport_supplier_cost: Number(e.target.value) })}
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-600 mb-1">Bank Charges</label>
                            <input
                                type="number"
                                min="0"
                                step="0.01"
                                className="w-full rounded-lg border-gray-200 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-right font-mono py-3 px-3"
                                value={formData.bank_charges}
                                onChange={e => setFormData({ ...formData, bank_charges: Number(e.target.value) })}
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-600 mb-1">Transport to Kigali</label>
                            <input
                                type="number"
                                min="0"
                                step="0.01"
                                className="w-full rounded-lg border-gray-200 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-right font-mono py-3 px-3"
                                value={formData.transport_kigali_cost}
                                onChange={e => setFormData({ ...formData, transport_kigali_cost: Number(e.target.value) })}
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-600 mb-1">Laisse Suivre</label>
                            <input
                                type="number"
                                min="0"
                                step="0.01"
                                className="w-full rounded-lg border-gray-200 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-right font-mono py-3 px-3"
                                value={formData.laisse_suivre_cost}
                                onChange={e => setFormData({ ...formData, laisse_suivre_cost: Number(e.target.value) })}
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-600 mb-1">Import Taxes</label>
                            <input
                                type="number"
                                min="0"
                                step="0.01"
                                className="w-full rounded-lg border-gray-200 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-right font-mono py-3 px-3"
                                value={formData.import_taxes}
                                onChange={e => setFormData({ ...formData, import_taxes: Number(e.target.value) })}
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-600 mb-1">Storage</label>
                            <input
                                type="number"
                                min="0"
                                step="0.01"
                                className="w-full rounded-lg border-gray-200 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-right font-mono py-3 px-3"
                                value={formData.storage_cost}
                                onChange={e => setFormData({ ...formData, storage_cost: Number(e.target.value) })}
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-600 mb-1">Déclarant Fees</label>
                            <input
                                type="number"
                                min="0"
                                step="0.01"
                                className="w-full rounded-lg border-gray-200 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-right font-mono py-3 px-3"
                                value={formData.declarant_fees}
                                onChange={e => setFormData({ ...formData, declarant_fees: Number(e.target.value) })}
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-600 mb-1">Transport to Warehouse</label>
                            <input
                                type="number"
                                min="0"
                                step="0.01"
                                className="w-full rounded-lg border-gray-200 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-right font-mono py-3 px-3"
                                value={formData.transport_warehouse_cost}
                                onChange={e => setFormData({ ...formData, transport_warehouse_cost: Number(e.target.value) })}
                            />
                        </div>
                    </div>
                </div>

                {/* Summary */}
                <div>
                    <h4 className="text-md font-semibold text-gray-900 mb-4">Summary</h4>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                        <div className="space-y-4">
                            <div className="flex justify-between text-sm text-gray-600 border-b border-blue-200 pb-2">
                                <span>Items Subtotal:</span>
                                <span>{subtotal.toFixed(2)}</span>
                            </div>
                            <div className="flex justify-between text-sm text-gray-600 border-b border-blue-200 pb-2">
                                <span>Additional Costs:</span>
                                <span>{totalLandedCost.toFixed(2)}</span>
                            </div>
                            <div className="flex justify-between text-lg font-bold text-gray-900 pt-2">
                                <span>Total Invoice (Orig):</span>
                                <span>{totalInvoice.toFixed(2)}</span>
                            </div>
                        </div>
                        <div className="pl-0 md:pl-8 border-t md:border-t-0 md:border-l border-blue-200 pt-4 md:pt-0">
                            <p className="text-sm text-blue-800 font-medium mb-2 uppercase tracking-wide">Total Invoice in RFW</p>
                            <div className="text-4xl font-bold text-blue-600 tracking-tight">
                                {totalInvoiceRfw.toLocaleString('en-RW', { style: 'currency', currency: 'RWF' })}
                            </div>
                            <p className="text-xs text-blue-500 mt-2">
                                Based on rate: 1 Unit = {formData.currency_rate} RWF
                            </p>
                        </div>
                    </div>
                </div>

                <div className="flex items-center justify-end space-x-3 pt-6 border-t border-gray-200 mt-6">
                    <button
                        type="button"
                        onClick={onClose}
                        className="px-4 py-2 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
                    >
                        Cancel
                    </button>
                    <button
                        type="submit"
                        disabled={createPOMutation.isPending || updatePOMutation.isPending}
                        className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 flex items-center"
                    >
                        {createPOMutation.isPending || updatePOMutation.isPending ? 'Saving...' : (initialData ? 'Update Invoice' : 'Create Invoice')}
                    </button>
                </div>
            </form>
        </div>
    );
};

export default PurchaseOrderForm;
