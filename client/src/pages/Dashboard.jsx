import React, { useState, useEffect } from 'react';
import { useQuery } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import {

  DollarSign,
  ShoppingCart,
  Users,
  FileText,
  Package,
  Store,
  Activity,
  Calendar,

  Target,
  Award,
  Clock,
  CreditCard,
  Phone,
  Banknote,
  Smartphone,

  CheckCircle,
  XCircle,

  Eye,
  Filter,
  Download,
  RefreshCw,
  Settings,
  Bell,

  MoreHorizontal,
  ChevronRight,




  TrendingUp as TrendingUpIcon,
  TrendingDown as TrendingDownIcon
} from 'lucide-react';
import { dashboardAPI, shopsAPI } from '../lib/api';
import { useAuth } from '../contexts/AuthContext';
import LoadingSpinner from '../components/LoadingSpinner';
import SkeletonLoader from '../components/SkeletonLoader';
import ErrorBoundary from '../components/ErrorBoundary';
import OnboardingTour from '../components/OnboardingTour';

const Dashboard = () => {
  // Date state initialization
  const [dateFilter, setDateFilter] = useState({
    type: 'today', // today, yesterday, 7days, 30days, custom
    startDate: new Date().toISOString().split('T')[0],
    endDate: new Date().toISOString().split('T')[0],
    period: '30' // fallback for logic that needs it
  });

  const [selectedShop, setSelectedShop] = useState('all');
  const [showOnboarding, setShowOnboarding] = useState(false);

  // Helper to update date filter
  const handleFilterChange = (type) => {
    const today = new Date();
    const todayStr = today.toISOString().split('T')[0];

    let newFilter = { ...dateFilter, type };

    switch (type) {
      case 'today':
        newFilter.startDate = todayStr;
        newFilter.endDate = todayStr;
        break;
      case 'yesterday':
        const yest = new Date(today);
        yest.setDate(yest.getDate() - 1);
        newFilter.startDate = yest.toISOString().split('T')[0];
        newFilter.endDate = yest.toISOString().split('T')[0];
        break;
      case '7days':
        const last7 = new Date(today);
        last7.setDate(last7.getDate() - 6);
        newFilter.startDate = last7.toISOString().split('T')[0];
        newFilter.endDate = todayStr;
        break;
      case '30days':
        const last30 = new Date(today);
        last30.setDate(last30.getDate() - 29);
        newFilter.startDate = last30.toISOString().split('T')[0];
        newFilter.endDate = todayStr;
        break;
      case 'custom':
        // Keep existing start/end or default to today
        if (!newFilter.startDate) newFilter.startDate = todayStr;
        if (!newFilter.endDate) newFilter.endDate = todayStr;
        break;
      default:
        break;
    }
    setDateFilter(newFilter);
  };

  const navigate = useNavigate();
  const { user } = useAuth();

  // Check if user needs onboarding (first time or new user)
  useEffect(() => {
    const hasSeenOnboarding = localStorage.getItem('hasSeenOnboarding');
    if (!hasSeenOnboarding && user) {
      setShowOnboarding(true);
    }
  }, [user]);

  const handleOnboardingComplete = () => {
    localStorage.setItem('hasSeenOnboarding', 'true');
    setShowOnboarding(false);
  };

  const { data: dashboardData, isLoading, error } = useQuery({
    queryKey: ['dashboard', dateFilter.type, dateFilter.startDate, dateFilter.endDate, selectedShop, user?.role, user?.id],
    queryFn: () => dashboardAPI.getOverview({
      startDate: dateFilter.startDate,
      endDate: dateFilter.endDate,
      period: dateFilter.period,
      shop: selectedShop
    }).then(res => res.data),
    refetchInterval: 30000 // Refresh every 30 seconds
  });

  const { data: shopsData } = useQuery({
    queryKey: ['shops'],
    queryFn: () => shopsAPI.getAll().then(res => res.data),
    enabled: true
  });

  const shops = Array.isArray(shopsData?.shops) ? shopsData.shops : [];

  if (isLoading) {
    return (
      <div className="space-y-8">
        <div className="flex items-center justify-between">
          <div>
            <div className="h-8 bg-gray-200 rounded-lg w-1/4 mb-2 animate-pulse"></div>
            <div className="h-4 bg-gray-200 rounded w-1/2 animate-pulse"></div>
          </div>
          <div className="flex items-center gap-4">
            <div className="h-10 bg-gray-200 rounded w-32 animate-pulse"></div>
            <div className="h-10 bg-gray-200 rounded w-24 animate-pulse"></div>
          </div>
        </div>

        <SkeletonLoader type="card" count={4} className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6" />

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <SkeletonLoader type="chart" count={1} />
          <SkeletonLoader type="list" count={1} />
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <ErrorBoundary
        error={error}
        onRetry={() => window.location.reload()}
        onGoHome={() => navigate('/dashboard')}
        title="Dashboard Error"
        message="We couldn't load your dashboard data. This might be a temporary issue."
      />
    );
  }

  const data = dashboardData || {};

  const formatCurrency = (amount) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'RWF',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0,
    }).format(amount || 0);
  };

  const formatNumber = (num) => {
    return new Intl.NumberFormat('en-US').format(num || 0);
  };

  const getPercentageChange = (current, previous) => {
    const curr = parseFloat(current || 0);
    const prev = parseFloat(previous || 0);

    if (prev === 0) return curr > 0 ? 100 : 0;
    return ((curr - prev) / prev) * 100;
  };

  const getStatusColor = (status) => {
    switch (status) {
      case 'completed': return 'text-green-600 bg-green-50';
      case 'pending': return 'text-yellow-600 bg-yellow-50';
      case 'cancelled': return 'text-red-600 bg-red-50';
      default: return 'text-gray-600 bg-gray-50';
    }
  };

  const getStatusIcon = (status) => {
    switch (status) {
      case 'completed': return <CheckCircle className="h-4 w-4" />;
      case 'pending': return <Clock className="h-4 w-4" />;
      case 'cancelled': return <XCircle className="h-4 w-4" />;
      default: return <Activity className="h-4 w-4" />;
    }
  };

  return (
    <div className="min-h-screen bg-gray-50/50">
      {/* Onboarding Tour */}
      <OnboardingTour
        isVisible={showOnboarding}
        onComplete={handleOnboardingComplete}
        userRole={user?.role}
      />

      {/* Professional Header */}
      <div className="bg-white border-b border-gray-200 sticky top-0 z-40">
        <div className="px-6 py-4">
          <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
            <div className="flex items-center space-x-4">
              <div>
                <h1 className="text-2xl font-bold text-gray-900 tracking-tight">
                  {user?.role === 'cashier' ? 'Sales Dashboard' : 'Business Intelligence'}
                </h1>
                <p className="text-sm text-gray-600 mt-1">
                  {user?.role === 'cashier'
                    ? 'Monitor your sales performance and customer interactions'
                    : 'Comprehensive retail analytics and performance insights'
                  }
                </p>
              </div>
            </div>

            <div className="flex items-center space-x-3">
              {/* Filters */}
              <div className="flex flex-wrap items-center gap-2">
                {user?.role !== 'cashier' && (
                  <div className="relative">
                    <select
                      value={selectedShop}
                      onChange={(e) => setSelectedShop(e.target.value)}
                      className="appearance-none bg-white border border-gray-300 rounded-lg px-3 py-2 pr-8 text-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
                    >
                      <option value="all">All Locations</option>
                      {shops?.map(shop => (
                        <option key={shop.id} value={shop.id}>{shop.name}</option>
                      ))}
                    </select>
                    <Filter className="absolute right-2 top-2.5 h-4 w-4 text-gray-400 pointer-events-none" />
                  </div>
                )}

                {/* Date Range Inputs */}
                <div className="flex items-center gap-2">
                  <div className="relative">
                    <input
                      type="date"
                      value={dateFilter.startDate}
                      onChange={(e) => {
                        setDateFilter({
                          ...dateFilter,
                          type: 'custom',
                          startDate: e.target.value,
                          period: 'custom'
                        });
                      }}
                      className="bg-white border border-gray-300 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors cursor-pointer"
                      title="Start Date"
                    />
                  </div>
                  
                  <span className="text-gray-400 text-sm">to</span>
                  
                  <div className="relative">
                    <input
                      type="date"
                      value={dateFilter.endDate}
                      onChange={(e) => {
                        setDateFilter({
                          ...dateFilter,
                          type: 'custom',
                          endDate: e.target.value,
                          period: 'custom'
                        });
                      }}
                      className="bg-white border border-gray-300 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors cursor-pointer"
                      title="End Date"
                    />
                  </div>
                </div>

                {/* Quick Date Range Buttons */}
                <div className="flex items-center gap-1 bg-gray-100 rounded-lg p-1">
                  <button
                    onClick={() => handleFilterChange('today')}
                    className={`px-3 py-1 text-xs font-medium rounded transition-colors ${
                      dateFilter.type === 'today'
                        ? 'bg-white text-blue-600 shadow-sm'
                        : 'text-gray-600 hover:text-gray-900'
                    }`}
                  >
                    Today
                  </button>
                  <button
                    onClick={() => handleFilterChange('7days')}
                    className={`px-3 py-1 text-xs font-medium rounded transition-colors ${
                      dateFilter.type === '7days'
                        ? 'bg-white text-blue-600 shadow-sm'
                        : 'text-gray-600 hover:text-gray-900'
                    }`}
                  >
                    7D
                  </button>
                  <button
                    onClick={() => handleFilterChange('30days')}
                    className={`px-3 py-1 text-xs font-medium rounded transition-colors ${
                      dateFilter.type === '30days'
                        ? 'bg-white text-blue-600 shadow-sm'
                        : 'text-gray-600 hover:text-gray-900'
                    }`}
                  >
                    30D
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div className="px-4 sm:px-6 py-4 sm:py-8 space-y-6 sm:space-y-8">



        {/* Professional KPI Cards */}
        {user?.role !== 'cashier' && (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 sm:gap-6">
            {/* Revenue KPI */}
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-all duration-200">
              <div className="p-6">
                <div className="flex items-center justify-between mb-4">
                  <div className="p-3 bg-gradient-to-br from-emerald-500 to-emerald-600 rounded-xl shadow-lg">
                    <DollarSign className="h-6 w-6 text-white" />
                  </div>
                  <div className={`flex items-center px-2 py-1 rounded-full text-xs font-medium ${getPercentageChange(data.total_revenue, data.previous_revenue) >= 0
                    ? 'bg-emerald-100 text-emerald-700'
                    : 'bg-red-100 text-red-700'
                    }`}>
                    {getPercentageChange(data.total_revenue, data.previous_revenue) >= 0 ? (
                      <TrendingUpIcon className="h-3 w-3 mr-1" />
                    ) : (
                      <TrendingDownIcon className="h-3 w-3 mr-1" />
                    )}
                    {Math.abs(getPercentageChange(data.total_revenue, data.previous_revenue)).toFixed(1)}%
                  </div>
                </div>
                <div className="space-y-1">
                  <h3 className="text-sm font-medium text-gray-600">Total Revenue</h3>
                  <p className="text-2xl font-bold text-gray-900">{formatCurrency(data.total_revenue)}</p>
                  <p className="text-xs text-gray-500">vs {formatCurrency(data.previous_revenue)} previous</p>
                </div>
              </div>
              <div className="h-1 bg-gradient-to-r from-emerald-500 to-emerald-600"></div>
            </div>

            {/* Orders KPI */}
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-all duration-200">
              <div className="p-6">
                <div className="flex items-center justify-between mb-4">
                  <div className="p-3 bg-gradient-to-br from-blue-500 to-blue-600 rounded-xl shadow-lg">
                    <ShoppingCart className="h-6 w-6 text-white" />
                  </div>
                  <div className={`flex items-center px-2 py-1 rounded-full text-xs font-medium ${getPercentageChange(data.total_orders, data.previous_orders) >= 0
                    ? 'bg-emerald-100 text-emerald-700'
                    : 'bg-red-100 text-red-700'
                    }`}>
                    {getPercentageChange(data.total_orders, data.previous_orders) >= 0 ? (
                      <TrendingUpIcon className="h-3 w-3 mr-1" />
                    ) : (
                      <TrendingDownIcon className="h-3 w-3 mr-1" />
                    )}
                    {Math.abs(getPercentageChange(data.total_orders, data.previous_orders)).toFixed(1)}%
                  </div>
                </div>
                <div className="space-y-1">
                  <h3 className="text-sm font-medium text-gray-600">Total Orders</h3>
                  <p className="text-2xl font-bold text-gray-900">{formatNumber(data.total_orders)}</p>
                  <p className="text-xs text-gray-500">vs {formatNumber(data.previous_orders)} previous</p>
                </div>
              </div>
              <div className="h-1 bg-gradient-to-r from-blue-500 to-blue-600"></div>
            </div>

            {/* Expenses KPI */}
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-all duration-200">
              <div className="p-6">
                <div className="flex items-center justify-between mb-4">
                  <div className="p-3 bg-gradient-to-br from-red-500 to-red-600 rounded-xl shadow-lg">
                    <FileText className="h-6 w-6 text-white" />
                  </div>
                  <div className={`flex items-center px-2 py-1 rounded-full text-xs font-medium ${getPercentageChange(data.total_expenses, data.previous_expenses) >= 0
                    ? 'bg-red-100 text-red-700'
                    : 'bg-emerald-100 text-emerald-700'
                    }`}>
                    {getPercentageChange(data.total_expenses, data.previous_expenses) >= 0 ? (
                      <TrendingUpIcon className="h-3 w-3 mr-1" />
                    ) : (
                      <TrendingDownIcon className="h-3 w-3 mr-1" />
                    )}
                    {Math.abs(getPercentageChange(data.total_expenses, data.previous_expenses)).toFixed(1)}%
                  </div>
                </div>
                <div className="space-y-1">
                  <h3 className="text-sm font-medium text-gray-600">Total Expenses</h3>
                  <p className="text-2xl font-bold text-gray-900">{formatCurrency(data.total_expenses)}</p>
                  <p className="text-xs text-gray-500">vs {formatCurrency(data.previous_expenses)} previous</p>
                </div>
              </div>
              <div className="h-1 bg-gradient-to-r from-red-500 to-red-600"></div>
            </div>

            {/* AOV KPI */}
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-all duration-200">
              <div className="p-6">
                <div className="flex items-center justify-between mb-4">
                  <div className="p-3 bg-gradient-to-br from-amber-500 to-amber-600 rounded-xl shadow-lg">
                    <Target className="h-6 w-6 text-white" />
                  </div>
                  <div className={`flex items-center px-2 py-1 rounded-full text-xs font-medium ${getPercentageChange(data.avg_order_value, data.previous_avg_order_value) >= 0
                    ? 'bg-emerald-100 text-emerald-700'
                    : 'bg-red-100 text-red-700'
                    }`}>
                    {getPercentageChange(data.avg_order_value, data.previous_avg_order_value) >= 0 ? (
                      <TrendingUpIcon className="h-3 w-3 mr-1" />
                    ) : (
                      <TrendingDownIcon className="h-3 w-3 mr-1" />
                    )}
                    {Math.abs(getPercentageChange(data.avg_order_value, data.previous_avg_order_value)).toFixed(1)}%
                  </div>
                </div>
                <div className="space-y-1">
                  <h3 className="text-sm font-medium text-gray-600">Avg Order Value</h3>
                  <p className="text-2xl font-bold text-gray-900">{formatCurrency(data.avg_order_value)}</p>
                  <p className="text-xs text-gray-500">vs {formatCurrency(data.previous_avg_order_value)} previous</p>
                </div>
              </div>
              <div className="h-1 bg-gradient-to-r from-amber-500 to-amber-600"></div>
            </div>
          </div>
        )}

        {/* Payment Methods */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
          <div className="px-6 py-4 border-b border-gray-200">
            <h3 className="text-lg font-semibold text-gray-900">Payment Methods</h3>
            <p className="text-sm text-gray-600">Filter orders by payment type</p>
          </div>
          <div className="p-6 grid grid-cols-2 md:grid-cols-4 gap-4">
            {(() => {
              const getPaymentTotal = (keys) => {
                const stats = data.payment_method_stats || [];
                const total = stats.reduce((sum, item) => {
                  const method = (item.payment_method || '').toLowerCase();
                  if (keys.some(key => method.includes(key))) {
                    return sum + parseFloat(item.total_amount || 0);
                  }
                  return sum;
                }, 0);
                return formatCurrency(total);
              };

              return (
                <>
                  <button
                    onClick={() => navigate('/orders?payment_method=momo')}
                    className="flex flex-col items-start justify-start p-4 rounded-xl transition-all group w-full border-2 border-gray-100 hover:border-blue-200 bg-white"
                  >
                    <div className="p-0 mb-3 text-blue-600">
                      <Smartphone className="h-6 w-6" />
                    </div>
                    <span className="text-sm font-medium text-gray-500">Momo</span>
                    <h2 className="text-2xl font-bold text-blue-700 mt-1">{getPaymentTotal(['mono', 'momo'])}</h2>
                  </button>

                  <button
                    onClick={() => navigate('/orders?payment_method=cash')}
                    className="flex flex-col items-start justify-start p-4 rounded-xl transition-all group w-full border-2 border-gray-100 hover:border-purple-200 bg-white"
                  >
                    <div className="p-0 mb-3 text-purple-600">
                      <Banknote className="h-6 w-6" />
                    </div>
                    <span className="text-sm font-medium text-gray-500">Cash</span>
                    <h2 className="text-2xl font-bold text-purple-700 mt-1">{getPaymentTotal(['cash'])}</h2>
                  </button>

                  <button
                    onClick={() => navigate('/orders?payment_method=mobile')}
                    className="flex flex-col items-start justify-start p-4 rounded-xl transition-all group w-full border-2 border-gray-100 hover:border-emerald-200 bg-white"
                  >
                    <div className="p-0 mb-3 text-emerald-600">
                      <Phone className="h-6 w-6" />
                    </div>
                    <span className="text-sm font-medium text-gray-500">Mobile Money</span>
                    <h2 className="text-2xl font-bold text-emerald-700 mt-1">{getPaymentTotal(['mobile', 'money'])}</h2>
                  </button>

                  <button
                    onClick={() => navigate('/orders?payment_method=card')}
                    className="flex flex-col items-start justify-start p-4 rounded-xl transition-all group w-full border-2 border-gray-100 hover:border-amber-200 bg-white"
                  >
                    <div className="p-0 mb-3 text-amber-600">
                      <CreditCard className="h-6 w-6" />
                    </div>
                    <span className="text-sm font-medium text-gray-500">Card</span>
                    <h2 className="text-2xl font-bold text-amber-700 mt-1">{getPaymentTotal(['card', 'credit'])}</h2>
                  </button>
                </>
              );
            })()}
          </div>
        </div>



        {/* Professional Data Tables */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {/* Top Products Performance */}
          <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
            <div className="px-6 py-4 border-b border-gray-200">
              <div className="flex items-center justify-between">
                <div>
                  <h3 className="text-lg font-semibold text-gray-900">Top Products</h3>
                  <p className="text-sm text-gray-600">Best performing items this period</p>
                </div>
                <button
                  onClick={() => navigate('/products')}
                  className="flex items-center space-x-1 text-sm text-blue-600 hover:text-blue-700 font-medium"
                >
                  <span>View All</span>
                  <ChevronRight className="h-4 w-4" />
                </button>
              </div>
            </div>

            <div className="divide-y divide-gray-200">
              {data.top_products?.slice(0, 5).map((product, index) => (
                <div key={index} className="px-6 py-4 hover:bg-gray-50 transition-colors">
                  <div className="flex items-center justify-between">
                    <div className="flex items-center space-x-4">
                      <div className="w-10 h-10 bg-gradient-to-br from-blue-500 to-purple-600 rounded-lg flex items-center justify-center text-white font-bold text-sm shadow-lg">
                        {index + 1}
                      </div>
                      <div>
                        <h4 className="font-medium text-gray-900">{product.name}</h4>
                        <p className="text-sm text-gray-500">SKU: {product.sku}</p>
                      </div>
                    </div>
                    <div className="text-right">
                      <p className="font-semibold text-gray-900">{formatNumber(product.quantity_sold)}</p>
                      <p className="text-sm text-gray-500">{formatCurrency(product.revenue)}</p>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>

          {/* Payment Methods Analysis */}
          <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
            <div className="px-6 py-4 border-b border-gray-200">
              <div className="flex items-center justify-between">
                <div>
                  <h3 className="text-lg font-semibold text-gray-900">Payment Methods</h3>
                  <p className="text-sm text-gray-600">Revenue distribution by payment type</p>
                </div>
                <CreditCard className="h-5 w-5 text-gray-400" />
              </div>
            </div>

            <div className="p-6">
              <div className="space-y-4">
                {(() => {
                  const stats = data.payment_method_stats || [];
                  const totalRevenue = stats.reduce((sum, item) => sum + parseFloat(item.total_amount || 0), 0);

                  if (stats.length === 0) {
                    return <div className="text-center py-4 text-gray-500">No payment data available</div>;
                  }

                  // Sort by amount desc
                  const sortedStats = [...stats].sort((a, b) => parseFloat(b.total_amount) - parseFloat(a.total_amount));

                  return sortedStats.map((item, index) => {
                    const amount = parseFloat(item.total_amount || 0);
                    const percentage = totalRevenue > 0 ? Math.round((amount / totalRevenue) * 100) : 0;
                    const method = (item.payment_method || 'Other').toLowerCase();

                    let color = 'from-gray-500 to-gray-600';
                    if (method.includes('card') || method.includes('credit')) color = 'from-blue-500 to-blue-600';
                    else if (method.includes('cash')) color = 'from-emerald-500 to-emerald-600';
                    else if (method.includes('mobile') || method.includes('momo') || method.includes('mpesa')) color = 'from-purple-500 to-purple-600';
                    else if (method.includes('bank') || method.includes('transfer')) color = 'from-amber-500 to-amber-600';

                    return (
                      <div key={index} className="space-y-2">
                        <div className="flex items-center justify-between">
                          <div className="flex items-center space-x-3">
                            <div className={`w-3 h-3 bg-gradient-to-r ${color} rounded-full`}></div>
                            <span className="text-sm font-medium text-gray-900 capitalize">{item.payment_method || 'Unknown'}</span>
                          </div>
                          <div className="text-right">
                            <span className="text-sm font-semibold text-gray-900">{percentage}%</span>
                            <p className="text-xs text-gray-500">{formatCurrency(amount)}</p>
                          </div>
                        </div>
                        <div className="w-full bg-gray-200 rounded-full h-2">
                          <div
                            className={`bg-gradient-to-r ${color} h-2 rounded-full transition-all duration-500`}
                            style={{ width: `${percentage}%` }}
                          ></div>
                        </div>
                      </div>
                    );
                  });
                })()}
              </div>
            </div>
          </div>
        </div>

        {/* Professional Activity Feed */}


        {/* Professional Footer Stats */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <div className="bg-gradient-to-br from-blue-500 to-blue-600 rounded-xl p-6 text-white shadow-lg">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm opacity-90 font-medium">Active Locations</p>
                <p className="text-3xl font-bold mt-2">{data.active_shops || 0}</p>
                <p className="text-xs opacity-75 mt-1">Operating stores</p>
              </div>
              <Store className="h-12 w-12 opacity-80" />
            </div>
          </div>

          <div className="bg-gradient-to-br from-emerald-500 to-emerald-600 rounded-xl p-6 text-white shadow-lg">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm opacity-90 font-medium">Product Catalog</p>
                <p className="text-3xl font-bold mt-2">{formatNumber(data.total_products || 0)}</p>
                <p className="text-xs opacity-75 mt-1">Available items</p>
              </div>
              <Package className="h-12 w-12 opacity-80" />
            </div>
          </div>

          <div className="bg-gradient-to-br from-purple-500 to-purple-600 rounded-xl p-6 text-white shadow-lg">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm opacity-90 font-medium">Loyalty Members</p>
                <p className="text-3xl font-bold mt-2">{formatNumber(data.loyalty_members || 0)}</p>
                <p className="text-xs opacity-75 mt-1">Active members</p>
              </div>
              <Award className="h-12 w-12 opacity-80" />
            </div>
          </div>

          <div className="bg-gradient-to-br from-amber-500 to-amber-600 rounded-xl p-6 text-white shadow-lg">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm opacity-90 font-medium">Pending Orders</p>
                <p className="text-3xl font-bold mt-2">{formatNumber(data.pending_orders || 0)}</p>
                <p className="text-xs opacity-75 mt-1">Awaiting processing</p>
              </div>
              <Clock className="h-12 w-12 opacity-80" />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard; 