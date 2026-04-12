import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import { SidebarProvider, useSidebar } from './contexts/SidebarContext';
import Navigation from './components/Navigation';
import TopNavigation from './components/TopNavigation';
import RoleBasedRoute from './components/RoleBasedRoute';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import Products from './pages/Products';
import Customers from './pages/Customers';
import Orders from './pages/Orders';
import CreateOrder from './pages/CreateOrder';
import OrderEdit from './pages/OrderEdit';
import SalesDashboard from './pages/SalesDashboard';
import Shops from './pages/Shops';
import Perfumes from './pages/Perfumes';

import Production from './pages/Production';
import Procurement from './pages/Procurement';
import Notifications from './pages/Notifications';
import Integrations from './pages/Integrations';
import Inventory from './pages/Inventory';
import Stocks from './pages/Stocks';
import Loyalty from './pages/Loyalty';
import Layaway from './pages/Layaway';
import Expenses from './pages/Expenses';
import GLAccounts from './pages/GLAccounts';
import Analytics from './pages/Analytics';
import IncomeReport from './pages/IncomeReport';
import Settings from './pages/Settings';
import Categories from './pages/Categories';
import Brands from './pages/Brands';
import Discounts from './pages/Discounts';
import PricingManagement from './pages/PricingManagement';
import GiftCards from './pages/GiftCards';
import './index.css';

// Create a client for React Query
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      refetchOnWindowFocus: false,
      staleTime: 5 * 60 * 1000, // 5 minutes
    },
  },
});

// Protected Route Component
const ProtectedRoute = ({ children }) => children;

// Layout Component
const Layout = ({ children }) => {
  const { isDesktopSidebarOpen } = useSidebar();

  return (
    <div className="min-h-screen bg-gray-50">
      <Navigation />
      <div className={`transition-all duration-300 ease-in-out ${isDesktopSidebarOpen ? 'lg:pl-64' : 'lg:pl-0'}`}>
        <TopNavigation />
        <main className="flex-1 px-2 sm:px-4 lg:px-6 xl:px-8 pb-4 sm:pb-6">
          {children}
        </main>
      </div>
    </div>
  );
};

// Main App Component
const AppContent = () => {
  const { user } = useAuth();

  return (
    <Routes>
      {/* Public Routes */}
      <Route
        path="/login"
        element={
          user ? (
            user.role === 'manager' ? <Navigate to="/stocks" replace /> :
              <Navigate to="/dashboard" replace />
          ) : <Login />
        }
      />

      {/* Protected Routes */}
      <Route path="/" element={
        user?.role === 'manager' ? <Navigate to="/stocks" replace /> :
          <Navigate to="/dashboard" replace />
      } />

      <Route
        path="/dashboard"
        element={
          <RoleBasedRoute allowedRoles={['admin', 'inventory', 'cashier']} fallbackPath="/stocks">
            <Layout>
              <Dashboard />
            </Layout>
          </RoleBasedRoute>
        }
      />

      <Route
        path="/products"
        element={
          <RoleBasedRoute allowedRoles={['admin', 'inventory', 'manager']} fallbackPath="/stocks">
            <Layout>
              <Products />
            </Layout>
          </RoleBasedRoute>
        }
      />

      <Route
        path="/categories"
        element={
          <RoleBasedRoute allowedRoles={['admin', 'manager']} fallbackPath="/stocks">
            <Layout>
              <Categories />
            </Layout>
          </RoleBasedRoute>
        }
      />

      <Route
        path="/brands"
        element={
          <RoleBasedRoute allowedRoles={['admin', 'manager']} fallbackPath="/stocks">
            <Layout>
              <Brands />
            </Layout>
          </RoleBasedRoute>
        }
      />

      <Route
        path="/customers"
        element={
          <RoleBasedRoute allowedRoles={['admin', 'cashier']}>
            <Layout>
              <Customers />
            </Layout>
          </RoleBasedRoute>
        }
      />

      <Route
        path="/sales-dashboard"
        element={
          <RoleBasedRoute allowedRoles={['admin', 'manager', 'inventory', 'cashier']} fallbackPath="/orders/create">
            <Layout>
              <SalesDashboard />
            </Layout>
          </RoleBasedRoute>
        }
      />

      <Route
        path="/orders"
        element={
          <RoleBasedRoute allowedRoles={['admin', 'cashier']}>
            <Layout>
              <Orders />
            </Layout>
          </RoleBasedRoute>
        }
      />

      <Route
        path="/orders/create"
        element={
          <RoleBasedRoute allowedRoles={['admin', 'cashier']}>
            <CreateOrder />
          </RoleBasedRoute>
        }
      />

      <Route
        path="/orders/edit/:orderId"
        element={
          <RoleBasedRoute allowedRoles={['admin']}>
            <Layout>
              <Orders />
            </Layout>
          </RoleBasedRoute>
        }
      />

      <Route
        path="/shops"
        element={
          <RoleBasedRoute allowedRoles={['admin']}>
            <Layout>
              <Shops />
            </Layout>
          </RoleBasedRoute>
        }
      />

      <Route
        path="/perfumes"
        element={
          <RoleBasedRoute allowedRoles={['admin', 'manager', 'inventory']}>
            <Layout>
              <Perfumes />
            </Layout>
          </RoleBasedRoute>
        }
      />

      {/* SMART BOTTLING ROUTE - COMMENTED OUT */}
      {/* Uncomment the route below to enable Smart Bottling system */}
      {/* 
      <Route 
        path="/smart-bottling" 
        element={
          <RoleBasedRoute allowedRoles={['admin', 'inventory']}>
            <Layout>
              <SmartBottling />
            </Layout>
          </RoleBasedRoute>
        } 
      />
      */}

      <Route
        path="/production"
        element={
          <RoleBasedRoute allowedRoles={['admin', 'inventory']}>
            <Layout>
              <Production />
            </Layout>
          </RoleBasedRoute>
        }
      />

      <Route
        path="/procurement"
        element={
          <RoleBasedRoute allowedRoles={['admin', 'inventory']}>
            <Layout>
              <Procurement />
            </Layout>
          </RoleBasedRoute>
        }
      />

      <Route
        path="/production/:tab"
        element={
          <RoleBasedRoute allowedRoles={['admin', 'inventory']}>
            <Layout>
              <Production />
            </Layout>
          </RoleBasedRoute>
        }
      />

      <Route
        path="/inventory"
        element={
          <RoleBasedRoute allowedRoles={['admin', 'inventory']}>
            <Layout>
              <Inventory />
            </Layout>
          </RoleBasedRoute>
        }
      />

      <Route
        path="/inventory/:tab"
        element={
          <RoleBasedRoute allowedRoles={['admin', 'inventory']}>
            <Layout>
              <Inventory />
            </Layout>
          </RoleBasedRoute>
        }
      />

      <Route
        path="/stocks"
        element={
          <RoleBasedRoute allowedRoles={['admin', 'manager', 'inventory']}>
            <Layout>
              <Stocks />
            </Layout>
          </RoleBasedRoute>
        }
      />

      <Route
        path="/notifications"
        element={
          <RoleBasedRoute allowedRoles={['admin', 'cashier']}>
            <Layout>
              <Notifications />
            </Layout>
          </RoleBasedRoute>
        }
      />

      <Route
        path="/integrations"
        element={
          <ProtectedRoute>
            <Layout>
              <Integrations />
            </Layout>
          </ProtectedRoute>
        }
      />

      <Route
        path="/loyalty"
        element={
          <ProtectedRoute>
            <Layout>
              <Loyalty />
            </Layout>
          </ProtectedRoute>
        }
      />

      <Route
        path="/layaway"
        element={
          <ProtectedRoute>
            <Layout>
              <Layaway />
            </Layout>
          </ProtectedRoute>
        }
      />

      <Route
        path="/discounts"
        element={
          <ProtectedRoute>
            <Layout>
              <Discounts />
            </Layout>
          </ProtectedRoute>
        }
      />

      <Route
        path="/expenses"
        element={
          <RoleBasedRoute allowedRoles={['admin', 'cashier']}>
            <Layout>
              <Expenses />
            </Layout>
          </RoleBasedRoute>
        }
      />

      <Route
        path="/gl-accounts"
        element={
          <RoleBasedRoute allowedRoles={['admin', 'manager']}>
            <Layout>
              <GLAccounts />
            </Layout>
          </RoleBasedRoute>
        }
      />

      <Route
        path="/analytics"
        element={
          <RoleBasedRoute allowedRoles={['admin']}>
            <Layout>
              <Analytics />
            </Layout>
          </RoleBasedRoute>
        }
      />

      <Route
        path="/reports/income"
        element={
          <RoleBasedRoute allowedRoles={['admin', 'manager']}>
            <Layout>
              <IncomeReport />
            </Layout>
          </RoleBasedRoute>
        }
      />

      <Route
        path="/pricing"
        element={
          <RoleBasedRoute allowedRoles={['admin']}>
            <Layout>
              <PricingManagement />
            </Layout>
          </RoleBasedRoute>
        }
      />

      <Route
        path="/gift-cards"
        element={
          <RoleBasedRoute allowedRoles={['admin', 'cashier']}>
            <Layout>
              <GiftCards />
            </Layout>
          </RoleBasedRoute>
        }
      />

      <Route
        path="/settings"
        element={
          <RoleBasedRoute allowedRoles={['admin']}>
            <Layout>
              <Settings />
            </Layout>
          </RoleBasedRoute>
        }
      />

      {/* Catch all route */}
      <Route
        path="*"
        element={
          <ProtectedRoute>
            {user?.role === 'manager' ? (
              <Navigate to="/stocks" replace />
            ) : (
              <Navigate to="/dashboard" replace />
            )}
          </ProtectedRoute>
        }
      />
    </Routes>
  );
};

// Root App Component
const App = () => {
  return (
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <SidebarProvider>
          <AppContent />
        </SidebarProvider>
      </AuthProvider>
    </QueryClientProvider>
  );
};

export default App; 