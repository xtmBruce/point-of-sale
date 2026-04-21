const RATES = {
  USD: 1300, EUR: 1400, GBP: 1650, JPY: 8.7,
  CNY: 180, INR: 15.6, AED: 354, CAD: 950, AUD: 850, CHF: 1450, RWF: 1
};

const SYMBOLS = {
  USD: '$', EUR: '€', GBP: '£', JPY: '¥', CNY: '¥',
  INR: '₹', AED: 'د.إ', RWF: 'FRw'
};

export const useCurrencyConversion = () => {
  const convertToRwf = (amount, fromCurrency) => {
    if (!amount || !fromCurrency) return null;
    const rate = RATES[fromCurrency];
    if (!rate) return null;
    return parseFloat((amount * rate).toFixed(2));
  };

  const convertFromRwf = (rwfAmount, toCurrency) => {
    if (!rwfAmount || !toCurrency) return null;
    const rate = RATES[toCurrency];
    if (!rate) return null;
    return parseFloat((rwfAmount / rate).toFixed(2));
  };

  const formatCurrency = (amount, currency = 'RWF') => {
    if (!amount) return '0';
    const symbol = SYMBOLS[currency] || currency;
    if (currency === 'RWF') return `${symbol} ${Math.round(amount).toLocaleString()}`;
    return `${symbol}${parseFloat(amount).toFixed(2)}`;
  };

  const getRwfDisplay = (amount, currency) => {
    const rwf = convertToRwf(amount, currency);
    return rwf !== null ? `≈ ${formatCurrency(rwf, 'RWF')}` : null;
  };

  return {
    conversionRates: RATES,
    isLoading: false,
    convertToRwf,
    convertFromRwf,
    formatCurrency,
    getRwfDisplay,
    lastUpdated: null
  };
};
