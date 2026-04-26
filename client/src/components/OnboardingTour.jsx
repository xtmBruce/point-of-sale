import React, { useState, useEffect } from 'react';
import { X, ChevronRight, ChevronLeft } from 'lucide-react';

const OnboardingTour = ({ isVisible, onComplete, userRole = 'admin' }) => {
  const [currentStep, setCurrentStep] = useState(0);

  const tourSteps = {
    admin: [
      {
        title: 'Welcome to SmartPOS',
        description: 'This dashboard gives you a comprehensive view of your business performance.',
        target: 'header'
      },
      {
        title: 'Quick Stats',
        description: 'View key metrics including total orders, revenue, and customer insights.',
        target: 'stats'
      },
      {
        title: 'Transactions',
        description: 'Monitor all your transactions in real-time.',
        target: 'transactions'
      },
      {
        title: 'Ready to Go!',
        description: 'You can now navigate through all features. Click "Complete" to finish the tour.',
        target: 'footer'
      }
    ],
    cashier: [
      {
        title: 'Welcome Cashier',
        description: 'You can create orders and process transactions here.',
        target: 'header'
      },
      {
        title: 'Quick Stats',
        description: 'Track your daily sales and order count.',
        target: 'stats'
      },
      {
        title: 'All Set!',
        description: 'Start processing orders now.',
        target: 'footer'
      }
    ],
    manager: [
      {
        title: 'Welcome Manager',
        description: 'Monitor your inventory and sales reports from this dashboard.',
        target: 'header'
      },
      {
        title: 'Business Metrics',
        description: 'View detailed analytics about your operations.',
        target: 'stats'
      },
      {
        title: 'Ready!',
        description: 'Start exploring the dashboard.',
        target: 'footer'
      }
    ]
  };

  const steps = tourSteps[userRole] || tourSteps.admin;
  const currentTour = steps[currentStep];

  if (!isVisible) {
    return null;
  }

  const handleNext = () => {
    if (currentStep < steps.length - 1) {
      setCurrentStep(currentStep + 1);
    } else {
      handleComplete();
    }
  };

  const handlePrevious = () => {
    if (currentStep > 0) {
      setCurrentStep(currentStep - 1);
    }
  };

  const handleComplete = () => {
    setCurrentStep(0);
    onComplete();
  };

  const handleSkip = () => {
    setCurrentStep(0);
    onComplete();
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-40 z-50 flex items-center justify-center">
      <div className="bg-white rounded-lg shadow-2xl max-w-md w-full mx-4 overflow-hidden">
        {/* Header */}
        <div className="bg-gradient-to-r from-blue-600 to-blue-700 px-6 py-4 flex items-center justify-between">
          <h2 className="text-lg font-bold text-white">{currentTour.title}</h2>
          <button
            onClick={handleSkip}
            className="text-blue-100 hover:text-white transition-colors"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        {/* Content */}
        <div className="p-6">
          <p className="text-gray-700 mb-6">{currentTour.description}</p>

          {/* Progress Dots */}
          <div className="flex justify-center gap-2 mb-6">
            {steps.map((_, index) => (
              <div
                key={index}
                className={`h-2 rounded-full transition-all ${
                  index === currentStep
                    ? 'w-6 bg-blue-600'
                    : index < currentStep
                    ? 'w-2 bg-blue-400'
                    : 'w-2 bg-gray-300'
                }`}
              />
            ))}
          </div>

          {/* Step Counter */}
          <p className="text-sm text-gray-500 text-center mb-4">
            Step {currentStep + 1} of {steps.length}
          </p>
        </div>

        {/* Footer */}
        <div className="bg-gray-50 px-6 py-4 flex items-center justify-between border-t">
          <button
            onClick={handlePrevious}
            disabled={currentStep === 0}
            className={`flex items-center gap-1 px-3 py-2 rounded-lg transition-all ${
              currentStep === 0
                ? 'text-gray-300 cursor-not-allowed'
                : 'text-gray-700 hover:bg-gray-200'
            }`}
          >
            <ChevronLeft className="w-4 h-4" />
            Previous
          </button>

          <button
            onClick={handleSkip}
            className="px-4 py-2 text-sm text-gray-600 hover:bg-gray-200 rounded-lg transition-all"
          >
            Skip Tour
          </button>

          <button
            onClick={handleNext}
            className="flex items-center gap-1 px-3 py-2 rounded-lg bg-blue-600 text-white hover:bg-blue-700 transition-all"
          >
            {currentStep === steps.length - 1 ? 'Complete' : 'Next'}
            {currentStep < steps.length - 1 && <ChevronRight className="w-4 h-4" />}
          </button>
        </div>
      </div>
    </div>
  );
};

export default OnboardingTour;
