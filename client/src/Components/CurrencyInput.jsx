import { useEffect, useState } from 'react';

const CurrencyInput = ({
  value,
  onChange,
  onCurrencyChange,
  defaultCurrency = 'RWF',
  label,
  required,
  error,
  placeholder = '0.00',
}) => {
  const [currency, setCurrency] = useState(defaultCurrency);

  useEffect(() => {
    setCurrency(defaultCurrency);
  }, [defaultCurrency]);

  return (
    <div>
      {label && <label className="mb-2 block text-sm font-medium text-gray-700">{label}{required ? ' *' : ''}</label>}
      <div className="grid grid-cols-3 gap-2">
        <select
          value={currency}
          onChange={(e) => {
            setCurrency(e.target.value);
            onCurrencyChange?.(e.target.value);
          }}
          className="rounded-lg border border-gray-300 px-3 py-2"
        >
          <option value="RWF">RWF</option>
          <option value="USD">USD</option>
          <option value="EUR">EUR</option>
        </select>
        <input
          type="number"
          step="0.01"
          value={value ?? ''}
          onChange={(e) => onChange?.(e.target.value)}
          placeholder={placeholder}
          className="col-span-2 rounded-lg border border-gray-300 px-3 py-2"
        />
      </div>
      {error ? <p className="mt-1 text-sm text-red-600">{error}</p> : null}
    </div>
  );
};

export default CurrencyInput;
