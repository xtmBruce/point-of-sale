import React from 'react';

const PageLayout = ({ 
  title, 
  subtitle, 
  children, 
  actions = null,
  breadcrumbs = [] 
}) => {
  return (
    <div className="min-h-screen bg-gray-50">
      {/* Breadcrumbs */}
      {breadcrumbs.length > 0 && (
        <div className="bg-white border-b border-gray-200 px-6 py-3">
          <nav className="flex gap-2">
            {breadcrumbs.map((crumb, index) => (
              <React.Fragment key={index}>
                {index > 0 && <span className="text-gray-400">/</span>}
                <span className={index === breadcrumbs.length - 1 ? 'text-gray-600' : 'text-blue-600 cursor-pointer hover:underline'}>
                  {crumb}
                </span>
              </React.Fragment>
            ))}
          </nav>
        </div>
      )}

      {/* Header */}
      <div className="bg-white border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-6 py-8">
          <div className="flex items-start justify-between gap-4">
            <div>
              <h1 className="text-3xl font-bold text-gray-900">{title}</h1>
              {subtitle && <p className="mt-2 text-gray-600">{subtitle}</p>}
            </div>
            {actions && <div className="flex gap-2">{actions}</div>}
          </div>
        </div>
      </div>

      {/* Content */}
      <div className="max-w-7xl mx-auto px-6 py-8">
        {children}
      </div>
    </div>
  );
};

export default PageLayout;
