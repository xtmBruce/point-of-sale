export const StatsGrid = ({ children }) => <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">{children}</div>;

const StatsCard = ({ title, value, children }) => {
  return (
    <div className="rounded-lg border border-gray-200 bg-white p-4">
      {title ? <p className="text-sm text-gray-500">{title}</p> : null}
      {value ? <p className="mt-1 text-lg font-semibold text-gray-900">{value}</p> : null}
      {children}
    </div>
  );
};

export default StatsCard;
