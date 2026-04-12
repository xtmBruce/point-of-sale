import React, { useState, useEffect } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Plus,
  Search,
  Filter,
  FlaskConical,
  Package,
  TrendingUp,
  DollarSign,
  Wine,
  Edit,
  Trash2,
  Eye,
  BarChart3,
  X,
  Save,
  AlertCircle,
  CheckCircle,
  Clock,
  ChevronLeft,
  ChevronRight,
  Download,
  CheckCircle2,
  RotateCcw,
  Beaker,
  SprayCan
} from 'lucide-react';
import { api } from '../lib/api';
import toast from 'react-hot-toast';
import { perfumeAPI, categoriesAPI, shopsAPI } from '../lib/api';
import { useAuth } from '../contexts/AuthContext';
import Swal from 'sweetalert2';

const Perfumes = () => {
  const { user } = useAuth();
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedFilter, setSelectedFilter] = useState('all');
  const [selectedCategory, setSelectedCategory] = useState('all');
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(12);
  const [showAddModal, setShowAddModal] = useState(false);
  const [showBottleModal, setShowBottleModal] = useState(false);
  const [showBottleSizeModal, setShowBottleSizeModal] = useState(false);
  const [showBottleSizeListModal, setShowBottleSizeListModal] = useState(false);
  const [showQuickBottleModal, setShowQuickBottleModal] = useState(false);
  const [selectedPerfume, setSelectedPerfume] = useState(null);
  const [selectedBottleSize, setSelectedBottleSize] = useState(null);
  const [editMode, setEditMode] = useState(false);
  const [quickBottleShopId, setQuickBottleShopId] = useState('');
  const [quickBottleModalShopId, setQuickBottleModalShopId] = useState('');

  const queryClient = useQueryClient();

  const isAdmin = user?.role === 'admin';
  const isManagerOrAdmin = user?.role === 'admin' || user?.role === 'manager';

  // Form states
  const [perfumeForm, setPerfumeForm] = useState({
    name: '',
    scent_description: '',
    bulk_quantity_ml: '',
    cost_per_ml: '',
    supplier: '',
    batch_number: '',
    expiry_date: '',
    category_id: ''
  });

  const [bottlingForm, setBottlingForm] = useState({
    bottle_size: '', // 30, 50, or 100
    quantity: 1,
    batch_number: '',
    shop_id: ''
  });

  // Fetch perfumes with pagination (or all if searching)
  const { data: perfumesResponse, isLoading, error } = useQuery({
    queryKey: ['perfumes', currentPage, pageSize, searchTerm, selectedFilter, selectedCategory],
    queryFn: () => {
      // If searching, fetch all perfumes to search across entire database
      const shouldFetchAll = searchTerm.trim() !== '' || selectedFilter !== 'all' || selectedCategory !== 'all';
      const limit = shouldFetchAll ? 10000 : pageSize;
      const page = shouldFetchAll ? 1 : currentPage;

      return perfumeAPI.getBulk({ page, limit }).then(res => res.data);
    },
    refetchInterval: 30000, // Refetch every 30 seconds
  });

  // Fetch ALL perfumes for statistics (no pagination)
  // First get total count, then fetch all perfumes
  const { data: allPerfumesResponse } = useQuery({
    queryKey: ['perfumes-all-stats'],
    queryFn: async () => {
      // First request to get total count
      const firstPage = await perfumeAPI.getBulk({ page: 1, limit: 1 }).then(res => res.data);
      const totalCount = firstPage?.pagination?.total || 0;

      // Fetch all perfumes using the total count
      if (totalCount > 0) {
        return perfumeAPI.getBulk({ page: 1, limit: totalCount }).then(res => res.data);
      }
      return { perfumes: [], pagination: { total: 0 } };
    },
    refetchInterval: 30000, // Refetch every 30 seconds
  });

  const perfumesData = perfumesResponse?.perfumes || [];
  const allPerfumesForStats = allPerfumesResponse?.perfumes || [];
  const pagination = perfumesResponse?.pagination || {
    page: 1,
    limit: 12,
    total: 0,
    totalPages: 1,
    hasNextPage: false,
    hasPrevPage: false
  };

  // Fetch bottle sizes for bottling
  const { data: bottleSizesData } = useQuery({
    queryKey: ['bottle-sizes'],
    queryFn: () => perfumeAPI.getBottleSizes().then(res => res.data.sizes),
  });

  // Fetch comprehensive statistics
  const { data: statsData } = useQuery({
    queryKey: ['perfume-stats'],
    queryFn: () => perfumeAPI.getBottleSizes().then(res => res.data),
    refetchInterval: 60000, // Refetch every minute
  });

  // Fetch categories
  const { data: categoriesData } = useQuery({
    queryKey: ['categories'],
    queryFn: () => categoriesAPI.getAll().then(res => res.data),
  });

  // Fetch shops
  const { data: shopsData } = useQuery({
    queryKey: ['shops'],
    queryFn: () => shopsAPI.getAll().then(res => res.data),
  });

  const categories = categoriesData?.flat || [];
  const shops = shopsData?.shops || [];

  const perfumes = perfumesData || [];
  const bottleSizes = bottleSizesData || [];
  const backendStats = statsData || {};

  // Low quantity threshold for warnings
  const LOW_QUANTITY_THRESHOLD = 10;

  // Pricing matrix based on category and bottle size
  const getPricingMatrix = () => {
    return {
      men_women: {
        30: 500,
        50: 500,
        100: 400
      },
      selective_labels: {
        30: 833.3333333333,
        50: 700,
        100: 550
      }
    };
  };

  // Calculate selling price per ml based on category and size
  const getSellingPricePerMl = (category, bottleSize) => {
    const matrix = getPricingMatrix();
    return matrix[category]?.[bottleSize] || 0;
  };

  // Determine pricing category based on selected perfume's category
  const getPricingCategory = () => {
    if (!selectedPerfume || !selectedPerfume.category_name) {
      return 'men_women'; // Default
    }
    const categoryName = selectedPerfume.category_name.toLowerCase();
    return (categoryName.includes('selective labels') || categoryName.includes('selective'))
      ? 'selective_labels'
      : 'men_women';
  };

  // Calculate batch details in real-time
  const calculateBatchDetails = () => {
    if (!selectedPerfume || !bottlingForm.bottle_size || !bottlingForm.quantity) {
      return null;
    }

    const quantity = parseInt(bottlingForm.quantity) || 0;
    const bottleSizeMl = parseInt(bottlingForm.bottle_size);
    const pricingCategory = getPricingCategory();
    const totalMlUsed = quantity * bottleSizeMl;
    const totalLitersUsed = totalMlUsed / 1000;
    const currentStock = parseFloat(selectedPerfume.bulk_quantity_ml) || 0;
    const remainingMl = currentStock - totalMlUsed;

    // Find matching bottle size from database for cost calculation
    const selectedBottleSize = bottleSizes.find(size => size.size_ml === bottleSizeMl);

    // Calculate costs
    const perfumeCostPerMl = parseFloat(selectedPerfume.cost_per_ml) || 0;
    const perfumeCost = totalMlUsed * perfumeCostPerMl;
    const bottleCost = selectedBottleSize ? quantity * (parseFloat(selectedBottleSize.bottle_cost) || 0) : 0;
    const labelCost = selectedBottleSize ? quantity * (parseFloat(selectedBottleSize.label_cost) || 0) : 0;
    const packagingCost = selectedBottleSize ? quantity * (parseFloat(selectedBottleSize.packaging_cost) || 0) : 0;
    const totalCost = perfumeCost + bottleCost + labelCost + packagingCost;
    const unitCost = quantity > 0 ? totalCost / quantity : 0;

    // Calculate selling price based on category and size
    let sellingPrice = 0;
    const mikadoCategoryId = '58c00e78-4fa1-4363-8519-4a8b02b03389';
    const bodySprayCategoryId = '6a0997f1-c35b-45db-a92c-c17cc249f731';

    if (selectedPerfume.category_id === mikadoCategoryId) {
      if (bottleSizeMl === 100 || bottleSizeMl === 101) sellingPrice = 15000;
      else if (bottleSizeMl === 200) sellingPrice = 25000;
      else sellingPrice = getSellingPricePerMl(pricingCategory, bottleSizeMl) * bottleSizeMl;
    } else if (selectedPerfume.category_id === bodySprayCategoryId) {
      if (bottleSizeMl === 250) sellingPrice = 15000;
      else sellingPrice = getSellingPricePerMl(pricingCategory, bottleSizeMl) * bottleSizeMl;
    } else {
      sellingPrice = getSellingPricePerMl(pricingCategory, bottleSizeMl) * bottleSizeMl;
    }

    const profitMargin = unitCost > 0 ? ((sellingPrice - unitCost) / unitCost) * 100 : 0;
    const markupMultiplier = unitCost > 0 ? sellingPrice / unitCost : 1;

    // Check bottle availability
    const bottleSizeQuantity = selectedBottleSize ? (parseInt(selectedBottleSize.quantity) || 0) : 0;
    const hasEnoughBottles = bottleSizeQuantity >= quantity;
    const remainingBottles = bottleSizeQuantity - quantity;
    const isLowQuantity = bottleSizeQuantity < LOW_QUANTITY_THRESHOLD;

    return {
      bottlesToCreate: quantity,
      bottleSizeMl,
      totalMlUsed,
      totalLitersUsed,
      currentStock,
      remainingMl,
      canBottle: remainingMl >= 0 && hasEnoughBottles,
      selectedBottleSize,
      bottleSizeQuantity,
      hasEnoughBottles,
      remainingBottles,
      isLowQuantity,
      // Cost breakdown
      perfumeCost,
      bottleCost,
      labelCost,
      packagingCost,
      totalCost,
      unitCost,
      // Pricing
      profitMargin,
      markupMultiplier,
      sellingPricePerMl,
      sellingPrice
    };
  };

  const batchDetails = calculateBatchDetails();

  // Filter perfumes (client-side filtering for search, status, and category)
  const filteredPerfumes = perfumesData
    .filter(perfume => {
      const matchesSearch = perfume.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        perfume.scent_description?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        perfume.supplier?.toLowerCase().includes(searchTerm.toLowerCase());
      const matchesFilter = selectedFilter === 'all' ||
        (selectedFilter === 'active' && perfume.is_active) ||
        (selectedFilter === 'inactive' && !perfume.is_active);
      const matchesCategory = selectedCategory === 'all' ||
        perfume.category_id === selectedCategory ||
        perfume.category_name?.toLowerCase().includes(selectedCategory.toLowerCase());
      return matchesSearch && matchesFilter && matchesCategory;
    })
    .sort((a, b) => {
      // Priority 1: Show unbottled perfumes first
      const aBottled = (a.used_in_bottling || 0) > 0;
      const bBottled = (b.used_in_bottling || 0) > 0;

      if (aBottled && !bBottled) return 1; // a is bottled, b is not - a goes to bottom
      if (!aBottled && bBottled) return -1; // a is not bottled, b is - a stays on top

      // Priority 2: Among perfumes with same bottling status, show low stock (< 360ml) first
      const aQuantity = parseFloat(a.bulk_quantity_ml) || 0;
      const bQuantity = parseFloat(b.bulk_quantity_ml) || 0;
      const aLowStock = aQuantity < 360;
      const bLowStock = bQuantity < 360;

      if (aLowStock && !bLowStock) return -1; // a has low stock, show first
      if (!aLowStock && bLowStock) return 1; // b has low stock, show first

      return 0; // Both have same status, keep original order
    });

  // Reset to page 1 when search or filter changes
  useEffect(() => {
    setCurrentPage(1);
  }, [searchTerm, selectedFilter, selectedCategory]);

  // CSV Export function with professional popup
  const handleDownloadCSV = async () => {
    const result = await Swal.fire({
      title: '📊 Export Perfume Data',
      html: `
        <div class="text-left">
          <p class="text-gray-600 mb-4">Export your perfume inventory data to CSV format for analysis, backup, or reporting purposes.</p>
          
          <div class="bg-blue-50 border border-blue-200 rounded-lg p-4 mb-4">
            <h4 class="font-semibold text-blue-900 mb-2">📋 Export Details</h4>
            <ul class="text-sm text-blue-800 space-y-1">
              <li>• All perfume records with current filters applied</li>
              <li>• Complete inventory data including stock levels</li>
              <li>• Cost analysis and supplier information</li>
              <li>• Batch numbers and expiry dates</li>
            </ul>
          </div>
          
          <div class="bg-green-50 border border-green-200 rounded-lg p-3">
            <p class="text-sm text-green-800">
              <strong>💡 Tip:</strong> The exported file will be saved to your Downloads folder and can be opened in Excel, Google Sheets, or any spreadsheet application.
            </p>
          </div>
        </div>
      `,
      icon: 'question',
      showCancelButton: true,
      confirmButtonText: '📥 Download CSV',
      cancelButtonText: 'Cancel',
      confirmButtonColor: '#0891b2',
      cancelButtonColor: '#6b7280',
      reverseButtons: true,
      customClass: {
        popup: 'rounded-xl',
        confirmButton: 'rounded-lg px-6 py-2.5',
        cancelButton: 'rounded-lg px-6 py-2.5'
      }
    });

    if (!result.isConfirmed) return;

    try {
      // Show loading state
      Swal.fire({
        title: 'Preparing Export...',
        html: 'Please wait while we prepare your CSV file.',
        allowOutsideClick: false,
        allowEscapeKey: false,
        showConfirmButton: false,
        didOpen: () => {
          Swal.showLoading();
        }
      });

      // Fetch all perfumes (not paginated) for export
      const response = await perfumeAPI.getBulk({ page: 1, limit: 10000 });
      const allPerfumes = response.data?.perfumes || [];

      // Filter perfumes based on current filters (same as displayed)
      const perfumesToExport = allPerfumes.filter(perfume => {
        const matchesSearch = perfume.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
          perfume.scent_description?.toLowerCase().includes(searchTerm.toLowerCase()) ||
          perfume.supplier?.toLowerCase().includes(searchTerm.toLowerCase());
        const matchesFilter = selectedFilter === 'all' ||
          (selectedFilter === 'active' && perfume.is_active) ||
          (selectedFilter === 'inactive' && !perfume.is_active);
        const matchesCategory = selectedCategory === 'all' ||
          perfume.category_id === selectedCategory ||
          perfume.category_name?.toLowerCase().includes(selectedCategory.toLowerCase());
        return matchesSearch && matchesFilter && matchesCategory;
      });

      // Define CSV headers
      const headers = [
        'ID',
        'Name',
        'Category',
        'Scent Description',
        'Bulk Quantity (ML)',
        'Cost per ML',
        'Supplier',
        'Batch Number',
        'Expiry Date',
        'Status',
        'Created At'
      ];

      // Convert perfumes to CSV rows
      const csvRows = perfumesToExport.map(perfume => {
        const row = [
          perfume.id || '',
          perfume.name || '',
          perfume.category_name || '',
          perfume.scent_description || '',
          perfume.bulk_quantity_ml || perfume.total_stock_ml || '0',
          perfume.cost_per_ml || '0',
          perfume.supplier || '',
          perfume.batch_number || '',
          perfume.expiry_date || '',
          perfume.is_active ? 'Active' : 'Inactive',
          perfume.created_at || ''
        ];

        // Escape commas and quotes in CSV
        return row.map(field => {
          const stringField = String(field);
          if (stringField.includes(',') || stringField.includes('"') || stringField.includes('\n')) {
            return `"${stringField.replace(/"/g, '""')}"`;
          }
          return stringField;
        });
      });

      // Combine headers and rows
      const csvContent = [
        headers.join(','),
        ...csvRows.map(row => row.join(','))
      ].join('\n');

      // Create blob and download
      const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
      const link = document.createElement('a');
      const url = URL.createObjectURL(blob);

      link.setAttribute('href', url);
      link.setAttribute('download', `perfumes_export_${new Date().toISOString().split('T')[0]}.csv`);
      link.style.visibility = 'hidden';

      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);

      // Success popup
      Swal.fire({
        title: '✅ Export Successful!',
        html: `
          <div class="text-center">
            <p class="text-gray-600 mb-4">Your perfume data has been successfully exported.</p>
            <div class="bg-green-50 border border-green-200 rounded-lg p-4">
              <p class="text-green-800 font-medium">📁 ${perfumesToExport.length} perfume records exported</p>
              <p class="text-sm text-green-600 mt-1">File saved to your Downloads folder</p>
            </div>
          </div>
        `,
        icon: 'success',
        confirmButtonText: 'Great!',
        confirmButtonColor: '#059669',
        customClass: {
          popup: 'rounded-xl',
          confirmButton: 'rounded-lg px-6 py-2.5'
        }
      });

      toast.success(`Exported ${perfumesToExport.length} perfume(s) to CSV`);
    } catch (error) {
      console.error('Error exporting CSV:', error);
      Swal.fire({
        title: '❌ Export Failed',
        text: 'There was an error exporting your data. Please try again.',
        icon: 'error',
        confirmButtonText: 'OK',
        confirmButtonColor: '#dc2626',
        customClass: {
          popup: 'rounded-xl',
          confirmButton: 'rounded-lg px-6 py-2.5'
        }
      });
      toast.error('Failed to export CSV');
    }
  };

  // Mutations
  const createPerfumeMutation = useMutation({
    mutationFn: (perfumeData) => perfumeAPI.createBulk(perfumeData),
    onSuccess: () => {
      queryClient.invalidateQueries(['perfumes']);
      queryClient.invalidateQueries(['perfume-stats']);
      toast.success('Perfume added successfully!');
      resetForm();
      setShowAddModal(false);
    },
    onError: (error) => {
      toast.error(error.response?.data?.error || 'Failed to add perfume');
    }
  });

  const updatePerfumeMutation = useMutation({
    mutationFn: ({ id, data }) => {
      console.log('Sending PUT request to:', `/perfume/bulk/${id}`);
      console.log('Request payload:', JSON.stringify(data, null, 2));
      return api.put(`/perfume/bulk/${id}`, data);
    },
    onSuccess: () => {
      queryClient.invalidateQueries(['perfumes']);
      queryClient.invalidateQueries(['perfume-stats']);
      queryClient.invalidateQueries(['perfumes-all-stats']);
      toast.success('Perfume updated successfully!');
      resetForm();
      setShowAddModal(false);
      setEditMode(false);
      setSelectedPerfume(null);
    },
    onError: (error) => {
      console.error('Update perfume error - Full error object:', error);
      console.error('Error response:', error.response);
      console.error('Error response data:', error.response?.data);
      console.error('Error response status:', error.response?.status);

      // Handle validation errors
      if (error.response?.data?.errors) {
        const errorMessages = error.response.data.errors.map(err => {
          const field = err.param || err.path || 'field';
          const msg = err.msg || err.message || 'Validation error';
          return `${field}: ${msg}`;
        }).join(', ');
        console.error('Validation errors:', error.response.data.errors);
        toast.error(`Validation error: ${errorMessages}`, { duration: 6000 });
      } else if (error.response?.data?.error) {
        console.error('Server error:', error.response.data.error);
        toast.error(error.response.data.error);
      } else {
        console.error('Unknown error:', error.message);
        toast.error('Failed to update perfume. Check console for details.');
      }
    }
  });

  const deletePerfumeMutation = useMutation({
    mutationFn: (id) => api.delete(`/perfume/bulk/${id}`),
    onSuccess: () => {
      queryClient.invalidateQueries(['perfumes']);
      queryClient.invalidateQueries(['perfume-stats']);
      toast.success('Perfume deleted successfully!');
    },
    onError: (error) => {
      toast.error(error.response?.data?.error || 'Failed to delete perfume');
    }
  });

  const bottlePerfumeMutation = useMutation({
    mutationFn: (bottlingData) => perfumeAPI.bottle(bottlingData),
    onSuccess: (response) => {
      const { bottling } = response.data;
      toast.success(
        `Successfully bottled ${bottling.quantity_bottled} bottles!`,
        { duration: 5000 }
      );
      queryClient.invalidateQueries(['perfumes']);
      queryClient.invalidateQueries(['perfume-stats']);
      resetBottlingForm();
      setShowBottleModal(false);
      setSelectedPerfume(null);
    },
    onError: (error) => {
      toast.error(error.response?.data?.error || 'Failed to bottle perfume');
    }
  });

  const quickBottleAllSizesMutation = useMutation({
    mutationFn: (bottlingData) => perfumeAPI.bottleAllSizes(bottlingData),
    onSuccess: (response) => {
      const { message, shops_assigned, remaining_ml } = response.data;
      toast.success(
        `${message}`,
        { duration: 5000 }
      );
      queryClient.invalidateQueries(['perfumes']);
      queryClient.invalidateQueries(['perfume-stats']);
      setQuickBottleShopId('');
      setQuickBottleModalShopId('');
      setShowBottleModal(false);
      setShowQuickBottleModal(false);
      setSelectedPerfume(null);
    },
    onError: (error) => {
      toast.error(error.response?.data?.error || 'Failed to quick bottle perfume');
    }
  });

  const returnShopInventoryMutation = useMutation({
    mutationFn: () => perfumeAPI.returnShopInventory(),
    onSuccess: (response) => {
      const { message, recovered_stats } = response.data;
      toast.success(message);
      if (recovered_stats) {
        toast.success(
          `Recovered: ${recovered_stats.liquid_ml}ML, ${recovered_stats.bottles} bottles`,
          { duration: 6000 }
        );
      }
      queryClient.invalidateQueries(['perfumes']);
      queryClient.invalidateQueries(['perfume-stats']);
    },
    onError: (error) => {
      toast.error(error.response?.data?.error || 'Failed to return shop inventory');
    }
  });

  const returnSinglePerfumeInventoryMutation = useMutation({
    mutationFn: (perfumeId) => perfumeAPI.returnSinglePerfumeInventory(perfumeId),
    onSuccess: (response) => {
      const { message, recovered_stats } = response.data;
      toast.success(message);
      if (recovered_stats) {
        toast.success(
          `Recovered: ${recovered_stats.liquid_ml}ML, ${recovered_stats.bottles} bottles`,
          { duration: 6000 }
        );
      }
      queryClient.invalidateQueries(['perfumes']);
      queryClient.invalidateQueries(['perfume-stats']);
    },
    onError: (error) => {
      toast.error(error.response?.data?.error || 'Failed to return perfume inventory');
    }
  });

  const bottleAllBulkMutation = useMutation({
    mutationFn: () => perfumeAPI.bottleAllBulk({ shop_id: 'all' }),
    onSuccess: (response) => {
      const { message, results } = response.data;
      if (results) {
        toast.success(`Success: ${results.success}, Skipped: ${results.failed}`, { duration: 5000 });
        if (results.details && results.details.length > 0) {
          console.log('Skipped details:', results.details);
        }
      } else {
        toast.success(message);
      }
      queryClient.invalidateQueries(['perfumes']);
      queryClient.invalidateQueries(['perfume-stats']);
    },
    onError: (error) => {
      toast.error(error.response?.data?.error || 'Failed to bottle all perfumes');
    }
  });

  const handleBottleAllBulk = async () => {
    const result = await Swal.fire({
      title: '🍾 Bottle ALL Inventory',
      html: `
        <div class="text-left">
          <p class="text-gray-600 mb-4">This will automatically bottle <strong>1 set (30ml, 50ml, 100ml)</strong> of EVERY available bulk perfume in your inventory.</p>
          
          <div class="bg-amber-50 border border-amber-200 rounded-lg p-4 mb-4">
            <h4 class="font-semibold text-amber-900 mb-2">⚠️ Important Notice</h4>
            <ul class="text-sm text-amber-800 space-y-1">
              <li>• This operation will process ALL active bulk perfumes</li>
              <li>• Each perfume will be bottled into 30ml, 50ml, and 100ml sizes</li>
              <li>• The process may take several minutes to complete</li>
              <li>• Insufficient stock perfumes will be skipped automatically</li>
            </ul>
          </div>
          
          <div class="bg-blue-50 border border-blue-200 rounded-lg p-3">
            <p class="text-sm text-blue-800">
              <strong>💡 Tip:</strong> You'll receive a detailed report showing successful bottlings and any skipped items.
            </p>
          </div>
        </div>
      `,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: '🚀 Start Bottling ALL',
      cancelButtonText: 'Cancel',
      confirmButtonColor: '#d97706',
      cancelButtonColor: '#6b7280',
      reverseButtons: true,
      customClass: {
        popup: 'rounded-xl',
        confirmButton: 'rounded-lg px-6 py-2.5',
        cancelButton: 'rounded-lg px-6 py-2.5'
      }
    });

    if (result.isConfirmed) {
      bottleAllBulkMutation.mutate();
    }
  };

  const handleReturnShopInventory = async () => {
    const result = await Swal.fire({
      title: '🔄 Return Shop Inventory',
      html: `
        <div class="text-left">
          <p class="text-gray-600 mb-4">This will return <strong>ALL unsold shop inventory</strong> back to bulk stock across all shops and perfumes.</p>
          
          <div class="bg-red-50 border border-red-200 rounded-lg p-4 mb-4">
            <h4 class="font-semibold text-red-900 mb-2">⚠️ Critical Operation</h4>
            <ul class="text-sm text-red-800 space-y-1">
              <li>• All unsold bottled perfumes will be converted back to bulk liquid</li>
              <li>• Empty bottles will be returned to bottle inventory</li>
              <li>• This action cannot be undone once executed</li>
              <li>• All shop inventories will be affected simultaneously</li>
            </ul>
          </div>
          
          <div class="bg-green-50 border border-green-200 rounded-lg p-3">
            <p class="text-sm text-green-800">
              <strong>✅ Recovery:</strong> You'll receive a detailed report showing recovered liquid amounts and bottle quantities.
            </p>
          </div>
        </div>
      `,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: '🔄 Return ALL Inventory',
      cancelButtonText: 'Cancel',
      confirmButtonColor: '#dc2626',
      cancelButtonColor: '#6b7280',
      reverseButtons: true,
      customClass: {
        popup: 'rounded-xl',
        confirmButton: 'rounded-lg px-6 py-2.5',
        cancelButton: 'rounded-lg px-6 py-2.5'
      }
    });

    if (result.isConfirmed) {
      returnShopInventoryMutation.mutate();
    }
  };

  const handleReturnSinglePerfumeInventory = (perfume) => {
    if (window.confirm(`Are you sure you want to return ALL shop inventory for "${perfume.name}" back to bulk stock? This action cannot be undone.`)) {
      returnSinglePerfumeInventoryMutation.mutate(perfume.id);
    }
  };

  const returnSpecializedInventoryMutation = useMutation({
    mutationFn: () => perfumeAPI.returnSpecializedInventory(),
    onSuccess: (response) => {
      const { message, recovered_stats } = response.data;
      toast.success(message);
      if (recovered_stats) {
        toast.success(
          `Recovered: ${recovered_stats.liquid_ml}ML, ${recovered_stats.bottles} bottles`,
          { duration: 6000 }
        );
      }
      queryClient.invalidateQueries(['perfumes']);
      queryClient.invalidateQueries(['perfume-stats']);
    },
    onError: (error) => {
      toast.error(error.response?.data?.error || 'Failed to return specialized inventory');
    }
  });

  const handleReturnSpecialized = async () => {
    const result = await Swal.fire({
      title: '🗑️ Return Specialized Products',
      html: `
        <div class="text-left">
          <p class="text-gray-600 mb-4">This will return <strong>"Body Spray" & "Mikado"</strong> shop inventory AND permanently delete those specialized products.</p>
          
          <div class="bg-red-50 border border-red-200 rounded-lg p-4 mb-4">
            <h4 class="font-semibold text-red-900 mb-2">🚨 DESTRUCTIVE OPERATION</h4>
            <ul class="text-sm text-red-800 space-y-1">
              <li>• Body Spray and Mikado products will be PERMANENTLY DELETED</li>
              <li>• Liquid will be recovered and returned to bulk inventory</li>
              <li>• Bottles will be returned to bottle stock</li>
              <li>• This action is IRREVERSIBLE - no recovery possible</li>
            </ul>
          </div>
          
          <div class="bg-amber-50 border border-amber-200 rounded-lg p-3">
            <p class="text-sm text-amber-800">
              <strong>⚠️ Warning:</strong> Only proceed if you're certain you want to permanently remove these specialized product categories.
            </p>
          </div>
        </div>
      `,
      icon: 'error',
      showCancelButton: true,
      confirmButtonText: '🗑️ DELETE Specialized Products',
      cancelButtonText: 'Cancel',
      confirmButtonColor: '#dc2626',
      cancelButtonColor: '#6b7280',
      reverseButtons: true,
      customClass: {
        popup: 'rounded-xl',
        confirmButton: 'rounded-lg px-6 py-2.5',
        cancelButton: 'rounded-lg px-6 py-2.5'
      }
    });

    if (result.isConfirmed) {
      returnSpecializedInventoryMutation.mutate();
    }
  };

  const bottleMikadoSpecializedMutation = useMutation({
    mutationFn: (data) => perfumeAPI.bottleMikadoSpecialized(data),
    onSuccess: (response) => {
      const { message, results } = response.data;
      toast.success(message);
      if (results) {
        toast.success(
          `Success: ${results.success}, Failed: ${results.failed}`,
          { duration: 6000 }
        );
      }
      queryClient.invalidateQueries(['perfumes']);
      queryClient.invalidateQueries(['perfume-stats']);
    },
    onError: (error) => {
      toast.error(error.response?.data?.error || 'Failed to bottle Mikado perfumes');
    }
  });

  const handleBottleMikadoSpecialized = async () => {
    const result = await Swal.fire({
      title: '🏺 Bottle Mikado Perfumes',
      html: `
        <div class="text-left">
          <p class="text-gray-600 mb-4">This will bottle <strong>ALL active "Mikado Home Perfumes"</strong> into specialized bottle sizes for all shops.</p>
          
          <div class="bg-purple-50 border border-purple-200 rounded-lg p-4 mb-4">
            <h4 class="font-semibold text-purple-900 mb-2">🏺 Mikado Bottling Details</h4>
            <ul class="text-sm text-purple-800 space-y-1">
              <li>• Target: All active Mikado Home Perfumes</li>
              <li>• Bottle Sizes: 101ml and 200ml containers</li>
              <li>• Distribution: ALL active shops will receive inventory</li>
              <li>• Automatic stock validation and error handling</li>
            </ul>
          </div>
          
          <div class="bg-blue-50 border border-blue-200 rounded-lg p-3">
            <p class="text-sm text-blue-800">
              <strong>📊 Report:</strong> You'll receive a summary showing successful bottlings and any failures with reasons.
            </p>
          </div>
        </div>
      `,
      icon: 'question',
      showCancelButton: true,
      confirmButtonText: '🏺 Bottle Mikado Products',
      cancelButtonText: 'Cancel',
      confirmButtonColor: '#7c3aed',
      cancelButtonColor: '#6b7280',
      reverseButtons: true,
      customClass: {
        popup: 'rounded-xl',
        confirmButton: 'rounded-lg px-6 py-2.5',
        cancelButton: 'rounded-lg px-6 py-2.5'
      }
    });

    if (result.isConfirmed) {
      bottleMikadoSpecializedMutation.mutate({ shop_id: 'all' });
    }
  };

  const bottleBodySpraySpecializedMutation = useMutation({
    mutationFn: (data) => perfumeAPI.bottleBodySpraySpecialized(data),
    onSuccess: (response) => {
      const { message, results } = response.data;
      toast.success(message);
      if (results) {
        toast.success(
          `Success: ${results.success}, Failed: ${results.failed}`,
          { duration: 6000 }
        );
      }
      queryClient.invalidateQueries(['perfumes']);
      queryClient.invalidateQueries(['perfume-stats']);
    },
    onError: (error) => {
      toast.error(error.response?.data?.error || 'Failed to bottle Body Spray perfumes');
    }
  });

  const handleBottleBodySpraySpecialized = async () => {
    const result = await Swal.fire({
      title: '💨 Bottle Body Spray Perfumes',
      html: `
        <div class="text-left">
          <p class="text-gray-600 mb-4">This will bottle <strong>ALL active "Body Spray"</strong> perfumes into 250ml bottles for all shops.</p>
          
          <div class="bg-green-50 border border-green-200 rounded-lg p-4 mb-4">
            <h4 class="font-semibold text-green-900 mb-2">💨 Body Spray Bottling Details</h4>
            <ul class="text-sm text-green-800 space-y-1">
              <li>• Target: All active Body Spray perfumes</li>
              <li>• Bottle Size: 250ml containers only</li>
              <li>• Distribution: ALL active shops will receive inventory</li>
              <li>• Optimized for body spray product specifications</li>
            </ul>
          </div>
          
          <div class="bg-blue-50 border border-blue-200 rounded-lg p-3">
            <p class="text-sm text-blue-800">
              <strong>📈 Tracking:</strong> Complete bottling report with success/failure counts and detailed logs.
            </p>
          </div>
        </div>
      `,
      icon: 'question',
      showCancelButton: true,
      confirmButtonText: '💨 Bottle Body Spray Products',
      cancelButtonText: 'Cancel',
      confirmButtonColor: '#059669',
      cancelButtonColor: '#6b7280',
      reverseButtons: true,
      customClass: {
        popup: 'rounded-xl',
        confirmButton: 'rounded-lg px-6 py-2.5',
        cancelButton: 'rounded-lg px-6 py-2.5'
      }
    });

    if (result.isConfirmed) {
      bottleBodySpraySpecializedMutation.mutate({ shop_id: 'all' });
    }
  };

  const handleQuickBottleAllSizes = async (perfume) => {
    setSelectedPerfume(perfume);

    // Check local storage for last used shop
    const lastUsedShopId = localStorage.getItem('last_bottling_shop_id');
    const lastUsedQuantity = localStorage.getItem('last_bottling_quantity') || '1';

    // Check category for specialized bottling
    const mikadoCategoryId = '58c00e78-4fa1-4363-8519-4a8b02b03389';
    const bodySprayCategoryId = '6a0997f1-c35b-45db-a92c-c17cc249f731';

    let isSpecialized = false;
    let specializedType = '';

    if (perfume.category_id === mikadoCategoryId) {
      isSpecialized = true;
      specializedType = 'Mikado';
    } else if (perfume.category_id === bodySprayCategoryId) {
      isSpecialized = true;
      specializedType = 'Body Spray';
    }

    // Ensure shops is an array
    const safeShops = Array.isArray(shops) ? shops : [];

    const availableMl = parseFloat(perfume.remaining_ml || perfume.bulk_quantity_ml || 0).toFixed(2);

    const { value: formData } = await Swal.fire({
      title: isSpecialized ? `Bottling ${specializedType}` : 'Quick Bottle All Sizes',
      html: `
        <div class="text-left mb-4">
          <p class="text-sm text-gray-600 mb-4">
            Bottle <b>${perfume.name}</b> in ${isSpecialized ? `<b>${specializedType === 'Mikado' ? '101ml & 200ml' : '250ml'}</b>` : 'all sizes (30ml, 50ml, 100ml)'}.
          </p>
          
          <div class="bg-gray-50 p-3 rounded-lg border border-gray-200 mb-4">
            <h4 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-2">Selected Perfume</h4>
            <div class="flex justify-between text-sm mb-1">
              <span class="text-gray-600">Name:</span>
              <span class="font-medium text-gray-900">${perfume.name}</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-gray-600">Available:</span>
              <span class="font-medium text-primary-600">${availableMl} ML</span>
            </div>
          </div>

          <label class="block text-sm font-medium text-gray-700 mb-1">Select Shop <span class="text-red-500">*</span></label>
          <select id="swal-shop-select" class="w-full p-2 border border-gray-300 rounded-md mb-3">
            <option value="">Select shop or choose "All Shops"</option>
            <option value="all">All Active Shops</option>
            ${safeShops.map(shop => `<option value="${shop.id}" ${shop.id === lastUsedShopId ? 'selected' : ''}>${shop.name}</option>`).join('')}
          </select>

          ${!isSpecialized ? `
          <label class="block text-sm font-medium text-gray-700 mb-1">Quantity per Shop <span class="text-red-500">*</span></label>
          <input id="swal-quantity-input" type="number" min="1" max="10" value="${lastUsedQuantity}" class="w-full p-2 border border-gray-300 rounded-md mb-3" placeholder="Enter quantity (1-10)">
          <p class="text-xs text-gray-500 mb-3">Number of units to bottle per shop for each size</p>
          ` : ''}
        </div>
      `,
      showCancelButton: true,
      confirmButtonText: 'Start Bottling',
      confirmButtonColor: '#3b82f6',
      preConfirm: () => {
        const shopId = document.getElementById('swal-shop-select').value;
        const quantity = isSpecialized ? 1 : parseInt(document.getElementById('swal-quantity-input').value) || 1;
        
        if (!shopId) {
          Swal.showValidationMessage('You need to select a shop!');
          return false;
        }
        
        if (!isSpecialized && (quantity < 1 || quantity > 10)) {
          Swal.showValidationMessage('Quantity must be between 1 and 10!');
          return false;
        }
        
        return { shopId, quantity };
      },
      footer: `<div class="text-xs text-center text-gray-500 w-full mt-2">
        ${isSpecialized 
          ? `Each shop will receive 1 unit of ${specializedType === 'Mikado' ? '101ml and 200ml' : '250ml'}`
          : 'Each shop will receive the specified quantity of 30ml, 50ml, and 100ml'
        }
      </div>`
    });

    if (formData) {
      const { shopId, quantity } = formData;
      localStorage.setItem('last_bottling_shop_id', shopId);
      if (!isSpecialized) {
        localStorage.setItem('last_bottling_quantity', quantity.toString());
      }

      const payload = {
        bulk_perfume_id: perfume.id,
        shop_id: shopId,
        ...(isSpecialized ? {} : { quantity })
      };

      // Always use the unified quick bottle endpoint - it handles all categories automatically
      quickBottleAllSizesMutation.mutate(payload);
    }
  };



  // Bottle Size Mutations
  const createBottleSizeMutation = useMutation({
    mutationFn: (data) => perfumeAPI.createBottleSize(data),
    onSuccess: () => {
      queryClient.invalidateQueries(['bottle-sizes']);
      toast.success('Bottle size created successfully!');
      setShowBottleSizeModal(false);
      setSelectedBottleSize(null);
    },
    onError: (error) => {
      toast.error(error.response?.data?.error || 'Failed to create bottle size');
    }
  });

  const updateBottleSizeMutation = useMutation({
    mutationFn: ({ id, data }) => perfumeAPI.updateBottleSize(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries(['bottle-sizes']);
      toast.success('Bottle size updated successfully!');
      setShowBottleSizeModal(false);
      setSelectedBottleSize(null);
    },
    onError: (error) => {
      toast.error(error.response?.data?.error || 'Failed to update bottle size');
    }
  });

  const deleteBottleSizeMutation = useMutation({
    mutationFn: (id) => perfumeAPI.deleteBottleSize(id),
    onSuccess: () => {
      queryClient.invalidateQueries(['bottle-sizes']);
      toast.success('Bottle size deleted successfully!');
    },
    onError: (error) => {
      toast.error(error.response?.data?.error || 'Failed to delete bottle size');
    }
  });

  // Form handlers
  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setPerfumeForm(prev => ({ ...prev, [name]: value }));
  };

  const handleBottlingInputChange = (e) => {
    const { name, value } = e.target;
    setBottlingForm(prev => ({ ...prev, [name]: value }));
  };

  const resetForm = () => {
    setPerfumeForm({
      name: '',
      scent_description: '',
      bulk_quantity_ml: '',
      cost_per_ml: '',
      supplier: '',
      batch_number: '',
      expiry_date: '',
      category_id: ''
    });
  };

  const resetBottlingForm = () => {
    setBottlingForm({
      bottle_size: '',
      quantity: 1,
      batch_number: '',
      shop_id: ''
    });
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    // Validate required fields
    if (!perfumeForm.name || !perfumeForm.name.trim()) {
      toast.error('Perfume name is required');
      return;
    }

    // Prepare form data - convert empty expiry_date to null and format date properly
    let formattedExpiryDate = null;
    if (perfumeForm.expiry_date && perfumeForm.expiry_date.trim() !== '') {
      // Convert yyyy-MM-dd to ISO string for backend
      const date = new Date(perfumeForm.expiry_date);
      if (!isNaN(date.getTime())) {
        formattedExpiryDate = date.toISOString();
      }
    }

    // Convert numeric fields to proper types and validate
    const formData = {};

    // Always include name (required)
    if (perfumeForm.name && perfumeForm.name.trim()) {
      formData.name = perfumeForm.name.trim();
    }

    // Only include scent_description if it has a value
    if (perfumeForm.scent_description && perfumeForm.scent_description.trim()) {
      formData.scent_description = perfumeForm.scent_description.trim();
    }

    // Convert and validate bulk_quantity_ml
    if (perfumeForm.bulk_quantity_ml) {
      const quantity = parseInt(perfumeForm.bulk_quantity_ml, 10);
      if (!isNaN(quantity) && quantity > 0) {
        formData.bulk_quantity_ml = quantity;
      }
    }

    // Convert and validate cost_per_ml
    if (perfumeForm.cost_per_ml) {
      const cost = parseFloat(perfumeForm.cost_per_ml);
      if (!isNaN(cost) && cost >= 0) {
        formData.cost_per_ml = cost;
      }
    }

    // Optional string fields
    if (perfumeForm.supplier && perfumeForm.supplier.trim()) {
      formData.supplier = perfumeForm.supplier.trim();
    }

    if (perfumeForm.batch_number && perfumeForm.batch_number.trim()) {
      formData.batch_number = perfumeForm.batch_number.trim();
    }

    // Date field
    if (formattedExpiryDate) {
      formData.expiry_date = formattedExpiryDate;
    }

    // Category ID - only include if valid UUID
    if (perfumeForm.category_id && perfumeForm.category_id.trim() !== '') {
      const categoryId = perfumeForm.category_id.trim();
      // Basic UUID validation
      const uuidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
      if (uuidRegex.test(categoryId)) {
        formData.category_id = categoryId;
      }
    }

    console.log('Form data being sent:', formData);

    if (editMode && selectedPerfume) {
      updatePerfumeMutation.mutate({ id: selectedPerfume.id, data: formData });
    } else {
      createPerfumeMutation.mutate(formData);
    }
  };

  const handleBottlingSubmit = (e) => {
    e.preventDefault();
    if (!selectedPerfume) return;

    const quantity = parseInt(bottlingForm.quantity);
    if (!quantity || quantity < 1) {
      toast.error('Please enter a valid quantity (minimum 1)');
      return;
    }

    if (!bottlingForm.bottle_size) {
      toast.error('Please select a bottle size');
      return;
    }

    if (!bottlingForm.shop_id) {
      toast.error('Please select a shop');
      return;
    }

    // Check bottle size quantity
    const bottleSizeMl = parseInt(bottlingForm.bottle_size);
    const selectedBottleSize = bottleSizes.find(size => size.size_ml === bottleSizeMl);
    if (selectedBottleSize) {
      const bottleSizeQuantity = parseInt(selectedBottleSize.quantity) || 0;
      if (bottleSizeQuantity < quantity) {
        toast.error(`Insufficient bottle stock! Available: ${bottleSizeQuantity}, Requested: ${quantity}`);
        return;
      }
    }

    const bottlingData = {
      bulk_perfume_id: selectedPerfume.id,
      bottle_size: parseInt(bottlingForm.bottle_size),
      quantity_bottled: quantity,
      shop_id: bottlingForm.shop_id
    };

    // Only include batch_number if it's provided
    if (bottlingForm.batch_number && bottlingForm.batch_number.trim()) {
      bottlingData.batch_number = bottlingForm.batch_number.trim();
    }

    bottlePerfumeMutation.mutate(bottlingData);
  };

  // Helper function to format date from ISO string to yyyy-MM-dd for date input
  const formatDateForInput = (dateString) => {
    if (!dateString) return '';
    try {
      const date = new Date(dateString);
      if (isNaN(date.getTime())) return '';
      // Format as yyyy-MM-dd
      const year = date.getFullYear();
      const month = String(date.getMonth() + 1).padStart(2, '0');
      const day = String(date.getDate()).padStart(2, '0');
      return `${year}-${month}-${day}`;
    } catch (error) {
      return '';
    }
  };

  const handleEdit = (perfume) => {
    setSelectedPerfume(perfume);
    setPerfumeForm({
      name: perfume.name,
      scent_description: perfume.scent_description || '',
      bulk_quantity_ml: perfume.bulk_quantity_ml,
      cost_per_ml: perfume.cost_per_ml,
      supplier: perfume.supplier || '',
      batch_number: perfume.batch_number || '',
      expiry_date: formatDateForInput(perfume.expiry_date),
      category_id: perfume.category_id || ''
    });
    setEditMode(true);
    setShowAddModal(true);
  };

  const handleDelete = (perfume) => {
    if (window.confirm(`Are you sure you want to delete "${perfume.name}"?`)) {
      deletePerfumeMutation.mutate(perfume.id);
    }
  };

  const handleBottle = (perfume) => {
    setSelectedPerfume(perfume);
    setShowBottleModal(true);
  };

  const handleQuickBottleClick = (perfume) => {
    setSelectedPerfume(perfume);
    setQuickBottleModalShopId('');
    setShowQuickBottleModal(true);
  };

  const handleQuickBottleFromModal = () => {
    if (!selectedPerfume) return;

    if (!quickBottleModalShopId) {
      toast.error('Please select a shop or choose "All Shops"');
      return;
    }

    const quantity = bottlingForm.quantity || 1;
    if (quantity < 1 || quantity > 10) {
      toast.error('Quantity must be between 1 and 10');
      return;
    }

    const bottlingData = {
      bulk_perfume_id: selectedPerfume.id,
      shop_id: quickBottleModalShopId,
      quantity: quantity
    };

    if (bottlingForm.batch_number && bottlingForm.batch_number.trim()) {
      bottlingData.batch_number = bottlingForm.batch_number.trim();
    }

    quickBottleAllSizesMutation.mutate(bottlingData);
  };

  const openAddModal = () => {
    setEditMode(false);
    setSelectedPerfume(null);
    resetForm();
    setShowAddModal(true);
  };

  // Calculate real-time statistics from ALL perfumes (not paginated)
  const calculateStats = () => {
    // Use allPerfumesForStats instead of perfumes for accurate statistics
    const allPerfumes = allPerfumesForStats;
    const totalMl = allPerfumes.reduce((sum, p) => sum + (parseFloat(p.total_stock_ml || p.bulk_quantity_ml) || 0), 0);
    const usedMl = allPerfumes.reduce((sum, p) => sum + (parseFloat(p.used_in_bottling || 0) || 0), 0);
    const remainingMl = allPerfumes.reduce((sum, p) => sum + (parseFloat(p.remaining_ml || p.bulk_quantity_ml) || 0), 0);
    const totalValue = allPerfumes.reduce((sum, p) => sum + ((parseFloat(p.remaining_ml || p.bulk_quantity_ml) || 0) * (parseFloat(p.cost_per_ml) || 0)), 0);
    const activePerfumes = allPerfumes.filter(p => p.is_active).length;

    return {
      totalMl: totalMl.toFixed(0),
      usedMl: usedMl.toFixed(0),
      remainingMl: remainingMl.toFixed(0),
      totalValue: totalValue.toFixed(2),
      activePerfumes,
      totalPerfumes: allPerfumes.length
    };
  };

  const frontendStats = calculateStats();

  if (error) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-gray-50 to-gray-100 p-6">
        <div className="max-w-7xl mx-auto">
          <div className="bg-red-50 border border-red-200 rounded-xl p-6 text-center">
            <AlertCircle className="h-12 w-12 text-red-500 mx-auto mb-4" />
            <h3 className="text-lg font-medium text-red-800 mb-2">Error Loading Perfumes</h3>
            <p className="text-red-600 mb-4">{error.message || 'Failed to load perfume data'}</p>
            <button
              onClick={() => window.location.reload()}
              className="px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700"
            >
              Retry
            </button>
          </div>
        </div>
      </div>
    );
  }

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-gray-50 to-gray-100 p-6">
        <div className="max-w-7xl mx-auto">
          <div className="animate-pulse">
            <div className="h-8 bg-gray-200 rounded-lg mb-6 w-1/4"></div>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {[...Array(6)].map((_, i) => (
                <div key={i} className="bg-white rounded-xl p-6 shadow-sm">
                  <div className="h-4 bg-gray-200 rounded w-1/2 mb-4"></div>
                  <div className="h-8 bg-gray-200 rounded w-3/4"></div>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-gray-100">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 py-4 sm:py-8">
        {/* Header */}
        <div className="mb-6 sm:mb-8">
          <div className="flex flex-col gap-4 sm:gap-6">
            {/* Title Section */}
            <div className="text-center sm:text-left">
              <h1 className="text-2xl sm:text-3xl font-bold text-gray-900">Perfume Management</h1>
              <p className="text-gray-600 mt-1 sm:mt-2 text-sm sm:text-base">Bulk perfume inventory and bottling system</p>
            </div>
            
            {/* Action Buttons - Responsive Grid Layout */}
            <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 xl:grid-cols-6 gap-2 sm:gap-3">
              <button
                onClick={handleDownloadCSV}
                className="inline-flex items-center justify-center px-3 sm:px-4 py-2.5 bg-primary-600 text-white font-medium rounded-lg sm:rounded-xl hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 transition-all duration-200 shadow-sm hover:shadow-md text-xs sm:text-sm lg:text-base min-h-[44px] col-span-1"
                title="Download CSV Export"
              >
                <Download className="h-4 w-4 sm:h-5 sm:w-5 mr-1 sm:mr-2" />
                <span className="hidden sm:inline">Download CSV</span>
                <span className="sm:hidden">CSV</span>
              </button>
              
              {isManagerOrAdmin && (
                <>
                  <button
                    onClick={handleBottleAllBulk}
                    disabled={bottleAllBulkMutation.isLoading}
                    className="inline-flex items-center justify-center px-3 sm:px-4 py-2.5 bg-primary-600 text-white font-medium rounded-lg sm:rounded-xl hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 transition-all duration-200 shadow-sm hover:shadow-md text-xs sm:text-sm lg:text-base min-h-[44px] col-span-1 disabled:opacity-50"
                    title="Bottle All Inventory"
                  >
                    <Beaker className={`h-4 w-4 sm:h-5 sm:w-5 mr-1 sm:mr-2 ${bottleAllBulkMutation.isLoading ? 'animate-spin' : ''}`} />
                    <span className="hidden lg:inline">Bottle ALL</span>
                    <span className="lg:hidden">Bottle</span>
                  </button>

                  <button
                    onClick={handleReturnShopInventory}
                    disabled={returnShopInventoryMutation.isLoading}
                    className="inline-flex items-center justify-center px-3 sm:px-4 py-2.5 bg-primary-600 text-white font-medium rounded-lg sm:rounded-xl hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 transition-all duration-200 shadow-sm hover:shadow-md text-xs sm:text-sm lg:text-base min-h-[44px] col-span-1 disabled:opacity-50"
                    title="Return Shop Inventory"
                  >
                    <RotateCcw className={`h-4 w-4 sm:h-5 sm:w-5 mr-1 sm:mr-2 ${returnShopInventoryMutation.isLoading ? 'animate-spin' : ''}`} />
                    <span className="hidden lg:inline">Return</span>
                    <span className="lg:hidden">Return</span>
                  </button>

                  <button
                    onClick={handleReturnSpecialized}
                    disabled={returnSpecializedInventoryMutation.isLoading}
                    className="inline-flex items-center justify-center px-3 sm:px-4 py-2.5 bg-red-600 text-white font-medium rounded-lg sm:rounded-xl hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500 transition-all duration-200 shadow-sm hover:shadow-md text-xs sm:text-sm lg:text-base min-h-[44px] col-span-1 disabled:opacity-50"
                    title="Return and Delete Body Spray & Mikado Products"
                  >
                    {returnSpecializedInventoryMutation.isLoading ? (
                      <div className="animate-spin rounded-full h-4 w-4 sm:h-5 sm:w-5 border-b-2 border-white mr-1 sm:mr-2"></div>
                    ) : (
                      <Trash2 className="h-4 w-4 sm:h-5 sm:w-5 mr-1 sm:mr-2" />
                    )}
                    <span className="hidden lg:inline">Special</span>
                    <span className="lg:hidden">Del</span>
                  </button>

                  <button
                    onClick={handleBottleMikadoSpecialized}
                    disabled={bottleMikadoSpecializedMutation.isLoading}
                    className="inline-flex items-center justify-center px-3 sm:px-4 py-2.5 bg-primary-600 text-white font-medium rounded-lg sm:rounded-xl hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 transition-all duration-200 shadow-sm hover:shadow-md text-xs sm:text-sm lg:text-base min-h-[44px] col-span-1 disabled:opacity-50"
                    title="Bottle all Mikado perfumes into 101ml and 200ml bottles"
                  >
                    {bottleMikadoSpecializedMutation.isLoading ? (
                      <div className="animate-spin rounded-full h-4 w-4 sm:h-5 sm:w-5 border-b-2 border-white mr-1 sm:mr-2"></div>
                    ) : (
                      <Package className="h-4 w-4 sm:h-5 sm:w-5 mr-1 sm:mr-2" />
                    )}
                    <span className="hidden lg:inline">Mikado</span>
                    <span className="lg:hidden">Mik</span>
                  </button>

                  <button
                    onClick={handleBottleBodySpraySpecialized}
                    disabled={bottleBodySpraySpecializedMutation.isLoading}
                    className="inline-flex items-center justify-center px-3 sm:px-4 py-2.5 bg-primary-600 text-white font-medium rounded-lg sm:rounded-xl hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 transition-all duration-200 shadow-sm hover:shadow-md text-xs sm:text-sm lg:text-base min-h-[44px] col-span-1 disabled:opacity-50"
                    title="Bottle all Body Spray perfumes into 250ml bottles"
                  >
                    {bottleBodySpraySpecializedMutation.isLoading ? (
                      <div className="animate-spin rounded-full h-4 w-4 sm:h-5 sm:w-5 border-b-2 border-white mr-1 sm:mr-2"></div>
                    ) : (
                      <SprayCan className="h-4 w-4 sm:h-5 sm:w-5 mr-1 sm:mr-2" />
                    )}
                    <span className="hidden lg:inline">Body</span>
                    <span className="lg:hidden">Body</span>
                  </button>
                </>
              )}
              
              <button
                onClick={async () => {
                  const result = await Swal.fire({
                    title: '📦 Manage Bottle Sizes',
                    html: `
                      <div class="text-left">
                        <p class="text-gray-600 mb-4">Access the bottle size management system to configure bottle specifications, costs, and inventory levels.</p>
                        
                        <div class="bg-blue-50 border border-blue-200 rounded-lg p-4 mb-4">
                          <h4 class="font-semibold text-blue-900 mb-2">🔧 Management Features</h4>
                          <ul class="text-sm text-blue-800 space-y-1">
                            <li>• Create and edit bottle size configurations</li>
                            <li>• Set bottle, label, and packaging costs</li>
                            <li>• Monitor bottle inventory levels</li>
                            <li>• Track usage statistics and low stock alerts</li>
                          </ul>
                        </div>
                        
                        <div class="bg-green-50 border border-green-200 rounded-lg p-3">
                          <p class="text-sm text-green-800">
                            <strong>💡 Tip:</strong> Keep bottle sizes updated to ensure accurate cost calculations and inventory management.
                          </p>
                        </div>
                      </div>
                    `,
                    icon: 'info',
                    showCancelButton: true,
                    confirmButtonText: '📦 Open Bottle Manager',
                    cancelButtonText: 'Cancel',
                    confirmButtonColor: '#0891b2',
                    cancelButtonColor: '#6b7280',
                    reverseButtons: true,
                    customClass: {
                      popup: 'rounded-xl',
                      confirmButton: 'rounded-lg px-6 py-2.5',
                      cancelButton: 'rounded-lg px-6 py-2.5'
                    }
                  });

                  if (result.isConfirmed) {
                    setShowBottleSizeListModal(true);
                  }
                }}
                className="inline-flex items-center justify-center px-3 sm:px-4 py-2.5 bg-primary-600 text-white font-medium rounded-lg sm:rounded-xl hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 transition-all duration-200 shadow-sm hover:shadow-md text-xs sm:text-sm lg:text-base min-h-[44px] col-span-1"
                title="Manage Bottle Sizes"
              >
                <Package className="h-4 w-4 sm:h-5 sm:w-5 mr-1 sm:mr-2" />
                <span className="hidden lg:inline">Sizes</span>
                <span className="lg:hidden">Size</span>
              </button>
              
              <button
                onClick={async () => {
                  const result = await Swal.fire({
                    title: '➕ Add New Bulk Perfume',
                    html: `
                      <div class="text-left">
                        <p class="text-gray-600 mb-4">Add a new bulk perfume to your inventory system with complete product information and specifications.</p>
                        
                        <div class="bg-green-50 border border-green-200 rounded-lg p-4 mb-4">
                          <h4 class="font-semibold text-green-900 mb-2">📝 Required Information</h4>
                          <ul class="text-sm text-green-800 space-y-1">
                            <li>• Perfume name and scent description</li>
                            <li>• Bulk quantity in milliliters</li>
                            <li>• Cost per milliliter for pricing calculations</li>
                            <li>• Supplier information and batch details</li>
                          </ul>
                        </div>
                        
                        <div class="bg-blue-50 border border-blue-200 rounded-lg p-3">
                          <p class="text-sm text-blue-800">
                            <strong>🎯 Next Steps:</strong> After adding, you can bottle the perfume into different sizes and assign to shops.
                          </p>
                        </div>
                      </div>
                    `,
                    icon: 'question',
                    showCancelButton: true,
                    confirmButtonText: '➕ Open Add Form',
                    cancelButtonText: 'Cancel',
                    confirmButtonColor: '#059669',
                    cancelButtonColor: '#6b7280',
                    reverseButtons: true,
                    customClass: {
                      popup: 'rounded-xl',
                      confirmButton: 'rounded-lg px-6 py-2.5',
                      cancelButton: 'rounded-lg px-6 py-2.5'
                    }
                  });

                  if (result.isConfirmed) {
                    openAddModal();
                  }
                }}
                className="inline-flex items-center justify-center px-3 sm:px-4 py-2.5 bg-primary-600 text-white font-medium rounded-lg sm:rounded-xl hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 transition-all duration-200 shadow-lg hover:shadow-xl transform hover:-translate-y-0.5 text-xs sm:text-sm lg:text-base min-h-[44px] col-span-1"
                title="Add New Bulk Perfume"
              >
                <Plus className="h-4 w-4 sm:h-5 sm:w-5 mr-1 sm:mr-2" />
                <span className="hidden lg:inline">Add</span>
                <span className="lg:hidden">Add</span>
              </button>
            </div>
          </div>
        </div>

        {/* Stats Cards - Responsive Grid */}
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 2xl:grid-cols-5 gap-3 sm:gap-4 lg:gap-6 mb-6 sm:mb-8">
          <div className="bg-white rounded-xl sm:rounded-2xl p-4 sm:p-6 shadow-sm border border-gray-100">
            <div className="flex items-center">
              <div className="p-2 sm:p-3 bg-primary-100 rounded-lg sm:rounded-xl">
                <FlaskConical className="h-5 w-5 sm:h-6 sm:w-6 text-primary-600" />
              </div>
              <div className="ml-3 sm:ml-4 min-w-0 flex-1">
                <p className="text-xs sm:text-sm font-medium text-gray-600 truncate">Bulk Perfumes</p>
                <p className="text-lg sm:text-2xl font-bold text-gray-900">{frontendStats.totalPerfumes}</p>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-xl sm:rounded-2xl p-4 sm:p-6 shadow-sm border border-gray-100">
            <div className="flex items-center">
              <div className="p-2 sm:p-3 bg-primary-100 rounded-lg sm:rounded-xl">
                <Wine className="h-5 w-5 sm:h-6 sm:w-6 text-primary-600" />
              </div>
              <div className="ml-3 sm:ml-4 min-w-0 flex-1">
                <p className="text-xs sm:text-sm font-medium text-gray-600 truncate">Total ML</p>
                <p className="text-lg sm:text-2xl font-bold text-gray-900">{frontendStats.totalMl}ML</p>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-xl sm:rounded-2xl p-4 sm:p-6 shadow-sm border border-gray-100">
            <div className="flex items-center">
              <div className="p-2 sm:p-3 bg-orange-100 rounded-lg sm:rounded-xl">
                <Package className="h-5 w-5 sm:h-6 sm:w-6 text-orange-600" />
              </div>
              <div className="ml-3 sm:ml-4 min-w-0 flex-1">
                <p className="text-xs sm:text-sm font-medium text-gray-600 truncate">Used ML</p>
                <p className="text-lg sm:text-2xl font-bold text-orange-600">{frontendStats.usedMl}ML</p>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-xl sm:rounded-2xl p-4 sm:p-6 shadow-sm border border-gray-100">
            <div className="flex items-center">
              <div className="p-2 sm:p-3 bg-green-100 rounded-lg sm:rounded-xl">
                <CheckCircle className="h-5 w-5 sm:h-6 sm:w-6 text-green-600" />
              </div>
              <div className="ml-3 sm:ml-4 min-w-0 flex-1">
                <p className="text-xs sm:text-sm font-medium text-gray-600 truncate">Active</p>
                <p className="text-lg sm:text-2xl font-bold text-gray-900">{frontendStats.activePerfumes}</p>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-xl sm:rounded-2xl p-4 sm:p-6 shadow-sm border border-gray-100 sm:col-span-2 lg:col-span-1">
            <div className="flex items-center">
              <div className="p-2 sm:p-3 bg-primary-100 rounded-lg sm:rounded-xl">
                <DollarSign className="h-5 w-5 sm:h-6 sm:w-6 text-primary-600" />
              </div>
              <div className="ml-3 sm:ml-4 min-w-0 flex-1">
                <p className="text-xs sm:text-sm font-medium text-gray-600 truncate">Inventory Value</p>
                <p className="text-lg sm:text-2xl font-bold text-primary-600">RWF {parseFloat(frontendStats.totalValue || 0).toLocaleString()}</p>
              </div>
            </div>
          </div>
        </div>

        {/* Filters and Search - Responsive Layout */}
        <div className="bg-white rounded-xl sm:rounded-2xl p-4 sm:p-6 shadow-sm border border-gray-100 mb-6 sm:mb-8">
          <div className="flex flex-col gap-4">
            {/* Search Bar */}
            <div className="w-full">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 sm:h-5 sm:w-5 text-gray-400" />
                <input
                  type="text"
                  placeholder="Search perfumes by name, scent, or supplier..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="w-full pl-9 sm:pl-10 pr-4 py-2.5 sm:py-3 border border-gray-300 rounded-lg sm:rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm sm:text-base"
                />
              </div>
            </div>
            
            {/* Filter Controls */}
            <div className="flex flex-col sm:flex-row gap-3 sm:gap-4">
              <div className="flex-1 sm:flex-none">
                <select
                  value={selectedFilter}
                  onChange={(e) => setSelectedFilter(e.target.value)}
                  className="w-full sm:w-auto px-3 sm:px-4 py-2.5 sm:py-3 border border-gray-300 rounded-lg sm:rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm sm:text-base min-w-0 sm:min-w-[140px]"
                >
                  <option value="all">All Perfumes</option>
                  <option value="active">Active</option>
                  <option value="inactive">Inactive</option>
                </select>
              </div>

              <div className="flex-1 sm:flex-none">
                <select
                  value={selectedCategory}
                  onChange={(e) => setSelectedCategory(e.target.value)}
                  className="w-full sm:w-auto px-3 sm:px-4 py-2.5 sm:py-3 border border-gray-300 rounded-lg sm:rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm sm:text-base min-w-0 sm:min-w-[160px]"
                >
                  <option value="all">All Categories</option>
                  {categories.map((category) => (
                    <option key={category.id} value={category.id}>
                      {category.name}
                    </option>
                  ))}
                </select>
              </div>
            </div>
          </div>
        </div>

        {/* Perfumes Grid - Responsive Layout */}
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4 sm:gap-6">
          {filteredPerfumes.map((perfume) => (
            <div key={perfume.id} className="bg-white rounded-xl sm:rounded-2xl shadow-sm border border-gray-100 hover:shadow-md transition-shadow">
              <div className="p-4 sm:p-6 pb-2">
                {/* Perfume Header */}
                <div className="flex items-start mb-4">
                  <div className="flex items-center min-w-0 flex-1">
                    <div className="w-10 h-10 sm:w-12 sm:h-12 bg-gradient-to-r from-purple-500 to-pink-600 rounded-lg sm:rounded-xl flex items-center justify-center relative flex-shrink-0">
                      <FlaskConical className="h-5 w-5 sm:h-6 sm:w-6 text-white" />
                      {/* Bottled Indicator - Green Checkmark on Flask Icon */}
                      {(perfume.used_in_bottling && perfume.used_in_bottling > 0) && (
                        <CheckCircle2 className="absolute -top-1 -right-1 h-4 w-4 sm:h-5 sm:w-5 text-green-600 bg-white rounded-full" />
                      )}
                    </div>
                    <div className="ml-2 sm:ml-3 min-w-0 flex-1">
                      <h3 className="text-base sm:text-lg font-semibold text-gray-900 truncate">{perfume.name}</h3>
                    </div>
                  </div>
                </div>

                {/* Perfume Details */}
                <div className="space-y-2 sm:space-y-3">
                  {perfume.category_name && (
                    <div className="flex items-center justify-between text-xs sm:text-sm">
                      <span className="text-gray-600">Category:</span>
                      <span className="font-medium text-primary-600 truncate ml-2">{perfume.category_name}</span>
                    </div>
                  )}
                  <div className="text-xs sm:text-sm text-gray-600">
                    <span className="font-medium">Scent:</span> 
                    <span className="ml-1 break-words">{perfume.scent_description || 'No description'}</span>
                  </div>
                  <div className="flex items-center justify-between text-xs sm:text-sm">
                    <span className="text-gray-600">Total Stock:</span>
                    <span className="font-semibold text-gray-900">{perfume.total_stock_ml || perfume.bulk_quantity_ml}ML</span>
                  </div>
                  <div className="flex items-center justify-between text-xs sm:text-sm">
                    <span className="text-gray-600">Used:</span>
                    <span className="font-semibold text-orange-600">{perfume.used_in_bottling || 0}ML</span>
                  </div>

                  {/* Usage Progress Bar */}
                  {perfume.total_stock_ml && perfume.used_in_bottling && (
                    <div className="mt-2">
                      <div className="flex justify-between text-xs text-gray-500 mb-1">
                        <span>Usage</span>
                        <span>{((perfume.used_in_bottling / perfume.total_stock_ml) * 100).toFixed(1)}%</span>
                      </div>
                      <div className="w-full bg-gray-200 rounded-full h-1.5 sm:h-2">
                        <div
                          className="bg-gradient-to-r from-orange-400 to-red-500 h-1.5 sm:h-2 rounded-full transition-all duration-300"
                          style={{
                            width: `${Math.min((perfume.used_in_bottling / perfume.total_stock_ml) * 100, 100)}%`
                          }}
                        ></div>
                      </div>
                    </div>
                  )}
                  <div className="flex items-center justify-between text-xs sm:text-sm">
                    <span className="text-gray-600">Cost per ML:</span>
                    <span className="font-semibold text-gray-900">RWF {parseFloat(perfume.cost_per_ml || 0).toLocaleString()}</span>
                  </div>
                  {perfume.batch_number && (
                    <div className="flex items-center justify-between text-xs sm:text-sm">
                      <span className="text-gray-600">Batch:</span>
                      <span className="font-mono text-gray-900 truncate ml-2">{perfume.batch_number}</span>
                    </div>
                  )}
                  {perfume.expiry_date && (
                    <div className="flex items-center justify-between text-xs sm:text-sm">
                      <span className="text-gray-600">Expiry:</span>
                      <span className="font-mono text-gray-900">{new Date(perfume.expiry_date).toLocaleDateString()}</span>
                    </div>
                  )}
                </div>

                {/* Status and Actions */}
                <div className="mt-3 sm:mt-4 pt-3 sm:pt-4 border-t border-gray-100">
                  <div className="flex items-center justify-between flex-wrap gap-2">
                    <span className={`inline-flex items-center px-2 sm:px-2.5 py-0.5 rounded-full text-xs font-medium ${perfume.is_active
                      ? 'bg-green-100 text-green-800'
                      : 'bg-red-100 text-red-800'
                      }`}>
                      {perfume.is_active ? 'Active' : 'Inactive'}
                    </span>
                    {isManagerOrAdmin && (
                      <div className="flex gap-1 sm:gap-2 flex-wrap">
                        <button
                          onClick={() => handleQuickBottleAllSizes(perfume)}
                          className="text-xs sm:text-sm text-primary-600 hover:text-primary-700 font-medium px-2 sm:px-3 py-1.5 sm:py-2 rounded-lg hover:bg-primary-50 transition-colors min-h-[44px] touch-target border border-primary-200"
                          title="Quick Bottle (Smart Selection)"
                        >
                          <span className="hidden sm:inline">Quick Bottle</span>
                          <span className="sm:hidden">Quick</span>
                        </button>
                        <button
                          onClick={() => handleReturnSinglePerfumeInventory(perfume)}
                          className="text-xs sm:text-sm text-primary-600 hover:text-primary-700 font-medium px-2 sm:px-3 py-1.5 sm:py-2 rounded-lg hover:bg-primary-50 transition-colors min-h-[44px] touch-target border border-primary-200"
                          title="Return shop inventory for this perfume"
                        >
                          <RotateCcw className="h-3 w-3 sm:h-4 sm:w-4 mr-0.5 sm:mr-1 inline" />
                          <span className="hidden sm:inline">Return</span>
                          <span className="sm:hidden">↩</span>
                        </button>
                      </div>
                    )}
                  </div>
                  {/* Edit and Delete Icons - Bottom Right */}
                  <div className="flex items-center justify-end gap-1 sm:gap-2 mt-2">
                    <button
                      onClick={() => handleEdit(perfume)}
                      className="p-2 text-gray-400 hover:text-primary-600 hover:bg-primary-50 rounded-lg transition-colors min-h-[44px] min-w-[44px] touch-target flex items-center justify-center"
                      title="Edit Perfume"
                    >
                      <Edit className="h-4 w-4" />
                    </button>
                    <button
                      onClick={() => handleDelete(perfume)}
                      className="p-2 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors min-h-[44px] min-w-[44px] touch-target flex items-center justify-center"
                      title="Delete Perfume"
                    >
                      <Trash2 className="h-4 w-4" />
                    </button>
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>

        {/* Empty State */}
        {filteredPerfumes.length === 0 && !isLoading && (
          <div className="text-center py-12">
            <FlaskConical className="h-12 w-12 text-gray-400 mx-auto mb-4" />
            <h3 className="text-lg font-medium text-gray-900 mb-2">No perfumes found</h3>
            <p className="text-gray-500 mb-6">
              {searchTerm || selectedFilter !== 'all' || selectedCategory !== 'all'
                ? 'Try adjusting your search or filter criteria.'
                : 'Get started by adding your first bulk perfume.'
              }
            </p>
            <button
              onClick={openAddModal}
              className="inline-flex items-center px-4 py-2 bg-primary-600 text-white font-medium rounded-xl hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500"
            >
              <Plus className="h-5 w-5 mr-2" />
              Add First Perfume
            </button>
          </div>
        )}

        {/* Pagination - Responsive Layout */}
        {!isLoading && filteredPerfumes.length > 0 && pagination.totalPages > 1 && (
          <div className="mt-6 sm:mt-8 bg-white rounded-xl p-4 shadow-sm border border-gray-100">
            {/* Mobile-first pagination info */}
            <div className="text-xs sm:text-sm text-gray-600 text-center sm:text-left mb-4 sm:mb-0">
              Showing <span className="font-medium">{(currentPage - 1) * pageSize + 1}</span> to{' '}
              <span className="font-medium">
                {Math.min(currentPage * pageSize, pagination.total)}
              </span>{' '}
              of <span className="font-medium">{pagination.total}</span> perfumes
            </div>

            {/* Pagination Controls */}
            <div className="flex flex-col sm:flex-row items-center justify-between gap-4">
              {/* Page Navigation */}
              <div className="flex items-center gap-1 sm:gap-2 order-2 sm:order-1">
                <button
                  onClick={() => setCurrentPage(prev => Math.max(1, prev - 1))}
                  disabled={!pagination.hasPrevPage || isLoading}
                  className="px-2 sm:px-3 py-2 text-xs sm:text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-1 transition-colors min-h-[44px] touch-target"
                >
                  <ChevronLeft className="h-3 w-3 sm:h-4 sm:w-4" />
                  <span className="hidden sm:inline">Previous</span>
                  <span className="sm:hidden">Prev</span>
                </button>

                {/* Page Numbers - Responsive */}
                <div className="flex items-center gap-1">
                  {Array.from({ length: Math.min(5, pagination.totalPages) }, (_, i) => {
                    let pageNum;
                    if (pagination.totalPages <= 5) {
                      pageNum = i + 1;
                    } else if (currentPage <= 3) {
                      pageNum = i + 1;
                    } else if (currentPage >= pagination.totalPages - 2) {
                      pageNum = pagination.totalPages - 4 + i;
                    } else {
                      pageNum = currentPage - 2 + i;
                    }

                    return (
                      <button
                        key={pageNum}
                        onClick={() => setCurrentPage(pageNum)}
                        disabled={isLoading}
                        className={`px-2 sm:px-3 py-2 text-xs sm:text-sm font-medium rounded-lg transition-colors min-h-[44px] min-w-[44px] touch-target ${currentPage === pageNum
                          ? 'bg-primary-600 text-white'
                          : 'text-gray-700 bg-white border border-gray-300 hover:bg-gray-50'
                          } disabled:opacity-50 disabled:cursor-not-allowed`}
                      >
                        {pageNum}
                      </button>
                    );
                  })}
                </div>

                <button
                  onClick={() => setCurrentPage(prev => Math.min(pagination.totalPages, prev + 1))}
                  disabled={!pagination.hasNextPage || isLoading}
                  className="px-2 sm:px-3 py-2 text-xs sm:text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-1 transition-colors min-h-[44px] touch-target"
                >
                  <span className="hidden sm:inline">Next</span>
                  <span className="sm:hidden">Next</span>
                  <ChevronRight className="h-3 w-3 sm:h-4 sm:w-4" />
                </button>
              </div>

              {/* Per Page Selector */}
              <div className="flex items-center gap-2 order-1 sm:order-2">
                <label className="text-xs sm:text-sm text-gray-600 whitespace-nowrap">Per page:</label>
                <select
                  value={pageSize}
                  onChange={(e) => {
                    setPageSize(Number(e.target.value));
                    setCurrentPage(1);
                  }}
                  className="px-2 py-1 text-xs sm:text-sm border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent min-h-[44px] touch-target"
                >
                  <option value={6}>6</option>
                  <option value={12}>12</option>
                  <option value={24}>24</option>
                  <option value={48}>48</option>
                </select>
              </div>
            </div>
          </div>
        )}
      </div>

      {/* Add/Edit Perfume Modal - Responsive */}
      {showAddModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-2 sm:p-4">
          <div className="bg-white rounded-lg sm:rounded-xl max-w-2xl w-full max-h-[98vh] sm:max-h-[95vh] overflow-y-auto">
            <div className="flex items-center justify-between p-4 sm:p-6 border-b">
              <div className="min-w-0 flex-1 mr-4">
                <h3 className="text-lg sm:text-xl font-semibold text-gray-900 truncate">
                  {editMode ? 'Edit Perfume' : 'Add New Perfume'}
                </h3>
                <p className="text-xs sm:text-sm text-gray-500 mt-1">
                  {editMode ? 'Update perfume information' : 'Add a new bulk perfume to inventory'}
                </p>
              </div>
              <button
                onClick={() => {
                  setShowAddModal(false);
                  setEditMode(false);
                  setSelectedPerfume(null);
                  resetForm();
                }}
                className="text-gray-400 hover:text-gray-600 transition-colors p-2 min-h-[44px] min-w-[44px] touch-target flex items-center justify-center"
              >
                <X className="h-5 w-5 sm:h-6 sm:w-6" />
              </button>
            </div>

            <form onSubmit={handleSubmit} className="p-4 sm:p-6 space-y-4">
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Perfume Name *
                  </label>
                  <input
                    type="text"
                    name="name"
                    value={perfumeForm.name}
                    onChange={handleInputChange}
                    required
                    className="w-full px-3 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm sm:text-base"
                    placeholder="Enter perfume name"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Supplier
                  </label>
                  <input
                    type="text"
                    name="supplier"
                    value={perfumeForm.supplier}
                    onChange={handleInputChange}
                    className="w-full px-3 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm sm:text-base"
                    placeholder="Enter supplier name"
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Scent Description *
                </label>
                <textarea
                  name="scent_description"
                  value={perfumeForm.scent_description}
                  onChange={handleInputChange}
                  required
                  rows={3}
                  className="w-full px-3 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm sm:text-base resize-none"
                  placeholder="Describe the scent profile"
                />
              </div>

              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Quantity (Milliliters) *
                  </label>
                  <input
                    type="number"
                    name="bulk_quantity_ml"
                    value={perfumeForm.bulk_quantity_ml}
                    onChange={handleInputChange}
                    required
                    step="1"
                    min="1"
                    className="w-full px-3 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm sm:text-base"
                    placeholder="0"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Cost per Milliliter (RWF) *
                  </label>
                  <input
                    type="number"
                    name="cost_per_ml"
                    value={perfumeForm.cost_per_ml}
                    onChange={handleInputChange}
                    required
                    step="0.0001"
                    min="0"
                    className="w-full px-3 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm sm:text-base"
                    placeholder="0.0000"
                  />
                </div>
              </div>

              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Batch Number
                  </label>
                  <input
                    type="text"
                    name="batch_number"
                    value={perfumeForm.batch_number}
                    onChange={handleInputChange}
                    className="w-full px-3 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm sm:text-base"
                    placeholder="Enter batch number"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Category
                  </label>
                  <select
                    name="category_id"
                    value={perfumeForm.category_id}
                    onChange={handleInputChange}
                    className="w-full px-3 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm sm:text-base"
                  >
                    <option value="">Select a category</option>
                    {categories.map(category => (
                      <option key={category.id} value={category.id}>
                        {category.name}
                      </option>
                    ))}
                  </select>
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Expiry Date <span className="text-gray-400 text-xs">(Optional)</span>
                </label>
                <input
                  type="date"
                  name="expiry_date"
                  value={perfumeForm.expiry_date}
                  onChange={handleInputChange}
                  className="w-full px-3 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm sm:text-base"
                />
              </div>

              <div className="flex flex-col sm:flex-row justify-end space-y-3 sm:space-y-0 sm:space-x-3 pt-6 border-t">
                <button
                  type="button"
                  onClick={() => {
                    setShowAddModal(false);
                    setEditMode(false);
                    setSelectedPerfume(null);
                    resetForm();
                  }}
                  className="w-full sm:w-auto px-4 py-2.5 text-gray-700 bg-gray-100 hover:bg-gray-200 rounded-lg transition-colors min-h-[44px] touch-target"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={createPerfumeMutation.isLoading || updatePerfumeMutation.isLoading}
                  className="w-full sm:w-auto px-6 py-2.5 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center min-h-[44px] touch-target"
                >
                  {(createPerfumeMutation.isLoading || updatePerfumeMutation.isLoading) ? (
                    <>
                      <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                      {editMode ? 'Updating...' : 'Adding...'}
                    </>
                  ) : (
                    <>
                      <Save className="h-4 w-4 mr-2" />
                      {editMode ? 'Update Perfume' : 'Add Perfume'}
                    </>
                  )}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Bottling Modal */}
      {showBottleModal && selectedPerfume && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-2 sm:p-4">
          <div className="bg-white rounded-xl max-w-2xl w-full max-h-[98vh] sm:max-h-[95vh] overflow-y-auto">
            <div className="flex items-center justify-between p-4 sm:p-6 border-b">
              <div>
                <h3 className="text-xl font-semibold text-gray-900">Bottle Perfume</h3>
                <p className="text-sm text-gray-500 mt-1">
                  Convert bulk perfume to bottled products
                </p>
              </div>
              <button
                onClick={() => {
                  setShowBottleModal(false);
                  setSelectedPerfume(null);
                  resetBottlingForm();
                }}
                className="text-gray-400 hover:text-gray-600 transition-colors"
              >
                <X className="h-6 w-6" />
              </button>
            </div>

            <div className="p-4 sm:p-6">
              {/* Perfume Info */}
              <div className="bg-gray-50 rounded-lg p-4 mb-6">
                <h4 className="font-medium text-gray-900 mb-2">Selected Perfume</h4>
                <div className="grid grid-cols-2 gap-4 text-sm">
                  <div>
                    <span className="text-gray-600">Name:</span>
                    <span className="ml-2 font-medium">{selectedPerfume.name}</span>
                  </div>
                  <div>
                    <span className="text-gray-600">Available:</span>
                    <span className="ml-2 font-medium">{selectedPerfume.bulk_quantity_ml}ML</span>
                  </div>
                  <div>
                    <span className="text-gray-600">Cost/ML:</span>
                    <span className="ml-2 font-medium">RWF {parseFloat(selectedPerfume.cost_per_ml || 0).toLocaleString()}</span>
                  </div>
                  <div>
                    <span className="text-gray-600">Supplier:</span>
                    <span className="ml-2 font-medium">{selectedPerfume.supplier || 'N/A'}</span>
                  </div>
                </div>
              </div>

              <form onSubmit={handleBottlingSubmit} className="space-y-4">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Bottle Size (ML) *
                    </label>
                    <select
                      name="bottle_size"
                      value={bottlingForm.bottle_size}
                      onChange={handleBottlingInputChange}
                      required
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    >
                      <option value="">Select bottle size</option>
                      <option value="30">30 ML</option>
                      <option value="50">50 ML</option>
                      <option value="100">100 ML</option>
                    </select>
                    {bottlingForm.bottle_size && (() => {
                      const bottleSizeMl = parseInt(bottlingForm.bottle_size);
                      const selectedSize = bottleSizes.find(s => s.size_ml === bottleSizeMl);
                      if (selectedSize) {
                        const quantity = parseInt(selectedSize.quantity) || 0;
                        const isLow = quantity < LOW_QUANTITY_THRESHOLD;
                        if (isLow) {
                          return (
                            <div className="mt-2 p-2 bg-yellow-50 border border-yellow-200 rounded text-yellow-800 text-xs flex items-center">
                              <AlertCircle className="h-4 w-4 mr-2" />
                              <span><strong>Warning:</strong> Low stock! Only {quantity} bottles available for this size.</span>
                            </div>
                          );
                        }
                      }
                      return null;
                    })()}
                  </div>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Quantity to Bottle *
                    </label>
                    <input
                      type="number"
                      name="quantity"
                      value={bottlingForm.quantity}
                      onChange={handleBottlingInputChange}
                      required
                      min="1"
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      placeholder="Enter quantity"
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Shop to Assign *
                    </label>
                    <select
                      name="shop_id"
                      value={bottlingForm.shop_id}
                      onChange={handleBottlingInputChange}
                      required
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    >
                      <option value="">Select shop</option>
                      {shops.map(shop => (
                        <option key={shop.id} value={shop.id}>{shop.name}</option>
                      ))}
                    </select>
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Bottling Batch Number
                  </label>
                  <input
                    type="text"
                    name="batch_number"
                    value={bottlingForm.batch_number}
                    onChange={handleBottlingInputChange}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    placeholder="Enter bottling batch number (optional)"
                  />
                </div>

                {/* Display calculated selling price per ml */}
                {bottlingForm.bottle_size && selectedPerfume && (
                  <div className="bg-primary-50 border border-primary-200 rounded-lg p-3">
                    <p className="text-sm text-primary-800">
                      <strong>Selling Price per ML:</strong> RWF {getSellingPricePerMl(getPricingCategory(), parseInt(bottlingForm.bottle_size)).toLocaleString()}
                      {selectedPerfume.category_name && (
                        <span className="ml-2 text-xs text-gray-600">
                          ({selectedPerfume.category_name})
                        </span>
                      )}
                    </p>
                  </div>
                )}

                {/* Batch Calculation Display */}
                {batchDetails && (
                  <div className="bg-primary-50 border border-primary-200 rounded-lg p-3 sm:p-4">
                    <h4 className="font-medium text-primary-900 mb-3 flex items-center text-sm sm:text-base">
                      <BarChart3 className="h-4 w-4 mr-2" />
                      Batch Calculation Summary
                    </h4>
                    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3 sm:gap-4 text-xs sm:text-sm">
                      <div className="space-y-2">
                        <h5 className="font-medium text-primary-800 mb-2">📦 Production Details</h5>
                        <div className="flex justify-between">
                          <span className="text-primary-700">Bottles to Create:</span>
                          <span className="font-medium text-primary-900">{batchDetails.bottlesToCreate}</span>
                        </div>
                        <div className="flex justify-between">
                          <span className="text-primary-700">Bottle Size:</span>
                          <span className="font-medium text-primary-900">{batchDetails.bottleSizeMl}ml</span>
                        </div>
                        <div className="flex justify-between">
                          <span className="text-primary-700">Total Volume Used:</span>
                          <span className="font-medium text-primary-900">{batchDetails.totalLitersUsed.toFixed(3)}L</span>
                        </div>
                        <div className="flex justify-between">
                          <span className="text-primary-700">Current Stock:</span>
                          <span className="font-medium text-primary-900">{batchDetails.currentStock}ML</span>
                        </div>
                        <div className="flex justify-between border-t pt-2 mt-2">
                          <span className="text-primary-700">Bottle Stock Available:</span>
                          <span className="font-medium text-primary-900">{batchDetails.bottleSizeQuantity || 0}</span>
                        </div>
                        <div className="flex justify-between">
                          <span className="text-primary-700">Bottles Used (Total):</span>
                          <span className="font-medium text-primary-600">
                            {batchDetails.selectedBottleSize?.used_quantity || 0}
                          </span>
                        </div>
                      </div>

                      <div className="space-y-2">
                        <h5 className="font-medium text-primary-800 mb-2">💰 Cost Breakdown</h5>
                        <div className="flex justify-between">
                          <span className="text-primary-700">Perfume Cost:</span>
                          <span className="font-medium text-primary-900">RWF {batchDetails.perfumeCost.toLocaleString()}</span>
                        </div>
                        <div className="flex justify-between">
                          <span className="text-primary-700">Bottle Cost:</span>
                          <span className="font-medium text-primary-900">RWF {batchDetails.bottleCost.toLocaleString()}</span>
                        </div>
                        <div className="flex justify-between">
                          <span className="text-primary-700">Label Cost:</span>
                          <span className="font-medium text-primary-900">RWF {batchDetails.labelCost.toLocaleString()}</span>
                        </div>
                        <div className="flex justify-between">
                          <span className="text-primary-700">Packaging Cost:</span>
                          <span className="font-medium text-primary-900">RWF {batchDetails.packagingCost.toLocaleString()}</span>
                        </div>
                        <div className="flex justify-between border-t pt-1">
                          <span className="text-primary-700 font-medium">Total Cost:</span>
                          <span className="font-bold text-primary-900">RWF {batchDetails.totalCost.toLocaleString()}</span>
                        </div>
                        <div className="flex justify-between">
                          <span className="text-primary-700 font-medium">Unit Cost:</span>
                          <span className="font-bold text-primary-900">RWF {batchDetails.unitCost.toLocaleString()}</span>
                        </div>
                      </div>

                      <div className="space-y-2">
                        <h5 className="font-medium text-primary-800 mb-2">💵 Pricing</h5>
                        <div className="flex justify-between">
                          <span className="text-primary-700">Selling Price per ML:</span>
                          <span className="font-medium text-primary-900">RWF {batchDetails.sellingPricePerMl?.toLocaleString() || '0'}</span>
                        </div>
                        <div className="flex justify-between">
                          <span className="text-primary-700">Bottle Size:</span>
                          <span className="font-medium text-primary-900">{batchDetails.bottleSizeMl}ml</span>
                        </div>
                        <div className="flex justify-between">
                          <span className="text-primary-700">Calculated Profit Margin:</span>
                          <span className="font-medium text-primary-900">{batchDetails.profitMargin.toFixed(2)}%</span>
                        </div>
                        <div className="flex justify-between border-t pt-1">
                          <span className="text-primary-700 font-medium">Selling Price:</span>
                          <span className="font-bold text-green-600">RWF {batchDetails.sellingPrice.toLocaleString()}</span>
                        </div>
                        <div className="flex justify-between">
                          <span className="text-primary-700">Profit per Bottle:</span>
                          <span className="font-bold text-green-600">RWF {(batchDetails.sellingPrice - batchDetails.unitCost).toLocaleString()}</span>
                        </div>
                        <div className="flex justify-between">
                          <span className="text-primary-700">Status:</span>
                          <span className={`font-medium flex items-center ${batchDetails.canBottle ? 'text-green-600' : 'text-red-600'}`}>
                            {batchDetails.canBottle ? (
                              <>
                                <CheckCircle className="h-3 w-3 mr-1" />
                                Ready to Bottle
                              </>
                            ) : (
                              <>
                                <AlertCircle className="h-3 w-3 mr-1" />
                                Insufficient Stock
                              </>
                            )}
                          </span>
                        </div>
                      </div>
                    </div>
                    {!batchDetails.canBottle && (
                      <div className="mt-3 p-2 bg-red-100 border border-red-200 rounded text-red-700 text-xs">
                        <strong>Warning:</strong> You cannot bottle {batchDetails.bottlesToCreate} bottles of {batchDetails.bottleSizeMl}ml.
                        {!batchDetails.hasEnoughBottles ? (
                          <span> Insufficient bottle stock! Available: {batchDetails.bottleSizeQuantity}, Requested: {batchDetails.bottlesToCreate}.</span>
                        ) : (
                          <span> Maximum possible: {Math.floor((batchDetails.currentStock * 1000) / batchDetails.bottleSizeMl)} bottles.</span>
                        )}
                      </div>
                    )}
                    {batchDetails && batchDetails.canBottle && batchDetails.isLowQuantity && (
                      <div className="mt-3 p-2 bg-yellow-100 border border-yellow-200 rounded text-yellow-800 text-xs flex items-center">
                        <AlertCircle className="h-4 w-4 mr-2" />
                        <span><strong>Warning:</strong> Low bottle stock! Only {batchDetails.bottleSizeQuantity} bottles available.</span>
                      </div>
                    )}
                  </div>
                )}

                {/* Quick Bottle All Sizes Section (Manager & Admin) */}
                {isManagerOrAdmin && (
                  <div className="mt-6 pt-6 border-t border-gray-200">
                    <div className="bg-gradient-to-r from-purple-50 to-blue-50 rounded-lg p-4 mb-4">
                      <h4 className="font-semibold text-gray-900 mb-2 flex items-center">
                        <Package className="h-5 w-5 mr-2 text-primary-600" />
                        Quick Bottle All Sizes
                      </h4>
                      <p className="text-sm text-gray-600 mb-4">
                        Automatically bottle this perfume in all three sizes (30ml, 50ml, 100ml) with quantity 1 each for the selected shop(s).
                      </p>
                      <div className="mb-4">
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                          Select Shop *
                        </label>
                        <select
                          value={quickBottleShopId}
                          onChange={(e) => setQuickBottleShopId(e.target.value)}
                          className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
                        >
                          <option value="">Select shop or choose "All Shops"</option>
                          <option value="all">All Shops</option>
                          {shops.map(shop => (
                            <option key={shop.id} value={shop.id}>{shop.name}</option>
                          ))}
                        </select>
                      </div>
                      <button
                        type="button"
                        onClick={handleQuickBottleAllSizes}
                        disabled={quickBottleAllSizesMutation.isLoading || !quickBottleShopId || !selectedPerfume}
                        className="w-full px-6 py-3 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center font-medium"
                      >
                        {quickBottleAllSizesMutation.isLoading ? (
                          <>
                            <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-white mr-2"></div>
                            Bottling All Sizes...
                          </>
                        ) : (
                          <>
                            <Package className="h-5 w-5 mr-2" />
                            Quick Bottle All Sizes (30ml, 50ml, 100ml)
                          </>
                        )}
                      </button>
                    </div>
                  </div>
                )}

                <div className="flex justify-end space-x-3 pt-6 border-t">
                  <button
                    type="button"
                    onClick={() => {
                      setShowBottleModal(false);
                      setSelectedPerfume(null);
                      resetBottlingForm();
                      setQuickBottleShopId('');
                    }}
                    className="px-4 py-2 text-gray-700 bg-gray-100 hover:bg-gray-200 rounded-lg transition-colors"
                  >
                    Cancel
                  </button>
                  <button
                    type="submit"
                    disabled={bottlePerfumeMutation.isLoading || (batchDetails && !batchDetails.canBottle)}
                    className="px-6 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center"
                  >
                    {bottlePerfumeMutation.isLoading ? (
                      <>
                        <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                        Bottling...
                      </>
                    ) : (
                      <>
                        <Wine className="h-4 w-4 mr-2" />
                        Start Bottling
                      </>
                    )}
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}

      {/* Bottle Size List Modal */}
      {showBottleSizeListModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-2 sm:p-4">
          <div className="bg-white rounded-xl max-w-4xl w-full max-h-[98vh] sm:max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between p-6 border-b">
              <div>
                <h3 className="text-xl font-semibold text-gray-900">Manage Bottle Sizes</h3>
                <p className="text-sm text-gray-500 mt-1">Create and manage bottle size configurations</p>
              </div>
              <div className="flex gap-2">
                <button
                  onClick={() => {
                    setSelectedBottleSize(null);
                    setShowBottleSizeModal(true);
                  }}
                  className="inline-flex items-center px-3 py-2 bg-primary-600 text-white text-sm font-medium rounded-lg hover:bg-primary-700"
                >
                  <Plus className="h-4 w-4 mr-1" />
                  Add Size
                </button>
                <button
                  onClick={() => {
                    setShowBottleSizeListModal(false);
                    setSelectedBottleSize(null);
                  }}
                  className="text-gray-400 hover:text-gray-600 transition-colors"
                >
                  <X className="h-6 w-6" />
                </button>
              </div>
            </div>

            <div className="p-4 sm:p-6">
              {bottleSizes.length === 0 ? (
                <div className="text-center py-12">
                  <Package className="h-12 w-12 text-gray-400 mx-auto mb-4" />
                  <h3 className="text-lg font-medium text-gray-900 mb-2">No bottle sizes found</h3>
                  <p className="text-gray-500 mb-6">Get started by adding your first bottle size.</p>
                  <button
                    onClick={() => {
                      setSelectedBottleSize(null);
                      setShowBottleSizeModal(true);
                    }}
                    className="inline-flex items-center px-4 py-2 bg-primary-600 text-white font-medium rounded-lg hover:bg-primary-700"
                  >
                    <Plus className="h-5 w-5 mr-2" />
                    Add First Bottle Size
                  </button>
                </div>
              ) : (
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3 sm:gap-4">
                  {bottleSizes.map((size) => (
                    <div key={size.id} className="bg-gray-50 rounded-lg p-4 border border-gray-200 hover:border-blue-300 transition-colors">
                      <div className="flex items-start justify-between mb-3">
                        <div>
                          <h4 className="text-lg font-semibold text-gray-900">{size.size_ml}ml</h4>
                          <p className="text-sm text-gray-500">Bottle Size</p>
                        </div>
                        <div className="flex gap-1">
                          <button
                            onClick={() => {
                              setSelectedBottleSize(size);
                              setShowBottleSizeModal(true);
                            }}
                            className="p-1 text-primary-600 hover:text-primary-800 hover:bg-primary-50 rounded"
                            title="Edit"
                          >
                            <Edit className="h-4 w-4" />
                          </button>
                          <button
                            onClick={() => {
                              if (window.confirm(`Delete bottle size ${size.size_ml}ml?`)) {
                                deleteBottleSizeMutation.mutate(size.id);
                              }
                            }}
                            className="p-1 text-red-600 hover:text-red-800 hover:bg-red-50 rounded"
                            title="Delete"
                          >
                            <Trash2 className="h-4 w-4" />
                          </button>
                        </div>
                      </div>
                      <div className="space-y-2 text-sm">
                        <div className="flex justify-between">
                          <span className="text-gray-600">Bottle Cost:</span>
                          <span className="font-medium">RWF {parseFloat(size.bottle_cost || 0).toLocaleString()}</span>
                        </div>
                        <div className="flex justify-between">
                          <span className="text-gray-600">Label Cost:</span>
                          <span className="font-medium">RWF {parseFloat(size.label_cost || 0).toLocaleString()}</span>
                        </div>
                        <div className="flex justify-between">
                          <span className="text-gray-600">Packaging Cost:</span>
                          <span className="font-medium">RWF {parseFloat(size.packaging_cost || 0).toLocaleString()}</span>
                        </div>
                        <div className="flex justify-between border-t pt-2 mt-2">
                          <span className="text-gray-600 font-medium">Total Cost:</span>
                          <span className="font-semibold text-primary-600">
                            RWF {(parseFloat(size.bottle_cost || 0) + parseFloat(size.label_cost || 0) + parseFloat(size.packaging_cost || 0)).toLocaleString()}
                          </span>
                        </div>
                        <div className="flex justify-between items-center">
                          <span className="text-gray-600">Stock Quantity:</span>
                          <span className={`font-medium flex items-center ${(parseInt(size.quantity) || 0) < LOW_QUANTITY_THRESHOLD ? 'text-yellow-600' : 'text-gray-900'}`}>
                            {size.quantity || 0}
                            {(parseInt(size.quantity) || 0) < LOW_QUANTITY_THRESHOLD && (
                              <AlertCircle className="h-4 w-4 ml-1 text-yellow-600" />
                            )}
                          </span>
                        </div>
                        <div className="flex justify-between items-center">
                          <span className="text-gray-600">Used Quantity:</span>
                          <span className="font-medium text-primary-600">
                            {size.used_quantity || 0}
                          </span>
                        </div>
                        {(parseInt(size.quantity) || 0) < LOW_QUANTITY_THRESHOLD && (
                          <div className="mt-2 p-2 bg-yellow-50 border border-yellow-200 rounded text-yellow-800 text-xs flex items-center">
                            <AlertCircle className="h-3 w-3 mr-1" />
                            <span>Low stock warning</span>
                          </div>
                        )}
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        </div>
      )}

      {/* Bottle Size Form Modal */}
      {showBottleSizeModal && (
        <BottleSizeModal
          onClose={() => {
            setShowBottleSizeModal(false);
            setSelectedBottleSize(null);
          }}
          onSubmit={(data) => {
            if (selectedBottleSize) {
              updateBottleSizeMutation.mutate({ id: selectedBottleSize.id, data });
            } else {
              createBottleSizeMutation.mutate(data);
            }
          }}
          bottleSize={selectedBottleSize}
          isLoading={createBottleSizeMutation.isLoading || updateBottleSizeMutation.isLoading}
        />
      )}

      {/* Quick Bottle All Sizes Modal */}
      {showQuickBottleModal && selectedPerfume && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-2 sm:p-4">
          <div className="bg-white rounded-xl max-w-md w-full">
            <div className="flex items-center justify-between p-4 sm:p-6 border-b">
              <div>
                <h3 className="text-xl font-semibold text-gray-900">Quick Bottle All Sizes</h3>
                <p className="text-sm text-gray-500 mt-1">
                  Bottle {selectedPerfume.name} in all sizes (30ml, 50ml, 100ml)
                </p>
              </div>
              <button
                onClick={() => {
                  setShowQuickBottleModal(false);
                  setSelectedPerfume(null);
                  setQuickBottleModalShopId('');
                }}
                className="text-gray-400 hover:text-gray-600 transition-colors"
              >
                <X className="h-6 w-6" />
              </button>
            </div>

            <div className="p-4 sm:p-6">
              {/* Perfume Info */}
              <div className="bg-gray-50 rounded-lg p-4 mb-6">
                <h4 className="font-medium text-gray-900 mb-2">Selected Perfume</h4>
                <div className="grid grid-cols-2 gap-4 text-sm">
                  <div>
                    <span className="text-gray-600">Name:</span>
                    <span className="ml-2 font-medium">{selectedPerfume.name}</span>
                  </div>
                  <div>
                    <span className="text-gray-600">Available:</span>
                    <span className="ml-2 font-medium">{selectedPerfume.bulk_quantity_ml}ML</span>
                  </div>
                </div>
              </div>

              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Select Shop *
                  </label>
                  <select
                    value={quickBottleModalShopId}
                    onChange={(e) => setQuickBottleModalShopId(e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
                  >
                    <option value="">Select shop or choose "All Shops"</option>
                    <option value="all">All Shops</option>
                    {shops.map(shop => (
                      <option key={shop.id} value={shop.id}>{shop.name}</option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Quantity per Shop *
                  </label>
                  <input
                    type="number"
                    min="1"
                    max="10"
                    value={bottlingForm.quantity || 1}
                    onChange={(e) => handleBottlingInputChange({ target: { name: 'quantity', value: parseInt(e.target.value) || 1 } })}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
                    placeholder="Enter quantity (1-10)"
                  />
                  <p className="mt-1 text-xs text-gray-500">
                    Number of units to bottle per shop for each size (30ml, 50ml, 100ml)
                  </p>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Batch Number (Optional)
                  </label>
                  <input
                    type="text"
                    value={bottlingForm.batch_number}
                    onChange={(e) => handleBottlingInputChange({ target: { name: 'batch_number', value: e.target.value } })}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
                    placeholder="Enter batch number (optional)"
                  />
                </div>

                <div className="bg-primary-50 border border-primary-200 rounded-lg p-3">
                  <p className="text-sm text-primary-800">
                    <strong>Note:</strong> This will create {bottlingForm.quantity || 1} unit(s) of each size (30ml, 50ml, 100ml) for the selected shop(s).
                    Total: {quickBottleModalShopId === 'all' ? (shops.length * 3 * (bottlingForm.quantity || 1)) : quickBottleModalShopId ? (3 * (bottlingForm.quantity || 1)) : 0} bottles will be created.
                  </p>
                </div>
              </div>

              <div className="flex justify-end space-x-3 pt-6 border-t mt-6">
                <button
                  type="button"
                  onClick={() => {
                    setShowQuickBottleModal(false);
                    setSelectedPerfume(null);
                    setQuickBottleModalShopId('');
                  }}
                  className="px-4 py-2 text-gray-700 bg-gray-100 hover:bg-gray-200 rounded-lg transition-colors"
                >
                  Cancel
                </button>
                <button
                  type="button"
                  onClick={handleQuickBottleFromModal}
                  disabled={quickBottleAllSizesMutation.isLoading || !quickBottleModalShopId}
                  className="px-6 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center"
                >
                  {quickBottleAllSizesMutation.isLoading ? (
                    <>
                      <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                      Bottling...
                    </>
                  ) : (
                    <>
                      <Package className="h-4 w-4 mr-2" />
                      Quick Bottle All Sizes
                    </>
                  )}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

// Bottle Size Modal Component
const BottleSizeModal = ({ onClose, onSubmit, bottleSize, isLoading }) => {
  const [formData, setFormData] = useState({
    size_ml: bottleSize?.size_ml || '',
    bottle_cost: bottleSize?.bottle_cost || '',
    label_cost: bottleSize?.label_cost || '',
    packaging_cost: bottleSize?.packaging_cost || '',
    quantity: bottleSize?.quantity || 0
  });

  const [errors, setErrors] = useState({});

  const handleSubmit = (e) => {
    e.preventDefault();

    // Validation
    const newErrors = {};
    if (!formData.size_ml || formData.size_ml <= 0) {
      newErrors.size_ml = 'Size in ml is required and must be greater than 0';
    }
    if (!formData.bottle_cost || formData.bottle_cost < 0) {
      newErrors.bottle_cost = 'Bottle cost is required and must be 0 or greater';
    }
    if (formData.label_cost < 0) {
      newErrors.label_cost = 'Label cost must be 0 or greater';
    }
    if (formData.packaging_cost < 0) {
      newErrors.packaging_cost = 'Packaging cost must be 0 or greater';
    }
    if (formData.quantity < 0) {
      newErrors.quantity = 'Quantity must be 0 or greater';
    }

    if (Object.keys(newErrors).length > 0) {
      setErrors(newErrors);
      return;
    }

    setErrors({});
    onSubmit(formData);
  };

  const handleInputChange = (field, value) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    // Clear error when user starts typing
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: '' }));
    }
  };

  const formatCurrency = (amount) => {
    return `RWF ${parseFloat(amount || 0).toLocaleString()}`;
  };

  return (
    <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div className="relative top-10 mx-auto p-6 border w-11/12 max-w-2xl shadow-lg rounded-md bg-white">
        <div className="flex justify-between items-center mb-6">
          <h3 className="text-xl font-semibold text-gray-900">
            {bottleSize ? 'Edit Bottle Size' : 'Add New Bottle Size'}
          </h3>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600"
          >
            <X className="h-6 w-6" />
          </button>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {/* Size in ml */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Size in ml *
              </label>
              <input
                type="number"
                min="1"
                step="1"
                value={formData.size_ml}
                onChange={(e) => handleInputChange('size_ml', parseFloat(e.target.value) || '')}
                className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent ${errors.size_ml ? 'border-red-500' : 'border-gray-300'
                  }`}
                placeholder="e.g., 50"
                required
              />
              {errors.size_ml && (
                <p className="mt-1 text-sm text-red-600">{errors.size_ml}</p>
              )}
            </div>

            {/* Bottle Cost */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Bottle Cost (RWF) *
              </label>
              <input
                type="number"
                min="0"
                step="0.01"
                value={formData.bottle_cost}
                onChange={(e) => handleInputChange('bottle_cost', parseFloat(e.target.value) || '')}
                className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent ${errors.bottle_cost ? 'border-red-500' : 'border-gray-300'
                  }`}
                placeholder="e.g., 2500"
                required
              />
              {errors.bottle_cost && (
                <p className="mt-1 text-sm text-red-600">{errors.bottle_cost}</p>
              )}
            </div>

            {/* Label Cost */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Label Cost (RWF)
              </label>
              <input
                type="number"
                min="0"
                step="0.01"
                value={formData.label_cost}
                onChange={(e) => handleInputChange('label_cost', parseFloat(e.target.value) || '')}
                className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent ${errors.label_cost ? 'border-red-500' : 'border-gray-300'
                  }`}
                placeholder="e.g., 500"
              />
              {errors.label_cost && (
                <p className="mt-1 text-sm text-red-600">{errors.label_cost}</p>
              )}
            </div>

            {/* Packaging Cost */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Packaging Cost (RWF)
              </label>
              <input
                type="number"
                min="0"
                step="0.01"
                value={formData.packaging_cost}
                onChange={(e) => handleInputChange('packaging_cost', parseFloat(e.target.value) || '')}
                className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent ${errors.packaging_cost ? 'border-red-500' : 'border-gray-300'
                  }`}
                placeholder="e.g., 1000"
              />
              {errors.packaging_cost && (
                <p className="mt-1 text-sm text-red-600">{errors.packaging_cost}</p>
              )}
            </div>

            {/* Quantity */}
            <div className="md:col-span-2">
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Quantity (Stock) *
              </label>
              <input
                type="number"
                min="0"
                step="1"
                value={formData.quantity}
                onChange={(e) => handleInputChange('quantity', parseInt(e.target.value) || 0)}
                className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent ${errors.quantity ? 'border-red-500' : 'border-gray-300'
                  }`}
                placeholder="e.g., 100"
                required
              />
              {errors.quantity && (
                <p className="mt-1 text-sm text-red-600">{errors.quantity}</p>
              )}
              <p className="mt-1 text-xs text-gray-500">Number of bottles in stock for this size</p>
            </div>
          </div>

          {/* Total Cost Preview */}
          <div className="bg-primary-50 p-4 rounded-lg">
            <h4 className="text-sm font-medium text-blue-900 mb-2">Total Cost Preview</h4>
            <div className="text-lg font-semibold text-blue-700">
              {formatCurrency((formData.bottle_cost || 0) +
                (formData.label_cost || 0) +
                (formData.packaging_cost || 0))}
            </div>
          </div>

          {/* Form Actions */}
          <div className="flex justify-end space-x-3 pt-4">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 border border-gray-300 rounded-lg text-sm font-medium text-gray-700 hover:bg-gray-50 focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isLoading}
              className="px-4 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700 focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isLoading ? (
                <div className="flex items-center">
                  <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                  {bottleSize ? 'Updating...' : 'Creating...'}
                </div>
              ) : (
                bottleSize ? 'Update Bottle Size' : 'Create Bottle Size'
              )}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default Perfumes; 