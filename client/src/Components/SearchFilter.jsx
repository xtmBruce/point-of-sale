const SearchFilter = ({ value = '', onChange, placeholder = 'Search...' }) => {
  return (
    <input
      type="text"
      value={value}
      onChange={(e) => onChange?.(e.target.value)}
      placeholder={placeholder}
      className="w-full rounded-lg border border-gray-300 px-3 py-2"
    />
  );
};

export default SearchFilter;
