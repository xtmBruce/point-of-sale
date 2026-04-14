const OrderEditModal = ({ onClose }) => {
  return (
    <div className="rounded-lg border border-gray-200 bg-white p-6 shadow-sm">
      <h3 className="text-lg font-semibold text-gray-900">Order Editing</h3>
      <p className="mt-2 text-sm text-gray-600">
        Order editing UI is not available in this build yet.
      </p>
      <button
        type="button"
        onClick={onClose}
        className="mt-4 rounded bg-primary-600 px-4 py-2 text-sm font-medium text-white"
      >
        Close
      </button>
    </div>
  );
};

export default OrderEditModal;
