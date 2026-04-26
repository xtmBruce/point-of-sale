import React from 'react';

const SkeletonLoader = ({ count = 3, height = 'h-12', className = '' }) => {
  return (
    <div className={`space-y-4 ${className}`}>
      {Array.from({ length: count }).map((_, i) => (
        <div key={i} className={`${height} bg-gray-200 rounded-lg animate-pulse`} />
      ))}
    </div>
  );
};

export default SkeletonLoader;
