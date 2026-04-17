import React, { useState, useEffect } from 'react';
import {
  ArrowLeft,
  ShoppingCart,
  User,
  CreditCard,
  Scan,
  Search,
  Package,
  DollarSign,
  Clock,
  TrendingUp,
  Users,
  Zap,
  Settings,
  HelpCircle,
  Keyboard,
  Monitor,
  Smartphone,
  Tablet
} from 'lucide-react';
import { useNavigate } from 'react-router-dom';
<<<<<<< HEAD
import OrderForm from '../Components/OrderForm';
=======
import OrderForm from '../components/OrderForm';
>>>>>>> d10bc65ca0e2784567c21698cb5ed72221dedbd3
import toast from 'react-hot-toast';

const CreateOrder = () => {
  const navigate = useNavigate();
  const [currentTime, setCurrentTime] = useState(new Date());
  const [isFullscreen, setIsFullscreen] = useState(false);
  const [showKeyboardShortcuts, setShowKeyboardShortcuts] = useState(false);

  useEffect(() => {
    // Update time every second
    const timer = setInterval(() => {
      setCurrentTime(new Date());
    }, 1000);

    return () => clearInterval(timer);
  }, []);

  const handleOrderCreated = (newOrder) => {
    toast.success('Order created successfully!', {
      icon: '🎉',
      duration: 3000,
      style: {
        borderRadius: '10px',
        background: '#363636',
        color: '#fff',
      },
    });
    navigate('/orders');
  };

  const handleGoBack = () => {
    navigate('/orders');
  };

  const toggleFullscreen = () => {
    if (!document.fullscreenElement) {
      document.documentElement.requestFullscreen();
      setIsFullscreen(true);
    } else {
      document.exitFullscreen();
      setIsFullscreen(false);
    }
  };

  const formatTime = (date) => {
    return date.toLocaleTimeString('en-US', {
      hour12: false,
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    });
  };

  const formatDate = (date) => {
    return date.toLocaleDateString('en-US', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50 to-indigo-50 flex flex-col">
      {/* Professional Header */}
      {/* Header removed as requested */}

      {/* Keyboard Shortcuts Modal */}
      {showKeyboardShortcuts && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl shadow-2xl max-w-md w-full p-6">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-semibold text-gray-900 flex items-center">
                <Keyboard className="h-5 w-5 mr-2" />
                Keyboard Shortcuts
              </h3>
              <button
                onClick={() => setShowKeyboardShortcuts(false)}
                className="p-1 text-gray-400 hover:text-gray-600"
              >
                <span className="text-2xl">&times;</span>
              </button>
            </div>

            <div className="space-y-3">
              <div className="flex justify-between items-center">
                <span className="text-sm text-gray-600">Focus Barcode Scanner</span>
                <kbd className="px-2 py-1 bg-gray-100 border border-gray-300 rounded text-xs">F2</kbd>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-sm text-gray-600">Activate Scanner</span>
                <kbd className="px-2 py-1 bg-gray-100 border border-gray-300 rounded text-xs">F3</kbd>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-sm text-gray-600">Toggle Fullscreen</span>
                <kbd className="px-2 py-1 bg-gray-100 border border-gray-300 rounded text-xs">F11</kbd>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-sm text-gray-600">Quick Search</span>
                <kbd className="px-2 py-1 bg-gray-100 border border-gray-300 rounded text-xs">Ctrl + K</kbd>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-sm text-gray-600">Complete Order</span>
                <kbd className="px-2 py-1 bg-gray-100 border border-gray-300 rounded text-xs">Ctrl + Enter</kbd>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-sm text-gray-600">Quick Invoice</span>
                <kbd className="px-2 py-1 bg-gray-100 border border-gray-300 rounded text-xs">F4</kbd>
              </div>
            </div>

            <div className="mt-6 pt-4 border-t border-gray-200">
              <p className="text-xs text-gray-500 text-center">
                💡 Pro tip: Use keyboard shortcuts for faster checkout experience
              </p>
            </div>
          </div>
        </div>
      )}

      {/* Main POS Layout */}
      <main className="flex-1 w-full flex flex-col lg:flex-row gap-4 py-4 px-2 sm:px-4 lg:px-6">
        {/* Left: Product & Customer Search */}
        <section className="flex-1 min-w-0">
          <OrderForm
            onOrderCreated={handleOrderCreated}
            onClose={handleGoBack}
            isFullPage={true}
            modernPOS={true}
          />
        </section>
      </main>

      {/* Floating Action Bar */}
      <div className="fixed bottom-4 right-4 flex flex-col gap-2 z-40">
        {/* Quick Actions */}
        <div className="flex gap-2">
          <button
            onClick={() => setShowKeyboardShortcuts(true)}
            className="p-3 bg-white shadow-lg rounded-full text-gray-600 hover:text-gray-900 hover:shadow-xl transition-all duration-200"
            title="Keyboard Shortcuts"
          >
            <Keyboard className="h-5 w-5" />
          </button>

          <button
            onClick={toggleFullscreen}
            className="p-3 bg-white shadow-lg rounded-full text-gray-600 hover:text-gray-900 hover:shadow-xl transition-all duration-200"
            title={isFullscreen ? "Exit Fullscreen" : "Enter Fullscreen"}
          >
            {isFullscreen ? <Monitor className="h-5 w-5" /> : <Smartphone className="h-5 w-5" />}
          </button>
        </div>

        {/* Status Indicator */}
        <div className="bg-white shadow-lg rounded-full px-4 py-2 flex items-center gap-2">
          <div className="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
          <span className="text-xs font-medium text-gray-700">POS Active</span>
        </div>
      </div>

      {/* Performance Tips */}
      {/* <div className="fixed top-20 right-4 bg-white shadow-lg rounded-lg p-4 max-w-xs z-20">
        <h4 className="font-semibold text-gray-900 mb-2 flex items-center">
          <Zap className="h-4 w-4 mr-2 text-yellow-500" />
          Performance Tips
        </h4>
        <ul className="text-xs text-gray-600 space-y-1">
          <li>• Use F2 to quickly focus barcode scanner</li>
          <li>• F3 activates scanner for hands-free operation</li>
          <li>• F4 generates invoice instantly for busy queues</li>
          <li>• Customer search supports partial names</li>
          <li>• Products auto-add on valid barcode scan</li>
        </ul>
      </div> */}
    </div>
  );
};

export default CreateOrder;
