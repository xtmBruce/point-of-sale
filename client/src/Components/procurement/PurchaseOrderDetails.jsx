import React from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { procurementAPI } from '../../lib/api';
import { X, Calendar, User, Truck, FileText, CheckCircle, Trash2, Edit } from 'lucide-react';
import toast from 'react-hot-toast';
import Swal from 'sweetalert2';

const PurchaseOrderDetails = ({ orderId, onClose, onEdit }) => {
    const { data: orderResponse, isLoading } = useQuery({
        queryKey: ['purchase-order', orderId],
        queryFn: () => procurementAPI.getPurchaseOrderById(orderId),
        enabled: !!orderId
    });

    const queryClient = useQueryClient();

    const order = orderResponse?.data?.purchase_order;

    const updateStatusMutation = useMutation({
        mutationFn: (status) => procurementAPI.updatePurchaseOrderStatus(orderId, status),
        onSuccess: () => {
            queryClient.invalidateQueries(['purchase-order', orderId]);
            queryClient.invalidateQueries(['purchase-orders']);
            toast.success('Order approved successfully');
            if (onClose) onClose();
        },
        onError: (error) => {
            toast.error('Failed to update status');
            console.error(error);
        }
    });

    const handleApprove = () => {
        updateStatusMutation.mutate('approved');
    };

    const deleteOrderMutation = useMutation({
        mutationFn: () => procurementAPI.deletePurchaseOrder(orderId),
        onSuccess: () => {
            queryClient.invalidateQueries(['purchase-orders']);
            toast.success('Order deleted successfully');
            if (onClose) onClose();
        },
        onError: (error) => {
            toast.error('Failed to delete order');
            console.error(error);
        }
    });

    const handleDelete = async () => {
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
            deleteOrderMutation.mutate();
        }
    };

    if (isLoading) {
        return (
            <div className="bg-white rounded-lg shadow-xl w-full max-w-4xl mx-auto p-8 flex justify-center items-center h-64">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
            </div>
        );
    }

    if (!order) return null;

    const formatDate = (dateString) => {
        return new Date(dateString).toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'long',
            day: 'numeric'
        });
    };

    const formatCurrency = (amount, currency = 'USD') => {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: currency
        }).format(amount || 0);
    };

    return (
        <div className="bg-white rounded-lg shadow-xl w-full max-w-4xl mx-auto flex flex-col max-h-[90vh]">
            {/* Header */}
            <div className="p-6 border-b border-gray-100 flex justify-between items-center bg-gray-50 rounded-t-lg">
                <div className="flex items-center space-x-4">
                    <div className="p-2 bg-blue-100 rounded-lg">
                        <FileText className="h-6 w-6 text-blue-600" />
                    </div>
                    <div>
                        <h2 className="text-xl font-bold text-gray-800">{order.po_number}</h2>
                        <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${order.status === 'approved' ? 'bg-green-100 text-green-800' :
                            order.status === 'pending' ? 'bg-yellow-100 text-yellow-800' :
                                'bg-gray-100 text-gray-800'
                            }`}>
                            {order.status.toUpperCase()}
                        </span>
                    </div>
                </div>
                <button onClick={onClose} className="text-gray-400 hover:text-gray-600 transition-colors">
                    <X size={24} />
                </button>
            </div>

            <div className="p-6 overflow-y-auto custom-scrollbar space-y-8">
                {/* Info Grid */}
                <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                    <div className="space-y-4">
                        <h3 className="text-sm font-semibold text-gray-500 uppercase tracking-wider">Supplier Details</h3>
                        <div className="bg-gray-50 p-4 rounded-lg border border-gray-200">
                            <p className="text-lg font-medium text-gray-900">{order.supplier_name}</p>
                            <div className="mt-2 space-y-1 text-sm text-gray-600">
                                <p>{order.contact_person}</p>
                                <p>{order.supplier_email}</p>
                                <p>{order.supplier_phone}</p>
                            </div>
                        </div>
                    </div>

                    <div className="space-y-4">
                        <h3 className="text-sm font-semibold text-gray-500 uppercase tracking-wider">Order Information</h3>
                        <div className="bg-gray-50 p-4 rounded-lg border border-gray-200 space-y-3">
                            <div className="flex justify-between">
                                <span className="text-sm text-gray-600">Order Date</span>
                                <span className="text-sm font-medium text-gray-900">{formatDate(order.order_date)}</span>
                            </div>
                            <div className="flex justify-between">
                                <span className="text-sm text-gray-600">Expected Delivery</span>
                                <span className="text-sm font-medium text-gray-900">
                                    {order.expected_delivery_date ? formatDate(order.expected_delivery_date) : '-'}
                                </span>
                            </div>
                            <div className="flex justify-between">
                                <span className="text-sm text-gray-600">Created By</span>
                                <span className="text-sm font-medium text-gray-900">{order.first_name} {order.last_name}</span>
                            </div>
                        </div>
                    </div>
                </div>

                {/* Items Table */}
                <div>
                    <h3 className="text-sm font-semibold text-gray-500 uppercase tracking-wider mb-4">Items</h3>
                    <div className="border border-gray-200 rounded-lg overflow-hidden">
                        <table className="min-w-full divide-y divide-gray-200">
                            <thead className="bg-gray-50">
                                <tr>
                                    <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Item</th>
                                    <th scope="col" className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Quantity</th>
                                    <th scope="col" className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Unit Cost</th>
                                    <th scope="col" className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Total</th>
                                </tr>
                            </thead>
                            <tbody className="bg-white divide-y divide-gray-200">
                                {order.items?.map((item, index) => (
                                    <tr key={item.id || index}>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                                            {/* Display item_name if available, otherwise material_name */}
                                            {item.item_name || item.material_name || 'Unknown Item'}
                                            {item.unit_of_measure && <span className="text-gray-500 ml-1">({item.unit_of_measure})</span>}
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500 text-right">
                                            {Number(item.quantity_ordered)}
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500 text-right">
                                            {formatCurrency(item.unit_cost, order.currency)}
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 font-medium text-right">
                                            {formatCurrency(item.total_cost, order.currency)}
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                            <tfoot className="bg-gray-50">
                                <tr>
                                    <td colSpan="3" className="px-6 py-4 text-right text-sm font-medium text-gray-500">Subtotal</td>
                                    <td className="px-6 py-4 text-right text-sm font-bold text-gray-900 text-right">
                                        {formatCurrency(order.items?.reduce((sum, item) => sum + Number(item.total_cost), 0), order.currency)}
                                    </td>
                                </tr>
                            </tfoot>
                        </table>
                    </div>
                </div>

                {/* Financials & Landed Costs */}
                <div className="bg-blue-50 rounded-lg p-6 border border-blue-100">
                    <h3 className="text-sm font-semibold text-blue-800 uppercase tracking-wider mb-4">Financial Summary</h3>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                        <div className="space-y-2">
                            {/* Display landed costs if they exist */}
                            {['transport_supplier_cost', 'bank_charges', 'transport_kigali_cost', 'laisse_suivre_cost', 'import_taxes', 'storage_cost', 'declarant_fees', 'transport_warehouse_cost'].map(costField => (
                                order[costField] > 0 && (
                                    <div key={costField} className="flex justify-between text-sm text-gray-600">
                                        <span className="capitalize">{costField.replace(/_/g, ' ')}</span>
                                        <span>{formatCurrency(order[costField], order.currency)}</span>
                                    </div>
                                )
                            ))}
                        </div>
                        <div className="space-y-3">
                            <div className="flex justify-between items-center pt-4 border-t border-blue-200">
                                <span className="text-lg font-bold text-gray-900">Total ({order.currency || 'USD'})</span>
                                <span className="text-lg font-bold text-gray-900">{formatCurrency(order.total_amount, order.currency)}</span>
                            </div>
                            {order.total_amount_rfw && (
                                <div className="flex justify-between items-center">
                                    <span className="text-lg font-bold text-blue-700">Total (RWF)</span>
                                    <span className="text-xl font-bold text-blue-700">
                                        {new Intl.NumberFormat('en-RW', { style: 'currency', currency: 'RWF' }).format(order.total_amount_rfw)}
                                    </span>
                                </div>
                            )}
                        </div>
                    </div>
                </div>

                {order.notes && (
                    <div>
                        <h3 className="text-sm font-semibold text-gray-500 uppercase tracking-wider mb-2">Notes</h3>
                        <div className="bg-gray-50 p-4 rounded-lg border border-gray-200">
                            <p className="text-sm text-gray-700 whitespace-pre-wrap">{order.notes}</p>
                        </div>
                    </div>
                )}
            </div>


            <div className="p-6 border-t border-gray-100 bg-gray-50 rounded-b-lg flex justify-end space-x-3">
                <button
                    onClick={onClose}
                    className="px-6 py-2.5 rounded-lg border border-gray-300 bg-white text-gray-700 hover:bg-gray-50 font-medium transition-colors"
                >
                    Close
                </button>
                {order.status === 'pending' && (
                    <>
                        <button
                            onClick={handleDelete}
                            className="px-6 py-2.5 rounded-lg border border-red-300 text-red-700 hover:bg-red-50 font-medium transition-colors flex items-center"
                        >
                            <Trash2 className="w-5 h-5 mr-2" />
                            Delete
                        </button>
                        <button
                            onClick={() => onEdit(order)}
                            className="px-6 py-2.5 rounded-lg border border-blue-300 text-blue-700 hover:bg-blue-50 font-medium transition-colors flex items-center"
                        >
                            <Edit className="w-5 h-5 mr-2" />
                            Edit
                        </button>
                        <button
                            onClick={handleApprove}
                            disabled={updateStatusMutation.isPending}
                            className="px-6 py-2.5 rounded-lg bg-green-600 text-white hover:bg-green-700 font-medium transition-colors flex items-center"
                        >
                            <CheckCircle className="w-5 h-5 mr-2" />
                            {updateStatusMutation.isPending ? 'Approving...' : 'Approve Order'}
                        </button>
                    </>
                )}
            </div>
        </div>
    );
};

export default PurchaseOrderDetails;
