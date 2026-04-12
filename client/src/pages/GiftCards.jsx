import React, { useState, useRef, useEffect } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Plus,
  Search,
  CreditCard,
  Gift,
  X,
  ChevronDown,
  User
} from 'lucide-react';
import { api, giftCardAPI } from '../lib/api';
import toast from 'react-hot-toast';

const GiftCards = () => {
  const [showPurchaseModal, setShowPurchaseModal] = useState(false);
  const [showBalanceModal, setShowBalanceModal] = useState(false);
  const [showCreateCustomerModal, setShowCreateCustomerModal] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize] = useState(20);

  const queryClient = useQueryClient();

  // Form states
  const [purchaseForm, setPurchaseForm] = useState({
    initial_value: '',
    payment_method: 'cash',
    shop_id: '',
    customer_id: '',
    purchaser_name: '', // Who is buying the card (e.g., Serge)
    recipient_name: '' // Who will use the card (e.g., Ian)
  });

  const [balanceForm, setBalanceForm] = useState({
    card_number: ''
  });

  // New customer form state
  const [newCustomerForm, setNewCustomerForm] = useState({
    first_name: '',
    last_name: '',
    email: '',
    phone: '',
    address: '',
    city: '',
    birthday: ''
  });

  // Customer search state
  const [customerSearch, setCustomerSearch] = useState('');
  const [showCustomerDropdown, setShowCustomerDropdown] = useState(false);
  const [selectedCustomer, setSelectedCustomer] = useState(null);
  const customerDropdownRef = useRef(null);

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (customerDropdownRef.current && !customerDropdownRef.current.contains(event.target)) {
        setShowCustomerDropdown(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, []);

  // Fetch shops
  const { data: shopsData } = useQuery({
    queryKey: ['shops'],
    queryFn: () => api.get('/shops').then(res => res.data),
    onError: (error) => {
      console.error('Error fetching shops:', error);
    }
  });

  const shops = shopsData?.shops || [];

  // Fetch customers
  const { data: customersData, isLoading: customersLoading, error: customersError } = useQuery({
    queryKey: ['customers'],
    queryFn: () => api.get('/customers').then(res => res.data),
    onError: (error) => {
      console.error('Error fetching customers:', error);
    }
  });

  const customers = customersData?.customers || [];

  // Filter customers based on search
  const filteredCustomers = customers.filter(customer => {
    if (!customerSearch) return true; // Show all customers when no search term
    const fullName = `${customer.first_name} ${customer.last_name}`.toLowerCase();
    return fullName.includes(customerSearch.toLowerCase());
  });

  // Handle customer selection
  const handleCustomerSelect = (customer) => {
    setSelectedCustomer(customer);
    setCustomerSearch(`${customer.first_name} ${customer.last_name}`);
    setPurchaseForm(prev => ({ ...prev, customer_id: customer.id }));
    setShowCustomerDropdown(false);
  };

  // Handle customer search input
  const handleCustomerSearchChange = (value) => {
    setCustomerSearch(value);
    setShowCustomerDropdown(true);
    
    // Clear selection if search is cleared
    if (!value) {
      setSelectedCustomer(null);
      setPurchaseForm(prev => ({ ...prev, customer_id: '' }));
    }
  };

  // Handle input focus - show all customers
  const handleCustomerInputFocus = () => {
    setShowCustomerDropdown(true);
  };

  // Toggle dropdown visibility
  const toggleCustomerDropdown = () => {
    setShowCustomerDropdown(!showCustomerDropdown);
  };

  // Handle create new customer
  const handleCreateNewCustomer = () => {
    setShowCreateCustomerModal(true);
    setShowCustomerDropdown(false);
  };

  // Reset new customer form
  const resetNewCustomerForm = () => {
    setNewCustomerForm({
      first_name: '',
      last_name: '',
      email: '',
      phone: '',
      address: '',
      city: '',
      birthday: ''
    });
  };

  // Fetch gift cards
  const { data: giftCardsResponse, isLoading, error } = useQuery({
    queryKey: ['gift-cards', currentPage, pageSize, searchTerm],
    queryFn: () => {
      const params = new URLSearchParams({
        page: currentPage.toString(),
        limit: pageSize.toString()
      });
      if (searchTerm) params.append('search', searchTerm);
      return api.get(`/gift-cards?${params}`).then(res => res.data);
    },
    onError: (error) => {
      console.error('Error fetching gift cards:', error);
      toast.error('Failed to load gift cards');
    }
  });

  // Purchase gift card mutation
  const purchaseGiftCardMutation = useMutation({
    mutationFn: (data) => giftCardAPI.purchase(data),
    onSuccess: (response) => {
      toast.success('Gift card purchased successfully!');
      setShowPurchaseModal(false);
      resetPurchaseForm();
      queryClient.invalidateQueries(['gift-cards']);
    },
    onError: (error) => {
      console.error('Gift card purchase error:', error);
      console.error('Error response:', error.response?.data);
      toast.error(error.response?.data?.error || 'Failed to purchase gift card');
    }
  });

  // Check balance mutation
  const checkBalanceMutation = useMutation({
    mutationFn: (cardNumber) => api.get(`/gift-cards/${cardNumber}/balance`),
    onSuccess: (response) => {
      const { balance, status } = response.data;
      toast.success(`Balance: ${Math.round(balance)} RWF (${status})`, { duration: 5000 });
      setBalanceForm({ card_number: '' });
    },
    onError: (error) => {
      toast.error(error.response?.data?.error || 'Failed to check balance');
    }
  });

  // Create customer mutation
  const createCustomerMutation = useMutation({
    mutationFn: (data) => api.post('/customers', data),
    onSuccess: (response) => {
      const newCustomer = response.data.customer;
      toast.success('Customer created successfully!');
      
      // Select the newly created customer
      handleCustomerSelect(newCustomer);
      
      // Close modal and reset form
      setShowCreateCustomerModal(false);
      resetNewCustomerForm();
      
      // Refresh customers list
      queryClient.invalidateQueries(['customers']);
    },
    onError: (error) => {
      console.error('Create customer error:', error);
      console.error('Error response:', error.response?.data);
      
      // Show detailed validation errors
      if (error.response?.data?.errors) {
        const errorMessages = error.response.data.errors.map(err => err.msg).join(', ');
        toast.error(`Validation failed: ${errorMessages}`);
      } else if (error.response?.data?.message) {
        toast.error(error.response.data.message);
      } else {
        toast.error(error.response?.data?.error || 'Failed to create customer');
      }
    }
  });

  const resetPurchaseForm = () => {
    setPurchaseForm({
      initial_value: '',
      payment_method: 'cash',
      shop_id: '',
      customer_id: '',
      purchaser_name: '',
      recipient_name: ''
    });
    setCustomerSearch('');
    setSelectedCustomer(null);
    setShowCustomerDropdown(false);
  };

  const handlePurchaseSubmit = (e) => {
    e.preventDefault();
    
    // Validation
    if (!purchaseForm.initial_value || !purchaseForm.shop_id || !purchaseForm.purchaser_name || !purchaseForm.recipient_name) {
      toast.error('Please fill in all required fields');
      return;
    }

    if (parseInt(purchaseForm.initial_value) <= 0) {
      toast.error('Gift card value must be greater than 0');
      return;
    }

    const data = {
      initial_value: parseInt(purchaseForm.initial_value), // RWF is typically whole numbers
      payment_method: purchaseForm.payment_method,
      payment_amount: parseInt(purchaseForm.initial_value), // Payment equals card value
      shop_id: purchaseForm.shop_id,
      recipient_name: purchaseForm.recipient_name,
      purchaser_name: purchaseForm.purchaser_name,
      is_digital: false // Always physical cards
    };

    // Only include optional fields if they have values
    if (purchaseForm.customer_id && purchaseForm.customer_id.trim()) {
      data.customer_id = purchaseForm.customer_id;
    }

    console.log('Submitting gift card data:', data);
    purchaseGiftCardMutation.mutate(data);
  };

  const handleBalanceCheck = (e) => {
    e.preventDefault();
    
    if (!balanceForm.card_number || balanceForm.card_number.length !== 16) {
      toast.error('Please enter a valid 16-digit card number');
      return;
    }

    checkBalanceMutation.mutate(balanceForm.card_number);
  };

  const handleCreateCustomerSubmit = (e) => {
    e.preventDefault();
    
    // Validation
    if (!newCustomerForm.first_name || !newCustomerForm.last_name) {
      toast.error('First name and last name are required');
      return;
    }

    // Map form data to API expected format
    const customerData = {
      firstName: newCustomerForm.first_name,
      lastName: newCustomerForm.last_name,
      email: newCustomerForm.email || undefined,
      phone: newCustomerForm.phone || undefined,
      address: newCustomerForm.address || undefined,
      city: newCustomerForm.city || undefined,
      birthday: newCustomerForm.birthday || undefined
    };

    // Remove undefined values to avoid sending empty strings
    Object.keys(customerData).forEach(key => {
      if (customerData[key] === undefined || customerData[key] === '') {
        delete customerData[key];
      }
    });

    console.log('Creating customer with data:', customerData);
    createCustomerMutation.mutate(customerData);
  };

  const handleInputChange = (field, value) => {
    setPurchaseForm(prev => ({ ...prev, [field]: value }));
  };

  const giftCards = giftCardsResponse?.gift_cards || [];
  const pagination = giftCardsResponse?.pagination || {};

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Gift Cards</h1>
          <p className="text-gray-600">Manage gift card sales and redemptions</p>
        </div>
        
        <div className="flex flex-col sm:flex-row gap-2">
          <button
            onClick={() => setShowBalanceModal(true)}
            className="inline-flex items-center px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors"
          >
            <Search className="h-4 w-4 mr-2" />
            Check Balance
          </button>
          
          <button
            onClick={() => setShowPurchaseModal(true)}
            className="inline-flex items-center px-4 py-2 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-lg hover:from-purple-700 hover:to-blue-700 transition-colors"
          >
            <Plus className="h-4 w-4 mr-2" />
            Sell Gift Card
          </button>
        </div>
      </div>

      {/* Search */}
      <div className="flex items-center space-x-4">
        <div className="flex-1 relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4" />
          <input
            type="text"
            placeholder="Search gift cards..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
          />
        </div>
      </div>

      {/* Gift Cards List */}
      <div className="bg-white rounded-lg shadow overflow-hidden">
        {error && (
          <div className="p-4 bg-red-50 border-l-4 border-red-400">
            <div className="flex">
              <div className="ml-3">
                <p className="text-sm text-red-700">
                  Failed to load gift cards. Please try refreshing the page.
                </p>
              </div>
            </div>
          </div>
        )}
        
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Card Number
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Value
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Balance
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Purchaser
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Recipient
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Status
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Purchased
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Expires
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {isLoading ? (
                <tr>
                  <td colSpan="8" className="px-6 py-4 text-center text-gray-500">
                    Loading gift cards...
                  </td>
                </tr>
              ) : giftCards.length === 0 ? (
                <tr>
                  <td colSpan="8" className="px-6 py-4 text-center text-gray-500">
                    No gift cards found
                  </td>
                </tr>
              ) : (
                giftCards.map((card) => (
                  <tr key={card.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="flex items-center">
                        <CreditCard className="h-4 w-4 text-gray-400 mr-2" />
                        <span className="font-mono text-sm">{card.card_number}</span>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {Math.round(card.initial_value)} RWF
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {Math.round(card.current_balance)} RWF
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {card.purchaser_name || 'N/A'}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {card.recipient_name || 'N/A'}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
                        card.status === 'active' ? 'bg-green-100 text-green-800' :
                        card.status === 'redeemed' ? 'bg-gray-100 text-gray-800' :
                        card.status === 'expired' ? 'bg-red-100 text-red-800' :
                        'bg-yellow-100 text-yellow-800'
                      }`}>
                        {card.status}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {new Date(card.purchased_at).toLocaleDateString()}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {new Date(card.expires_at).toLocaleDateString()}
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>

        {/* Pagination */}
        {pagination.total_pages > 1 && (
          <div className="bg-white px-4 py-3 flex items-center justify-between border-t border-gray-200 sm:px-6">
            <div className="flex-1 flex justify-between sm:hidden">
              <button
                onClick={() => setCurrentPage(prev => Math.max(1, prev - 1))}
                disabled={!pagination.has_prev}
                className="relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
              >
                Previous
              </button>
              <button
                onClick={() => setCurrentPage(prev => Math.min(pagination.total_pages, prev + 1))}
                disabled={!pagination.has_next}
                className="ml-3 relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
              >
                Next
              </button>
            </div>
            <div className="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
              <div>
                <p className="text-sm text-gray-700">
                  Showing page <span className="font-medium">{pagination.current_page}</span> of{' '}
                  <span className="font-medium">{pagination.total_pages}</span>
                </p>
              </div>
              <div>
                <nav className="relative z-0 inline-flex rounded-md shadow-sm -space-x-px">
                  <button
                    onClick={() => setCurrentPage(prev => Math.max(1, prev - 1))}
                    disabled={!pagination.has_prev}
                    className="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
                  >
                    Previous
                  </button>
                  <button
                    onClick={() => setCurrentPage(prev => Math.min(pagination.total_pages, prev + 1))}
                    disabled={!pagination.has_next}
                    className="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
                  >
                    Next
                  </button>
                </nav>
              </div>
            </div>
          </div>
        )}
      </div>

      {/* Purchase Gift Card Modal - SIMPLIFIED */}
      {showPurchaseModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between p-6 border-b">
              <h3 className="text-xl font-semibold text-gray-900">Sell Gift Card</h3>
              <button
                onClick={() => setShowPurchaseModal(false)}
                className="text-gray-400 hover:text-gray-600"
              >
                <X className="h-6 w-6" />
              </button>
            </div>

            <form onSubmit={handlePurchaseSubmit} className="p-6 space-y-6">
              {/* Gift Card Value */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Gift Card Value *
                </label>
                <p className="text-xs text-gray-500 mb-2">Amount that will be loaded onto the gift card (RWF)</p>
                <div className="relative">
                  <span className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 text-sm font-medium">RWF</span>
                  <input
                    type="number"
                    min="100"
                    max="10000000"
                    step="1"
                    value={purchaseForm.initial_value}
                    onChange={(e) => handleInputChange('initial_value', e.target.value)}
                    className="w-full pl-12 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
                    placeholder="0"
                    required
                  />
                </div>
              </div>

              {/* Payment & Shop */}
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Payment Method *
                  </label>
                  <select
                    value={purchaseForm.payment_method}
                    onChange={(e) => handleInputChange('payment_method', e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
                    required
                  >
                    <option value="cash">Cash</option>
                    <option value="card">Card</option>
                    <option value="gift_card">Gift Card</option>
                    <option value="mobile_money">Mobile Money</option>
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Shop *
                  </label>
                  <select
                    value={purchaseForm.shop_id}
                    onChange={(e) => handleInputChange('shop_id', e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
                    required
                  >
                    <option value="">Select shop</option>
                    {shops.length > 0 ? (
                      shops.map(shop => (
                        <option key={shop.id} value={shop.id}>{shop.name}</option>
                      ))
                    ) : (
                      <option disabled>Loading shops...</option>
                    )}
                  </select>
                </div>
              </div>

              {/* Customer Search */}
              <div className="relative" ref={customerDropdownRef}>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Customer (Optional)
                </label>
                <div className="relative">
                  <div className="flex">
                    <input
                      type="text"
                      value={customerSearch}
                      onChange={(e) => handleCustomerSearchChange(e.target.value)}
                      onFocus={handleCustomerInputFocus}
                      className="flex-1 px-3 py-2 border border-gray-300 rounded-l-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
                      placeholder="Search customer by name..."
                    />
                    <button
                      type="button"
                      onClick={toggleCustomerDropdown}
                      className="px-3 py-2 border border-l-0 border-gray-300 rounded-r-lg bg-gray-50 hover:bg-gray-100 focus:ring-2 focus:ring-purple-500 focus:border-transparent"
                    >
                      <ChevronDown className={`h-4 w-4 text-gray-500 transition-transform ${showCustomerDropdown ? 'rotate-180' : ''}`} />
                    </button>
                  </div>
                  
                  {/* Customer Dropdown */}
                  {showCustomerDropdown && (
                    <div className="absolute z-10 w-full mt-1 bg-white border border-gray-300 rounded-lg shadow-lg max-h-60 overflow-y-auto">
                      {customersLoading ? (
                        <div className="px-3 py-2 text-gray-500">Loading customers...</div>
                      ) : customersError ? (
                        <div className="px-3 py-2 text-red-500">Error loading customers</div>
                      ) : (
                        <div>
                          {/* Always show create new customer option at the top */}
                          <div
                            onClick={handleCreateNewCustomer}
                            className="px-3 py-2 hover:bg-primary-50 cursor-pointer text-primary-600 flex items-center gap-2 border-b border-gray-200 bg-primary-25"
                          >
                            <Plus className="h-4 w-4" />
                            <span>Create new customer</span>
                          </div>
                          
                          {/* Show customers if any */}
                          {filteredCustomers.length > 0 && (
                            <>
                              {/* Show search info if filtering */}
                              {customerSearch && (
                                <div className="px-3 py-2 bg-gray-50 border-b text-sm text-gray-600">
                                  {filteredCustomers.length} customer{filteredCustomers.length !== 1 ? 's' : ''} found
                                </div>
                              )}
                              {filteredCustomers.map(customer => (
                                <div
                                  key={customer.id}
                                  onClick={() => handleCustomerSelect(customer)}
                                  className="px-3 py-2 hover:bg-gray-100 cursor-pointer border-b border-gray-100 last:border-b-0"
                                >
                                  <div className="font-medium">{customer.first_name} {customer.last_name}</div>
                                  <div className="flex items-center gap-4 text-sm text-gray-500">
                                    {customer.email && <span>{customer.email}</span>}
                                    {customer.phone && <span>{customer.phone}</span>}
                                    {customer.loyalty_tier && (
                                      <span className="px-2 py-1 bg-primary-100 text-primary-800 rounded-full text-xs">
                                        {customer.loyalty_tier}
                                      </span>
                                    )}
                                  </div>
                                </div>
                              ))}
                            </>
                          )}
                          
                          {/* Show no results message if searching and no results */}
                          {customerSearch && filteredCustomers.length === 0 && (
                            <div className="px-3 py-2 text-gray-500">
                              No customers found matching your search
                            </div>
                          )}
                          
                          {/* Show no customers message if no customers at all */}
                          {!customerSearch && filteredCustomers.length === 0 && (
                            <div className="px-3 py-2 text-gray-500">
                              No customers available
                            </div>
                          )}
                        </div>
                      )}
                    </div>
                  )}
                  
                  {/* Clear button */}
                  {customerSearch && (
                    <button
                      type="button"
                      onClick={() => {
                        setCustomerSearch('');
                        setSelectedCustomer(null);
                        setPurchaseForm(prev => ({ ...prev, customer_id: '' }));
                        setShowCustomerDropdown(false);
                      }}
                      className="absolute right-12 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-gray-600"
                    >
                      <X className="h-4 w-4" />
                    </button>
                  )}
                </div>
                
                {/* Selected customer info */}
                {selectedCustomer && (
                  <div className="mt-2 p-3 bg-green-50 border border-green-200 rounded-lg">
                    <div className="flex items-center justify-between">
                      <div>
                        <span className="text-green-800 font-medium">
                          Selected: {selectedCustomer.first_name} {selectedCustomer.last_name}
                        </span>
                        <div className="flex items-center gap-2 mt-1">
                          {selectedCustomer.loyalty_tier && (
                            <span className="px-2 py-1 bg-primary-100 text-primary-800 rounded-full text-xs">
                              {selectedCustomer.loyalty_tier} member
                            </span>
                          )}
                          {selectedCustomer.loyalty_points > 0 && (
                            <span className="text-sm text-green-600">
                              {selectedCustomer.loyalty_points} points
                            </span>
                          )}
                        </div>
                      </div>
                      <button
                        type="button"
                        onClick={() => {
                          setCustomerSearch('');
                          setSelectedCustomer(null);
                          setPurchaseForm(prev => ({ ...prev, customer_id: '' }));
                        }}
                        className="text-green-600 hover:text-green-800"
                      >
                        <X className="h-4 w-4" />
                      </button>
                    </div>
                  </div>
                )}
              </div>

              {/* Purchaser Name */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Purchaser Name *
                </label>
                <p className="text-xs text-gray-500 mb-2">Name of person buying this gift card (e.g., Serge)</p>
                <input
                  type="text"
                  value={purchaseForm.purchaser_name}
                  onChange={(e) => handleInputChange('purchaser_name', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
                  placeholder="Enter purchaser's name"
                  required
                />
              </div>

              {/* Recipient Name */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Recipient Name *
                </label>
                <p className="text-xs text-gray-500 mb-2">Name of person who will use this gift card (e.g., Ian)</p>
                <input
                  type="text"
                  value={purchaseForm.recipient_name}
                  onChange={(e) => handleInputChange('recipient_name', e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
                  placeholder="Enter recipient's name"
                  required
                />
              </div>

              {/* Actions */}
              <div className="flex justify-end space-x-3 pt-6 border-t">
                <button
                  type="button"
                  onClick={() => setShowPurchaseModal(false)}
                  className="px-4 py-2 text-gray-700 bg-gray-100 hover:bg-gray-200 rounded-lg transition-colors"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={purchaseGiftCardMutation.isLoading}
                  className="px-6 py-2 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-lg hover:from-purple-700 hover:to-blue-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center"
                >
                  {purchaseGiftCardMutation.isLoading ? (
                    <>
                      <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                      Processing...
                    </>
                  ) : (
                    <>
                      <Gift className="h-4 w-4 mr-2" />
                      Complete Purchase
                    </>
                  )}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Balance Check Modal */}
      {showBalanceModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-md w-full">
            <div className="flex items-center justify-between p-6 border-b">
              <h3 className="text-xl font-semibold text-gray-900">Check Gift Card Balance</h3>
              <button
                onClick={() => setShowBalanceModal(false)}
                className="text-gray-400 hover:text-gray-600"
              >
                <X className="h-6 w-6" />
              </button>
            </div>

            <form onSubmit={handleBalanceCheck} className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Gift Card Number
                </label>
                <input
                  type="text"
                  value={balanceForm.card_number}
                  onChange={(e) => setBalanceForm({ card_number: e.target.value.replace(/\D/g, '').slice(0, 16) })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent font-mono"
                  placeholder="1234567890123456"
                  maxLength={16}
                  required
                />
                <p className="mt-1 text-xs text-gray-500">Enter the 16-digit card number</p>
              </div>

              <div className="flex justify-end space-x-3 pt-4 border-t">
                <button
                  type="button"
                  onClick={() => setShowBalanceModal(false)}
                  className="px-4 py-2 text-gray-700 bg-gray-100 hover:bg-gray-200 rounded-lg transition-colors"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={checkBalanceMutation.isLoading}
                  className="px-6 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center"
                >
                  {checkBalanceMutation.isLoading ? (
                    <>
                      <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                      Checking...
                    </>
                  ) : (
                    <>
                      <Search className="h-4 w-4 mr-2" />
                      Check Balance
                    </>
                  )}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Create Customer Modal */}
      {showCreateCustomerModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-md w-full max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between p-6 border-b">
              <h3 className="text-xl font-semibold text-gray-900">Create New Customer</h3>
              <button
                onClick={() => {
                  setShowCreateCustomerModal(false);
                  resetNewCustomerForm();
                }}
                className="text-gray-400 hover:text-gray-600"
              >
                <X className="h-6 w-6" />
              </button>
            </div>

            <form onSubmit={handleCreateCustomerSubmit} className="p-6 space-y-4">
              {/* Name Fields */}
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    First Name *
                  </label>
                  <input
                    type="text"
                    value={newCustomerForm.first_name}
                    onChange={(e) => setNewCustomerForm(prev => ({ ...prev, first_name: e.target.value }))}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    placeholder="Enter first name"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Last Name *
                  </label>
                  <input
                    type="text"
                    value={newCustomerForm.last_name}
                    onChange={(e) => setNewCustomerForm(prev => ({ ...prev, last_name: e.target.value }))}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    placeholder="Enter last name"
                    required
                  />
                </div>
              </div>

              {/* Contact Fields */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Email
                </label>
                <input
                  type="email"
                  value={newCustomerForm.email}
                  onChange={(e) => setNewCustomerForm(prev => ({ ...prev, email: e.target.value }))}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="Enter email address"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Phone
                </label>
                <input
                  type="tel"
                  value={newCustomerForm.phone}
                  onChange={(e) => setNewCustomerForm(prev => ({ ...prev, phone: e.target.value }))}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="Enter phone number"
                />
              </div>

              {/* Address Fields */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Address
                </label>
                <input
                  type="text"
                  value={newCustomerForm.address}
                  onChange={(e) => setNewCustomerForm(prev => ({ ...prev, address: e.target.value }))}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="Enter address"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  City
                </label>
                <input
                  type="text"
                  value={newCustomerForm.city}
                  onChange={(e) => setNewCustomerForm(prev => ({ ...prev, city: e.target.value }))}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="Enter city"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Birthday
                </label>
                <input
                  type="date"
                  value={newCustomerForm.birthday}
                  onChange={(e) => setNewCustomerForm(prev => ({ ...prev, birthday: e.target.value }))}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>

              {/* Actions */}
              <div className="flex justify-end space-x-3 pt-6 border-t">
                <button
                  type="button"
                  onClick={() => {
                    setShowCreateCustomerModal(false);
                    resetNewCustomerForm();
                  }}
                  className="px-4 py-2 text-gray-700 bg-gray-100 hover:bg-gray-200 rounded-lg transition-colors"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={createCustomerMutation.isLoading}
                  className="px-6 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center"
                >
                  {createCustomerMutation.isLoading ? (
                    <>
                      <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                      Creating...
                    </>
                  ) : (
                    <>
                      <User className="h-4 w-4 mr-2" />
                      Create Customer
                    </>
                  )}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default GiftCards;