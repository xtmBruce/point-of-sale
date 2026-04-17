import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import OrderEditModal from '../components/OrderEditModal';

const OrderEdit = () => {
  const { orderId } = useParams();
  const navigate = useNavigate();

  return (
    <OrderEditModal
      orderId={orderId}
      onClose={() => navigate('/orders')}
      onSave={() => navigate('/orders')}
    />
  );
};

export default OrderEdit;
