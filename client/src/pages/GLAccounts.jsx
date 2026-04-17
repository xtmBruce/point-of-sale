import React from 'react';
<<<<<<< HEAD
import GLAccountManagement from '../Components/GLAccountManagement';
=======
import GLAccountManagement from '../components/GLAccountManagement';
>>>>>>> d10bc65ca0e2784567c21698cb5ed72221dedbd3

const GLAccounts = () => {
  return (
    <div className="space-y-6">
      <div className="bg-white rounded-lg shadow-sm border border-gray-200">
        <div className="px-6 py-4 border-b border-gray-200">
          <h1 className="text-2xl font-semibold text-gray-900">General Ledger Accounts</h1>
          <p className="text-gray-600 mt-1">Manage your chart of accounts and account structure</p>
        </div>
        <div className="p-6">
          <GLAccountManagement />
        </div>
      </div>
    </div>
  );
};

export default GLAccounts;

