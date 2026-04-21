import React, { useState, useEffect } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import {
  LayoutDashboard,
  Package,
  Users,
  ShoppingCart,
  Store,
  Settings,
  LogOut,
  X,
  ChevronDown,
  BarChart3,
  Award,
  CreditCard,
  FileText,
  FlaskConical,
  Home,
  Shield,
  Palette,
  Receipt,
  Globe,
  Database,
  ClipboardList,
  FileCheck,
  Eye,
  Plus,
  Percent,
  PanelLeftClose,
  PanelLeftOpen,
  Bell
} from 'lucide-react';
import { useAuth } from '../contexts/AuthContext';
import { useSidebar } from '../contexts/SidebarContext';
import UrutiLaRoseLogo from './UrutiLaRoseLogo';

const Navigation = () => {
  const [isSettingsExpanded, setIsSettingsExpanded] = useState(false);
  const [isProductManagerExpanded, setIsProductManagerExpanded] = useState(false);
  const [isCRMExpanded, setIsCRMExpanded] = useState(false);
  const [isOrdersManagerExpanded, setIsOrdersManagerExpanded] = useState(false);
  const [isInventoryExpanded, setIsInventoryExpanded] = useState(false);

  // Accordion behavior: close all menus except the one being toggled
  const toggleMenuExpansion = (menuName) => {
    // Check if the clicked menu is already expanded
    let isCurrentlyExpanded = false;
    switch (menuName) {
      case 'Settings':
        isCurrentlyExpanded = isSettingsExpanded;
        break;
      case 'Product Manager':
        isCurrentlyExpanded = isProductManagerExpanded;
        break;
      case 'CRM':
        isCurrentlyExpanded = isCRMExpanded;
        break;
      case 'Orders Manager':
        isCurrentlyExpanded = isOrdersManagerExpanded;
        break;
      case 'Inventory':
        isCurrentlyExpanded = isInventoryExpanded;
        break;
      default:
        break;
    }

    // Close all menus first
    setIsSettingsExpanded(false);
    setIsProductManagerExpanded(false);
    setIsCRMExpanded(false);
    setIsOrdersManagerExpanded(false);
    setIsInventoryExpanded(false);

    // If the clicked menu wasn't already expanded, open it
    if (!isCurrentlyExpanded) {
      switch (menuName) {
        case 'Settings':
          setIsSettingsExpanded(true);
          break;
        case 'Product Manager':
          setIsProductManagerExpanded(true);
          break;
        case 'CRM':
          setIsCRMExpanded(true);
          break;
        case 'Orders Manager':
          setIsOrdersManagerExpanded(true);
          break;
        case 'Inventory':
          setIsInventoryExpanded(true);
          break;
        default:
          break;
      }
    }
  };
  const location = useLocation();
  const navigate = useNavigate();
  const { user, logout } = useAuth();
  const { isDesktopSidebarOpen, setIsDesktopSidebarOpen, isMobileMenuOpen, setIsMobileMenuOpen } = useSidebar();

  // Close mobile menu when window is resized to desktop size
  useEffect(() => {
    const handleResize = () => {
      if (window.innerWidth >= 1024 && isMobileMenuOpen) {
        setIsMobileMenuOpen(false);
      }
    };

    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, [isMobileMenuOpen, setIsMobileMenuOpen]);

  // Role-based navigation filtering
  const getFilteredNavigationItems = () => {
    if (!user) return [];

    // Cashier role - limited access
    if (user.role === 'cashier') {
      return [
        {
          name: 'Dashboard',
          href: '/dashboard',
          icon: LayoutDashboard,
          description: 'Overview and analytics'
        },
        {
          name: 'Sales Dashboard',
          href: '/sales-dashboard',
          icon: BarChart3,
          description: 'Sales analytics and reports'
        },
        {
          name: 'Sales',
          icon: ShoppingCart,
          description: 'Create new sales orders',
          href: '/orders/create'
        },
        {
          name: 'Sales Records',
          icon: FileText,
          description: 'View your sales history',
          href: '/orders'
        },
        {
          name: 'Customers',
          icon: Users,
          description: 'Customer management',
          href: '/customers'
        },
        {
          name: 'Expenses',
          icon: Receipt,
          description: 'Manage shop expenses',
          href: '/expenses'
        }
      ];
    }

    // Manager role - limited access
    if (user.role === 'manager') {
      return [
        {
          name: 'Product Manager',
          icon: Package,
          description: 'Manage products and inventory',
          hasSubmenu: true,
          submenu: [
            {
              name: 'Products',
              href: '/products',
              description: 'Manage inventory and catalog'
            },
            {
              name: 'Categories',
              href: '/categories',
              description: 'Organize product categories'
            },
            {
              name: 'Brands',
              href: '/brands',
              description: 'Manage product brands'
            }
          ]
        },
        {
          name: 'Inventory',
          icon: Package,
          description: 'Real-time stock tracking & management',
          hasSubmenu: true,
          submenu: [
            {
              name: 'Shop Stocks',
              href: '/stocks',
              description: 'Shop-specific stock management'
            }
          ]
        },
      ];
    }

    // For admin and other roles - show all navigation items
    return navigationItems;
  };

  const navigationItems = [
    {
      name: 'Dashboard',
      href: '/dashboard',
      icon: LayoutDashboard,
      description: 'Overview and analytics'
    },
    {
      name: 'Product Manager',
      icon: Package,
      description: 'Manage products and categories',
      hasSubmenu: true,
      submenu: [
        {
          name: 'Products',
          href: '/products',
          description: 'Manage inventory and catalog'
        },
        {
          name: 'Categories',
          href: '/categories',
          description: 'Organize product categories'
        },
        {
          name: 'Brands',
          href: '/brands',
          description: 'Manage product brands'
        },
        {
          name: 'Shops',
          href: '/shops',
          description: 'Manage shop locations and settings'
        }
      ]
    },
    {
      name: 'CRM',
      icon: Users,
      description: 'Customer relationship management',
      hasSubmenu: true,
      submenu: [
        {
          name: 'Customers',
          href: '/customers',
          description: 'Customer management and profiles'
        },
        {
          name: 'Loyalty',
          href: '/loyalty',
          description: 'Customer loyalty program'
        },
        {
          name: 'Gift Cards',
          href: '/gift-cards',
          description: 'Gift card sales and management'
        },
        {
          name: 'Layaway',
          href: '/layaway',
          description: 'Advance payment system'
        }
      ]
    },
    {
      name: 'Orders Manager',
      icon: ShoppingCart,
      description: 'Complete order management system',
      hasSubmenu: true,
      submenu: [
        {
          name: 'Sales Dashboard',
          href: '/sales-dashboard',
          description: 'Sales analytics and invoice management'
        },
        {
          name: 'Sales',
          href: '/orders/create',
          description: 'Create new sales orders'
        },
        {
          name: 'Sales Records',
          href: '/orders',
          description: 'View and manage existing orders'
        }
      ]
    },
    {
      name: 'Inventory',
      icon: Package,
      description: 'Real-time stock tracking & management',
      hasSubmenu: true,
      submenu: [
        {
          name: 'Stock Levels',
          href: '/inventory',
          description: 'View real-time inventory levels'
        },
        {
          name: 'Shop Stocks',
          href: '/stocks',
          description: 'Shop-specific stock management'
        }
      ]
    },
    {
      name: 'Procurement',
      href: '/procurement',
      icon: ClipboardList,
      description: 'Manage suppliers and purchase orders'
    },
    {
      name: 'Notifications',
      href: '/notifications',
      icon: Bell,
      description: 'SMS, Email & Push notifications'
    },
    {
      name: 'Expenses',
      href: '/expenses',
      icon: FileText,
      description: 'Expense tracking and management'
    },
    {
      name: 'Income Report',
      href: '/reports/income',
      icon: FileText,
      description: 'Orders, revenue, costs, and expenses analysis'
    }
  ];

  const settingsSubmenus = [
    {
      name: 'User Management',
      href: '/settings?tab=users',
      icon: Users,
      description: 'Manage system users'
    }
  ];

  const isActive = (href) => {
    if (href.startsWith('/settings')) {
      const urlParams = new URLSearchParams(location.search);
      const tab = urlParams.get('tab');
      if (href.includes('tab=')) {
        const hrefTab = href.split('tab=')[1];
        return tab === hrefTab;
      }
      return location.pathname === '/settings' && !tab;
    }
    return location.pathname === href;
  };

  const isSubmenuActive = (submenu) => {
    return submenu.some(item => isActive(item.href));
  };

  const handleLogout = async () => {
    try {
      // Close all dropdowns
      setIsMobileMenuOpen(false);
      setIsSettingsExpanded(false);
      setIsProductManagerExpanded(false);
      setIsCRMExpanded(false);
      setIsOrdersManagerExpanded(false);
        setIsInventoryExpanded(false);

      await logout();
      // Navigation will be handled by the ProtectedRoute component
      // when user becomes null, it will redirect to /login
    } catch (error) {
      console.error('Logout failed:', error);
      // Force navigation to login even if logout fails
      navigate('/login');
    }
  };

  const getInitials = (name) => {
    if (!name) return 'U';
    return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  };

  return (
    <>
      {/* Mobile menu backdrop */}
      {isMobileMenuOpen && (
        <div
          className="fixed inset-0 bg-black bg-opacity-50 z-40 lg:hidden"
          onClick={() => setIsMobileMenuOpen(false)}
        />
      )}

      {/* Mobile menu */}
      <div className={`fixed inset-y-0 left-0 z-50 w-64 bg-white shadow-xl transform transition-transform duration-300 ease-in-out lg:hidden flex flex-col ${isMobileMenuOpen ? 'translate-x-0' : '-translate-x-full'
        }`}>
        <div className="flex items-center justify-between p-6 border-b border-gray-200">
          <div className="flex items-center">
            <UrutiLaRoseLogo className="h-16 w-48" />
          </div>
          <button
            onClick={() => setIsMobileMenuOpen(false)}
            className="p-2 rounded-lg hover:bg-gray-100 transition-colors min-h-[44px] min-w-[44px] touch-target flex items-center justify-center"
            aria-label="Close mobile menu"
          >
            <X className="h-5 w-5 text-gray-600" />
          </button>
        </div>

        <nav className="mt-6 px-3 flex-1 overflow-y-auto custom-scrollbar scroll-mobile relative max-h-[calc(100vh-200px)]">
          <div className="space-y-1">
            {getFilteredNavigationItems().map((item) => {
              const Icon = item.icon;

              // Handle submenu items
              if (item.hasSubmenu) {
                const isExpanded = item.name === 'Product Manager' ? isProductManagerExpanded :
                  item.name === 'CRM' ? isCRMExpanded :
                    item.name === 'Orders Manager' ? isOrdersManagerExpanded :
                      item.name === 'Inventory' ? isInventoryExpanded : false;
                const setExpanded = item.name === 'Product Manager' ? setIsProductManagerExpanded :
                  item.name === 'CRM' ? setIsCRMExpanded :
                    item.name === 'Orders Manager' ? setIsOrdersManagerExpanded :
                      item.name === 'Inventory' ? setIsInventoryExpanded : () => { };

                return (
                  <div key={item.name}>
                    <button
                      onClick={() => toggleMenuExpansion(item.name)}
                      className={`w-full group flex items-center justify-between px-3 py-3 text-sm font-medium rounded-xl transition-all duration-200 ${isSubmenuActive(item.submenu)
                        ? 'bg-gradient-to-r from-blue-50 to-purple-50 text-blue-700 border border-blue-200'
                        : 'text-gray-700 hover:bg-gray-50 hover:text-gray-900'
                        }`}
                    >
                      <div className="flex items-center">
                        <Icon className={`mr-3 h-5 w-5 ${isSubmenuActive(item.submenu) ? 'text-blue-600' : 'text-gray-400 group-hover:text-gray-600'
                          }`} />
                        {item.name}
                      </div>
                      <ChevronDown className={`h-4 w-4 transition-transform ${isExpanded ? 'rotate-180' : ''
                        }`} />
                    </button>

                    {isExpanded && (
                      <div className="mt-2 ml-6 space-y-1">
                        {item.submenu.map((subItem) => (
                          <Link
                            key={subItem.name}
                            to={subItem.href}
                            onClick={() => setIsMobileMenuOpen(false)}
                            className={`group flex items-center px-3 py-2 text-sm font-medium rounded-lg transition-all duration-200 ${isActive(subItem.href)
                              ? 'bg-blue-50 text-blue-700'
                              : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
                              }`}
                          >
                            {subItem.name}
                          </Link>
                        ))}
                      </div>
                    )}
                  </div>
                );
              }

              // Handle regular navigation items (only if they have href)
              if (item.href) {
                return (
                  <div key={item.name}>
                    <Link
                      to={item.href}
                      onClick={() => setIsMobileMenuOpen(false)}
                      className={`group flex items-center px-3 py-3 text-sm font-medium rounded-xl transition-all duration-200 ${isActive(item.href)
                        ? 'bg-gradient-to-r from-blue-50 to-purple-50 text-blue-700 border border-blue-200'
                        : 'text-gray-700 hover:bg-gray-50 hover:text-gray-900'
                        }`}
                    >
                      <Icon className={`mr-3 h-5 w-5 ${isActive(item.href) ? 'text-blue-600' : 'text-gray-400 group-hover:text-gray-600'
                        }`} />
                      {item.name}
                    </Link>

                    {/* Quick Action Button */}
                    {item.quickAction && (
                      <Link
                        to={item.quickAction.href}
                        onClick={() => setIsMobileMenuOpen(false)}
                        className="mt-2 ml-8 group flex items-center px-3 py-2 text-xs font-medium text-blue-600 hover:text-blue-700 hover:bg-blue-50 rounded-lg transition-all duration-200"
                      >
                        <Plus className="mr-2 h-4 w-4" />
                        {item.quickAction.label}
                      </Link>
                    )}
                  </div>
                );
              }

              // If item has no href and no submenu, return null
              return null;
            })}

            {/* Settings Menu - Only for Admin */}
            {user?.role === 'admin' && (
              <div className="border-t border-gray-200 pt-4 mt-4">
                <button
                  onClick={() => toggleMenuExpansion('Settings')}
                  className={`w-full group flex items-center justify-between px-3 py-3 text-sm font-medium rounded-xl transition-all duration-200 ${location.pathname === '/settings'
                    ? 'bg-gradient-to-r from-blue-50 to-purple-50 text-blue-700 border border-blue-200'
                    : 'text-gray-700 hover:bg-gray-50 hover:text-gray-900'
                    }`}
                >
                  <div className="flex items-center">
                    <Settings className={`mr-3 h-5 w-5 ${location.pathname === '/settings' ? 'text-blue-600' : 'text-gray-400 group-hover:text-gray-600'
                      }`} />
                    Settings
                  </div>
                  <ChevronDown className={`h-4 w-4 transition-transform ${isSettingsExpanded ? 'rotate-180' : ''
                    }`} />
                </button>

                {isSettingsExpanded && (
                  <div className="mt-2 ml-6 space-y-1">
                    {settingsSubmenus.map((item) => {
                      const Icon = item.icon;
                      return (
                        <Link
                          key={item.name}
                          to={item.href}
                          onClick={() => setIsMobileMenuOpen(false)}
                          className={`group flex items-center px-3 py-2 text-sm font-medium rounded-lg transition-all duration-200 ${isActive(item.href)
                            ? 'bg-blue-50 text-blue-700'
                            : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
                            }`}
                        >
                          <Icon className={`mr-2 h-4 w-4 ${isActive(item.href) ? 'text-blue-600' : 'text-gray-400 group-hover:text-gray-600'
                            }`} />
                          {item.name}
                        </Link>
                      );
                    })}
                  </div>
                )}
              </div>
            )}
          </div>
        </nav>

        {/* Mobile user profile */}
        <div className="mt-auto p-4 border-t border-gray-200">
          <div className="flex items-center">
            <div className="w-10 h-10 bg-gradient-to-r from-blue-600 to-purple-600 rounded-full flex items-center justify-center text-white font-semibold">
              {getInitials(user?.firstName + ' ' + user?.lastName)}
            </div>
            <div className="ml-3 flex-1">
              <p className="text-sm font-medium text-gray-900">
                {user?.firstName} {user?.lastName}
              </p>
              <p className="text-xs text-gray-500 capitalize">{user?.role}</p>
            </div>
            <button
              onClick={handleLogout}
              className="p-2 rounded-lg hover:bg-gray-100 transition-colors"
            >
              <LogOut className="h-4 w-4 text-gray-600" />
            </button>
          </div>
        </div>
      </div>

      {/* Desktop sidebar */}
      <div className={`hidden lg:flex lg:flex-col lg:w-64 lg:fixed lg:inset-y-0 lg:z-50 transition-transform duration-300 ease-in-out ${isDesktopSidebarOpen ? 'translate-x-0' : '-translate-x-full'}`}>
        <div className="flex flex-col flex-grow bg-white border-r border-gray-200 shadow-sm">
          {/* Logo */}
          <div className="flex items-center justify-between px-6 py-6 border-b border-gray-200">
            <div className="flex items-center">
              <UrutiLaRoseLogo className="h-16 w-48" />
            </div>
            <button
              onClick={() => setIsDesktopSidebarOpen(false)}
              className="p-2 rounded-lg hover:bg-gray-100 transition-colors"
              title="Close sidebar"
            >
              <PanelLeftClose className="h-5 w-5 text-gray-600" />
            </button>
          </div>

          {/* Navigation */}
          <nav className="flex-1 px-3 py-6 space-y-1 overflow-y-auto custom-scrollbar scroll-mobile relative max-h-[calc(100vh-200px)]">
            {getFilteredNavigationItems().map((item) => {
              const Icon = item.icon;

              // Handle submenu items
              if (item.hasSubmenu) {
                const isExpanded = item.name === 'Product Manager' ? isProductManagerExpanded :
                  item.name === 'CRM' ? isCRMExpanded :
                    item.name === 'Orders Manager' ? isOrdersManagerExpanded :
                      item.name === 'Inventory' ? isInventoryExpanded : false;
                const setExpanded = item.name === 'Product Manager' ? setIsProductManagerExpanded :
                  item.name === 'CRM' ? setIsCRMExpanded :
                    item.name === 'Orders Manager' ? setIsOrdersManagerExpanded :
                      item.name === 'Inventory' ? setIsInventoryExpanded : () => { };

                return (
                  <div key={item.name}>
                    <button
                      onClick={() => toggleMenuExpansion(item.name)}
                      className={`w-full group flex items-center justify-between px-3 py-3 text-sm font-medium rounded-xl transition-all duration-200 relative ${isSubmenuActive(item.submenu)
                        ? 'bg-gradient-to-r from-blue-50 to-purple-50 text-blue-700 border border-blue-200 shadow-sm'
                        : 'text-gray-700 hover:bg-gray-50 hover:text-gray-900'
                        }`}
                      title={item.description}
                    >
                      <div className="flex items-center">
                        <Icon className={`mr-3 h-5 w-5 ${isSubmenuActive(item.submenu) ? 'text-blue-600' : 'text-gray-400 group-hover:text-gray-600'
                          }`} />
                        {item.name}
                      </div>
                      <ChevronDown className={`h-4 w-4 transition-transform ${isExpanded ? 'rotate-180' : ''
                        }`} />
                      {isSubmenuActive(item.submenu) && (
                        <div className="absolute right-2 w-2 h-2 bg-blue-600 rounded-full"></div>
                      )}
                    </button>

                    {isExpanded && (
                      <div className="mt-2 ml-6 space-y-1">
                        {item.submenu.map((subItem) => (
                          <Link
                            key={subItem.name}
                            to={subItem.href}
                            onClick={() => setIsDesktopSidebarOpen(false)}
                            className={`group flex items-center px-3 py-2 text-sm font-medium rounded-lg transition-all duration-200 relative ${isActive(subItem.href)
                              ? 'bg-blue-50 text-blue-700'
                              : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
                              }`}
                            title={subItem.description}
                          >
                            {subItem.name}
                            {isActive(subItem.href) && (
                              <div className="absolute right-2 w-2 h-2 bg-blue-600 rounded-full"></div>
                            )}
                          </Link>
                        ))}
                      </div>
                    )}
                  </div>
                );
              }

              // Handle regular navigation items (only if they have href)
              if (item.href) {
                return (
                  <div key={item.name}>
                    <Link
                      to={item.href}
                      onClick={() => setIsDesktopSidebarOpen(false)}
                      className={`group flex items-center px-3 py-3 text-sm font-medium rounded-xl transition-all duration-200 relative ${isActive(item.href)
                        ? 'bg-gradient-to-r from-blue-50 to-purple-50 text-blue-700 border border-blue-200 shadow-sm'
                        : 'text-gray-700 hover:bg-gray-50 hover:text-gray-900'
                        }`}
                      title={item.description}
                    >
                      <Icon className={`mr-3 h-5 w-5 ${isActive(item.href) ? 'text-blue-600' : 'text-gray-400 group-hover:text-gray-600'
                        }`} />
                      {item.name}
                      {isActive(item.href) && (
                        <div className="absolute right-2 w-2 h-2 bg-blue-600 rounded-full"></div>
                      )}
                    </Link>

                    {/* Quick Action Button */}
                    {item.quickAction && (
                      <Link
                        to={item.quickAction.href}
                        onClick={() => setIsDesktopSidebarOpen(false)}
                        className="mt-2 ml-8 group flex items-center px-3 py-2 text-xs font-medium text-blue-600 hover:text-blue-700 hover:bg-blue-50 rounded-lg transition-all duration-200"
                        title={item.quickAction.description}
                      >
                        <Plus className="mr-2 h-4 w-4" />
                        {item.quickAction.label}
                      </Link>
                    )}
                  </div>
                );
              }

              // If item has no href and no submenu, return null
              return null;
            })}

            {/* Settings Menu - Only for Admin */}
            {user?.role === 'admin' && (
              <div className="border-t border-gray-200 pt-4 mt-4">
                <button
                  onClick={() => toggleMenuExpansion('Settings')}
                  className={`w-full group flex items-center justify-between px-3 py-3 text-sm font-medium rounded-xl transition-all duration-200 relative ${location.pathname === '/settings'
                    ? 'bg-gradient-to-r from-blue-50 to-purple-50 text-blue-700 border border-blue-200 shadow-sm'
                    : 'text-gray-700 hover:bg-gray-50 hover:text-gray-900'
                    }`}
                  title="System configuration and settings"
                >
                  <div className="flex items-center">
                    <Settings className={`mr-3 h-5 w-5 ${location.pathname === '/settings' ? 'text-blue-600' : 'text-gray-400 group-hover:text-gray-600'
                      }`} />
                    Settings
                  </div>
                  <ChevronDown className={`h-4 w-4 transition-transform ${isSettingsExpanded ? 'rotate-180' : ''
                    }`} />
                  {location.pathname === '/settings' && (
                    <div className="absolute right-2 w-2 h-2 bg-blue-600 rounded-full"></div>
                  )}
                </button>

                {isSettingsExpanded && (
                  <div className="mt-2 ml-6 space-y-1">
                    {settingsSubmenus.map((item) => {
                      const Icon = item.icon;
                      return (
                        <Link
                          key={item.name}
                          to={item.href}
                          onClick={() => setIsDesktopSidebarOpen(false)}
                          className={`group flex items-center px-3 py-2 text-sm font-medium rounded-lg transition-all duration-200 relative ${isActive(item.href)
                            ? 'bg-blue-50 text-blue-700'
                            : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
                            }`}
                          title={item.description}
                        >
                          <Icon className={`mr-2 h-4 w-4 ${isActive(item.href) ? 'text-blue-600' : 'text-gray-400 group-hover:text-gray-600'
                            }`} />
                          {item.name}
                          {isActive(item.href) && (
                            <div className="absolute right-2 w-2 h-2 bg-blue-600 rounded-full"></div>
                          )}
                        </Link>
                      );
                    })}
                  </div>
                )}
              </div>
            )}
          </nav>

          {/* User profile */}
          <div className="p-4 border-t border-gray-200">
            <div className="flex items-center">
              <div className="w-10 h-10 bg-gradient-to-r from-blue-600 to-purple-600 rounded-full flex items-center justify-center text-white font-semibold">
                {getInitials(user?.firstName + ' ' + user?.lastName)}
              </div>
              <div className="ml-3 flex-1">
                <p className="text-sm font-medium text-gray-900">
                  {user?.firstName} {user?.lastName}
                </p>
                <p className="text-xs text-gray-500 capitalize">{user?.role}</p>
              </div>
              <button
                onClick={handleLogout}
                className="p-2 rounded-lg hover:bg-gray-100 transition-colors"
                title="Logout"
              >
                <LogOut className="h-4 w-4 text-gray-600" />
              </button>
            </div>
          </div>
        </div>
      </div>





      {/* Floating sidebar toggle button (when sidebar is closed) */}
      {!isDesktopSidebarOpen && (
        <button
          onClick={() => setIsDesktopSidebarOpen(true)}
          className="fixed left-4 top-20 z-50 hidden lg:flex p-3 bg-white border border-gray-200 rounded-lg shadow-lg hover:bg-gray-50 transition-all duration-200"
          title="Open sidebar"
        >
          <PanelLeftOpen className="h-5 w-5 text-gray-600" />
        </button>
      )}


    </>
  );
};

export default Navigation; 






