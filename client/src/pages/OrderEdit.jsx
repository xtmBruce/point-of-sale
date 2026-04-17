import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
<<<<<<< HEAD
import OrderEditModal from '../Components/OrderEditModal';
=======
import OrderEditModal from '../components/OrderEditModal';
>>>>>>> d10bc65ca0e2784567c21698cb5ed72221dedbd3

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
