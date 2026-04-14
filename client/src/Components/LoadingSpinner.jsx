const LoadingSpinner = () => {
  return (
    <div className="flex items-center justify-center p-6">
      <div className="h-8 w-8 animate-spin rounded-full border-2 border-gray-300 border-t-primary-600" />
    </div>
  );
};

export default LoadingSpinner;
