import React from 'react';

const UrutiLaRoseLogo = ({ className = '', width = 40, height = 40 }) => {
  return (
    <div className={`flex items-center justify-center ${className}`}>
      <svg
        width={width}
        height={height}
        viewBox="0 0 40 40"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <rect width="40" height="40" rx="8" fill="#2563eb" />
        <text
          x="50%"
          y="50%"
          dominantBaseline="middle"
          textAnchor="middle"
          fontSize="20"
          fontWeight="bold"
          fill="white"
          fontFamily="Arial, sans-serif"
        >
          POS
        </text>
      </svg>
    </div>
  );
};

export default UrutiLaRoseLogo;
